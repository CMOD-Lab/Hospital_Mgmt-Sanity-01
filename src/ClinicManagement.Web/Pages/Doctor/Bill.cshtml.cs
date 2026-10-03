using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class BillModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<BillModel> _logger;

    public BillModel(AppointmentService appointmentService, ILogger<BillModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public IEnumerable<AppointmentDto> CompletedAppointments { get; set; } = Enumerable.Empty<AppointmentDto>();
    public string? Message { get; set; }
    public bool IsError { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Doctor")
            return RedirectToPage("/Account/Login");

        var doctorId = HttpContext.Session.GetInt32("UserId") ?? 0;
        try
        {
            var all = await _appointmentService.GetByDoctorIdAsync(doctorId, cancellationToken);
            CompletedAppointments = all.Where(a => a.Status == "Completed");
        }
        catch (Exception ex) { _logger.LogError(ex, "Error loading billing for doctor {DoctorId}", doctorId); }
        return Page();
    }

    public async Task<IActionResult> OnPostMarkPaidAsync(int appointmentId, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Doctor")
            return RedirectToPage("/Account/Login");

        var doctorId = HttpContext.Session.GetInt32("UserId") ?? 0;
        try
        {
            await _appointmentService.MarkPaidAsync(doctorId, appointmentId, cancellationToken);
            Message = "Appointment marked as paid.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking appointment {AppointmentId} as paid", appointmentId);
            Message = "Error updating payment status.";
            IsError = true;
        }

        var all = await _appointmentService.GetByDoctorIdAsync(doctorId, cancellationToken);
        CompletedAppointments = all.Where(a => a.Status == "Completed");
        return Page();
    }

    public async Task<IActionResult> OnPostMarkUnpaidAsync(int appointmentId, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Doctor")
            return RedirectToPage("/Account/Login");

        var doctorId = HttpContext.Session.GetInt32("UserId") ?? 0;
        try
        {
            await _appointmentService.MarkUnpaidAsync(doctorId, appointmentId, cancellationToken);
            Message = "Appointment marked as unpaid.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking appointment {AppointmentId} as unpaid", appointmentId);
            Message = "Error updating payment status.";
            IsError = true;
        }

        var all = await _appointmentService.GetByDoctorIdAsync(doctorId, cancellationToken);
        CompletedAppointments = all.Where(a => a.Status == "Completed");
        return Page();
    }
}
