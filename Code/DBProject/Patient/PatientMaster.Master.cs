// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (PatientMaster.Master.cs) was the Web Forms code-behind for
// PatientMaster.Master (Master Page).
// It has been migrated to ASP.NET Core MVC Razor Layout:
//
//   • PatientMaster.Master    → Views/Shared/_PatientLayout.cshtml (Razor Layout)
//   • PatientMaster.Master.cs → (no equivalent; layout has no code-behind in MVC)
//
// Web Forms patterns removed / replaced (cr-dotnet-0026):
//   using System.Web;                → removed (not in ASP.NET Core)
//   using System.Web.UI;             → removed (not in ASP.NET Core)
//   using System.Web.UI.WebControls; → removed (not in ASP.NET Core)
//   public partial class PatientMaster : System.Web.UI.MasterPage
//                                    → (no equivalent class; Razor layout is a .cshtml file)
//   Page_Load (empty)                → (no equivalent; layout has no lifecycle events)
//
// The active Razor layout is at Views/Shared/_PatientLayout.cshtml.
// ============================================================================

using System;

namespace DBProject
{
    // This class is retained for reference only.
    // The active implementation is in Views/Shared/_PatientLayout.cshtml.
    // Web Forms base class (System.Web.UI.MasterPage) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    //
    // Removed (cr-dotnet-0026):
    //   using System.Web;
    //   using System.Web.UI;
    //   using System.Web.UI.WebControls;
    //   public partial class PatientMaster : System.Web.UI.MasterPage
    //   Page_Load (empty) → no equivalent in Razor layout
    [Obsolete("Migrated to Views/Shared/_PatientLayout.cshtml (cr-dotnet-0026)")]
    public class PatientMaster_Legacy
    {
        // Original Page_Load was empty – no business logic to preserve.
        // The layout structure (navigation, footer, content placeholders) has been
        // fully migrated to Views/Shared/_PatientLayout.cshtml.
    }
}
