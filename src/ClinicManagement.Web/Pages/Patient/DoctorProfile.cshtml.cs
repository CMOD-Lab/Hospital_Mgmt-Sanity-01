using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Page model for Doctor Profile page (migrated from DoctorProfile.aspx).</summary>
public class DoctorProfileModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<DoctorProfileModel> _logger;

    public DoctorProfileModel(IPatientService patientService, ILogger<DoctorProfileModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public DoctorProfileDto? DoctorProfile { get; set; }

    public async Task<IActionResult> OnGetAsync(int doctorId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 1)
            return RedirectToPage("/SignUp");

        try
        {
            DoctorProfile = await _patientService.GetDoctorProfileAsync(doctorId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor profile for ID: {DoctorId}", doctorId);
        }

        return Page();
    }
}
