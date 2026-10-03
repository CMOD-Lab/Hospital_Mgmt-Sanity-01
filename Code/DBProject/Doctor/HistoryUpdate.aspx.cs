// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (HistoryUpdate.aspx.cs) was the Web Forms code-behind for
// HistoryUpdate.aspx.
// It has been migrated to ASP.NET Core MVC:
//
//   • HistoryUpdate.aspx    → Views/Doctor/HistoryUpdate.cshtml  (Razor View)
//   • HistoryUpdate.aspx.cs → Controllers/HistoryUpdateController.cs (MVC Controller)
//   • (new)                 → Models/HistoryUpdateViewModel.cs   (ViewModel)
//
// Web Forms patterns removed / replaced:
//   Line 5  – using System.Web.UI;              → removed (not in ASP.NET Core)
//   Line 6  – using System.Web.UI.WebControls;  → removed (not in ASP.NET Core)
//   Line 12 – System.Web.UI.Page base class     → Controller base class
//   Line 14 – public partial class Historyupdate : System.Web.UI.Page
//             → HistoryUpdateController (MVC Controller)
//   Page_Load (empty)                           → no equivalent needed
//   saveindatabase() / Session / Response.Write → HistoryUpdateController.Save() [HttpPost]
//   generate_bill() / Response.Redirect         → HistoryUpdateController.GenerateBill() [HttpPost]
//
// All business logic (update_prescription_DAL) has been preserved in
// Controllers/HistoryUpdateController.cs.
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0045 – Session State Provider):
// In-process HttpSessionState (InProc) usage has been replaced with
// Amazon ElastiCache for Redis distributed session in HistoryUpdateController.cs.
//
//   Line 23 – Session["idoriginal"]  → HttpContext.Session.GetInt32("idoriginal")
//   Line 28 – Session["appointid"]   → HttpContext.Session.GetInt32("appointid")
//
// Session is now stored in Amazon ElastiCache for Redis, enabling stateless
// horizontal scaling across multiple ECS tasks or Kubernetes pods.
// Redis connection is configured via REDIS_CONNECTION_STRING environment variable.
// ============================================================================

using System;
using DBProject.DAL;

namespace doctor
{
    // This class is retained for reference only.
    // The active implementation is in Controllers/HistoryUpdateController.cs.
    // Web Forms base class (System.Web.UI.Page) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    //
    // Removed (cr-dotnet-0026):
    //   Line 5  – using System.Web.UI;
    //   Line 6  – using System.Web.UI.WebControls;
    //   Line 12 – public partial class Historyupdate : System.Web.UI.Page
    //   Line 14 – protected void Page_Load(object sender, EventArgs e) { }
    [Obsolete("Migrated to Controllers/HistoryUpdateController.cs (cr-dotnet-0026, cr-dotnet-0045)")]
    public class Historyupdate_Legacy
    {
        // Original Page_Load (empty)  → no equivalent needed
        // Original saveindatabase()   → HistoryUpdateController.Save()        [HttpPost]
        // Original generate_bill()    → HistoryUpdateController.GenerateBill() [HttpPost]
        //
        // cr-dotnet-0045: Session["idoriginal"] (Line 23) and Session["appointid"] (Line 28)
        // now use Redis-backed distributed session in HistoryUpdateController.cs.
    }
}
