using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

public class AdminHomeModel : PageModel
{
    private readonly AdminService _adminService;

    public AdminHomeDto Stats { get; set; } = new AdminHomeDto();

    public AdminHomeModel(AdminService adminService)
    {
        _adminService = adminService;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userId == null || userType != 3) return RedirectToPage("/SignUp");

        Stats = await _adminService.GetAdminHomeInfoAsync(cancellationToken);
        return Page();
    }
}
