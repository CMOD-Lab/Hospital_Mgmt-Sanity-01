using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>
/// Previous history page model for doctors.
/// </summary>
public class PreviousHistoryModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<PreviousHistoryModel> _logger;

    public PreviousHistoryModel(IAppointmentService appointmentService, ILogger<PreviousHistoryModel> logger)
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
            Appointments = await _appointmentService.GetByDoctorIdAsync(doctorId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading previous history for doctor {DoctorId}", doctorId);
        }

        return Page();
    }
}
