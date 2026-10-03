using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class HomeModel : PageModel
{
    private readonly DoctorService _doctorService;
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<HomeModel> _logger;

    public HomeModel(DoctorService doctorService, AppointmentService appointmentService, ILogger<HomeModel> logger)
    {
        _doctorService = doctorService;
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public DoctorDto? DoctorInfo { get; set; }
    public IEnumerable<AppointmentDto> TodaysAppointments { get; set; } = Enumerable.Empty<AppointmentDto>();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Doctor")
            return RedirectToPage("/Account/Login");

        var doctorId = HttpContext.Session.GetInt32("UserId") ?? 0;

        try
        {
            DoctorInfo = await _doctorService.GetByIdAsync(doctorId, cancellationToken);
            TodaysAppointments = await _appointmentService.GetTodaysByDoctorIdAsync(doctorId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor home for doctor {DoctorId}", doctorId);
        }

        return Page();
    }
}
