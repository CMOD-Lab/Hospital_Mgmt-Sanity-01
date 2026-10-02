using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class PatientHistoryModel : PageModel
{
    private readonly IDoctorService _doctorService;
    public IEnumerable<TodayAppointmentItem> TodayAppointments { get; private set; } = Enumerable.Empty<TodayAppointmentItem>();
    public string? Message { get; private set; }
    public bool IsSuccess { get; private set; }

    public PatientHistoryModel(IDoctorService doctorService) => _doctorService = doctorService;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");
        TodayAppointments = await _doctorService.GetTodayAppointmentsAsync(userId.Value, ct);
        return Page();
    }

    public async Task<IActionResult> OnPostUpdatePrescriptionAsync(int appointId, string disease, string progress, string prescription, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");

        IsSuccess = await _doctorService.UpdatePrescriptionAsync(userId.Value, appointId, disease, progress, prescription, ct);
        Message = IsSuccess ? "Prescription updated successfully." : "Failed to update prescription.";
        TodayAppointments = await _doctorService.GetTodayAppointmentsAsync(userId.Value, ct);
        return Page();
    }
}
