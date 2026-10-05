using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class PatientFeedbackModel : PageModel
{
    private readonly PatientService _patientService;

    public AppointmentDto? PendingFeedback { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }

    public PatientFeedbackModel(PatientService patientService)
    {
        _patientService = patientService;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        PendingFeedback = await _patientService.GetPendingFeedbackAsync(userId.Value, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int appointmentId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        var result = await _patientService.SubmitFeedbackAsync(appointmentId, cancellationToken);
        Success = result;
        Message = result ? "Thank you for your feedback!" : "Failed to submit feedback. Please try again.";

        if (!result)
        {
            PendingFeedback = await _patientService.GetPendingFeedbackAsync(userId.Value, cancellationToken);
        }

        return Page();
    }
}
