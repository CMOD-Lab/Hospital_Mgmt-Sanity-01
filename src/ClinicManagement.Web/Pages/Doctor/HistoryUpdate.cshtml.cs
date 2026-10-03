using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.Pages.Doctor;

public class HistoryUpdateModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<HistoryUpdateModel> _logger;

    public HistoryUpdateModel(AppointmentService appointmentService, ILogger<HistoryUpdateModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    [BindProperty] public int AppointmentId { get; set; }
    [BindProperty][Required] public string Disease { get; set; } = string.Empty;
    [BindProperty][Required] public string Progress { get; set; } = string.Empty;
    [BindProperty][Required] public string Prescription { get; set; } = string.Empty;

    public AppointmentDto? Appointment { get; set; }
    public string? Message { get; set; }
    public bool IsError { get; set; }

    public async Task<IActionResult> OnGetAsync(int? appointmentId, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Doctor")
            return RedirectToPage("/Account/Login");

        if (appointmentId.HasValue)
        {
            AppointmentId = appointmentId.Value;
            Appointment = await _appointmentService.GetByIdAsync(appointmentId.Value, cancellationToken);
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Doctor")
            return RedirectToPage("/Account/Login");

        if (!ModelState.IsValid)
        {
            Appointment = await _appointmentService.GetByIdAsync(AppointmentId, cancellationToken);
            return Page();
        }

        var doctorId = HttpContext.Session.GetInt32("UserId") ?? 0;

        try
        {
            var dto = new PrescriptionUpdateDto
            {
                DoctorId = doctorId,
                AppointmentId = AppointmentId,
                Disease = Disease,
                Progress = Progress,
                Prescription = Prescription
            };
            await _appointmentService.UpdatePrescriptionAsync(dto, cancellationToken);
            Message = "Patient history updated successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription for appointment {AppointmentId}", AppointmentId);
            Message = "Error updating patient history.";
            IsError = true;
        }

        Appointment = await _appointmentService.GetByIdAsync(AppointmentId, cancellationToken);
        return Page();
    }
}
