using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class NotificationsModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<NotificationsModel> _logger;

    public NotificationsModel(AppointmentService appointmentService, ILogger<NotificationsModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public AppointmentDto? PendingFeedback { get; set; }
    public AppointmentDto? CurrentAppointment { get; set; }
    public string? Message { get; set; }
    public bool IsError { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Patient")
            return RedirectToPage("/Account/Login");

        var patientId = HttpContext.Session.GetInt32("UserId") ?? 0;
        try
        {
            PendingFeedback = await _appointmentService.GetPendingFeedbackByPatientIdAsync(patientId, cancellationToken);
            CurrentAppointment = await _appointmentService.GetCurrentByPatientIdAsync(patientId, cancellationToken);
        }
        catch (Exception ex) { _logger.LogError(ex, "Error loading notifications for patient {PatientId}", patientId); }
        return Page();
    }

    public async Task<IActionResult> OnPostGiveFeedbackAsync(int appointmentId, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Patient")
            return RedirectToPage("/Account/Login");

        try
        {
            await _appointmentService.StoreFeedbackAsync(appointmentId, cancellationToken);
            Message = "Thank you for your feedback!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing feedback for appointment {AppointmentId}", appointmentId);
            Message = "Error submitting feedback.";
            IsError = true;
        }

        var patientId = HttpContext.Session.GetInt32("UserId") ?? 0;
        PendingFeedback = await _appointmentService.GetPendingFeedbackByPatientIdAsync(patientId, cancellationToken);
        CurrentAppointment = await _appointmentService.GetCurrentByPatientIdAsync(patientId, cancellationToken);
        return Page();
    }
}
