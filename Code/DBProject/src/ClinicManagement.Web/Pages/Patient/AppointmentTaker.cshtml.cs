using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class AppointmentTakerModel : PageModel
{
    private readonly AppointmentService _appointmentService;

    public IEnumerable<AppointmentDto> FreeSlots { get; set; } = Enumerable.Empty<AppointmentDto>();
    public int DoctorId { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }

    public AppointmentTakerModel(AppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    public async Task<IActionResult> OnGetAsync(int doctorId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        DoctorId = doctorId;
        FreeSlots = await _appointmentService.GetFreeSlotsAsync(doctorId, userId.Value, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int slotId, int doctorId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        DoctorId = doctorId;

        var result = await _appointmentService.BookAppointmentAsync(new InsertAppointmentDto
        {
            DoctorId = doctorId,
            PatientId = userId.Value,
            FreeSlotAppointId = slotId
        }, cancellationToken);

        if (result)
        {
            Success = true;
            Message = "Appointment request sent successfully! Waiting for doctor approval.";
        }
        else
        {
            Message = "Failed to book appointment. Please try again.";
        }

        FreeSlots = await _appointmentService.GetFreeSlotsAsync(doctorId, userId.Value, cancellationToken);
        return Page();
    }
}
