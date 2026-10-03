// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (DoctorMaster.Master.cs) was the Web Forms code-behind for
// DoctorMaster.Master (the doctor section Master Page).
// It has been migrated to ASP.NET Core MVC Razor Layout:
//
//   • DoctorMaster.Master    → Views/Shared/_DoctorLayout.cshtml  (Razor Layout)
//   • DoctorMaster.Master.cs → (no code-behind needed in ASP.NET Core MVC)
//
// Web Forms patterns removed / replaced:
//   Line 6  – using System.Web.UI;              → removed (not in ASP.NET Core)
//   Line 10 – System.Web.UI.MasterPage base class → Razor Layout (no base class)
//   Line 12 – Page_Load (empty)                 → no equivalent needed
//
// In ASP.NET Core MVC, layout files are plain Razor (.cshtml) files and do not
// require a code-behind class. Any shared logic previously placed in a Master
// Page code-behind should be moved to a base Controller, ViewComponent, or
// Tag Helper as appropriate.
// ============================================================================

using System;

namespace doctor
{
    // This class is retained for reference only.
    // The active layout is Views/Shared/_DoctorLayout.cshtml.
    // Web Forms MasterPage base class (System.Web.UI.MasterPage) and all
    // server-control references have been removed as part of the
    // ASP.NET Core MVC migration.
    //
    // Removed (cr-dotnet-0026):
    //   Line 6  – using System.Web.UI;
    //   Line 10 – public partial class doctormaster : System.Web.UI.MasterPage
    //   Line 12 – protected void Page_Load(object sender, EventArgs e) { }
    [Obsolete("Migrated to Views/Shared/_DoctorLayout.cshtml (cr-dotnet-0026)")]
    public class doctormaster_Legacy
    {
        // Original Page_Load (empty) → no equivalent needed in Razor Layout
    }
}
