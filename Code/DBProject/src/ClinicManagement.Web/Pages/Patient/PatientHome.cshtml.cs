using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>
/// Page model for the Patient Home page.
/// </summary>
public class PatientHomeModel : PageModel
{
    private readonly PatientService _patientService;
    private readonly ILogger<PatientHomeModel> _logger;

    public PatientDto? Patient { get; set; }
    public string PatientName => Patient?.Name ?? "Patient";

    public PatientHomeModel(PatientService patientService, ILogger<PatientHomeModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        Patient = await _patientService.GetPatientInfoAsync(userId.Value, cancellationToken);
        return Page();
    }
}
