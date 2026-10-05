using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class PendingAppointmentModel : PageModel
{
    private readonly DoctorService _doctorService;

    public IEnumerable<AppointmentDto> Appointments { get; set; } = Enumerable.Empty<AppointmentDto>();
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }

    public PendingAppointmentModel(DoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        Appointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(int appointmentId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        var result = await _doctorService.ApproveAppointmentAsync(appointmentId, cancellationToken);
        Success = result;
        Message = result ? "Appointment approved successfully." : "Failed to approve appointment.";

        Appointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostRejectAsync(int appointmentId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        var result = await _doctorService.DeleteAppointmentAsync(appointmentId, cancellationToken);
        Success = result;
        Message = result ? "Appointment rejected." : "Failed to reject appointment.";

        Appointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value, cancellationToken);
        return Page();
    }
}
