using ClinicManagement.Domain.DTOs;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>Page model for Doctor Home (migrated from DoctorHome.aspx).</summary>
public class DoctorHomeModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<DoctorHomeModel> _logger;

    public DoctorHomeModel(IDoctorService doctorService, ILogger<DoctorHomeModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public DoctorInfoDto? DoctorInfo { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 2)
            return RedirectToPage("/SignUp");

        try
        {
            DoctorInfo = await _doctorService.GetDoctorInfoAsync(userId.Value);

            if (DoctorInfo == null)
            {
                ErrorMessage = "There was some error loading doctor information.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor home for user ID: {UserId}", userId);
            ErrorMessage = "There was some error.";
        }

        return Page();
    }
}
