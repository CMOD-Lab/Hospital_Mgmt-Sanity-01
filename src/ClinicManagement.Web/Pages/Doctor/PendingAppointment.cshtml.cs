using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>Page model for Pending Appointments (migrated from PendingAppointment.aspx).</summary>
public class PendingAppointmentModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<PendingAppointmentModel> _logger;

    public PendingAppointmentModel(IDoctorService doctorService, ILogger<PendingAppointmentModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public IEnumerable<PendingAppointmentDto> PendingAppointments { get; set; } = Enumerable.Empty<PendingAppointmentDto>();

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 2)
            return RedirectToPage("/SignUp");

        try
        {
            PendingAppointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading pending appointments for doctor: {UserId}", userId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        try
        {
            await _doctorService.ApproveAppointmentAsync(appointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment: {AppointmentId}", appointmentId);
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        try
        {
            await _doctorService.DeleteAppointmentAsync(appointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting appointment: {AppointmentId}", appointmentId);
        }

        return RedirectToPage();
    }
}
