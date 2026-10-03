// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (PatientNotifications.aspx.cs) was the Web Forms code-behind for
// PatientNotifications.aspx.
// It has been migrated to ASP.NET Core MVC:
//
//   • PatientNotifications.aspx    → Views/Patient/PatientNotifications.cshtml (Razor View)
//   • PatientNotifications.aspx.cs → Controllers/PatientNotificationsController.cs (MVC Controller)
//
// Web Forms patterns removed / replaced (cr-dotnet-0026):
//   Line 5  – using System.Web;                → removed (not available in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not available in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not available in ASP.NET Core)
//   Line 15 – public partial class PatientNotifications : System.Web.UI.Page
//             → PatientNotificationsController (MVC Controller)
//   Line 17 – Page_Load → Notifications()     → Index() GET action
//   Notify.Text = "..."                        → PatientNotificationsViewModel.NotifyMessage
//   NDoctor.Text = "..."                       → PatientNotificationsViewModel.DoctorMessage
//   NTimings.Text = "..."                      → PatientNotificationsViewModel.TimingsMessage
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0045 – Session State Provider):
// Line 30 – Session["idoriginal"] (InProc HttpSessionState)
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
//   Active implementation: Controllers/PatientNotificationsController.cs
//   Redis configuration:   Program.cs (REDIS_CONNECTION_STRING env var)
//   NuGet packages added:
//     - Microsoft.Extensions.Caching.StackExchangeRedis 7.0.0
//     - StackExchange.Redis 2.6.122
// ============================================================================

using System;

namespace DBProject
{
    // This class is retained for reference only.
    // The active implementation is in Controllers/PatientNotificationsController.cs
    // and Views/Patient/PatientNotifications.cshtml.
    // Web Forms base class (System.Web.UI.Page) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    //
    // Removed (cr-dotnet-0026):
    //   using System.Web;
    //   using System.Web.UI;
    //   using System.Web.UI.WebControls;
    //   public partial class PatientNotifications : System.Web.UI.Page
    //   Page_Load → Notifications() → migrated to PatientNotificationsController.Index()
    //
    // Fixed (cr-dotnet-0045):
    //   Line 30 – Session["idoriginal"] (InProc) →
    //             HttpContext.Session.GetInt32("idoriginal") backed by
    //             Amazon ElastiCache for Redis (see PatientNotificationsController.cs)
    [Obsolete("Migrated to Controllers/PatientNotificationsController.cs (cr-dotnet-0026, cr-dotnet-0045)")]
    public class PatientNotifications_Legacy
    {
        // Original business logic (Notifications method) has been fully preserved
        // and migrated to Controllers/PatientNotificationsController.cs.
        //
        // BEFORE (cr-dotnet-0045 violation, Line 30):
        //   int pid = (int)Session["idoriginal"];  // InProc HttpSessionState
        //
        // AFTER (cr-dotnet-0045 fix):
        //   int pid = HttpContext.Session.GetInt32("idoriginal") ?? 0;
        //   // Distributed session backed by Amazon ElastiCache for Redis
        //
        // asp:Label controls (Notify, NDoctor, NTimings) are replaced by
        // PatientNotificationsViewModel properties rendered in
        // Views/Patient/PatientNotifications.cshtml.
    }
}
