using Duende.AccessTokenManagement.DPoP;
using Duende.AccessTokenManagement.OpenIdConnect;
using McpWebClient.AiServices.Elicitation;
using McpWebClient.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace McpWebClient;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog((ctx, lc) => lc
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}")
            .WriteTo.File("../_logs-McpWebClient.txt")
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(ctx.Configuration));

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
       .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
       {
           options.ExpireTimeSpan = TimeSpan.FromHours(8);
           options.SlidingExpiration = false;
           options.Events.OnSigningOut = async e =>
           {
               await e.HttpContext.RevokeRefreshTokenAsync();
           };
       })
       .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
       {
           options.Authority = "https://localhost:5001";
           options.ClientId = "web-dpop";
           options.ClientSecret = "ddedF4f289k$3eDa23ed0iTk4Raq&tttk23d08nhzd";
           options.ResponseType = "code";
           options.ResponseMode = "query";
           options.UsePkce = true;

           options.Scope.Clear();
           options.Scope.Add("openid");
           options.Scope.Add("profile");
           options.Scope.Add("scope-dpop");
           options.Scope.Add("offline_access");
           options.GetClaimsFromUserInfoEndpoint = true;
           options.SaveTokens = true;

           options.TokenValidationParameters = new TokenValidationParameters
           {
               NameClaimType = "name",
               RoleClaimType = "role"
           };
       });

        var privatePem = File.ReadAllText(Path.Combine(builder.Environment.ContentRootPath, "ecdsa384-private.pem"));
        var publicPem = File.ReadAllText(Path.Combine(builder.Environment.ContentRootPath, "ecdsa384-public.pem"));
        var ecdsaCertificate = X509Certificate2.CreateFromPem(publicPem, privatePem);
        var ecdsaCertificateKey = new ECDsaSecurityKey(ecdsaCertificate.GetECDsaPrivateKey());

        // add automatic token management
        builder.Services.AddOpenIdConnectAccessTokenManagement(options =>
        {
            var jwk = JsonWebKeyConverter.ConvertFromSecurityKey(ecdsaCertificateKey);
            jwk.Alg = "ES384";
            options.DPoPJsonWebKey = DPoPProofKey.ParseOrDefault(JsonSerializer.Serialize(jwk));
        });

        builder.Services.AddUserAccessTokenHttpClient("dpop-api-client", configureClient: client =>
        {
            client.BaseAddress = new Uri("https://klocalhost:5103");
        });

        builder.Services.AddAuthorization(options =>
        {
            options.FallbackPolicy = options.DefaultPolicy;
        });

        builder.Services.AddRazorPages();

        builder.Services.AddSignalR();
        builder.Services.AddSingleton<ElicitationCoordinator>();
        builder.Services.AddScoped<ChatService>();

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
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapRazorPages()
           .WithStaticAssets();
        app.MapControllers();
        app.MapHub<ElicitationHub>("/hubs/elicitation");

        app.Run();
    }
}
