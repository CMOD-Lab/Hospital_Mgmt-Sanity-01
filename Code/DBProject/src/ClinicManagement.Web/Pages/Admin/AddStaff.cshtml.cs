using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

public class AddStaffModel : PageModel
{
    private readonly AdminService _adminService;

    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }

    public AddStaffModel(AdminService adminService)
    {
        _adminService = adminService;
    }

    public IActionResult OnGet()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string name, string birthDate, string phone, string gender,
        string address, int salary, string qualification, string designation,
        CancellationToken cancellationToken)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        var dto = new AddStaffDto
        {
            Name = name,
            BirthDate = birthDate,
            Phone = phone,
            Gender = string.IsNullOrEmpty(gender) ? 'M' : gender[0],
            Address = address,
            Salary = salary,
            Qualification = qualification,
            Designation = designation
        };

        var result = await _adminService.AddStaffAsync(dto, cancellationToken);
        Success = result;
        Message = result ? "Staff member added successfully!" : "Failed to add staff member.";
        return Page();
    }
}
