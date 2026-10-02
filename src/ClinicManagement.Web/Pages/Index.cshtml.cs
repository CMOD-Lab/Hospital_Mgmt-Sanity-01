using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages;

/// <summary>Login and signup page model.</summary>
public class IndexModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly ILogger<IndexModel> _logger;

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public IndexModel(IAuthService authService, ILogger<IndexModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public void OnGet()
    {
        // Clear session on landing page
        HttpContext.Session.Clear();
    }

    /// <summary>Handles login form submission.</summary>
    public async Task<IActionResult> OnPostLoginAsync(
        string loginEmail,
        string loginPassword,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(loginEmail) || string.IsNullOrWhiteSpace(loginPassword))
        {
            ErrorMessage = "Please enter email and password.";
            return Page();
        }

        var result = await _authService.LoginAsync(loginEmail, loginPassword, ct);

        if (!result.Success)
        {
            ErrorMessage = result.Message;
            return Page();
        }

        HttpContext.Session.SetInt32("UserId", result.UserId);
        HttpContext.Session.SetInt32("UserType", (int)result.UserType);

        _logger.LogInformation("User {UserId} logged in as {UserType}", result.UserId, result.UserType);

        return result.UserType switch
        {
            UserType.Patient => RedirectToPage("/Patient/Home"),
            UserType.Doctor => RedirectToPage("/Doctor/Home"),
            UserType.Admin => RedirectToPage("/Admin/Home"),
            _ => Page()
        };
    }

    /// <summary>Handles patient signup form submission.</summary>
    public async Task<IActionResult> OnPostSignupAsync(
        string sName,
        string sBirthDate,
        string sEmail,
        string sPassword,
        string sPhone,
        string sGender,
        string sAddress,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(sName) || string.IsNullOrWhiteSpace(sEmail) || string.IsNullOrWhiteSpace(sPassword))
        {
            ErrorMessage = "Please fill in all required fields.";
            return Page();
        }

        var result = await _authService.SignupPatientAsync(sName, sBirthDate, sEmail, sPassword, sPhone, sGender, sAddress, ct);

        if (!result.Success)
        {
            ErrorMessage = result.Message;
            return Page();
        }

        HttpContext.Session.SetInt32("UserId", result.UserId);
        HttpContext.Session.SetInt32("UserType", (int)UserType.Patient);

        _logger.LogInformation("New patient registered with ID {UserId}", result.UserId);
        return RedirectToPage("/Patient/Home");
    }
}
