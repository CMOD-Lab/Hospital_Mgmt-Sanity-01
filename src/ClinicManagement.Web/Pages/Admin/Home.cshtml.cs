using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

public class HomeModel : PageModel
{
    private readonly IAdminService _adminService;
    public AdminDashboardData? Dashboard { get; private set; }

    public HomeModel(IAdminService adminService) => _adminService = adminService;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");
        Dashboard = await _adminService.GetDashboardDataAsync(ct);
        return Page();
    }
}
