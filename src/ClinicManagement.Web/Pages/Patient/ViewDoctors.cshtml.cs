using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>
/// View doctors page model for patients.
/// </summary>
public class ViewDoctorsModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly IDepartmentService _departmentService;
    private readonly ILogger<ViewDoctorsModel> _logger;

    public ViewDoctorsModel(IDoctorService doctorService, IDepartmentService departmentService, ILogger<ViewDoctorsModel> logger)
    {
        _doctorService = doctorService;
        _departmentService = departmentService;
        _logger = logger;
    }

    public IEnumerable<DoctorDto> Doctors { get; set; } = new List<DoctorDto>();
    public List<SelectListItem> DepartmentOptions { get; set; } = new();
    public string? SelectedDept { get; set; }

    public async Task<IActionResult> OnGetAsync(string? dept = null)
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 2)
            return RedirectToPage("/Account/Login");

        SelectedDept = dept;
        try
        {
            var departments = await _departmentService.GetAllAsync();
            DepartmentOptions = departments.Select(d => new SelectListItem
            {
                Value = d.DeptName,
                Text = d.DeptName,
                Selected = d.DeptName == dept
            }).ToList();

            if (!string.IsNullOrEmpty(dept))
            {
                Doctors = await _doctorService.GetByDepartmentAsync(dept);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctors by department");
        }

        return Page();
    }
}
