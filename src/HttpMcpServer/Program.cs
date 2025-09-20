using Duende.AspNetCore.Authentication.JwtBearer.DPoP;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using Serilog;
using ToolsLibrary.Prompts;
using ToolsLibrary.Resources;
using ToolsLibrary.Tools;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
    .WriteTo.File("../_logs-HttpMcpServer.txt")
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(ctx.Configuration));

var httpMcpServerUrl = builder.Configuration["HttpMcpServerUrl"];
var identityProvider = builder.Configuration["IdentityProvider"];

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.Authority = identityProvider;
        options.Audience = $"{identityProvider}/resources";
        
        options.TokenValidationParameters.ValidateAudience = true;
        options.TokenValidationParameters.ValidateIssuer = true;

        options.MapInboundClaims = false;
        options.TokenValidationParameters.ValidTypes = ["at+jwt"];
    })
    .AddMcp(options =>
    {
        options.ResourceMetadata = new()
        {
            Resource = new Uri(httpMcpServerUrl!), 
            ResourceName = "MCP demo server",
            AuthorizationServers = [ new Uri(identityProvider!) ], 
            DpopBoundAccessTokensRequired = true,
            ResourceDocumentation = new Uri("https://localhost:5103/health"),
            ScopesSupported = ["mcp:tools"], 
        };
    });

// layers DPoP onto the "token" scheme above
builder.Services.ConfigureDPoPTokensForScheme("Bearer", opt =>
{
    opt.ValidationMode = ExpirationValidationMode.IssuedAt; // IssuedAt is the default.
});

builder.Services.AddAuthorization();

builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithPrompts<PromptExamples>()
    .WithResources<DocumentationResource>()
    .WithTools<RandomNumberTools>()
    .WithTools<DateTools>();

// Add CORS for HTTP transport support in browsers
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddHttpClient();

// change to scp or scope if not using magic namespaces from MS
// The scope must be validate as we want to force only delegated access tokens
// The scope is requires to only allow access tokens intended for this API
builder.Services.AddAuthorizationBuilder()
  .AddPolicy("mcp_tools", policy =>
        policy.RequireClaim("scope", "mcp:tools"));

// Add services to the container.
var app = builder.Build();

IdentityModelEventSource.ShowPII = true;
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

// Enable CORS
app.UseCors();

app.MapGet("/health", () => $"Secure MCP server running deployed: UTC: {DateTime.UtcNow}, use /mcp path to use the tools");

app.UseAuthentication();
app.UseAuthorization();

app.MapMcp("/mcp").RequireAuthorization("mcp_tools");

app.Run();
