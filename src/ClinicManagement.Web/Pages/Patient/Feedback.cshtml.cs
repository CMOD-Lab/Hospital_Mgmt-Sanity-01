using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class FeedbackModel : PageModel
{
    private readonly IPatientService _patientService;
    public PendingFeedbackData? PendingFeedback { get; private set; }
    public string? Message { get; private set; }
    public bool IsSuccess { get; private set; }

    public FeedbackModel(IPatientService patientService) => _patientService = patientService;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");
        PendingFeedback = await _patientService.GetPendingFeedbackAsync(userId.Value, ct);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int appointId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");

        var success = await _patientService.SubmitFeedbackAsync(appointId, ct);
        IsSuccess = success;
        Message = success ? "Feedback submitted successfully!" : "Failed to submit feedback.";

        if (!success)
            PendingFeedback = await _patientService.GetPendingFeedbackAsync(userId.Value, ct);

        return Page();
    }
}
