using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class CurrentAppointmentModel : PageModel
{
    private readonly PatientService _patientService;

    public AppointmentDto? Appointment { get; set; }

    public CurrentAppointmentModel(PatientService patientService)
    {
        _patientService = patientService;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        Appointment = await _patientService.GetCurrentAppointmentAsync(userId.Value, cancellationToken);
        return Page();
    }
}
