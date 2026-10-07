using Microsoft.AspNetCore.Mvc.RazorPages;

// Migrated from ASP.NET Web Forms (System.Web.UI.MasterPage) to ASP.NET Core Razor Pages Layout
// Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages
// The Master Page concept is replaced by a Razor Pages Layout (_DoctorLayout.cshtml)

namespace doctor
{
    // The MasterPage class is no longer needed in ASP.NET Core Razor Pages.
    // Layout functionality is handled by _DoctorLayout.cshtml.
    // This file is retained as a placeholder for backward compatibility.
    public class DoctorMasterLayout : PageModel
    {
        public void OnGet()
        {
            // Layout pages do not have page models in ASP.NET Core Razor Pages.
        }
    }
}
