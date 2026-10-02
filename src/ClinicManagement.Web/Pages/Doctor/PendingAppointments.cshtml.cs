using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class PendingAppointmentsModel : PageModel
{
    private readonly IDoctorService _doctorService;
    public IEnumerable<PendingAppointmentItem> Appointments { get; private set; } = Enumerable.Empty<PendingAppointmentItem>();
    public string? Message { get; private set; }
    public bool IsSuccess { get; private set; }

    public PendingAppointmentsModel(IDoctorService doctorService) => _doctorService = doctorService;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");
        Appointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value, ct);
        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(int appointId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");

        IsSuccess = await _doctorService.ApproveAppointmentAsync(appointId, ct);
        Message = IsSuccess ? "Appointment approved successfully." : "Failed to approve appointment.";
        Appointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value, ct);
        return Page();
    }

    public async Task<IActionResult> OnPostRejectAsync(int appointId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");

        IsSuccess = await _doctorService.RejectAppointmentAsync(appointId, ct);
        Message = IsSuccess ? "Appointment rejected." : "Failed to reject appointment.";
        Appointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value, ct);
        return Page();
    }
}
