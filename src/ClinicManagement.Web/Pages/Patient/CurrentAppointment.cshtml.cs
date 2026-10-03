using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class CurrentAppointmentModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<CurrentAppointmentModel> _logger;

    public CurrentAppointmentModel(AppointmentService appointmentService, ILogger<CurrentAppointmentModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public AppointmentDto? CurrentAppointment { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Patient")
            return RedirectToPage("/Account/Login");

        var patientId = HttpContext.Session.GetInt32("UserId") ?? 0;
        try { CurrentAppointment = await _appointmentService.GetCurrentByPatientIdAsync(patientId, cancellationToken); }
        catch (Exception ex) { _logger.LogError(ex, "Error loading current appointment for patient {PatientId}", patientId); }
        return Page();
    }
}
