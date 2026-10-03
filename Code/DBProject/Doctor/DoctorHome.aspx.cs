// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (DoctorHome.aspx.cs) was the Web Forms code-behind for DoctorHome.aspx.
// It has been migrated to ASP.NET Core MVC:
//
//   • DoctorHome.aspx    → Views/Doctor/DoctorHome.cshtml  (Razor View)
//   • DoctorHome.aspx.cs → Controllers/DoctorHomeController.cs (MVC Controller)
//   • (new)              → Models/DoctorHomeViewModel.cs   (ViewModel)
//
// Web Forms patterns removed / replaced:
//   Line 5  – using System.Web.UI;              → removed (not in ASP.NET Core)
//   Line 6  – using System.Web.UI.WebControls;  → removed (not in ASP.NET Core)
//   Line 14 – System.Web.UI.Page base class     → Controller base class
//   Line 16 – Page_Load / Session["idoriginal"] → DoctorHomeController.Index() [HttpGet]
//             Label1..Label14.Text assignments  → DoctorHomeViewModel properties
//
// All business logic (docinfo_DAL) has been preserved in
// Controllers/DoctorHomeController.cs.
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0045 – Session State Provider):
// In-process HttpSessionState (InProc) usage has been replaced with
// Amazon ElastiCache for Redis distributed session in DoctorHomeController.cs.
//
//   Line 21 – Session["idoriginal"]  → HttpContext.Session.GetInt32("idoriginal")
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
    // The active implementation is in Controllers/DoctorHomeController.cs.
    // Web Forms base class (System.Web.UI.Page) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    [Obsolete("Migrated to Controllers/DoctorHomeController.cs (cr-dotnet-0026, cr-dotnet-0045)")]
    public class doctorhome_Legacy
    {
        // Original Page_Load → DoctorHomeController.Index() [HttpGet]
        // Label1..Label14    → DoctorHomeViewModel.Name, Phone, Address, etc.
        //
        // cr-dotnet-0045: Session["idoriginal"] now uses Redis-backed
        // distributed session in DoctorHomeController.cs.
    }
}
