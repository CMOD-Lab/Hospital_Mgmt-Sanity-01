using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>
/// Treatment history page model for patients.
/// </summary>
public class TreatmentHistoryModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<TreatmentHistoryModel> _logger;

    public TreatmentHistoryModel(IAppointmentService appointmentService, ILogger<TreatmentHistoryModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public IEnumerable<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 2)
            return RedirectToPage("/Account/Login");

        var patientId = HttpContext.Session.GetInt32("UserId")!.Value;
        try
        {
            Appointments = await _appointmentService.GetByPatientIdAsync(patientId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading treatment history for patient {PatientId}", patientId);
        }

        return Page();
    }
}
