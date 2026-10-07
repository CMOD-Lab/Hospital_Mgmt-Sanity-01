using Microsoft.AspNetCore.Mvc.RazorPages;

// Migrated from ASP.NET Web Forms (System.Web.UI.MasterPage) to ASP.NET Core Razor Pages Layout
// Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages
// The Master Page functionality is now handled by _PatientLayout.cshtml (Razor Pages layout)

namespace DBProject.Patient
{
    /// <summary>
    /// Base PageModel for Patient pages using the shared _PatientLayout.cshtml layout.
    /// Replaces the Web Forms PatientMaster MasterPage with an ASP.NET Core Razor Pages layout.
    /// </summary>
    public class PatientMasterModel : PageModel
    {
        public void OnGet()
        {
            // Layout-level initialization handled by _PatientLayout.cshtml
        }
    }
}
