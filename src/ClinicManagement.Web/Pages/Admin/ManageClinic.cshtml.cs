using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Page model for Manage Clinic page (migrated from ManageClinic.aspx).</summary>
public class ManageClinicModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<ManageClinicModel> _logger;

    public ManageClinicModel(IAdminService adminService, ILogger<ManageClinicModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public IEnumerable<DoctorListItemDto> Doctors { get; set; } = Enumerable.Empty<DoctorListItemDto>();
    public IEnumerable<PatientListItemDto> Patients { get; set; } = Enumerable.Empty<PatientListItemDto>();
    public IEnumerable<StaffListItemDto> Staff { get; set; } = Enumerable.Empty<StaffListItemDto>();
    public string? ErrorMessage { get; set; }
    public string DoctorSearch { get; set; } = string.Empty;
    public string PatientSearch { get; set; } = string.Empty;
    public string StaffSearch { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(
        string? doctorSearch = null,
        string? patientSearch = null,
        string? staffSearch = null)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 3)
            return RedirectToPage("/SignUp");

        DoctorSearch = doctorSearch ?? string.Empty;
        PatientSearch = patientSearch ?? string.Empty;
        StaffSearch = staffSearch ?? string.Empty;

        try
        {
            Doctors = await _adminService.GetDoctorsAsync(DoctorSearch);
            Patients = await _adminService.GetPatientsAsync(PatientSearch);
            Staff = await _adminService.GetStaffAsync(StaffSearch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading manage clinic data");
            ErrorMessage = "There was some error loading the data.";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteDoctorAsync(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 3)
            return RedirectToPage("/SignUp");

        try
        {
            await _adminService.DeleteDoctorAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor ID: {Id}", id);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteStaffAsync(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 3)
            return RedirectToPage("/SignUp");

        try
        {
            await _adminService.DeleteStaffAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff ID: {Id}", id);
        }

        return RedirectToPage();
    }
}
