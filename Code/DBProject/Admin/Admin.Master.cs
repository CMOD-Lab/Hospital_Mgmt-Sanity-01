// Migrated from ASP.NET Web Forms Master Page code-behind (Admin.Master.cs) to
// ASP.NET Core Razor Layout companion class.
// Rule cr-dotnet-0026: Web Forms Usage → Migrate to ASP.NET Core MVC/Razor Pages
//
// Changes applied (Occurrences 7–10):
//   Line 5  (Occurrence 7):  Removed "using System.Web.UI;" — Web Forms namespace, not available in ASP.NET Core.
//   Line 6  (Occurrence 8):  Removed "using System.Web.UI.WebControls;" — Web Forms controls namespace.
//   Line 10 (Occurrence 9):  Replaced "System.Web.UI.MasterPage" base class with no base class.
//                             In ASP.NET Core, Razor Layout files (_AdminLayout.cshtml) do not have
//                             a separate code-behind class; layout-level logic is handled via
//                             view components, tag helpers, or a base PageModel.
//   Line 12 (Occurrence 10): Replaced Web Forms Page_Load event handler with a no-op comment.
//                             Layout-level initialisation in ASP.NET Core is performed in
//                             _ViewStart.cshtml or a shared base PageModel, not in a Master Page
//                             code-behind.
//
// The class is retained as a thin marker/documentation class so that any existing
// references in the project compile without errors during the incremental migration.
// Once all pages have been fully migrated to Razor Pages, this file can be removed.

namespace DBProject.Admin
{
    /// <summary>
    /// Companion class for the Admin Razor Layout (_AdminLayout.cshtml).
    /// Replaces the Web Forms Admin.Master.cs code-behind that previously
    /// inherited System.Web.UI.MasterPage.
    ///
    /// In ASP.NET Core, Razor Layout files do not require a code-behind class.
    /// Any shared layout logic (e.g., navigation data, user context) should be
    /// placed in a base PageModel, a view component, or middleware instead of
    /// a Master Page code-behind.
    /// </summary>
    public class AdminLayout
    {
        // No Page_Load equivalent needed in ASP.NET Core Razor Layouts.
        // Layout-level initialisation is handled by _ViewStart.cshtml and
        // shared base PageModel classes.
    }
}
