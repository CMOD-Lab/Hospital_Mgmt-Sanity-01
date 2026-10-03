// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (Bill.aspx.cs) was the Web Forms code-behind for Bill.aspx.
// It has been migrated to ASP.NET Core MVC:
//
//   • Bill.aspx      → Views/Doctor/Bill.cshtml  (Razor View)
//   • Bill.aspx.cs   → Controllers/BillController.cs (MVC Controller)
//   • (new)          → Models/BillViewModel.cs   (ViewModel)
//
// Web Forms patterns removed / replaced:
//   Line 5  – using System.Web.UI;              → removed (not in ASP.NET Core)
//   Line 6  – using System.Web.UI.WebControls;  → removed (not in ASP.NET Core)
//   Line 12 – System.Web.UI.Page base class     → Controller base class
//   Line 14 – public partial class bill         → BillController (MVC Controller)
//   Line 13 – Page_Load / Session access        → BillController.Index() [HttpGet]
//   Line 30 – bill_paid() / Response.Redirect   → BillController.BillPaid() [HttpPost]
//   Line 42 – bill_Unpaid() / Response.Redirect → BillController.BillUnpaid() [HttpPost]
//
// All business logic (generate_bill_DAL, paid_bill_DAL, Unpaid_bill_DAL)
// has been preserved in Controllers/BillController.cs.
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0045 – Session State Provider):
// In-process HttpSessionState (InProc) usage has been replaced with
// Amazon ElastiCache for Redis distributed session in BillController.cs.
//
//   Line 21 – Session["idoriginal"]  → HttpContext.Session.GetInt32("idoriginal")
//   Line 38 – Session["idoriginal"]  → HttpContext.Session.GetInt32("idoriginal")
//   Line 39 – Session["appointid"]   → HttpContext.Session.GetInt32("appointid")
//   Line 51 – Session["idoriginal"]  → HttpContext.Session.GetInt32("idoriginal")
//   Line 52 – Session["appointid"]   → HttpContext.Session.GetInt32("appointid")
//
// Session is now stored in Amazon ElastiCache for Redis, enabling stateless
// horizontal scaling across multiple ECS tasks or Kubernetes pods.
// Redis connection is configured via REDIS_CONNECTION_STRING environment variable.
// ============================================================================

using System;
using DBProject.DAL;
using System.Data;

namespace doctor
{
    // This class is retained for reference only.
    // The active implementation is in Controllers/BillController.cs.
    // Web Forms base class (System.Web.UI.Page) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    [Obsolete("Migrated to Controllers/BillController.cs (cr-dotnet-0026, cr-dotnet-0045)")]
    public class bill_Legacy
    {
        // Original Page_Load  → BillController.Index()      [HttpGet]
        // Original bill_paid  → BillController.BillPaid()   [HttpPost]
        // Original bill_Unpaid→ BillController.BillUnpaid() [HttpPost]
        //
        // cr-dotnet-0045: All Session["idoriginal"] and Session["appointid"]
        // accesses now use Redis-backed distributed session in BillController.cs.
    }
}
