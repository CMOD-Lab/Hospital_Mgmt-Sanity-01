using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class DoctorProfileModel : PageModel
{
    private readonly DoctorService _doctorService;

    public DoctorProfileDto? Doctor { get; set; }

    public DoctorProfileModel(DoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    public async Task<IActionResult> OnGetAsync(int doctorId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        Doctor = await _doctorService.GetDoctorProfileAsync(doctorId, cancellationToken);
        return Page();
    }
}
