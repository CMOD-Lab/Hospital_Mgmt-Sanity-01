using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class PendingAppointmentsModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<PendingAppointmentsModel> _logger;

    public PendingAppointmentsModel(AppointmentService appointmentService, ILogger<PendingAppointmentsModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public IEnumerable<AppointmentDto> PendingAppointments { get; set; } = Enumerable.Empty<AppointmentDto>();
    public string? Message { get; set; }
    public bool IsError { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Doctor")
            return RedirectToPage("/Account/Login");

        var doctorId = HttpContext.Session.GetInt32("UserId") ?? 0;
        try { PendingAppointments = await _appointmentService.GetPendingByDoctorIdAsync(doctorId, cancellationToken); }
        catch (Exception ex) { _logger.LogError(ex, "Error loading pending appointments for doctor {DoctorId}", doctorId); }
        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(int id, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Doctor")
            return RedirectToPage("/Account/Login");

        try
        {
            await _appointmentService.ApproveAsync(id, cancellationToken);
            Message = "Appointment approved successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment {AppointmentId}", id);
            Message = "Error approving appointment.";
            IsError = true;
        }

        var doctorId = HttpContext.Session.GetInt32("UserId") ?? 0;
        PendingAppointments = await _appointmentService.GetPendingByDoctorIdAsync(doctorId, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostRejectAsync(int id, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Doctor")
            return RedirectToPage("/Account/Login");

        try
        {
            await _appointmentService.DeleteAsync(id, cancellationToken);
            Message = "Appointment rejected.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting appointment {AppointmentId}", id);
            Message = "Error rejecting appointment.";
            IsError = true;
        }

        var doctorId = HttpContext.Session.GetInt32("UserId") ?? 0;
        PendingAppointments = await _appointmentService.GetPendingByDoctorIdAsync(doctorId, cancellationToken);
        return Page();
    }
}
