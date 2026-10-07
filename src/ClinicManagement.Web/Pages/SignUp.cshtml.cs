using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.Pages;

/// <summary>Page model for the Sign Up / Login page (migrated from SignUp.aspx).</summary>
public class SignUpModel : PageModel
{
    private readonly IAuthService _authService;
    private readonly ILogger<SignUpModel> _logger;

    public SignUpModel(IAuthService authService, ILogger<SignUpModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [BindProperty]
    public LoginInputModel LoginInput { get; set; } = new();

    [BindProperty]
    public SignUpInputModel SignUpInput { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public void OnGet()
    {
        // Clear session on page load
        HttpContext.Session.Clear();
    }

    /// <summary>Handles login form submission.</summary>
    public async Task<IActionResult> OnPostLoginAsync()
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please fill in all required fields.";
            return Page();
        }

        try
        {
            var result = await _authService.ValidateLoginAsync(LoginInput.Email, LoginInput.Password);

            if (result.Status == 0)
            {
                // Successful login - store user ID in session
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

            ErrorMessage = result.ErrorMessage ?? "Login failed. Please try again.";
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", LoginInput.Email);
            ErrorMessage = "There was some error. Try Again!";
            return Page();
        }
    }

    /// <summary>Handles sign-up form submission.</summary>
    public async Task<IActionResult> OnPostSignUpAsync()
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please fill in all required fields correctly.";
            return Page();
        }

        try
        {
            var dto = new PatientSignUpDto
            {
                Name = SignUpInput.Name,
                BirthDate = SignUpInput.BirthDate,
                Email = SignUpInput.Email,
                Password = SignUpInput.Password,
                PhoneNo = SignUpInput.PhoneNo,
                Gender = SignUpInput.Gender,
                Address = SignUpInput.Address
            };

            var result = await _authService.SignUpPatientAsync(dto);

            if (result.Status == 1)
            {
                HttpContext.Session.SetInt32("UserId", result.PatientId);
                HttpContext.Session.SetInt32("UserType", 1);
                return RedirectToPage("/Patient/PatientHome");
            }

            ErrorMessage = result.ErrorMessage ?? "Registration failed. Please try again.";
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during sign-up for email: {Email}", SignUpInput.Email);
            ErrorMessage = "There was some error. Try again!";
            return Page();
        }
    }
}

/// <summary>View model for login form.</summary>
public class LoginInputModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

/// <summary>View model for sign-up form.</summary>
public class SignUpInputModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(30, ErrorMessage = "Name cannot exceed 30 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Birth date is required")]
    public string BirthDate { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
    public string PhoneNo { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required")]
    public string Gender { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [StringLength(40, ErrorMessage = "Address cannot exceed 40 characters")]
    public string Address { get; set; } = string.Empty;
}
