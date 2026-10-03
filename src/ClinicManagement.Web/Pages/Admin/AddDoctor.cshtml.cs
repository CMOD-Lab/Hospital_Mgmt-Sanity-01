using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Page model for adding a new doctor.
/// </summary>
public class AddDoctorModel : PageModel
{
    private readonly DoctorService _doctorService;
    private readonly DepartmentService _departmentService;
    private readonly ILogger<AddDoctorModel> _logger;

    public AddDoctorModel(DoctorService doctorService, DepartmentService departmentService, ILogger<AddDoctorModel> logger)
    {
        _doctorService = doctorService;
        _departmentService = departmentService;
        _logger = logger;
    }

    [BindProperty][Required] public string Name { get; set; } = string.Empty;
    [BindProperty][Required][EmailAddress] public string Email { get; set; } = string.Empty;
    [BindProperty][Required] public string Password { get; set; } = string.Empty;
    [BindProperty][Required] public string Phone { get; set; } = string.Empty;
    [BindProperty][Required] public DateTime BirthDate { get; set; }
    [BindProperty][Required] public string Gender { get; set; } = string.Empty;
    [BindProperty][Required] public int DepartmentId { get; set; }
    [BindProperty][Required] public string Specialization { get; set; } = string.Empty;
    [BindProperty][Required] public string Qualification { get; set; } = string.Empty;
    [BindProperty][Required] public int Experience { get; set; }
    [BindProperty][Required] public decimal Salary { get; set; }
    [BindProperty][Required] public decimal ChargesPerVisit { get; set; }
    [BindProperty][Required] public string Address { get; set; } = string.Empty;

    public List<SelectListItem> DepartmentOptions { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Admin")
            return RedirectToPage("/Account/Login");

        await LoadDepartmentsAsync(cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Admin")
            return RedirectToPage("/Account/Login");

        if (!ModelState.IsValid)
        {
            await LoadDepartmentsAsync(cancellationToken);
            return Page();
        }

        try
        {
            var dto = new DoctorCreateDto
            {
                Name = Name, Email = Email, Password = Password, Phone = Phone,
                BirthDate = BirthDate, Gender = Gender, DepartmentId = DepartmentId,
                Specialization = Specialization, Qualification = Qualification,
                Experience = Experience, Salary = Salary, ChargesPerVisit = ChargesPerVisit,
                Address = Address
            };

            await _doctorService.CreateAsync(dto, cancellationToken);
            return RedirectToPage("/Admin/ManageDoctors");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding doctor");
            ErrorMessage = ex.Message.Contains("already exists") ? "A doctor with this email already exists." : "Error adding doctor.";
            await LoadDepartmentsAsync(cancellationToken);
            return Page();
        }
    }

    private async Task LoadDepartmentsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var departments = await _departmentService.GetAllAsync(cancellationToken);
            DepartmentOptions = departments.Select(d => new SelectListItem(d.DeptName, d.DepartmentId.ToString())).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading departments");
        }
    }
}
