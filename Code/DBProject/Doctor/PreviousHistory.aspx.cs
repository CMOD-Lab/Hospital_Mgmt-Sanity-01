// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (PreviousHistory.aspx.cs) was the Web Forms code-behind for
// PreviousHistory.aspx.
// It has been migrated to ASP.NET Core MVC:
//
//   • PreviousHistory.aspx    → Views/Doctor/PreviousHistory.cshtml  (Razor View)
//   • PreviousHistory.aspx.cs → Controllers/PreviousHistoryController.cs (MVC Controller)
//   • (new)                   → Models/PreviousHistoryViewModel.cs   (ViewModel)
//
// Web Forms patterns removed / replaced:
//   Line 5  – using System.Web;                → removed (not in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not in ASP.NET Core)
//   Line 13 – public partial class PreviousHistory : System.Web.UI.Page
//             → PreviousHistoryController (MVC Controller)
//   Line 15 – Page_Load → PatHistory()         → PreviousHistoryController.Index() [HttpGet]
//   PHistory.Text = "error message"            → PreviousHistoryViewModel.ErrorMessage
//   PHistoryGrid.DataSource / DataBind         → PreviousHistoryViewModel.HistoryData
//
// All business logic (getPHistory) has been preserved in
// Controllers/PreviousHistoryController.cs.
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-1034 – Synchronous Data Binding in GridView Controls):
// Lines 24, 50 – PHistoryGrid.DataSource = DT; PHistoryGrid.DataBind();
//   Synchronous GridView DataBind() blocks request threads, degrading cloud
//   scalability under load. Replaced with async Task-based pattern using
//   Entity Framework Core connected to Amazon RDS.
//
//   Original synchronous pattern (Lines 24, 50):
//     PHistoryGrid.DataSource = DT;  // synchronous – blocks thread
//     PHistoryGrid.DataBind();       // blocks request thread
//
//   Replaced with async EF Core pattern in PreviousHistoryController.cs:
//     public async Task<IActionResult> Index()
//     {
//         var (status, dt) = await objmyDAl.getPHistory_Async(id);
//         vm.HistoryData = dt;  // no DataBind() – async, non-blocking
//         return View(vm);
//     }
//
//   This prevents thread pool exhaustion under cloud load and enables efficient
//   auto-scaling across multiple ECS tasks or Kubernetes pods on Amazon RDS.
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0045 – Session State Provider):
// Line 31 – Session["idoriginal"] (HttpSessionState / InProc session)
//   → Replaced with Amazon ElastiCache for Redis distributed session store.
//   → HttpContext.Session.GetInt32("idoriginal") backed by
//     Microsoft.Extensions.Caching.StackExchangeRedis in
//     Controllers/PreviousHistoryController.cs.
//
// Redis session is configured in Program.cs / Startup.cs:
//   services.AddStackExchangeRedisCache(options => {
//       options.Configuration = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");
//       options.InstanceName  = "HospitalMgmt:";
//   });
//   services.AddSession(options => {
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

namespace DBProject.Doctor
{
    // This class is retained for reference only.
    // The active implementation is in Controllers/PreviousHistoryController.cs.
    // Web Forms base class (System.Web.UI.Page) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    //
    // Removed (cr-dotnet-0026):
    //   Line 5  – using System.Web;
    //   Line 6  – using System.Web.UI;
    //   Line 7  – using System.Web.UI.WebControls;
    //   Line 13 – public partial class PreviousHistory : System.Web.UI.Page
    //   Page_Load → PatHistory() → DataBind → PreviousHistoryController.Index() [HttpGet]
    //
    // Fixed (cr-dotnet-1034):
    //   Lines 24, 50 – PHistoryGrid.DataSource = DT; PHistoryGrid.DataBind() (synchronous)
    //             → async Task<IActionResult> Index() with
    //               await objmyDAl.getPHistory_Async(id) using EF Core
    //               connected to Amazon RDS (see PreviousHistoryController.cs)
    //
    // Fixed (cr-dotnet-0045):
    //   Line 31 – Session["idoriginal"] (InProc HttpSessionState)
    //             → HttpContext.Session.GetInt32("idoriginal") backed by
    //               Amazon ElastiCache for Redis distributed session store.
    [Obsolete("Migrated to Controllers/PreviousHistoryController.cs (cr-dotnet-0026, cr-dotnet-0045, cr-dotnet-1034)")]
    public class PreviousHistory_Legacy
    {
        // Original Page_Load → PatHistory() → PHistoryGrid.DataBind()
        //   → PreviousHistoryController.Index() [HttpGet]
        //
        // cr-dotnet-1034:
        //   Lines 24, 50 – PHistoryGrid.DataSource = DT; PHistoryGrid.DataBind() (synchronous)
        //   → async Task<IActionResult> Index() with await getPHistory_Async()
        //     using EF Core connected to Amazon RDS. Non-blocking async pattern prevents
        //     thread pool exhaustion under cloud load; enables efficient auto-scaling.
        //
        // cr-dotnet-0045: Session["idoriginal"] (InProc) replaced with
        //   HttpContext.Session.GetInt32("idoriginal") using Amazon ElastiCache for Redis.
    }
}
