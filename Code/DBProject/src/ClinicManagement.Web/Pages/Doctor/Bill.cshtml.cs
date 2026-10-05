using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class BillModel : PageModel
{
    private readonly DoctorService _doctorService;

    public IEnumerable<AppointmentDto> Appointments { get; set; } = Enumerable.Empty<AppointmentDto>();
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }

    public BillModel(DoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        Appointments = await _doctorService.GetTodaysAppointmentsAsync(userId.Value, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostPaidAsync(int appointmentId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        var result = await _doctorService.MarkAppointmentPaidAsync(userId.Value, appointmentId, cancellationToken);
        Success = result;
        Message = result ? "Appointment marked as paid." : "Failed to update appointment.";

        Appointments = await _doctorService.GetTodaysAppointmentsAsync(userId.Value, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostUnpaidAsync(int appointmentId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        var result = await _doctorService.MarkAppointmentUnpaidAsync(userId.Value, appointmentId, cancellationToken);
        Success = result;
        Message = result ? "Appointment marked as unpaid." : "Failed to update appointment.";

        Appointments = await _doctorService.GetTodaysAppointmentsAsync(userId.Value, cancellationToken);
        return Page();
    }
}
