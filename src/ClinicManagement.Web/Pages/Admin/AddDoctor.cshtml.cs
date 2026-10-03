using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using ClinicManagement.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Add doctor page model.
/// </summary>
public class AddDoctorModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly IDepartmentService _departmentService;
    private readonly ILogger<AddDoctorModel> _logger;

    public AddDoctorModel(IDoctorService doctorService, IDepartmentService departmentService, ILogger<AddDoctorModel> logger)
    {
        _doctorService = doctorService;
        _departmentService = departmentService;
        _logger = logger;
    }

    [BindProperty]
    public AddDoctorInputModel Input { get; set; } = new();

    public List<SelectListItem> DepartmentOptions { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public class AddDoctorInputModel
    {
        [Required] [MaxLength(30)] public string Name { get; set; } = string.Empty;
        [Required] [EmailAddress] [MaxLength(30)] public string Email { get; set; } = string.Empty;
        [Required] [MaxLength(30)] public string Password { get; set; } = string.Empty;
        [Required] [MaxLength(15)] public string Phone { get; set; } = string.Empty;
        [Required] [MaxLength(40)] public string Address { get; set; } = string.Empty;
        [Required] public DateTime BirthDate { get; set; }
        [Required] [MaxLength(1)] public string Gender { get; set; } = string.Empty;
        [Required] [Range(1, int.MaxValue)] public int DepartmentId { get; set; }
        [Required] [MaxLength(50)] public string Specialization { get; set; } = string.Empty;
        [Required] [MaxLength(100)] public string Qualification { get; set; } = string.Empty;
        [Required] [Range(0, 50)] public int Experience { get; set; }
        [Required] [Range(0, double.MaxValue)] public decimal Salary { get; set; }
        [Required] [Range(0, double.MaxValue)] public decimal ChargesPerVisit { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 0)
            return RedirectToPage("/Account/Login");

        await LoadDepartmentsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 0)
            return RedirectToPage("/Account/Login");

        if (!ModelState.IsValid)
        {
            await LoadDepartmentsAsync();
            return Page();
        }

        try
        {
            // Manually map ViewModel to DTO
            var createDto = new DoctorCreateDto
            {
                Name = Input.Name,
                Email = Input.Email,
                Password = Input.Password,
                Phone = Input.Phone,
                Address = Input.Address,
                BirthDate = Input.BirthDate,
                Gender = Input.Gender,
                DepartmentId = Input.DepartmentId,
                Specialization = Input.Specialization,
                Qualification = Input.Qualification,
                Experience = Input.Experience,
                Salary = Input.Salary,
                ChargesPerVisit = Input.ChargesPerVisit
            };

            await _doctorService.CreateAsync(createDto);
            return RedirectToPage("/Admin/ManageDoctors");
        }
        catch (DuplicateEntityException)
        {
            ErrorMessage = "A doctor with this email already exists.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding doctor");
            ErrorMessage = "An error occurred. Please try again.";
        }

        await LoadDepartmentsAsync();
        return Page();
    }

    private async Task LoadDepartmentsAsync()
    {
        var departments = await _departmentService.GetAllAsync();
        DepartmentOptions = departments.Select(d => new SelectListItem
        {
            Value = d.DepartmentId.ToString(),
            Text = d.DeptName
        }).ToList();
    }
}
