using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.Pages.Account;

/// <summary>
/// Login page model.
/// </summary>
public class LoginModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(IPatientService patientService, ILogger<LoginModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    [BindProperty]
    public LoginInputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class LoginInputModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetInt32("UserId") != null)
        {
            return RedirectToUserHome();
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var result = await _patientService.ValidateLoginAsync(Input.Email, Input.Password);
            if (!result.Success)
            {
                ErrorMessage = "Invalid email or password. Please try again.";
                return Page();
            }

            HttpContext.Session.SetInt32("UserId", result.UserId);
            HttpContext.Session.SetInt32("UserType", result.UserType);
            HttpContext.Session.SetString("UserEmail", Input.Email);

            _logger.LogInformation("User {Email} logged in successfully as type {UserType}", Input.Email, result.UserType);

            return RedirectToUserHome(result.UserType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {Email}", Input.Email);
            ErrorMessage = "An error occurred during login. Please try again.";
            return Page();
        }
    }

    private IActionResult RedirectToUserHome(int? userType = null)
    {
        var type = userType ?? HttpContext.Session.GetInt32("UserType");
        return type switch
        {
            0 => RedirectToPage("/Admin/Dashboard"),
            1 => RedirectToPage("/Doctor/Home"),
            2 => RedirectToPage("/Patient/Home"),
            _ => RedirectToPage("/Account/Login")
        };
    }
}
