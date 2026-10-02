using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class HomeModel : PageModel
{
    private readonly IDoctorService _doctorService;
    public DoctorDashboardData? Dashboard { get; private set; }

    public HomeModel(IDoctorService doctorService) => _doctorService = doctorService;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");
        Dashboard = await _doctorService.GetDoctorDashboardAsync(userId.Value, ct);
        return Page();
    }
}
