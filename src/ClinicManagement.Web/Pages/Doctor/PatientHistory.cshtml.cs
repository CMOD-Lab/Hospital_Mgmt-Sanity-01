using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>
/// Today's patient history page model for doctors.
/// </summary>
public class PatientHistoryModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<PatientHistoryModel> _logger;

    public PatientHistoryModel(IAppointmentService appointmentService, ILogger<PatientHistoryModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public IEnumerable<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 1)
            return RedirectToPage("/Account/Login");

        var doctorId = HttpContext.Session.GetInt32("UserId")!.Value;
        try
        {
            Appointments = await _appointmentService.GetTodaysByDoctorIdAsync(doctorId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading today's appointments for doctor {DoctorId}", doctorId);
        }

        return Page();
    }
}
