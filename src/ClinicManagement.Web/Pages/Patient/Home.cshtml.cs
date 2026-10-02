using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Patient home page model.</summary>
public class HomeModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<HomeModel> _logger;

    public PatientProfileData? Profile { get; private set; }
    public string PatientName => Profile?.Name ?? "Patient";

    public HomeModel(IPatientService patientService, ILogger<HomeModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");

        try
        {
            Profile = await _patientService.GetPatientProfileAsync(userId.Value, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patient home for {UserId}", userId);
        }

        return Page();
    }
}
