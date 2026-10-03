using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.Pages.Account;

/// <summary>
/// Page model for the login page.
/// </summary>
public class LoginModel : PageModel
{
    private readonly PatientService _patientService;
    private readonly DoctorService _doctorService;
    private readonly AdminService _adminService;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(
        PatientService patientService,
        DoctorService doctorService,
        AdminService adminService,
        ILogger<LoginModel> logger)
    {
        _patientService = patientService;
        _doctorService = doctorService;
        _adminService = adminService;
        _logger = logger;
    }

    [BindProperty]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string UserType { get; set; } = "Patient";

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        // If already logged in, redirect to appropriate home
        var existingUserType = HttpContext.Session.GetString("UserType");
        if (!string.IsNullOrEmpty(existingUserType))
        {
            return RedirectToUserHome(existingUserType);
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            _logger.LogInformation("Login attempt for {Email} as {UserType}", Email, UserType);

            if (UserType == "Admin")
            {
                var adminResult = _adminService.ValidateAdminLogin(Email, Password);
                if (adminResult.Success)
                {
                    SetSession(adminResult.UserId, adminResult.Name, "Admin");
                    return RedirectToPage("/Admin/Dashboard");
                }
                ErrorMessage = adminResult.ErrorMessage;
            }
            else if (UserType == "Doctor")
            {
                var doctorResult = await _doctorService.ValidateLoginAsync(Email, Password, cancellationToken);
                if (doctorResult.Success)
                {
                    SetSession(doctorResult.UserId, doctorResult.Name, "Doctor");
                    return RedirectToPage("/Doctor/Home");
                }
                ErrorMessage = doctorResult.ErrorMessage;
            }
            else
            {
                var patientResult = await _patientService.ValidateLoginAsync(Email, Password, cancellationToken);
                if (patientResult.Success)
                {
                    SetSession(patientResult.UserId, patientResult.Name, "Patient");
                    return RedirectToPage("/Patient/Home");
                }
                ErrorMessage = patientResult.ErrorMessage;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", Email);
            ErrorMessage = "An error occurred during login. Please try again.";
        }

        return Page();
    }

    private void SetSession(int userId, string name, string userType)
    {
        HttpContext.Session.SetInt32("UserId", userId);
        HttpContext.Session.SetString("UserName", name);
        HttpContext.Session.SetString("UserType", userType);
    }

    private IActionResult RedirectToUserHome(string userType) => userType switch
    {
        "Admin" => RedirectToPage("/Admin/Dashboard"),
        "Doctor" => RedirectToPage("/Doctor/Home"),
        _ => RedirectToPage("/Patient/Home")
    };
}
