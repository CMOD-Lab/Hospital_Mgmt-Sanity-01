using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class TreatmentHistoryModel : PageModel
{
    private readonly PatientService _patientService;

    public IEnumerable<TreatmentHistoryDto> Treatments { get; set; } = Enumerable.Empty<TreatmentHistoryDto>();

    public TreatmentHistoryModel(PatientService patientService)
    {
        _patientService = patientService;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        Treatments = await _patientService.GetTreatmentHistoryAsync(userId.Value, cancellationToken);
        return Page();
    }
}
