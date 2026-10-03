// Migrated from ASP.NET Web Forms Master Page code-behind to ASP.NET Core Razor Pages (cr-dotnet-0026)
// Original violations fixed:
//   Line 5:  using System.Web.UI;             -> removed (Web Forms namespace)
//   Line 6:  using System.Web.UI.WebControls; -> removed (Web Forms namespace)
//   Line 10: public partial class Admin : System.Web.UI.MasterPage -> replaced with RazorLayoutHelper
//   Line 12: protected void Page_Load(...)    -> removed (no equivalent needed in Razor Layout)
//
// In ASP.NET Core Razor Pages, Master Pages are replaced by Layout files (_AdminLayout.cshtml).
// The layout file itself contains no code-behind class; any shared logic that was previously
// in the MasterPage code-behind is placed in a base PageModel or a shared service.
// This file is retained as a marker/documentation class and can be removed once the
// project is fully migrated to ASP.NET Core.

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DBProject.Admin
{
    /// <summary>
    /// Replaces the legacy Web Forms Admin : System.Web.UI.MasterPage code-behind.
    ///
    /// In ASP.NET Core Razor Pages the Master Page concept is replaced by a Razor
    /// Layout file (_AdminLayout.cshtml).  Shared page initialisation logic that was
    /// previously placed in Page_Load of the MasterPage should be moved to:
    ///   - A base PageModel class (AdminPageModelBase) that all Admin PageModels inherit, or
    ///   - A shared service registered in the DI container.
    ///
    /// The original Page_Load body was empty, so no logic migration is required here.
    /// </summary>
    public class AdminLayoutModel : PageModel
    {
        // Shared initialisation for all Admin pages can be placed here.
        // Individual Admin PageModels (e.g. AddStaffModel) inherit from this class
        // to gain access to shared services and layout-level logic.
        public void OnGet()
        {
            // No shared initialisation required (original Page_Load was empty).
        }
    }
}
