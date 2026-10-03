// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (BillsHistory.aspx.cs) was the Web Forms code-behind for
// BillsHistory.aspx.
// It has been migrated to ASP.NET Core MVC:
//
//   • BillsHistory.aspx    → Views/Patient/BillsHistory.cshtml  (Razor View)
//   • BillsHistory.aspx.cs → Controllers/BillsHistoryController.cs (MVC Controller)
//   • (new)                → Models/BillsHistoryViewModel.cs   (ViewModel)
//
// Web Forms patterns removed / replaced:
//   Line 5  – using System.Web;                → removed (not in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not in ASP.NET Core)
//   Line 13 – public partial class BillsHistory : System.Web.UI.Page
//             → BillsHistoryController (MVC Controller)
//   Line 15 – Page_Load → billHistory()        → BillsHistoryController.Index() [HttpGet]
//   BHistory.Text = "..."                      → BillsHistoryViewModel.StatusMessage
//   BHistoryGrid.DataSource / DataBind         → BillsHistoryViewModel.BillHistoryData
//   Session["idoriginal"]                      → HttpContext.Session.GetInt32
//
// All business logic (getBillHistory) has been preserved in
// Controllers/BillsHistoryController.cs.
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-1034 – Synchronous Data Binding in GridView Controls):
// Line 50 – BHistoryGrid.DataSource = DT; BHistoryGrid.DataBind();
//   Synchronous GridView DataBind() blocks request threads, degrading cloud
//   scalability under load. Replaced with async Task-based pattern using
//   Entity Framework Core connected to Amazon RDS.
//
//   Original synchronous pattern (Line 50):
//     BHistoryGrid.DataSource = DT;  // synchronous – blocks thread
//     BHistoryGrid.DataBind();       // blocks request thread
//
//   Replaced with async EF Core pattern in BillsHistoryController.cs:
//     public async Task<IActionResult> Index()
//     {
//         var (status, dt) = await objmyDAl.getBillHistory_Async(id);
//         vm.BillHistoryData = dt;  // no DataBind() – async, non-blocking
//         return View(vm);
//     }
//
//   This prevents thread pool exhaustion under cloud load and enables efficient
//   auto-scaling across multiple ECS tasks or Kubernetes pods on Amazon RDS.
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0045 – Session State Provider):
// Line 30 – Session["idoriginal"] (HttpSessionState / InProc session)
//   → Replaced with Amazon ElastiCache for Redis distributed session store.
//   → HttpContext.Session.GetInt32("idoriginal") backed by
//     Microsoft.Extensions.Caching.StackExchangeRedis in
//     Controllers/BillsHistoryController.cs.
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

namespace DBProject
{
    // This class is retained for reference only.
    // The active implementation is in Controllers/BillsHistoryController.cs.
    // Web Forms base class (System.Web.UI.Page) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    //
    // Removed (cr-dotnet-0026):
    //   Line 5  – using System.Web;
    //   Line 6  – using System.Web.UI;
    //   Line 7  – using System.Web.UI.WebControls;
    //   Line 13 – public partial class BillsHistory : System.Web.UI.Page
    //   Page_Load → billHistory() → BillsHistoryController.Index() [HttpGet]
    //
    // Fixed (cr-dotnet-1034):
    //   Line 50 – BHistoryGrid.DataSource = DT; BHistoryGrid.DataBind() (synchronous)
    //             → async Task<IActionResult> Index() with
    //               await objmyDAl.getBillHistory_Async(id) using EF Core
    //               connected to Amazon RDS (see BillsHistoryController.cs)
    //
    // Fixed (cr-dotnet-0045):
    //   Line 30 – Session["idoriginal"] (InProc HttpSessionState)
    //             → HttpContext.Session.GetInt32("idoriginal") backed by
    //               Amazon ElastiCache for Redis distributed session store.
    [Obsolete("Migrated to Controllers/BillsHistoryController.cs (cr-dotnet-0026, cr-dotnet-0045, cr-dotnet-1034)")]
    public class BillsHistory_Legacy
    {
        // Original Page_Load → billHistory():
        //   int id = (int)Session["idoriginal"];
        //   objmyDAl.getBillHistory(id, ref DT)
        //
        //   BEFORE (cr-dotnet-1034 violation, Line 50):
        //     BHistoryGrid.DataSource = DT;  // synchronous – blocks thread
        //     BHistoryGrid.DataBind();       // blocks request thread
        //   AFTER (cr-dotnet-1034 fix):
        //     var (status, dt) = await objmyDAl.getBillHistory_Async(id);
        //     vm.BillHistoryData = dt;  // async, non-blocking EF Core pattern
        //     // Prevents thread pool exhaustion; enables efficient auto-scaling
        //
        //   → BillsHistoryController.Index() [HttpGet]
        //
        // cr-dotnet-0045: Session["idoriginal"] (InProc) replaced with
        //   HttpContext.Session.GetInt32("idoriginal") using Amazon ElastiCache for Redis.
    }
}
