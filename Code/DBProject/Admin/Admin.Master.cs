using Microsoft.AspNetCore.Mvc.RazorPages;

// Migrated from ASP.NET Web Forms Master Page (System.Web.UI.MasterPage) to ASP.NET Core Razor Pages Layout
// Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages
// The Admin.Master Web Forms master page has been replaced by Admin/_AdminLayout.cshtml (Razor Layout)

namespace DBProject
{
    // Previously: public partial class Admin : System.Web.UI.MasterPage
    // Now represented as a Razor Layout page (_AdminLayout.cshtml) in ASP.NET Core.
    // This file is retained for reference; layout logic lives in Admin.Master (renamed to _AdminLayout.cshtml).
    public class AdminLayout : PageModel
    {
        public void OnGet()
        {
        }
    }
}
