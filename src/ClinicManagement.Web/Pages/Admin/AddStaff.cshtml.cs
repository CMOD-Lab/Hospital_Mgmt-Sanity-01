using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Add staff page model.
/// </summary>
public class AddStaffModel : PageModel
{
    private readonly IStaffService _staffService;
    private readonly ILogger<AddStaffModel> _logger;

    public AddStaffModel(IStaffService staffService, ILogger<AddStaffModel> logger)
    {
        _staffService = staffService;
        _logger = logger;
    }

    [BindProperty]
    public AddStaffInputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public class AddStaffInputModel
    {
        [Required] [MaxLength(30)] public string Name { get; set; } = string.Empty;
        [Required] [MaxLength(15)] public string Phone { get; set; } = string.Empty;
        [Required] [MaxLength(50)] public string Address { get; set; } = string.Empty;
        [Required] public DateTime BirthDate { get; set; }
        [Required] [MaxLength(1)] public string Gender { get; set; } = string.Empty;
        [Required] [MaxLength(30)] public string Designation { get; set; } = string.Empty;
        [Required] [MaxLength(100)] public string Qualification { get; set; } = string.Empty;
        [Required] [Range(0, double.MaxValue)] public decimal Salary { get; set; }
    }

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 0)
            return RedirectToPage("/Account/Login");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 0)
            return RedirectToPage("/Account/Login");

        if (!ModelState.IsValid)
            return Page();

        try
        {
            // Manually map ViewModel to DTO
            var createDto = new StaffCreateDto
            {
                Name = Input.Name,
                Phone = Input.Phone,
                Address = Input.Address,
                BirthDate = Input.BirthDate,
                Gender = Input.Gender,
                Designation = Input.Designation,
                Qualification = Input.Qualification,
                Salary = Input.Salary
            };

            await _staffService.CreateAsync(createDto);
            return RedirectToPage("/Admin/ManageStaff");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff member");
            ErrorMessage = "An error occurred. Please try again.";
            return Page();
        }
    }
}
