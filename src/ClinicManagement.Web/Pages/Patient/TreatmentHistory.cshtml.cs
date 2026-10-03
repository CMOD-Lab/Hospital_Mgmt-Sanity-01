using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class TreatmentHistoryModel : PageModel
{
    private readonly AppointmentService _appointmentService;
    private readonly ILogger<TreatmentHistoryModel> _logger;

    public TreatmentHistoryModel(AppointmentService appointmentService, ILogger<TreatmentHistoryModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public IEnumerable<AppointmentDto> TreatmentHistory { get; set; } = Enumerable.Empty<AppointmentDto>();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Patient")
            return RedirectToPage("/Account/Login");

        var patientId = HttpContext.Session.GetInt32("UserId") ?? 0;
        try
        {
            var all = await _appointmentService.GetByPatientIdAsync(patientId, cancellationToken);
            TreatmentHistory = all.Where(a => a.Status == "Completed");
        }
        catch (Exception ex) { _logger.LogError(ex, "Error loading treatment history for patient {PatientId}", patientId); }
        return Page();
    }
}
