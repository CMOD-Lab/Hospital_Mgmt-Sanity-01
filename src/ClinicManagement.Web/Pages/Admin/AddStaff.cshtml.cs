using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

public class AddStaffModel : PageModel
{
    private readonly IAdminService _adminService;
    public string? Message { get; private set; }
    public bool IsSuccess { get; private set; }

    public AddStaffModel(IAdminService adminService) => _adminService = adminService;

    public IActionResult OnGet()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string name, string birthDate, string phone, string gender,
        string address, int salary, string qualification, string designation,
        CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");

        var request = new AddStaffRequest(
            name, birthDate, phone,
            string.IsNullOrEmpty(gender) ? 'M' : gender[0],
            address, salary, qualification, designation);

        IsSuccess = await _adminService.AddStaffAsync(request, ct);
        Message = IsSuccess ? "Staff member added successfully!" : "Failed to add staff member.";
        return Page();
    }
}
