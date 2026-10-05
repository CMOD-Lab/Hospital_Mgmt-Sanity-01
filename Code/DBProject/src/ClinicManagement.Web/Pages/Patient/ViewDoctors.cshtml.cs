using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class ViewDoctorsModel : PageModel
{
    private readonly DoctorService _doctorService;

    public IEnumerable<DoctorListDto> Doctors { get; set; } = Enumerable.Empty<DoctorListDto>();
    public string SearchQuery { get; set; } = string.Empty;

    public ViewDoctorsModel(DoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    public async Task<IActionResult> OnGetAsync(string? search, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/SignUp");

        SearchQuery = search ?? string.Empty;
        Doctors = await _doctorService.GetDoctorsAsync(SearchQuery, cancellationToken);
        return Page();
    }
}
