using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class DoctorProfileModel : PageModel
{
    private readonly IPatientService _patientService;
    public DoctorProfileData? Profile { get; private set; }

    public DoctorProfileModel(IPatientService patientService) => _patientService = patientService;

    public async Task<IActionResult> OnGetAsync(int doctorId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");

        Profile = await _patientService.GetDoctorProfileAsync(doctorId, ct);
        return Page();
    }
}
