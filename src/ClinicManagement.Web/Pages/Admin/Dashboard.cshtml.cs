using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Admin dashboard page model.
/// </summary>
public class DashboardModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<DashboardModel> _logger;

    public DashboardModel(IAdminService adminService, ILogger<DashboardModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public AdminDashboardDto? Dashboard { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 0)
            return RedirectToPage("/Account/Login");

        try
        {
            Dashboard = await _adminService.GetDashboardDataAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard");
        }

        return Page();
    }
}
