using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>
/// Current appointment page model for patients.
/// </summary>
public class CurrentAppointmentModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<CurrentAppointmentModel> _logger;

    public CurrentAppointmentModel(IAppointmentService appointmentService, ILogger<CurrentAppointmentModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public AppointmentDto? CurrentAppointment { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 2)
            return RedirectToPage("/Account/Login");

        var patientId = HttpContext.Session.GetInt32("UserId")!.Value;
        try
        {
            CurrentAppointment = await _appointmentService.GetCurrentByPatientIdAsync(patientId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading current appointment for patient {PatientId}", patientId);
        }

        return Page();
    }
}
