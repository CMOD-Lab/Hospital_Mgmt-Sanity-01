using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Page model for Appointment Taker page (migrated from AppointmentTaker.aspx).</summary>
public class AppointmentTakerModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<AppointmentTakerModel> _logger;

    public AppointmentTakerModel(IPatientService patientService, ILogger<AppointmentTakerModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public IEnumerable<AppointmentSlotDto> FreeSlots { get; set; } = Enumerable.Empty<AppointmentSlotDto>();
    public string? ErrorMessage { get; set; }
    public int DoctorId { get; set; }

    public async Task<IActionResult> OnGetAsync(int doctorId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 1)
            return RedirectToPage("/SignUp");

        DoctorId = doctorId;

        try
        {
            FreeSlots = await _patientService.GetFreeSlotsAsync(doctorId, userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading free slots for doctor: {DoctorId}", doctorId);
            ErrorMessage = "Error loading available slots.";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int doctorId, int slotId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 1)
            return RedirectToPage("/SignUp");

        try
        {
            var success = await _patientService.BookAppointmentAsync(doctorId, userId.Value, slotId);
            if (success)
            {
                return RedirectToPage("/Patient/AppointmentRequestSent");
            }
            ErrorMessage = "Failed to book appointment. Please try again.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment for patient: {PatientId}", userId);
            ErrorMessage = "There was some error booking the appointment.";
        }

        DoctorId = doctorId;
        FreeSlots = await _patientService.GetFreeSlotsAsync(doctorId, userId.Value);
        return Page();
    }
}
