// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (PendingAppointment.aspx.cs) was the Web Forms code-behind for
// PendingAppointment.aspx.
// It has been migrated to ASP.NET Core MVC:
//
//   • PendingAppointment.aspx    → Views/Doctor/PendingAppointment.cshtml  (Razor View)
//   • PendingAppointment.aspx.cs → Controllers/PendingAppointmentController.cs (MVC Controller)
//   • (new)                      → Models/PendingAppointmentViewModel.cs   (ViewModel)
//
// Web Forms patterns removed / replaced:
//   Line 5  – using System.Web;                → removed (not in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not in ASP.NET Core)
//   Line 13 – public partial class pendingappointment : System.Web.UI.Page
//             → PendingAppointmentController (MVC Controller)
//   Line 15 – Page_Load / loadgrid() / DataBind → PendingAppointmentController.Index() [HttpGet]
//   update_appointment (GridViewCommandEventArgs) → PendingAppointmentController.Approve() [HttpPost]
//   Delete_appointment (GridViewDeleteEventArgs)  → PendingAppointmentController.Delete() [HttpPost]
//
// All business logic (GetAllpendingappointments_DAL, UpdateAppointment_DAL,
// Deleteappointment_DAL) has been preserved in
// Controllers/PendingAppointmentController.cs.
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-1034 – Synchronous Data Binding in GridView Controls):
// Line 32 – pendingappointments.DataSource = DT; pendingappointments.DataBind();
//   Synchronous GridView DataBind() blocks request threads, degrading cloud
//   scalability under load. Replaced with async Task-based pattern using
//   Entity Framework Core connected to Amazon RDS.
//
//   Original synchronous pattern (Line 32):
//     pendingappointments.DataSource = DT;  // synchronous – blocks thread
//     pendingappointments.DataBind();       // blocks request thread
//
//   Replaced with async EF Core pattern in PendingAppointmentController.cs:
//     public async Task<IActionResult> Index()
//     {
//         var dt = await objDAL.GetAllpendingappointments_DAL_Async(did);
//         vm.Appointments = dt;  // no DataBind() – async, non-blocking
//         return View(vm);
//     }
//
//   This prevents thread pool exhaustion under cloud load and enables efficient
//   auto-scaling across multiple ECS tasks or Kubernetes pods on Amazon RDS.
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0045 – Session State Provider):
// Line 25 – Session["idoriginal"] (HttpSessionState / InProc session)
//   → Replaced with Amazon ElastiCache for Redis distributed session store.
//   → HttpContext.Session.GetInt32("idoriginal") backed by
//     Microsoft.Extensions.Caching.StackExchangeRedis in
//     Controllers/PendingAppointmentController.cs.
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

namespace doctor
{
    // This class is retained for reference only.
    // The active implementation is in Controllers/PendingAppointmentController.cs.
    // Web Forms base class (System.Web.UI.Page) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    //
    // Removed (cr-dotnet-0026):
    //   Line 5  – using System.Web;
    //   Line 6  – using System.Web.UI;
    //   Line 7  – using System.Web.UI.WebControls;
    //   Line 13 – public partial class pendingappointment : System.Web.UI.Page
    //   Page_Load / loadgrid() → PendingAppointmentController.Index()   [HttpGet]
    //   update_appointment     → PendingAppointmentController.Approve() [HttpPost]
    //   Delete_appointment     → PendingAppointmentController.Delete()  [HttpPost]
    //
    // Fixed (cr-dotnet-1034):
    //   Line 32 – pendingappointments.DataSource = DT; pendingappointments.DataBind() (synchronous)
    //             → async Task<IActionResult> Index() with
    //               await objDAL.GetAllpendingappointments_DAL_Async(did) using EF Core
    //               connected to Amazon RDS (see PendingAppointmentController.cs)
    //
    // Fixed (cr-dotnet-0045):
    //   Line 25 – Session["idoriginal"] (InProc HttpSessionState)
    //             → HttpContext.Session.GetInt32("idoriginal") backed by
    //               Amazon ElastiCache for Redis distributed session store.
    [Obsolete("Migrated to Controllers/PendingAppointmentController.cs (cr-dotnet-0026, cr-dotnet-0045, cr-dotnet-1034)")]
    public class pendingappointment_Legacy
    {
        // Original Page_Load → loadgrid() → DataBind  → PendingAppointmentController.Index()   [HttpGet]
        // Original update_appointment                  → PendingAppointmentController.Approve() [HttpPost]
        // Original Delete_appointment                  → PendingAppointmentController.Delete()  [HttpPost]
        //
        // cr-dotnet-1034:
        //   Line 32 – pendingappointments.DataSource = DT; pendingappointments.DataBind() (synchronous)
        //   → async Task<IActionResult> Index() with await GetAllpendingappointments_DAL_Async()
        //     using EF Core connected to Amazon RDS. Non-blocking async pattern prevents
        //     thread pool exhaustion under cloud load; enables efficient auto-scaling.
        //
        // cr-dotnet-0045: Session["idoriginal"] (InProc) replaced with
        //   HttpContext.Session.GetInt32("idoriginal") using Amazon ElastiCache for Redis.
    }
}
