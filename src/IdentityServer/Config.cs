using Duende.IdentityServer.Models;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        ];

    public static IEnumerable<ApiScope> ApiScopes =>
        [
            new ApiScope("mcp:tools")
        ];

    public static IEnumerable<Client> Clients =>
        [
            new Client
            {
                ClientId = "McpWebClient",
                ClientSecrets = { new Secret("ddedF4f289k$3eDa23ed0iTk4Raq&tttk23d08nhzd".Sha256()) },

                AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,

                RedirectUris = { "https://localhost:5102/signin-oidc" },
                FrontChannelLogoutUri = "https://localhost:5102/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:5102/signout-callback-oidc" },

                RequireDPoP = true,

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "mcp:tools" }
            }
        ];
}
