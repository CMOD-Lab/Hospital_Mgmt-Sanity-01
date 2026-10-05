using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages;

/// <summary>
/// Page model for the Login/SignUp page.
/// </summary>
public class SignUpModel : PageModel
{
    private readonly AuthService _authService;
    private readonly ILogger<SignUpModel> _logger;

    public string LoginMessage { get; set; } = string.Empty;
    public string SignupMessage { get; set; } = string.Empty;
    public bool SignupSuccess { get; set; }

    public SignUpModel(AuthService authService, ILogger<SignUpModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public void OnGet()
    {
        // Clear session on page load (logout behavior)
        HttpContext.Session.Remove("UserId");
        HttpContext.Session.Remove("UserType");
    }

    /// <summary>
    /// Handles login form submission.
    /// </summary>
    public async Task<IActionResult> OnPostLoginAsync(
        string loginEmail,
        string loginPassword,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(loginEmail) || string.IsNullOrWhiteSpace(loginPassword))
        {
            LoginMessage = "Please enter email and password.";
            return Page();
        }

        var result = await _authService.ValidateLoginAsync(new LoginRequestDto
        {
            Email = loginEmail,
            Password = loginPassword
        }, cancellationToken);

        if (result.Success)
        {
            HttpContext.Session.SetInt32("UserId", result.UserId);
            HttpContext.Session.SetInt32("UserType", result.UserType);

            return result.UserType switch
            {
                1 => RedirectToPage("/Patient/PatientHome"),
                2 => RedirectToPage("/Doctor/DoctorHome"),
                3 => RedirectToPage("/Admin/AdminHome"),
                _ => Page()
            };
        }

        LoginMessage = result.Message;
        return Page();
    }

    /// <summary>
    /// Handles patient signup form submission.
    /// </summary>
    public async Task<IActionResult> OnPostSignupAsync(
        string sName,
        string sBirthDate,
        string sEmail,
        string sPassword,
        string Phone,
        string Gender,
        string Address,
        CancellationToken cancellationToken)
    {
        var dto = new PatientSignupDto
        {
            Name = sName,
            BirthDate = sBirthDate,
            Email = sEmail,
            Password = sPassword,
            PhoneNo = Phone,
            Gender = Gender,
            Address = Address
        };

        var result = await _authService.RegisterPatientAsync(dto, cancellationToken);

        if (result.Success)
        {
            HttpContext.Session.SetInt32("UserId", result.UserId);
            HttpContext.Session.SetInt32("UserType", result.UserType);
            return RedirectToPage("/Patient/PatientHome");
        }

        SignupMessage = result.Message;
        SignupSuccess = false;
        return Page();
    }
}
