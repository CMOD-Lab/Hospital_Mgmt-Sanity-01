using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class PatientHistoryModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<PatientHistoryModel> _logger;

    public PatientHistoryModel(AppointmentService appointmentService, ILogger<PatientHistoryModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public IEnumerable<AppointmentDto> Appointments { get; set; } = Enumerable.Empty<AppointmentDto>();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Doctor")
            return RedirectToPage("/Account/Login");

        var doctorId = HttpContext.Session.GetInt32("UserId") ?? 0;
        try { Appointments = await _appointmentService.GetByDoctorIdAsync(doctorId, cancellationToken); }
        catch (Exception ex) { _logger.LogError(ex, "Error loading patient history for doctor {DoctorId}", doctorId); }
        return Page();
    }
}
