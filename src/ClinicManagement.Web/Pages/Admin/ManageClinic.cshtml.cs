using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

public class ManageClinicModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly IDoctorRepository _doctorRepo;
    private readonly IStaffRepository _staffRepo;
    private readonly IPatientRepository _patientRepo;

    public IEnumerable<DoctorListItem> Doctors { get; private set; } = Enumerable.Empty<DoctorListItem>();
    public IEnumerable<OtherStaff> Staff { get; private set; } = Enumerable.Empty<OtherStaff>();
    public IEnumerable<Domain.Entities.Patient> Patients { get; private set; } = Enumerable.Empty<Domain.Entities.Patient>();
    public string? DoctorSearch { get; private set; }
    public string? StaffSearch { get; private set; }
    public string? PatientSearch { get; private set; }
    public string? Message { get; private set; }
    public bool IsSuccess { get; private set; }

    public ManageClinicModel(
        IAdminService adminService,
        IDoctorRepository doctorRepo,
        IStaffRepository staffRepo,
        IPatientRepository patientRepo)
    {
        _adminService = adminService;
        _doctorRepo = doctorRepo;
        _staffRepo = staffRepo;
        _patientRepo = patientRepo;
    }

    public async Task<IActionResult> OnGetAsync(
        string? doctorSearch, string? staffSearch, string? patientSearch,
        CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");

        DoctorSearch = doctorSearch;
        StaffSearch = staffSearch;
        PatientSearch = patientSearch;

        var doctors = string.IsNullOrEmpty(doctorSearch)
            ? await _doctorRepo.GetAllActiveAsync(ct)
            : await _doctorRepo.SearchAsync(doctorSearch, ct);

        Doctors = doctors.Select(d => new DoctorListItem(d.DoctorId, d.Name, d.Department?.DeptName));

        Staff = string.IsNullOrEmpty(staffSearch)
            ? await _staffRepo.GetAllAsync(ct)
            : await _staffRepo.SearchAsync(staffSearch, ct);

        Patients = string.IsNullOrEmpty(patientSearch)
            ? await _patientRepo.GetAllAsync(ct)
            : await _patientRepo.SearchAsync(patientSearch, ct);

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteDoctorAsync(int doctorId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");

        IsSuccess = await _adminService.DeleteDoctorAsync(doctorId, ct);
        Message = IsSuccess ? "Doctor removed successfully." : "Failed to remove doctor.";

        return await OnGetAsync(null, null, null, ct);
    }

    public async Task<IActionResult> OnPostDeleteStaffAsync(int staffId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");

        IsSuccess = await _adminService.DeleteStaffAsync(staffId, ct);
        Message = IsSuccess ? "Staff member deleted successfully." : "Failed to delete staff member.";

        return await OnGetAsync(null, null, null, ct);
    }
}
