using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class BillsHistoryModel : PageModel
{
    private readonly PatientService _patientService;

    public IEnumerable<BillHistoryDto> Bills { get; set; } = Enumerable.Empty<BillHistoryDto>();

    public BillsHistoryModel(PatientService patientService)
    {
        _patientService = patientService;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        Bills = await _patientService.GetBillHistoryAsync(userId.Value, cancellationToken);
        return Page();
    }
}
