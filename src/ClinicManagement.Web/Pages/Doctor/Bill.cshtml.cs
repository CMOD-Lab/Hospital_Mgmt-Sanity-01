using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class BillModel : PageModel
{
    private readonly IDoctorService _doctorService;
    public IEnumerable<BillItem> Bills { get; private set; } = Enumerable.Empty<BillItem>();
    public string? Message { get; private set; }
    public bool IsSuccess { get; private set; }

    public BillModel(IDoctorService doctorService) => _doctorService = doctorService;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");
        Bills = await _doctorService.GetBillsAsync(userId.Value, ct);
        return Page();
    }

    public async Task<IActionResult> OnPostMarkPaidAsync(int appointId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");

        IsSuccess = await _doctorService.MarkBillPaidAsync(userId.Value, appointId, ct);
        Message = IsSuccess ? "Bill marked as paid." : "Failed to update bill.";
        Bills = await _doctorService.GetBillsAsync(userId.Value, ct);
        return Page();
    }

    public async Task<IActionResult> OnPostMarkUnpaidAsync(int appointId, CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");

        IsSuccess = await _doctorService.MarkBillUnpaidAsync(userId.Value, appointId, ct);
        Message = IsSuccess ? "Bill marked as unpaid." : "Failed to update bill.";
        Bills = await _doctorService.GetBillsAsync(userId.Value, ct);
        return Page();
    }
}
