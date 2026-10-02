using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class TakeAppointmentModel : PageModel
{
    private readonly IPatientService _patientService;

    [BindProperty] public string? SelectedDept { get; set; }
    [BindProperty] public int SelectedDoctorId { get; set; }

    public IEnumerable<DeptInfo> Departments { get; private set; } = Enumerable.Empty<DeptInfo>();
    public IEnumerable<DoctorListItem> Doctors { get; private set; } = Enumerable.Empty<DoctorListItem>();
    public IEnumerable<AppointmentSlot> FreeSlots { get; private set; } = Enumerable.Empty<AppointmentSlot>();
    public string? Message { get; private set; }
    public bool IsSuccess { get; private set; }

    public TakeAppointmentModel(IPatientService patientService) => _patientService = patientService;

    public async Task<IActionResult> OnGetAsync(int? doctorId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");

        Departments = await _patientService.GetDepartmentsAsync(ct);

        if (doctorId.HasValue && doctorId.Value > 0)
        {
            SelectedDoctorId = doctorId.Value;
            FreeSlots = await _patientService.GetFreeSlotsAsync(doctorId.Value, userId.Value, ct);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? deptName, int doctorId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");

        Departments = await _patientService.GetDepartmentsAsync(ct);
        SelectedDept = deptName;

        if (!string.IsNullOrEmpty(deptName))
            Doctors = await _patientService.GetDoctorsByDepartmentAsync(deptName, ct);

        if (doctorId > 0)
        {
            SelectedDoctorId = doctorId;
            FreeSlots = await _patientService.GetFreeSlotsAsync(doctorId, userId.Value, ct);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostBookAsync(int doctorId, int slotId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");

        var success = await _patientService.BookAppointmentAsync(doctorId, userId.Value, slotId, ct);
        IsSuccess = success;
        Message = success ? "Appointment request sent successfully!" : "Failed to book appointment. Please try again.";

        Departments = await _patientService.GetDepartmentsAsync(ct);
        return Page();
    }
}
