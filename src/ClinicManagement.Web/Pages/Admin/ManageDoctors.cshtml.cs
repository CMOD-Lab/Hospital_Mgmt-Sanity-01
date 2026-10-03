using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Manage doctors page model.
/// </summary>
public class ManageDoctorsModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<ManageDoctorsModel> _logger;

    public ManageDoctorsModel(IDoctorService doctorService, ILogger<ManageDoctorsModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public IEnumerable<DoctorDto> Doctors { get; set; } = new List<DoctorDto>();
    public string? SearchQuery { get; set; }
    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string? search = null)
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 0)
            return RedirectToPage("/Account/Login");

        SearchQuery = search;
        try
        {
            Doctors = string.IsNullOrEmpty(search)
                ? await _doctorService.GetAllAsync()
                : await _doctorService.SearchAsync(search);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctors");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 0)
            return RedirectToPage("/Account/Login");

        try
        {
            await _doctorService.DeleteAsync(id);
            SuccessMessage = "Doctor removed successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor {DoctorId}", id);
        }

        Doctors = await _doctorService.GetAllAsync();
        return Page();
    }
}
