using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class NotificationsModel : PageModel
{
    private readonly IPatientService _patientService;
    public IEnumerable<NotificationData> Notifications { get; private set; } = Enumerable.Empty<NotificationData>();

    public NotificationsModel(IPatientService patientService) => _patientService = patientService;

    public async Task<IActionResult> OnGetAsync(CancellationToken ct)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Index");
        Notifications = await _patientService.GetNotificationsAsync(userId.Value, ct);
        return Page();
    }
}
