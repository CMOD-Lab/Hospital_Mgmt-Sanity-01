using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

public class ManageStaffModel : PageModel
{
    private readonly StaffService _staffService;
    private readonly ILogger<ManageStaffModel> _logger;

    public ManageStaffModel(StaffService staffService, ILogger<ManageStaffModel> logger)
    {
        _staffService = staffService;
        _logger = logger;
    }

    public IEnumerable<StaffDto> StaffList { get; set; } = Enumerable.Empty<StaffDto>();
    public string? Message { get; set; }
    public bool IsError { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Admin")
            return RedirectToPage("/Account/Login");

        try { StaffList = await _staffService.GetAllAsync(cancellationToken); }
        catch (Exception ex) { _logger.LogError(ex, "Error loading staff"); }
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Admin")
            return RedirectToPage("/Account/Login");

        try
        {
            await _staffService.DeleteAsync(id, cancellationToken);
            Message = "Staff member removed successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff {StaffId}", id);
            Message = "Error removing staff member.";
            IsError = true;
        }

        StaffList = await _staffService.GetAllAsync(cancellationToken);
        return Page();
    }
}
