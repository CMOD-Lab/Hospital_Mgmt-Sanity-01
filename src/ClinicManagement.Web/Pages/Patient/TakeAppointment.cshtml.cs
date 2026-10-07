using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicManagement.Web.Pages.Patient;

public class TakeAppointmentModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<TakeAppointmentModel> _logger;

    public TakeAppointmentModel(IPatientService patientService, ILogger<TakeAppointmentModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public IEnumerable<DoctorListItemDto> Doctors { get; set; } = Enumerable.Empty<DoctorListItemDto>();
    public List<SelectListItem> DepartmentSelectList { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string? deptName = null)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 1)
            return RedirectToPage("/SignUp");

        try
        {
            var departments = await _patientService.GetDepartmentInfoAsync();
            DepartmentSelectList = departments.Select(d => new SelectListItem
            {
                Value = d.DeptName,
                Text = d.DeptName,
                Selected = d.DeptName == deptName
            }).ToList();

            if (!string.IsNullOrEmpty(deptName))
            {
                Doctors = await _patientService.GetDoctorsByDepartmentAsync(deptName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading take appointment page");
        }

        return Page();
    }
}
