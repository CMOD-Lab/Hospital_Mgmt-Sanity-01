using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class TreatmentHistoryModel : PageModel
{
    private readonly IPatientService _patientService;
    public IEnumerable<TreatmentHistoryItem> History { get; private set; } = Enumerable.Empty<TreatmentHistoryItem>();

    public TreatmentHistoryModel(IPatientService patientService) => _patientService = patientService;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");
        History = await _patientService.GetTreatmentHistoryAsync(userId.Value, ct);
        return Page();
    }
}
