using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class PatientHistoryModel : PageModel
{
    private readonly DoctorService _doctorService;

    public IEnumerable<AppointmentDto> History { get; set; } = Enumerable.Empty<AppointmentDto>();

    public PatientHistoryModel(DoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        History = await _doctorService.GetPatientHistoryAsync(userId.Value, cancellationToken);
        return Page();
    }
}
