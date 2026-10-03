using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class HomeModel : PageModel
{
    private readonly PatientService _patientService;
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<HomeModel> _logger;

    public HomeModel(PatientService patientService, AppointmentService appointmentService, ILogger<HomeModel> logger)
    {
        _patientService = patientService;
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public PatientDto? PatientInfo { get; set; }
    public AppointmentDto? CurrentAppointment { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Patient")
            return RedirectToPage("/Account/Login");

        var patientId = HttpContext.Session.GetInt32("UserId") ?? 0;

        try
        {
            PatientInfo = await _patientService.GetByIdAsync(patientId, cancellationToken);
            CurrentAppointment = await _appointmentService.GetCurrentByPatientIdAsync(patientId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patient home for patient {PatientId}", patientId);
        }

        return Page();
    }
}
