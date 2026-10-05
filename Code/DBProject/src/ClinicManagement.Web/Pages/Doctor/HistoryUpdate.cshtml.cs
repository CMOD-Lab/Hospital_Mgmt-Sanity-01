using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class HistoryUpdateModel : PageModel
{
    private readonly DoctorService _doctorService;

    public int AppointmentId { get; set; }
    public string Disease { get; set; } = string.Empty;
    public string Progress { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }

    public HistoryUpdateModel(DoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    public IActionResult OnGet(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        AppointmentId = appointmentId;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        int appointmentId,
        string disease,
        string progress,
        string prescription,
        CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        AppointmentId = appointmentId;
        Disease = disease;
        Progress = progress;
        Prescription = prescription;

        var result = await _doctorService.UpdatePrescriptionAsync(new UpdatePrescriptionDto
        {
            DoctorId = userId.Value,
            AppointmentId = appointmentId,
            Disease = disease,
            Progress = progress,
            Prescription = prescription
        }, cancellationToken);

        Success = result;
        Message = result ? "History updated successfully." : "Failed to update history.";
        return Page();
    }
}
