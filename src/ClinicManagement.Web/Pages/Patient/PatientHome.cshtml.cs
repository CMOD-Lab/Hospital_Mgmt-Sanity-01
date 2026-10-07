using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Page model for Patient Home (migrated from PatientHome.aspx).</summary>
public class PatientHomeModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientHomeModel> _logger;

    public PatientHomeModel(IPatientService patientService, ILogger<PatientHomeModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public PatientInfoDto? PatientInfo { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 1)
            return RedirectToPage("/SignUp");

        try
        {
            PatientInfo = await _patientService.GetPatientInfoAsync(userId.Value);

            if (PatientInfo == null)
            {
                ErrorMessage = "There was some error in retrieving the Patient's Info.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patient home for user ID: {UserId}", userId);
            ErrorMessage = "There was some error loading your information.";
        }

        return Page();
    }
}
