using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class DoctorHomeModel : PageModel
{
    private readonly DoctorService _doctorService;

    public DoctorProfileDto? Doctor { get; set; }
    public string DoctorName => Doctor?.Name ?? "Doctor";

    public DoctorHomeModel(DoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        Doctor = await _doctorService.GetDoctorProfileAsync(userId.Value, cancellationToken);
        return Page();
    }
}
