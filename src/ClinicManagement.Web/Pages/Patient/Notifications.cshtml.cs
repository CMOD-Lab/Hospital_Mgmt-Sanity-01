using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>
/// Notifications page model for patients.
/// </summary>
public class NotificationsModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<NotificationsModel> _logger;

    public NotificationsModel(IAppointmentService appointmentService, ILogger<NotificationsModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public AppointmentDto? PendingFeedback { get; set; }
    public AppointmentDto? CurrentAppointment { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 2)
            return RedirectToPage("/Account/Login");

        var patientId = HttpContext.Session.GetInt32("UserId")!.Value;
        try
        {
            PendingFeedback = await _appointmentService.GetPendingFeedbackByPatientIdAsync(patientId);
            CurrentAppointment = await _appointmentService.GetCurrentByPatientIdAsync(patientId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading notifications for patient {PatientId}", patientId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int appointmentId)
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 2)
            return RedirectToPage("/Account/Login");

        try
        {
            await _appointmentService.SubmitFeedbackAsync(appointmentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback for appointment {AppointmentId}", appointmentId);
        }

        return RedirectToPage();
    }
}
