using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Page model for Doctor Registration Form (migrated from DoctorRegistrationForm.aspx).</summary>
public class DoctorRegistrationFormModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<DoctorRegistrationFormModel> _logger;

    public DoctorRegistrationFormModel(IAdminService adminService, ILogger<DoctorRegistrationFormModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    [BindProperty]
    public DoctorRegistrationViewModel Input { get; set; } = new();

    public List<SelectListItem> DepartmentSelectList { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 3)
            return RedirectToPage("/SignUp");

        await LoadDepartmentsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 3)
            return RedirectToPage("/SignUp");

        await LoadDepartmentsAsync();

        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please correct the validation errors.";
            return Page();
        }

        // Check if email already exists
        if (await _adminService.DoctorEmailExistsAsync(Input.Email))
        {
            ModelState.AddModelError("Input.Email", "This email already exists. Please choose a different one.");
            ErrorMessage = "This email already exists. Please choose a different one.";
            return Page();
        }

        if (Input.DeptNo == 0)
        {
            ModelState.AddModelError("Input.DeptNo", "Please select a department.");
            ErrorMessage = "Please select a department.";
            return Page();
        }

        try
        {
            var dto = new AddDoctorDto
            {
                Name = Input.Name,
                Email = Input.Email,
                Password = Input.Password,
                BirthDate = Input.BirthDate,
                DeptNo = Input.DeptNo,
                Phone = Input.Phone,
                Gender = string.IsNullOrEmpty(Input.Gender) ? 'M' : Input.Gender[0],
                Address = Input.Address,
                Experience = Input.Experience,
                Salary = Input.Salary,
                ChargesPerVisit = Input.ChargesPerVisit,
                Specialization = Input.Specialization,
                Qualification = Input.Qualification
            };

            var success = await _adminService.AddDoctorAsync(dto);

            if (success)
            {
                SuccessMessage = "Doctor added successfully!";
                Input = new DoctorRegistrationViewModel(); // Reset form
            }
            else
            {
                ErrorMessage = "Failed to add doctor. Please try again.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering doctor: {Email}", Input.Email);
            ErrorMessage = "There was some error. Please try again.";
        }

        return Page();
    }

    private async Task LoadDepartmentsAsync()
    {
        var departments = await _adminService.GetDepartmentsAsync();
        DepartmentSelectList = departments.Select(d => new SelectListItem
        {
            Value = d.DeptNo.ToString(),
            Text = d.DeptName
        }).ToList();
    }
}

/// <summary>View model for doctor registration form.</summary>
public class DoctorRegistrationViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(30)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(30, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Birth date is required")]
    public string BirthDate { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a department")]
    public int DeptNo { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    public string Gender { get; set; } = string.Empty;

    [StringLength(11)]
    public string Phone { get; set; } = string.Empty;

    [StringLength(40)]
    public string Address { get; set; } = string.Empty;

    [Range(0, 50)]
    public int Experience { get; set; }

    [Range(0, int.MaxValue)]
    public int Salary { get; set; }

    [Required(ErrorMessage = "Charges per visit is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Charges must be greater than 0")]
    public int ChargesPerVisit { get; set; }

    [StringLength(100)]
    public string Specialization { get; set; } = string.Empty;

    [Required(ErrorMessage = "Qualification is required")]
    [StringLength(100)]
    public string Qualification { get; set; } = string.Empty;
}
