using Duende.AspNetCore.Authentication.JwtBearer.DPoP;
using ToolsLibrary.Prompts;
using ToolsLibrary.Resources;
using ToolsLibrary.Tools;

var builder = WebApplication.CreateBuilder(args);
var httpMcpServerUrl = builder.Configuration["HttpMcpServerUrl"];
var identityProvider = builder.Configuration["IdentityProvider"];

builder.Services.AddAuthentication("dpoptokenscheme")
    //.AddJwtBearer("dpoptokenscheme", options =>
    //{
    //    options.Authority = identityProvider;
    //    // TODO add valid aud
    //    options.TokenValidationParameters.ValidateAudience = false;
    //    options.MapInboundClaims = false;
    //    options.TokenValidationParameters.ValidTypes = ["at+jwt"];
    //})
    .AddMcp("dpoptokenscheme", "mcp server", options =>
    {
        options.ResourceMetadata = new()
        {
            Resource = new Uri(httpMcpServerUrl),
            ResourceDocumentation = new Uri("https://klocalhost:5103/health"),
            ScopesSupported = ["scope-dpop"],
        };
    });

// layers DPoP onto the "token" scheme above
builder.Services.ConfigureDPoPTokensForScheme("dpoptokenscheme", opt =>
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
        policy.RequireClaim("scope", "scope-dpop"));

// Add services to the container.
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

// Enable CORS
app.UseCors();

app.MapGet("/health", () => $"Secure MCP server running deployed: UTC: {DateTime.UtcNow}, use /mcp path to use the tools");

app.UseAuthentication();
app.UseAuthorization();

app.MapMcp("/mcp").RequireAuthorization("mcp_tools");

app.Run();
