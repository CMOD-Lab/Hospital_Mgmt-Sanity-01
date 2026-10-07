using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Page model for Add Staff page (migrated from AddStaff.aspx).</summary>
public class AddStaffModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AddStaffModel> _logger;

    public AddStaffModel(IAdminService adminService, ILogger<AddStaffModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    [BindProperty]
    public AddStaffViewModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public IActionResult OnGet()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 3)
            return RedirectToPage("/SignUp");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 3)
            return RedirectToPage("/SignUp");

        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please correct the validation errors.";
            return Page();
        }

        try
        {
            var dto = new AddStaffDto
            {
                Name = Input.Name,
                BirthDate = Input.BirthDate,
                Phone = Input.Phone,
                Gender = string.IsNullOrEmpty(Input.Gender) ? 'M' : Input.Gender[0],
                Address = Input.Address,
                Salary = Input.Salary,
                Qualification = Input.Qualification,
                Designation = Input.Designation
            };

            var success = await _adminService.AddStaffAsync(dto);

            if (success)
            {
                SuccessMessage = "Staff member added successfully!";
                Input = new AddStaffViewModel();
            }
            else
            {
                ErrorMessage = "Failed to add staff member. Please try again.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff member: {Name}", Input.Name);
            ErrorMessage = "There was some error. Please try again.";
        }

        return Page();
    }
}

/// <summary>View model for add staff form.</summary>
public class AddStaffViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(30)]
    public string Name { get; set; } = string.Empty;

    public string BirthDate { get; set; } = string.Empty;

    [StringLength(11)]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required")]
    public string Gender { get; set; } = string.Empty;

    [StringLength(50)]
    public string Address { get; set; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int Salary { get; set; }

    [StringLength(50)]
    public string Qualification { get; set; } = string.Empty;

    [Required(ErrorMessage = "Designation is required")]
    [StringLength(15)]
    public string Designation { get; set; } = string.Empty;
}
