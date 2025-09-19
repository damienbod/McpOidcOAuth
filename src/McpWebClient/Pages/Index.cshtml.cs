using Duende.AccessTokenManagement;
using Duende.AccessTokenManagement.OpenIdConnect;
using McpWebClient.AiServices.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace McpWebClient.Pages;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ChatService _chatService;
    private readonly IHttpClientFactory _clientFactory;

    [BindProperty]
    public string? PromptResults { get; set; }

    [BindProperty]
    [Required]
    public string Prompt { get; set; } = "Please generate a random number with the range of -10 and 10";

    [BindProperty]
    [Required]
    public ApprovalMode SelectedMode { get; set; } = ApprovalMode.Manual;

    public List<PendingFunctionCall> PendingFunctions { get; set; } = new();

    public IndexModel(ILogger<IndexModel> logger,
        IHttpClientFactory clientFactory,
        ChatService llmPromptService)
    {
        _clientFactory = clientFactory;
        _logger = logger;
        _chatService = llmPromptService;
    }

    public IActionResult OnGet()
    {
        // No special processing on GET – state is managed via post backs
        return Page();
    }

    private string GetUserKey()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.Identity?.Name ?? "anonymous";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return OnGet();
        }

        _chatService.SetMode(SelectedMode);

        var accessToken = await HttpContext.GetUserAccessTokenAsync(
          new UserTokenRequestParameters
          {
              Scope = Scope.ParseOrDefault("scope-dpop")
          });

        await _chatService.EnsureSetupAsync(_clientFactory);

        // Begin a fresh chat with the prompt
        var response = await _chatService.BeginChatAsync(GetUserKey(), Prompt);
        PromptResults = response.FinalAnswer;
        PendingFunctions = response.PendingFunctions;
        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(string functionId)
    {
        _chatService.SetMode(SelectedMode);

        var accessToken = await HttpContext.GetUserAccessTokenAsync(
            new UserTokenRequestParameters
            {
                Scope = Scope.ParseOrDefault("scope-dpop")
            });

        await _chatService.EnsureSetupAsync(_clientFactory);

        var response = await _chatService.ApproveFunctionAsync(GetUserKey(), functionId);

        PromptResults = response.FinalAnswer;
        PendingFunctions = response.PendingFunctions;
        return Page();
    }

    public async Task<IActionResult> OnPostDeclineAsync(string functionId)
    {
        _chatService.SetMode(SelectedMode);

        var accessToken = await HttpContext.GetUserAccessTokenAsync(
          new UserTokenRequestParameters
          {
              Scope = Scope.ParseOrDefault("scope-dpop")
          });

        await _chatService.EnsureSetupAsync(_clientFactory);

        var response = await _chatService.DeclineFunctionAsync(GetUserKey(), functionId);

        PromptResults = response.FinalAnswer;
        PendingFunctions = response.PendingFunctions;
        return Page();
    }
}
