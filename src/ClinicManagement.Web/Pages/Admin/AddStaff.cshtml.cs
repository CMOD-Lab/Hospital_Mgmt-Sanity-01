using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.Pages.Admin;

public class AddStaffModel : PageModel
{
    private readonly StaffService _staffService;
    private readonly ILogger<AddStaffModel> _logger;

    public AddStaffModel(StaffService staffService, ILogger<AddStaffModel> logger)
    {
        _staffService = staffService;
        _logger = logger;
    }

    [BindProperty][Required] public string Name { get; set; } = string.Empty;
    [BindProperty][Required] public string Phone { get; set; } = string.Empty;
    [BindProperty][Required] public DateTime BirthDate { get; set; }
    [BindProperty][Required] public string Gender { get; set; } = string.Empty;
    [BindProperty][Required] public decimal Salary { get; set; }
    [BindProperty][Required] public string Designation { get; set; } = string.Empty;
    [BindProperty][Required] public string Qualification { get; set; } = string.Empty;
    [BindProperty][Required] public string Address { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("UserType") != "Admin")
            return RedirectToPage("/Account/Login");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Admin")
            return RedirectToPage("/Account/Login");

        if (!ModelState.IsValid) return Page();

        try
        {
            var dto = new StaffCreateDto
            {
                Name = Name, Phone = Phone, BirthDate = BirthDate, Gender = Gender,
                Salary = Salary, Designation = Designation, Qualification = Qualification, Address = Address
            };
            await _staffService.CreateAsync(dto, cancellationToken);
            return RedirectToPage("/Admin/ManageStaff");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff");
            ErrorMessage = "Error adding staff member.";
            return Page();
        }
    }
}
