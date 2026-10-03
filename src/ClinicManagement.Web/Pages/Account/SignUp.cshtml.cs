using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.Pages.Account;

/// <summary>
/// Patient sign-up page model.
/// </summary>
public class SignUpModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<SignUpModel> _logger;

    public SignUpModel(IPatientService patientService, ILogger<SignUpModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    [BindProperty]
    public SignUpInputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public class SignUpInputModel
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [MaxLength(30)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MaxLength(20)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required")]
        [MaxLength(15)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(40)]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [MaxLength(1)]
        public string Gender { get; set; } = string.Empty;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        try
        {
            // Manually map ViewModel to DTO (no AutoMapper in Web layer)
            var createDto = new PatientCreateDto
            {
                Name = Input.Name,
                Email = Input.Email,
                Password = Input.Password,
                Phone = Input.Phone,
                Address = Input.Address,
                BirthDate = Input.BirthDate,
                Gender = Input.Gender
            };

            var patient = await _patientService.CreateAsync(createDto);
            _logger.LogInformation("New patient registered: {Email}", Input.Email);

            // Auto-login after registration
            HttpContext.Session.SetInt32("UserId", patient.PatientId);
            HttpContext.Session.SetInt32("UserType", 2);
            HttpContext.Session.SetString("UserEmail", patient.Email);

            return RedirectToPage("/Patient/Home");
        }
        catch (DuplicateEntityException)
        {
            ErrorMessage = "An account with this email already exists.";
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during patient registration for {Email}", Input.Email);
            ErrorMessage = "An error occurred during registration. Please try again.";
            return Page();
        }
    }
}
