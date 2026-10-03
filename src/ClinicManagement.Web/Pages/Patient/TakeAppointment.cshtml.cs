using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class TakeAppointmentModel : PageModel
{
    private readonly DoctorService _doctorService;
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<TakeAppointmentModel> _logger;

    public TakeAppointmentModel(DoctorService doctorService, AppointmentService appointmentService, ILogger<TakeAppointmentModel> logger)
    {
        _doctorService = doctorService;
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public DoctorDto? Doctor { get; set; }
    public IEnumerable<TimeSlotDto> FreeSlots { get; set; } = Enumerable.Empty<TimeSlotDto>();
    public string? Message { get; set; }
    public bool IsError { get; set; }

    public async Task<IActionResult> OnGetAsync(int doctorId, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Patient")
            return RedirectToPage("/Account/Login");

        var patientId = HttpContext.Session.GetInt32("UserId") ?? 0;

        try
        {
            Doctor = await _doctorService.GetByIdAsync(doctorId, cancellationToken);
            FreeSlots = await _appointmentService.GetFreeSlotsAsync(doctorId, patientId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading appointment page for doctor {DoctorId}", doctorId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int doctorId, int timeSlotId, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Patient")
            return RedirectToPage("/Account/Login");

        var patientId = HttpContext.Session.GetInt32("UserId") ?? 0;

        try
        {
            var dto = new AppointmentCreateDto
            {
                DoctorId = doctorId,
                PatientId = patientId,
                TimeSlotId = timeSlotId
            };
            await _appointmentService.CreateAsync(dto, cancellationToken);
            Message = "Appointment request sent successfully! Please wait for doctor approval.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment for patient {PatientId}", patientId);
            Message = "Error booking appointment. Please try again.";
            IsError = true;
        }

        Doctor = await _doctorService.GetByIdAsync(doctorId, cancellationToken);
        FreeSlots = await _appointmentService.GetFreeSlotsAsync(doctorId, patientId, cancellationToken);
        return Page();
    }
}
