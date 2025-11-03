# MCP OAuth and OpenID Connect

[![.NET](https://github.com/damienbod/McpOidcOAuth/actions/workflows/dotnet.yml/badge.svg)](https://github.com/damienbod/McpOidcOAuth/actions/workflows/dotnet.yml)

[Implement a secure MCP server using OAuth DPoP and Duende identity provider](https://damienbod.com/2025/11/03/implement-a-secure-mcp-server-using-oauth-dpop-and-duende-identity-provider/)

## Setup 

The UI application uses OpenID Connect to authentication with Duende identity provider. A DPoP access token is returned which is used to access the model context protocol server. 

![Flow 1](https://github.com/damienbod/McpOidcOAuth/blob/main/images/OIDC_MCP.drawio.png)

## Blogs:
- [Implement a secure MCP server using OAuth and Entra ID](https://damienbod.com/2025/09/23/implement-a-secure-mcp-server-using-oauth-and-entra-id/)
- [Implement a secure MCP OAuth desktop client using OAuth and Entra ID](https://damienbod.com/2025/10/16/implement-a-secure-mcp-oauth-desktop-client-using-oauth-and-entra-id/)
- [Model Context Protocol in :NET](https://medium.com/@cedric.mendelin/model-context-protocol-in-net-06c6076b6385)
- [Developing an MCP Scenario with TypeScript: A production-ready reference implementation, Tobias Maestrini](https://tmaestrini.github.io/topics/developing-an-mcp-scenario-with-typescript-a-production-ready-reference-implementation)
  
## History

- 2025-11-02 Updated packages

## Links

https://github.com/damienbod/McpSecurity

https://learn.microsoft.com/en-us/agent-framework/migration-guide/from-semantic-kernel/?pivots=programming-language-csharp

https://devblogs.microsoft.com/dotnet/mcp-server-dotnet-nuget-quickstart/

https://github.com/microsoft/mcp-dotnet-samples

https://learn.microsoft.com/en-us/dotnet/ai/quickstarts/build-mcp-server

## Standards, draft Standards

[OAuth 2.0 Dynamic Client Registration Protocol](https://datatracker.ietf.org/doc/html/rfc7591)

[OAuth 2.0 Authorization Server Metadata](https://datatracker.ietf.org/doc/html/rfc8414)

https://modelcontextprotocol.io/specification/2025-06-18/basic/authorization

https://modelcontextprotocol.io/specification/2025-06-18/basic/security_best_practices

https://github.com/modelcontextprotocol/modelcontextprotocol/issues/1299

https://den.dev/blog/mcp-authorization-resource/

## AI UI agents with OAuth support

https://github.com/daodao97/chatmcp

https://claude.ai/download

https://cursor.com/

Visual Studio code

Visual Studio

## Links

https://github.com/MicrosoftDocs/mcp

https://devblogs.microsoft.com/dotnet/mcp-csharp-sdk-2025-06-18-update/

https://modelcontextprotocol.io/docs/learn/architecture

https://github.com/SonarSource/sonarqube-mcp-server

https://den.dev/blog/mcp-authorization-resource/

https://den.dev/blog/mcp-csharp-sdk-authorization/

https://github.com/modelcontextprotocol/modelcontextprotocol/issues/1299

https://blog.cloudflare.com/building-ai-agents-with-mcp-authn-authz-and-durable-objects/

https://blog.aidanjohn.org/2025/07/30/mcp-a-new-frontier-in.html

https://medium.com/kagenti-the-agentic-platform/security-in-and-around-mcp-part-1-oauth-in-mcp-3f15fed0dd6e

https://medium.com/kagenti-the-agentic-platform/security-in-and-around-mcp-part-2-mcp-in-deployment-65bdd0ba9dc6

https://blog.christianposta.com/implementing-mcp-dynamic-client-registration-with-spiffe/

https://blog.christianposta.com/authenticating-mcp-oauth-clients-with-spiffe/

https://luke.geek.nz/azure/akahu-mcp-apim/

https://github.com/modelcontextprotocol/inspector

## Copilot Links

https://github.com/dotnet/AspNetCore.Docs/issues/35798

https://docs.github.com/en/copilot/how-tos/custom-instructions/adding-repository-custom-instructions-for-github-copilot

https://github.com/dotnet/docs-aspire/blob/main/.github/copilot-instructions.md

## Azure OpenAI 

https://learn.microsoft.com/en-us/azure/ai-foundry/

## Ready to use servers

https://mcpservers.org/

https://devblogs.microsoft.com/azure-sdk/introducing-the-azure-mcp-server/
 
## Learning Courses

https://github.com/microsoft/mcp-for-beginners

## Azure App Service MCP server

https://github.com/Azure-Samples/remote-mcp-webapp-dotnet

## Testing MCP Client tool

```
npx @modelcontextprotocol/inspector
```

https://github.com/modelcontextprotocol/csharp-sdk/blob/main/samples/ProtectedMcpClient/Program.cs

https://github.com/Azure-Samples/ms-identity-dotnet-desktop-tutorial/blob/master/1-Calling-MSGraph/1-1-AzureAD/Console-Interactive-MultiTarget/Program.cs
