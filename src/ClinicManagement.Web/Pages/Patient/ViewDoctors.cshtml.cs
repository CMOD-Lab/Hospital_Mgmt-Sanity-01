using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>View doctors page model.</summary>
public class ViewDoctorsModel : PageModel
{
    private readonly IPatientService _patientService;

    public IEnumerable<DeptInfo> Departments { get; private set; } = Enumerable.Empty<DeptInfo>();
    public IEnumerable<DoctorListItem> Doctors { get; private set; } = Enumerable.Empty<DoctorListItem>();
    public string? SelectedDept { get; private set; }

    public ViewDoctorsModel(IPatientService patientService) => _patientService = patientService;

    public async Task<IActionResult> OnGetAsync(string? deptName, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");

        Departments = await _patientService.GetDepartmentsAsync(ct);

        if (!string.IsNullOrEmpty(deptName))
        {
            SelectedDept = deptName;
            Doctors = await _patientService.GetDoctorsByDepartmentAsync(deptName, ct);
        }

        return Page();
    }
}
