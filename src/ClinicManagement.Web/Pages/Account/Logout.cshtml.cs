using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Account;

/// <summary>
/// Page model for logout.
/// </summary>
public class LogoutModel : PageModel
{
    private readonly ILogger<LogoutModel> _logger;

    public LogoutModel(ILogger<LogoutModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnPost()
    {
        _logger.LogInformation("User {UserId} logged out", HttpContext.Session.GetInt32("UserId"));
        HttpContext.Session.Clear();
        return RedirectToPage("/Account/Login");
    }
}
