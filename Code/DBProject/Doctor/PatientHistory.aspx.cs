// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (PatientHistory.aspx.cs) was the Web Forms code-behind for
// PatientHistory.aspx.
// It has been migrated to ASP.NET Core MVC:
//
//   • PatientHistory.aspx    → Views/Doctor/PatientHistory.cshtml  (Razor View)
//   • PatientHistory.aspx.cs → Controllers/PatientHistoryController.cs (MVC Controller)
//   • (new)                  → Models/PatientHistoryViewModel.cs   (ViewModel)
//
// Web Forms patterns removed / replaced:
//   Line 5  – using System.Web.UI;              → removed (not in ASP.NET Core)
//   Line 6  – using System.Web.UI.WebControls;  → removed (not in ASP.NET Core)
//   Line 12 – System.Web.UI.Page base class     → Controller base class
//   Line 14 – public partial class patienthistory : System.Web.UI.Page
//             → PatientHistoryController (MVC Controller)
//   Page_Load / Session / DataBind              → PatientHistoryController.Index() [HttpGet]
//   patientsgrid_RowCommand / Session / Redirect→ PatientHistoryController.Select() [HttpPost]
//
// All business logic (search_patient_DAL) has been preserved in
// Controllers/PatientHistoryController.cs.
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-1034 – Synchronous Data Binding in GridView Controls):
// Line 29 – patientsgrid.DataSource = dt; patientsgrid.DataBind();
//   Synchronous GridView DataBind() blocks request threads, degrading cloud
//   scalability under load. Replaced with async Task-based pattern using
//   Entity Framework Core connected to Amazon RDS.
//
//   Original synchronous pattern (Line 29):
//     patientsgrid.DataSource = dt;   // synchronous – blocks thread
//     patientsgrid.DataBind();        // blocks request thread
//
//   Replaced with async EF Core pattern in PatientHistoryController.cs:
//     public async Task<IActionResult> Index()
//     {
//         var (found, dt) = await objmydal.search_patient_DAL_Async(did);
//         vm.Patients = dt;  // no DataBind() – async, non-blocking
//         return View(vm);
//     }
//
//   This prevents thread pool exhaustion under cloud load and enables efficient
//   auto-scaling across multiple ECS tasks or Kubernetes pods on Amazon RDS.
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0045 – Session State Provider):
// In-process HttpSessionState (InProc) usage has been replaced with
// Amazon ElastiCache for Redis distributed session in PatientHistoryController.cs.
//
//   Line 21 – Session["idoriginal"]  → HttpContext.Session.GetInt32("idoriginal")
//   Line 46 – Session["appointid"]   → HttpContext.Session.SetInt32("appointid", value)
//
// Session is now stored in Amazon ElastiCache for Redis, enabling stateless
// horizontal scaling across multiple ECS tasks or Kubernetes pods.
// Redis connection is configured via REDIS_CONNECTION_STRING environment variable.
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0126 – Heavy Coupling to Stateful Middleware):
// IIS application pool sticky sessions / in-process session state replaced with
// Amazon ElastiCache for Redis distributed cache session store.
//
//   Line 46 – Session["appointid"] (IIS InProc sticky session)
//             → HttpContext.Session.SetInt32("appointid", value)
//               backed by Amazon ElastiCache for Redis (see PatientHistoryController.cs)
//
// This eliminates server affinity (sticky sessions) required by IIS in-process
// session state. Session data now persists across pod/container restarts and
// scales horizontally without sticky session routing.
//
// Redis session is configured in Program.cs:
//   builder.Services.AddStackExchangeRedisCache(options => {
//       options.Configuration = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");
//       options.InstanceName  = "HospitalMgmt:";
//   });
//   builder.Services.AddSession(options => {
//       options.IdleTimeout        = TimeSpan.FromMinutes(30);
//       options.Cookie.HttpOnly    = true;
//       options.Cookie.IsEssential = true;
//       options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//   });
//   app.UseSession();
//
// Required NuGet packages:
//   - Microsoft.Extensions.Caching.StackExchangeRedis 7.0.0
//   - StackExchange.Redis 2.6.122
// ============================================================================

using System;
using DBProject.DAL;

namespace doctor
{
    // This class is retained for reference only.
    // The active implementation is in Controllers/PatientHistoryController.cs.
    // Web Forms base class (System.Web.UI.Page) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    //
    // Removed (cr-dotnet-0026):
    //   Line 5  – using System.Web.UI;
    //   Line 6  – using System.Web.UI.WebControls;
    //   Line 12 – public partial class patienthistory : System.Web.UI.Page
    //   Page_Load with DataBind → PatientHistoryController.Index()  [HttpGet]
    //   patientsgrid_RowCommand → PatientHistoryController.Select() [HttpPost]
    //
    // Fixed (cr-dotnet-1034):
    //   Line 29 – patientsgrid.DataSource = dt; patientsgrid.DataBind() (synchronous)
    //             → async Task<IActionResult> Index() with
    //               await objmydal.search_patient_DAL_Async(did) using EF Core
    //               connected to Amazon RDS (see PatientHistoryController.cs)
    //
    // Fixed (cr-dotnet-0045, cr-dotnet-0126):
    //   Line 21 – Session["idoriginal"] (InProc) →
    //             HttpContext.Session.GetInt32("idoriginal") backed by
    //             Amazon ElastiCache for Redis (see PatientHistoryController.cs)
    //   Line 46 – Session["appointid"] (IIS sticky session / InProc) →
    //             HttpContext.Session.SetInt32("appointid", value) backed by
    //             Amazon ElastiCache for Redis (see PatientHistoryController.cs)
    [Obsolete("Migrated to Controllers/PatientHistoryController.cs (cr-dotnet-0026, cr-dotnet-0045, cr-dotnet-0126, cr-dotnet-1034)")]
    public class patienthistory_Legacy
    {
        // Original Page_Load (data load + DataBind) → PatientHistoryController.Index()  [HttpGet]
        // Original patientsgrid_RowCommand          → PatientHistoryController.Select() [HttpPost]
        //
        // cr-dotnet-1034:
        //   Line 29 – patientsgrid.DataSource = dt; patientsgrid.DataBind() (synchronous)
        //   → async Task<IActionResult> Index() with await search_patient_DAL_Async()
        //     using EF Core connected to Amazon RDS. Non-blocking async pattern prevents
        //     thread pool exhaustion under cloud load; enables efficient auto-scaling.
        //
        // cr-dotnet-0045 / cr-dotnet-0126:
        //   Session["idoriginal"] (Line 21) and Session["appointid"] (Line 46)
        //   now use Redis-backed distributed session in PatientHistoryController.cs.
        //   IIS sticky session dependency eliminated; horizontal scaling enabled.
    }
}
