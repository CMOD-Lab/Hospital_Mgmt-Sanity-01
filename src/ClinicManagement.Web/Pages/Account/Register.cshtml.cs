using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.Pages.Account;

/// <summary>
/// Page model for patient registration.
/// </summary>
public class RegisterModel : PageModel
{
    private readonly PatientService _patientService;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(PatientService patientService, ILogger<RegisterModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    [BindProperty]
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Phone is required")]
    public string Phone { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Date of birth is required")]
    public DateTime BirthDate { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Gender is required")]
    public string Gender { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            _logger.LogInformation("Registering new patient with email {Email}", Email);

            var dto = new PatientCreateDto
            {
                Name = Name,
                Email = Email,
                Password = Password,
                Phone = Phone,
                BirthDate = BirthDate,
                Gender = Gender,
                Address = Address
            };

            var patient = await _patientService.CreateAsync(dto, cancellationToken);
            _logger.LogInformation("Patient registered successfully with ID {PatientId}", patient.PatientId);

            SuccessMessage = "Registration successful! You can now login.";
            return Page();
        }
        catch (Exception ex) when (ex.Message.Contains("already exists"))
        {
            ErrorMessage = "An account with this email already exists.";
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering patient with email {Email}", Email);
            ErrorMessage = "An error occurred during registration. Please try again.";
            return Page();
        }
    }
}
