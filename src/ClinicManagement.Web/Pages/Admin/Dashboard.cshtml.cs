using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Page model for the admin dashboard.
/// </summary>
public class DashboardModel : PageModel
{
    private readonly AdminService _adminService;
    private readonly ILogger<DashboardModel> _logger;

    public DashboardModel(AdminService adminService, ILogger<DashboardModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public AdminDashboardDto? Dashboard { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Admin")
        {
            return RedirectToPage("/Account/Login");
        }

        try
        {
            Dashboard = await _adminService.GetDashboardAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard");
        }

        return Page();
    }
}
