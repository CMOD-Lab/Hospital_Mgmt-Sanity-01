using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>
/// Pending appointments page model for doctors.
/// </summary>
public class PendingAppointmentsModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<PendingAppointmentsModel> _logger;

    public PendingAppointmentsModel(IAppointmentService appointmentService, ILogger<PendingAppointmentsModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public IEnumerable<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();
    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 1)
            return RedirectToPage("/Account/Login");

        var doctorId = HttpContext.Session.GetInt32("UserId")!.Value;
        try
        {
            Appointments = await _appointmentService.GetPendingByDoctorIdAsync(doctorId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading pending appointments for doctor {DoctorId}", doctorId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(int id)
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 1)
            return RedirectToPage("/Account/Login");

        try
        {
            await _appointmentService.ApproveAsync(id);
            SuccessMessage = "Appointment approved successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment {AppointmentId}", id);
        }

        var doctorId = HttpContext.Session.GetInt32("UserId")!.Value;
        Appointments = await _appointmentService.GetPendingByDoctorIdAsync(doctorId);
        return Page();
    }

    public async Task<IActionResult> OnPostRejectAsync(int id)
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 1)
            return RedirectToPage("/Account/Login");

        try
        {
            await _appointmentService.DeleteAsync(id);
            SuccessMessage = "Appointment rejected.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting appointment {AppointmentId}", id);
        }

        var doctorId = HttpContext.Session.GetInt32("UserId")!.Value;
        Appointments = await _appointmentService.GetPendingByDoctorIdAsync(doctorId);
        return Page();
    }
}
