using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Manage patients page model.
/// </summary>
public class ManagePatientsModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<ManagePatientsModel> _logger;

    public ManagePatientsModel(IPatientService patientService, ILogger<ManagePatientsModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public IEnumerable<PatientDto> Patients { get; set; } = new List<PatientDto>();
    public string? SearchQuery { get; set; }

    public async Task<IActionResult> OnGetAsync(string? search = null)
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 0)
            return RedirectToPage("/Account/Login");

        SearchQuery = search;
        try
        {
            Patients = string.IsNullOrEmpty(search)
                ? await _patientService.GetAllAsync()
                : await _patientService.SearchAsync(search);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patients");
        }

        return Page();
    }
}
