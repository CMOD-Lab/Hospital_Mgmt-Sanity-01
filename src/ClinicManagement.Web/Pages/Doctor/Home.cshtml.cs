using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>
/// Doctor home page model.
/// </summary>
public class HomeModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<HomeModel> _logger;

    public HomeModel(IDoctorService doctorService, ILogger<HomeModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public DoctorDto? DoctorInfo { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserId") == null || HttpContext.Session.GetInt32("UserType") != 1)
            return RedirectToPage("/Account/Login");

        var doctorId = HttpContext.Session.GetInt32("UserId")!.Value;
        try
        {
            DoctorInfo = await _doctorService.GetByIdAsync(doctorId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor home for {DoctorId}", doctorId);
        }

        return Page();
    }
}
