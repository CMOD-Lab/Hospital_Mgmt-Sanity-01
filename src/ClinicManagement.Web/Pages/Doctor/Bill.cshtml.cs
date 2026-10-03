using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>
/// Bill management page model for doctors.
/// </summary>
public class BillModel : PageModel
{
    private readonly IBillService _billService;
    private readonly ILogger<BillModel> _logger;

    public BillModel(IBillService billService, ILogger<BillModel> logger)
    {
        _billService = billService;
        _logger = logger;
    }

    public IEnumerable<BillDto> Bills { get; set; } = new List<BillDto>();
    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 1)
            return RedirectToPage("/Account/Login");

        var doctorId = HttpContext.Session.GetInt32("UserId")!.Value;
        try
        {
            Bills = await _billService.GetByDoctorIdAsync(doctorId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bills for doctor {DoctorId}", doctorId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostMarkPaidAsync(int appointmentId)
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 1)
            return RedirectToPage("/Account/Login");

        var doctorId = HttpContext.Session.GetInt32("UserId")!.Value;
        try
        {
            await _billService.MarkAsPaidAsync(doctorId, appointmentId);
            SuccessMessage = "Bill marked as paid.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as paid");
        }

        Bills = await _billService.GetByDoctorIdAsync(doctorId);
        return Page();
    }

    public async Task<IActionResult> OnPostMarkUnpaidAsync(int appointmentId)
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 1)
            return RedirectToPage("/Account/Login");

        var doctorId = HttpContext.Session.GetInt32("UserId")!.Value;
        try
        {
            await _billService.MarkAsUnpaidAsync(doctorId, appointmentId);
            SuccessMessage = "Bill marked as unpaid.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as unpaid");
        }

        Bills = await _billService.GetByDoctorIdAsync(doctorId);
        return Page();
    }
}
