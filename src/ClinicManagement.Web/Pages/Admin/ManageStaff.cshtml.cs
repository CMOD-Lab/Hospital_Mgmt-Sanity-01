using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Manage staff page model.
/// </summary>
public class ManageStaffModel : PageModel
{
    private readonly IStaffService _staffService;
    private readonly ILogger<ManageStaffModel> _logger;

    public ManageStaffModel(IStaffService staffService, ILogger<ManageStaffModel> logger)
    {
        _staffService = staffService;
        _logger = logger;
    }

    public IEnumerable<StaffDto> StaffList { get; set; } = new List<StaffDto>();
    public string? SearchQuery { get; set; }

    public async Task<IActionResult> OnGetAsync(string? search = null)
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 0)
            return RedirectToPage("/Account/Login");

        SearchQuery = search;
        try
        {
            StaffList = string.IsNullOrEmpty(search)
                ? await _staffService.GetAllAsync()
                : await _staffService.SearchAsync(search);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading staff");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 0)
            return RedirectToPage("/Account/Login");

        try
        {
            await _staffService.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff {StaffId}", id);
        }

        StaffList = await _staffService.GetAllAsync();
        return Page();
    }
}
