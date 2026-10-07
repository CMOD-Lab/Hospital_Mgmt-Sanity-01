using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>Page model for Bill page (migrated from Bill.aspx).</summary>
public class BillModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<BillModel> _logger;

    public BillModel(IDoctorService doctorService, ILogger<BillModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public IEnumerable<BillableAppointmentDto> BillableAppointments { get; set; } = Enumerable.Empty<BillableAppointmentDto>();
    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 2)
            return RedirectToPage("/SignUp");

        try
        {
            BillableAppointments = await _doctorService.GetBillableAppointmentsAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading billable appointments for doctor: {UserId}", userId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostMarkPaidAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        try
        {
            await _doctorService.MarkBillPaidAsync(userId.Value, appointmentId);
            SuccessMessage = "Bill marked as paid.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as paid: {AppointmentId}", appointmentId);
        }

        BillableAppointments = await _doctorService.GetBillableAppointmentsAsync(userId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostMarkUnpaidAsync(int appointmentId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        try
        {
            await _doctorService.MarkBillUnpaidAsync(userId.Value, appointmentId);
            SuccessMessage = "Bill marked as unpaid.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as unpaid: {AppointmentId}", appointmentId);
        }

        BillableAppointments = await _doctorService.GetBillableAppointmentsAsync(userId.Value);
        return Page();
    }
}
