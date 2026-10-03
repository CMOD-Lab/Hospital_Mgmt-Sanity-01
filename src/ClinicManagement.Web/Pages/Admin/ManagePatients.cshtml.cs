using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

public class ManagePatientsModel : PageModel
{
    private readonly PatientService _patientService;
    private readonly ILogger<ManagePatientsModel> _logger;

    public ManagePatientsModel(PatientService patientService, ILogger<ManagePatientsModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public IEnumerable<PatientDto> Patients { get; set; } = Enumerable.Empty<PatientDto>();
    public string? SearchQuery { get; set; }

    public async Task<IActionResult> OnGetAsync(string? search, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Admin")
            return RedirectToPage("/Account/Login");

        SearchQuery = search;
        try
        {
            Patients = string.IsNullOrWhiteSpace(search)
                ? await _patientService.GetAllAsync(cancellationToken)
                : await _patientService.SearchAsync(search, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patients");
        }
        return Page();
    }
}
