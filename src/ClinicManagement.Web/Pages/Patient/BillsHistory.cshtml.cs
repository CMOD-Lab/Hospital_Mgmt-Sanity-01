using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class BillsHistoryModel : PageModel
{
    private readonly IPatientService _patientService;
    public IEnumerable<BillHistoryItem> Bills { get; private set; } = Enumerable.Empty<BillHistoryItem>();

    public BillsHistoryModel(IPatientService patientService) => _patientService = patientService;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");
        Bills = await _patientService.GetBillHistoryAsync(userId.Value, ct);
        return Page();
    }
}
