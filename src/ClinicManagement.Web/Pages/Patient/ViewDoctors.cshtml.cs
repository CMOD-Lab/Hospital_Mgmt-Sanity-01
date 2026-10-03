using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class ViewDoctorsModel : PageModel
{
    private readonly DoctorService _doctorService;
    private readonly DepartmentService _departmentService;
    private readonly ILogger<ViewDoctorsModel> _logger;

    public ViewDoctorsModel(DoctorService doctorService, DepartmentService departmentService, ILogger<ViewDoctorsModel> logger)
    {
        _doctorService = doctorService;
        _departmentService = departmentService;
        _logger = logger;
    }

    public IEnumerable<DoctorDto> Doctors { get; set; } = Enumerable.Empty<DoctorDto>();
    public IEnumerable<DepartmentDto> Departments { get; set; } = Enumerable.Empty<DepartmentDto>();
    public string? SelectedDepartment { get; set; }

    public async Task<IActionResult> OnGetAsync(string? department, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Patient")
            return RedirectToPage("/Account/Login");

        SelectedDepartment = department;

        try
        {
            Departments = await _departmentService.GetAllAsync(cancellationToken);
            Doctors = string.IsNullOrWhiteSpace(department)
                ? await _doctorService.GetAllAsync(cancellationToken)
                : await _doctorService.GetByDepartmentAsync(department, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctors");
        }

        return Page();
    }
}
