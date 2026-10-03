using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class DoctorProfileModel : PageModel
{
    private readonly DoctorService _doctorService;
    private readonly ILogger<DoctorProfileModel> _logger;

    public DoctorProfileModel(DoctorService doctorService, ILogger<DoctorProfileModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public DoctorDto? Doctor { get; set; }

    public async Task<IActionResult> OnGetAsync(int doctorId, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Patient")
            return RedirectToPage("/Account/Login");

        try { Doctor = await _doctorService.GetByIdAsync(doctorId, cancellationToken); }
        catch (Exception ex) { _logger.LogError(ex, "Error loading doctor profile for doctor {DoctorId}", doctorId); }
        return Page();
    }
}
