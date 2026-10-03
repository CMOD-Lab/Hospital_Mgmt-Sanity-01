// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (AppointmentRequestSent.aspx.cs) was the Web Forms code-behind for
// AppointmentRequestSent.aspx.
// It has been migrated to ASP.NET Core MVC:
//
//   • AppointmentRequestSent.aspx    → Views/Patient/AppointmentRequestSent.cshtml  (Razor View)
//   • AppointmentRequestSent.aspx.cs → Controllers/AppointmentRequestSentController.cs (MVC Controller)
//   • (new)                          → Models/AppointmentRequestSentViewModel.cs   (ViewModel)
//
// Web Forms patterns removed / replaced:
//   Line 5  – using System.Web;                → removed (not in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not in ASP.NET Core)
//   Line 14 – public partial class AppointmentNotificationSent : System.Web.UI.Page
//             → AppointmentRequestSentController (MVC Controller)
//   Line 16 – Page_Load                        → AppointmentRequestSentController.Index() [HttpGet]
//   sendARequest (Button OnClick handler)      → AppointmentRequestSentController.SendRequest() [HttpPost]
//   Message.Text = "..."                       → AppointmentRequestSentViewModel.Message
//   Session["dID"], Session["idoriginal"],
//   Session["freeSlot"]                        → HttpContext.Session.GetString/GetInt32
//
// All business logic (insertAppointment) has been preserved in
// Controllers/AppointmentRequestSentController.cs.
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0045 – Session State Provider):
// Lines 28, 33, 36 – Session["dID"], Session["idoriginal"], Session["freeSlot"]
//   (HttpSessionState / InProc session)
//   → Replaced with Amazon ElastiCache for Redis distributed session store.
//   → HttpContext.Session.GetString("dID"), HttpContext.Session.GetInt32("idoriginal"),
//     HttpContext.Session.GetString("freeSlot") backed by
//     Microsoft.Extensions.Caching.StackExchangeRedis in
//     Controllers/AppointmentRequestSentController.cs.
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
    // The active implementation is in Controllers/AppointmentRequestSentController.cs.
    // Web Forms base class (System.Web.UI.Page) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    //
    // Removed (cr-dotnet-0026):
    //   Line 5  – using System.Web;
    //   Line 6  – using System.Web.UI;
    //   Line 7  – using System.Web.UI.WebControls;
    //   Line 14 – public partial class AppointmentNotificationSent : System.Web.UI.Page
    //   Page_Load → AppointmentRequestSentController.Index() [HttpGet]
    //   sendARequest → AppointmentRequestSentController.SendRequest() [HttpPost]
    //
    // Fixed (cr-dotnet-0045):
    //   Line 28 – Session["dID"] (InProc HttpSessionState)
    //             → HttpContext.Session.GetString("dID") backed by Amazon ElastiCache for Redis.
    //   Line 33 – Session["idoriginal"] (InProc HttpSessionState)
    //             → HttpContext.Session.GetInt32("idoriginal") backed by Amazon ElastiCache for Redis.
    //   Line 36 – Session["freeSlot"] (InProc HttpSessionState)
    //             → HttpContext.Session.GetString("freeSlot") backed by Amazon ElastiCache for Redis.
    [Obsolete("Migrated to Controllers/AppointmentRequestSentController.cs (cr-dotnet-0026, cr-dotnet-0045)")]
    public class AppointmentNotificationSent_Legacy
    {
        // Original Page_Load → (empty)
        //   → AppointmentRequestSentController.Index() [HttpGet]

        // Original sendARequest (Button OnClick):
        //   objmyDAl.insertAppointment(dID, pID, freeSlot, ref mes)
        //   Message.Text = mes / error message
        //   → AppointmentRequestSentController.SendRequest() [HttpPost]
        //
        // cr-dotnet-0045: Session["dID"], Session["idoriginal"], Session["freeSlot"] (InProc)
        //   replaced with HttpContext.Session.GetString/GetInt32 using Amazon ElastiCache for Redis.
    }
}
