using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Page model for managing doctors.
/// </summary>
public class ManageDoctorsModel : PageModel
{
    private readonly DoctorService _doctorService;
    private readonly ILogger<ManageDoctorsModel> _logger;

    public ManageDoctorsModel(DoctorService doctorService, ILogger<ManageDoctorsModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public IEnumerable<DoctorDto> Doctors { get; set; } = Enumerable.Empty<DoctorDto>();
    public string? SearchQuery { get; set; }
    public string? Message { get; set; }
    public bool IsError { get; set; }

    public async Task<IActionResult> OnGetAsync(string? search, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Admin")
        {
            return RedirectToPage("/Account/Login");
        }

        SearchQuery = search;

        try
        {
            Doctors = string.IsNullOrWhiteSpace(search)
                ? await _doctorService.GetAllAsync(cancellationToken)
                : await _doctorService.SearchAsync(search, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctors");
            Message = "Error loading doctors.";
            IsError = true;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (HttpContext.Session.GetString("UserType") != "Admin")
        {
            return RedirectToPage("/Account/Login");
        }

        try
        {
            await _doctorService.DeleteAsync(id, cancellationToken);
            Message = "Doctor removed successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor {DoctorId}", id);
            Message = "Error removing doctor.";
            IsError = true;
        }

        Doctors = await _doctorService.GetAllAsync(cancellationToken);
        return Page();
    }
}
