using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

public class DoctorRegistrationModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly IPatientService _patientService;

    public IEnumerable<DeptInfo> Departments { get; private set; } = Enumerable.Empty<DeptInfo>();
    public string? Message { get; private set; }
    public bool IsSuccess { get; private set; }

    public DoctorRegistrationModel(IAdminService adminService, IPatientService patientService)
    {
        _adminService = adminService;
        _patientService = patientService;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");
        Departments = await _patientService.GetDepartmentsAsync(ct);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string name, string email, string password, string birthDate,
        int deptNo, string gender, string phone, string address,
        int experience, int salary, int chargesPerVisit,
        string specialization, string qualification,
        CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");

        Departments = await _patientService.GetDepartmentsAsync(ct);

        if (await _adminService.DoctorEmailExistsAsync(email, ct))
        {
            Message = "This email already exists. Please choose a different one.";
            IsSuccess = false;
            return Page();
        }

        var request = new RegisterDoctorRequest(
            name, email, password, birthDate, deptNo,
            phone, string.IsNullOrEmpty(gender) ? 'M' : gender[0],
            address, experience, salary, chargesPerVisit,
            specialization, qualification);

        IsSuccess = await _adminService.RegisterDoctorAsync(request, ct);
        Message = IsSuccess ? "Doctor registered successfully!" : "Failed to register doctor. Please try again.";

        return Page();
    }
}
