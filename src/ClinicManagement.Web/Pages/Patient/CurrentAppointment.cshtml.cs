using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class CurrentAppointmentModel : PageModel
{
    private readonly IPatientService _patientService;
    public CurrentAppointmentData? CurrentAppointment { get; private set; }

    public CurrentAppointmentModel(IPatientService patientService) => _patientService = patientService;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");
        CurrentAppointment = await _patientService.GetCurrentAppointmentAsync(userId.Value, ct);
        return Page();
    }
}
