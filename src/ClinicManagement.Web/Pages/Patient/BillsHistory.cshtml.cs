using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>
/// Bills history page model for patients.
/// </summary>
public class BillsHistoryModel : PageModel
{
    private readonly IBillService _billService;
    private readonly ILogger<BillsHistoryModel> _logger;

    public BillsHistoryModel(IBillService billService, ILogger<BillsHistoryModel> logger)
    {
        _billService = billService;
        _logger = logger;
    }

    public IEnumerable<BillDto> Bills { get; set; } = new List<BillDto>();

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 2)
            return RedirectToPage("/Account/Login");

        var patientId = HttpContext.Session.GetInt32("UserId")!.Value;
        try
        {
            Bills = await _billService.GetByPatientIdAsync(patientId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bills for patient {PatientId}", patientId);
        }

        return Page();
    }
}
