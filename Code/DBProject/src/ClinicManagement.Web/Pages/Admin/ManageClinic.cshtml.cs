using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

public class ManageClinicModel : PageModel
{
    private readonly AdminService _adminService;
    private readonly DoctorService _doctorService;
    private readonly PatientService _patientService;

    public IEnumerable<DoctorListDto> Doctors { get; set; } = Enumerable.Empty<DoctorListDto>();
    public IEnumerable<PatientListDto> Patients { get; set; } = Enumerable.Empty<PatientListDto>();
    public IEnumerable<StaffDto> Staff { get; set; } = Enumerable.Empty<StaffDto>();
    public string DoctorSearch { get; set; } = string.Empty;
    public string PatientSearch { get; set; } = string.Empty;
    public string StaffSearch { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }

    public ManageClinicModel(AdminService adminService, DoctorService doctorService, PatientService patientService)
    {
        _adminService = adminService;
        _doctorService = doctorService;
        _patientService = patientService;
    }

    public async Task<IActionResult> OnGetAsync(
        string? doctorSearch,
        string? patientSearch,
        string? staffSearch,
        CancellationToken cancellationToken)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        DoctorSearch = doctorSearch ?? string.Empty;
        PatientSearch = patientSearch ?? string.Empty;
        StaffSearch = staffSearch ?? string.Empty;

        Doctors = await _doctorService.GetDoctorsAsync(DoctorSearch, cancellationToken);
        Patients = await _patientService.GetPatientsAsync(PatientSearch, cancellationToken);
        Staff = await _adminService.GetStaffAsync(StaffSearch, cancellationToken);

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteDoctorAsync(int doctorId, CancellationToken cancellationToken)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        var result = await _doctorService.DeleteDoctorAsync(doctorId, cancellationToken);
        Success = result;
        Message = result ? "Doctor removed successfully." : "Failed to remove doctor.";

        Doctors = await _doctorService.GetDoctorsAsync(string.Empty, cancellationToken);
        Patients = await _patientService.GetPatientsAsync(string.Empty, cancellationToken);
        Staff = await _adminService.GetStaffAsync(string.Empty, cancellationToken);

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteStaffAsync(int staffId, CancellationToken cancellationToken)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        var result = await _adminService.DeleteStaffAsync(staffId, cancellationToken);
        Success = result;
        Message = result ? "Staff member removed successfully." : "Failed to remove staff member.";

        Doctors = await _doctorService.GetDoctorsAsync(string.Empty, cancellationToken);
        Patients = await _patientService.GetPatientsAsync(string.Empty, cancellationToken);
        Staff = await _adminService.GetStaffAsync(string.Empty, cancellationToken);

        return Page();
    }
}
