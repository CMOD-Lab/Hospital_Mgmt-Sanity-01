// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (CurrentAppointment.aspx.cs) was the Web Forms code-behind for
// CurrentAppointment.aspx.
// It has been migrated to ASP.NET Core MVC:
//
//   • CurrentAppointment.aspx    → Views/Patient/CurrentAppointment.cshtml  (Razor View)
//   • CurrentAppointment.aspx.cs → Controllers/CurrentAppointmentController.cs (MVC Controller)
//   • (new)                      → Models/CurrentAppointmentViewModel.cs   (ViewModel)
//
// Web Forms patterns removed / replaced (cr-dotnet-0026):
//   Line 6  – using System.Web.UI;             → removed (not in ASP.NET Core)
//   Line 13 – public partial class CurrentAppointment : System.Web.UI.Page
//             → CurrentAppointmentController (MVC Controller)
//   Line 15 – Page_Load → appointmentToday()   → CurrentAppointmentController.Index() [HttpGet]
//   Appointment.Text = "..."                   → CurrentAppointmentViewModel.AppointmentMessage
//   ADoctor.Text = "..."                       → CurrentAppointmentViewModel.DoctorMessage
//   ATimings.Text = "..."                      → CurrentAppointmentViewModel.TimingsMessage
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0045 – Session State Provider):
// Line 28 – Session["idoriginal"] (InProc HttpSessionState)
//   BEFORE: int pid = (int)Session["idoriginal"];
//           Uses in-process (InProc) HttpSessionState which creates server
//           affinity and prevents horizontal scaling across multiple instances.
//
//   AFTER:  int pid = HttpContext.Session.GetInt32("idoriginal") ?? 0;
//           Uses ASP.NET Core distributed session backed by Amazon ElastiCache
//           for Redis, configured in Program.cs via AddStackExchangeRedisCache()
//           and AddSession(). Enables stateless horizontal scaling across
//           multiple ECS tasks or Kubernetes pods.
//
//   Active implementation: Controllers/CurrentAppointmentController.cs
//   Redis configuration:   Program.cs (REDIS_CONNECTION_STRING env var)
//   NuGet packages added:
//     - Microsoft.Extensions.Caching.StackExchangeRedis 7.0.0
//     - StackExchange.Redis 2.6.122
// ============================================================================

using System;
using DBProject.DAL;

namespace DBProject
{
    // This class is retained for reference only.
    // The active implementation is in Controllers/CurrentAppointmentController.cs.
    // Web Forms base class (System.Web.UI.Page) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    //
    // Removed (cr-dotnet-0026):
    //   Line 6  – using System.Web.UI;
    //   Line 13 – public partial class CurrentAppointment : System.Web.UI.Page
    //   Line 15 – Page_Load → appointmentToday() → CurrentAppointmentController.Index() [HttpGet]
    //
    // Fixed (cr-dotnet-0045):
    //   Line 28 – Session["idoriginal"] (InProc) →
    //             HttpContext.Session.GetInt32("idoriginal") backed by
    //             Amazon ElastiCache for Redis (see CurrentAppointmentController.cs)
    [Obsolete("Migrated to Controllers/CurrentAppointmentController.cs (cr-dotnet-0026, cr-dotnet-0045)")]
    public class CurrentAppointment_Legacy
    {
        // Original Page_Load → appointmentToday():
        //   BEFORE (cr-dotnet-0045 violation, Line 28):
        //     int pid = (int)Session["idoriginal"];  // InProc HttpSessionState
        //
        //   AFTER (cr-dotnet-0045 fix):
        //     int pid = HttpContext.Session.GetInt32("idoriginal") ?? 0;
        //     // Distributed session backed by Amazon ElastiCache for Redis
        //
        //   → CurrentAppointmentController.Index() [HttpGet]
    }
}
