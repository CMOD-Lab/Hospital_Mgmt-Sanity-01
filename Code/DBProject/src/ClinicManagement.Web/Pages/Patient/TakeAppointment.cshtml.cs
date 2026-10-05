using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class TakeAppointmentModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly DoctorService _doctorService;

    public IEnumerable<DepartmentDto> Departments { get; set; } = Enumerable.Empty<DepartmentDto>();
    public IEnumerable<DoctorListDto> Doctors { get; set; } = Enumerable.Empty<DoctorListDto>();
    public string SelectedDept { get; set; } = string.Empty;

    public TakeAppointmentModel(AppointmentService appointmentService, DoctorService doctorService)
    {
        _appointmentService = appointmentService;
        _doctorService = doctorService;
    }

    public async Task<IActionResult> OnGetAsync(string? deptName, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        Departments = await _appointmentService.GetDepartmentsAsync(cancellationToken);

        if (!string.IsNullOrEmpty(deptName))
        {
            SelectedDept = deptName;
            Doctors = await _doctorService.GetDoctorsByDepartmentAsync(deptName, cancellationToken);
        }

        return Page();
    }
}
