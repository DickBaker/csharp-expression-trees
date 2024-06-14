using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TitanicExplorer.Pages;

public class PrivacyModel(ILogger<PrivacyModel> logger) : PageModel
{
    public ILogger<PrivacyModel> Logger => logger;

    public void OnGet()
    { }
}