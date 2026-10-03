using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>
/// Patient home page model.
/// </summary>
public class HomeModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<HomeModel> _logger;

    public HomeModel(IPatientService patientService, ILogger<HomeModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public PatientDto? PatientInfo { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 2)
            return RedirectToPage("/Account/Login");

        var patientId = HttpContext.Session.GetInt32("UserId")!.Value;
        try
        {
            PatientInfo = await _patientService.GetByIdAsync(patientId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patient home for {PatientId}", patientId);
        }

        return Page();
    }
}
