using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class BillsHistoryModel : PageModel
{
    private readonly IBillRepository _billRepository;
    private readonly ILogger<BillsHistoryModel> _logger;

    public BillsHistoryModel(IBillRepository billRepository, ILogger<BillsHistoryModel> logger)
    {
        _billRepository = billRepository;
        _logger = logger;
    }

    public IEnumerable<BillDto> Bills { get; set; } = Enumerable.Empty<BillDto>();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Patient")
            return RedirectToPage("/Account/Login");

        var patientId = HttpContext.Session.GetInt32("UserId") ?? 0;

        try
        {
            var bills = await _billRepository.GetByPatientIdAsync(patientId, cancellationToken);
            Bills = bills.Select(b => new BillDto
            {
                BillId = b.BillId,
                AppointmentId = b.AppointmentId,
                PatientId = b.PatientId,
                DoctorId = b.DoctorId,
                DoctorName = b.Doctor?.Name ?? string.Empty,
                Amount = b.Amount,
                IsPaid = b.IsPaid,
                BillDate = b.BillDate
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bills for patient {PatientId}", patientId);
        }

        return Page();
    }
}
