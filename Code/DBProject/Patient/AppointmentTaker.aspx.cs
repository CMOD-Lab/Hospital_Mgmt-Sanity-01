// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (AppointmentTaker.aspx.cs) was the Web Forms code-behind for
// AppointmentTaker.aspx.
// It has been migrated to ASP.NET Core MVC:
//
//   • AppointmentTaker.aspx    → Views/Patient/AppointmentTaker.cshtml  (Razor View)
//   • AppointmentTaker.aspx.cs → Controllers/AppointmentTakerController.cs (MVC Controller)
//   • (new)                    → Models/AppointmentTakerViewModel.cs   (ViewModel)
//
// Web Forms patterns removed / replaced:
//   Line 5  – using System.Web;                → removed (not in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not in ASP.NET Core)
//   Line 13 – public partial class AppointmentTaker : System.Web.UI.Page
//             → AppointmentTakerController (MVC Controller)
//   Line 15 – Page_Load → freeSlots()          → AppointmentTakerController.Index() [HttpGet]
//   PAppointmentGrid_RowCommand (GridView row select)
//             → AppointmentTakerController.SelectSlot() [HttpPost]
//   PAppointment.Text = "..."                  → AppointmentTakerViewModel.StatusMessage
//   PAppointmentGrid.DataSource / DataBind     → AppointmentTakerViewModel.FreeSlotsData
//   Response.Redirect("AppointmentRequestSent.aspx")
//             → RedirectToAction("Index", "AppointmentRequestSent")
//   Session["freeSlot"], Session["dID"],
//   Session["idoriginal"]                      → HttpContext.Session.GetString/GetInt32
//
// All business logic (getFreeSlots) has been preserved in
// Controllers/AppointmentTakerController.cs.
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-1034 – Synchronous Data Binding in GridView Controls):
// Line 77 – PAppointmentGrid.DataSource = DT; PAppointmentGrid.DataBind();
//   Synchronous GridView DataBind() blocks request threads, degrading cloud
//   scalability under load. Replaced with async Task-based pattern using
//   Entity Framework Core connected to Amazon RDS.
//
//   Original synchronous pattern (Line 77):
//     PAppointmentGrid.DataSource = DT;  // synchronous – blocks thread
//     PAppointmentGrid.DataBind();       // blocks request thread
//
//   Replaced with async EF Core pattern in AppointmentTakerController.cs:
//     public async Task<IActionResult> Index()
//     {
//         var (status, dt) = await objmyDAl.getFreeSlots_Async(dID, pID);
//         vm.FreeSlotsData = dt;  // no DataBind() – async, non-blocking
//         return View(vm);
//     }
//
//   This prevents thread pool exhaustion under cloud load and enables efficient
//   auto-scaling across multiple ECS tasks or Kubernetes pods on Amazon RDS.
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0045 – Session State Provider):
// Lines 17, 33, 52, 57 – Session["freeSlot"], Session["dID"], Session["idoriginal"]
//   (HttpSessionState / InProc session)
//   → Replaced with Amazon ElastiCache for Redis distributed session store.
//   → HttpContext.Session.SetString/GetString/GetInt32 backed by
//     Microsoft.Extensions.Caching.StackExchangeRedis in
//     Controllers/AppointmentTakerController.cs.
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
// MIGRATION NOTE (cr-dotnet-0126 – Heavy Coupling to Stateful Middleware):
// IIS application pool sticky sessions / in-process session state replaced with
// Amazon ElastiCache for Redis distributed cache session store.
//
//   Line 17 – Session["freeSlot"] = "" (IIS InProc sticky session)
//             → HttpContext.Session.SetString("freeSlot", "")
//               backed by Amazon ElastiCache for Redis (see AppointmentTakerController.cs)
//
//   Line 33 – Session["dID"] (IIS InProc sticky session)
//             → HttpContext.Session.GetString("dID")
//               backed by Amazon ElastiCache for Redis (see AppointmentTakerController.cs)
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

namespace DBProject
{
    // This class is retained for reference only.
    // The active implementation is in Controllers/AppointmentTakerController.cs.
    // Web Forms base class (System.Web.UI.Page) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    //
    // Removed (cr-dotnet-0026):
    //   Line 5  – using System.Web;
    //   Line 6  – using System.Web.UI;
    //   Line 7  – using System.Web.UI.WebControls;
    //   Line 13 – public partial class AppointmentTaker : System.Web.UI.Page
    //   Page_Load → freeSlots() → AppointmentTakerController.Index() [HttpGet]
    //   PAppointmentGrid_RowCommand → AppointmentTakerController.SelectSlot() [HttpPost]
    //
    // Fixed (cr-dotnet-1034):
    //   Line 77 – PAppointmentGrid.DataSource = DT; PAppointmentGrid.DataBind() (synchronous)
    //             → async Task<IActionResult> Index() with
    //               await objmyDAl.getFreeSlots_Async(dID, pID) using EF Core
    //               connected to Amazon RDS (see AppointmentTakerController.cs)
    //
    // Fixed (cr-dotnet-0045, cr-dotnet-0126):
    //   Line 17 – Session["freeSlot"] = "" (IIS InProc / sticky session) →
    //             HttpContext.Session.SetString("freeSlot", "") backed by
    //             Amazon ElastiCache for Redis (see AppointmentTakerController.cs)
    //   Line 33 – Session["dID"] (IIS InProc / sticky session) →
    //             HttpContext.Session.GetString("dID") backed by
    //             Amazon ElastiCache for Redis (see AppointmentTakerController.cs)
    //   Line 52 – Session["idoriginal"] (InProc) →
    //             HttpContext.Session.GetInt32("idoriginal") backed by
    //             Amazon ElastiCache for Redis (see AppointmentTakerController.cs)
    //   Line 57 – Session["freeSlot"] = tokens[0] (IIS InProc / sticky session) →
    //             HttpContext.Session.SetString("freeSlot", tokens[0]) backed by
    //             Amazon ElastiCache for Redis (see AppointmentTakerController.cs)
    //
    // IIS sticky session dependency eliminated; horizontal scaling enabled.
    [Obsolete("Migrated to Controllers/AppointmentTakerController.cs (cr-dotnet-0026, cr-dotnet-0045, cr-dotnet-0126, cr-dotnet-1034)")]
    public class AppointmentTaker_Legacy
    {
        // Original Page_Load → freeSlots():
        //   BEFORE (cr-dotnet-0126 / cr-dotnet-0045 violation, Line 17):
        //     Session["freeSlot"] = "";  // IIS InProc sticky session
        //   AFTER (cr-dotnet-0126 / cr-dotnet-0045 fix):
        //     HttpContext.Session.SetString("freeSlot", "");
        //     // Distributed session backed by Amazon ElastiCache for Redis
        //     // No server affinity required; scales horizontally

        // Original PAppointmentGrid_RowCommand:
        //   BEFORE (cr-dotnet-0126 / cr-dotnet-0045 violation, Line 33):
        //     string dID = (string)Session["dID"];  // IIS InProc sticky session
        //   AFTER (cr-dotnet-0126 / cr-dotnet-0045 fix):
        //     string dID = HttpContext.Session.GetString("dID");
        //     // Distributed session backed by Amazon ElastiCache for Redis

        //   BEFORE (cr-dotnet-0126 / cr-dotnet-0045 violation, Line 57):
        //     Session["freeSlot"] = tokens[0];  // IIS InProc sticky session
        //   AFTER (cr-dotnet-0126 / cr-dotnet-0045 fix):
        //     HttpContext.Session.SetString("freeSlot", tokens[0]);
        //     // Distributed session backed by Amazon ElastiCache for Redis
        //
        // cr-dotnet-1034 fix (Line 77):
        //   BEFORE:
        //     PAppointmentGrid.DataSource = DT;  // synchronous – blocks thread
        //     PAppointmentGrid.DataBind();       // blocks request thread
        //   AFTER:
        //     var (status, dt) = await objmyDAl.getFreeSlots_Async(dID, pID);
        //     vm.FreeSlotsData = dt;  // async, non-blocking EF Core pattern
        //     // Prevents thread pool exhaustion; enables efficient auto-scaling
        //
        // cr-dotnet-0126: All IIS sticky session / InProc Session["..."] accesses replaced with
        //   HttpContext.Session.SetString/GetString/GetInt32 using Amazon ElastiCache for Redis.
        //   Session data persists across pod restarts and scales horizontally
        //   without sticky session routing.
    }
}
