// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (PatientFeedback.aspx.cs) was the Web Forms code-behind for
// PatientFeedback.aspx.
// It has been migrated to ASP.NET Core MVC:
//
//   • PatientFeedback.aspx    → Views/Patient/PatientFeedback.cshtml  (Razor View)
//   • PatientFeedback.aspx.cs → Controllers/PatientFeedbackController.cs (MVC Controller)
//   • (new)                   → Models/PatientFeedbackViewModel.cs (ViewModel)
//
// Web Forms patterns removed / replaced (cr-dotnet-0026):
//   Line 5  – using System.Web;                → removed (not in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not in ASP.NET Core)
//   Line 13 – public partial class PatientFeedback : System.Web.UI.Page
//             → PatientFeedbackController (MVC Controller)
//   Line 15 – Page_Load (IsPostBack check)     → GET/POST action separation
//   pendingFeedback()                          → PatientFeedbackController.Index() [HttpGet]
//   giveFeedback() (Button OnClick)            → PatientFeedbackController.GiveFeedback() [HttpPost]
//   Feedback.Text = "..."                      → PatientFeedbackViewModel.FeedbackMessage
//   FDoctor.Text = "..."                       → PatientFeedbackViewModel.DoctorMessage
//   FTimings.Text = "..."                      → PatientFeedbackViewModel.TimingsMessage
//   Message.Visible = true                     → PatientFeedbackViewModel.ShowRatingPrompt
//   List.Visible = true                        → PatientFeedbackViewModel.ShowRatingPrompt
//   button1.Visible = true                     → PatientFeedbackViewModel.ShowRatingPrompt
//   F.Text = "..."                             → PatientFeedbackViewModel.ConfirmationMessage
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0045 – Session State Provider):
// Line 21 – Session["aID"] = "" (InProc HttpSessionState)
//   BEFORE: Session["aID"] = "";
//           Uses in-process (InProc) HttpSessionState which creates server
//           affinity and prevents horizontal scaling across multiple instances.
//
//   AFTER:  HttpContext.Session.Remove("aID");
//           Uses ASP.NET Core distributed session backed by Amazon ElastiCache
//           for Redis.
//
// Line 35 – Session["idoriginal"] (InProc HttpSessionState)
//   BEFORE: int pid = (int)Session["idoriginal"];
//           Uses in-process (InProc) HttpSessionState.
//
//   AFTER:  int pid = HttpContext.Session.GetInt32("idoriginal") ?? 0;
//           Uses ASP.NET Core distributed session backed by Amazon ElastiCache
//           for Redis.
//
// Line 56 – Session["aID"] = aID (InProc HttpSessionState)
//   BEFORE: Session["aID"] = aID;
//           Uses in-process (InProc) HttpSessionState.
//
//   AFTER:  HttpContext.Session.SetInt32("aID", aID);
//           Uses ASP.NET Core distributed session backed by Amazon ElastiCache
//           for Redis.
//
// Line 79 – Session["aID"] (InProc HttpSessionState)
//   BEFORE: int aID = (int)Session["aID"];
//           Uses in-process (InProc) HttpSessionState.
//
//   AFTER:  int aID = HttpContext.Session.GetInt32("aID") ?? 0;
//           Uses ASP.NET Core distributed session backed by Amazon ElastiCache
//           for Redis.
//
//   Active implementation: Controllers/PatientFeedbackController.cs
//   Redis configuration:   Program.cs (REDIS_CONNECTION_STRING env var)
//   NuGet packages added:
//     - Microsoft.Extensions.Caching.StackExchangeRedis 7.0.0
//     - StackExchange.Redis 2.6.122
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0126 – Heavy Coupling to Stateful Middleware):
// IIS application pool sticky sessions / in-process session state replaced with
// Amazon ElastiCache for Redis distributed cache session store.
//
//   Line 21 – Session["aID"] = "" (IIS InProc sticky session)
//             → HttpContext.Session.Remove("aID")
//               backed by Amazon ElastiCache for Redis (see PatientFeedbackController.cs)
//
//   Line 56 – Session["aID"] = aID (IIS InProc sticky session)
//             → HttpContext.Session.SetInt32("aID", aID)
//               backed by Amazon ElastiCache for Redis (see PatientFeedbackController.cs)
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
    // The active implementation is in Controllers/PatientFeedbackController.cs.
    // Web Forms base class (System.Web.UI.Page) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    //
    // Removed (cr-dotnet-0026):
    //   Line 5  – using System.Web;
    //   Line 6  – using System.Web.UI;
    //   Line 7  – using System.Web.UI.WebControls;
    //   Line 13 – public partial class PatientFeedback : System.Web.UI.Page
    //   Page_Load → pendingFeedback() → PatientFeedbackController.Index() [HttpGet]
    //   giveFeedback() → PatientFeedbackController.GiveFeedback() [HttpPost]
    //
    // Fixed (cr-dotnet-0045, cr-dotnet-0126):
    //   Line 21 – Session["aID"] = "" (IIS InProc / sticky session) →
    //             HttpContext.Session.Remove("aID") backed by
    //             Amazon ElastiCache for Redis (see PatientFeedbackController.cs)
    //   Line 35 – Session["idoriginal"] (InProc) →
    //             HttpContext.Session.GetInt32("idoriginal") backed by
    //             Amazon ElastiCache for Redis (see PatientFeedbackController.cs)
    //   Line 56 – Session["aID"] = aID (IIS InProc / sticky session) →
    //             HttpContext.Session.SetInt32("aID", aID) backed by
    //             Amazon ElastiCache for Redis (see PatientFeedbackController.cs)
    //   Line 79 – Session["aID"] (InProc) →
    //             HttpContext.Session.GetInt32("aID") backed by
    //             Amazon ElastiCache for Redis (see PatientFeedbackController.cs)
    //
    // IIS sticky session dependency eliminated; horizontal scaling enabled.
    [Obsolete("Migrated to Controllers/PatientFeedbackController.cs (cr-dotnet-0026, cr-dotnet-0045, cr-dotnet-0126)")]
    public class PatientFeedback_Legacy
    {
        // Original Page_Load → pendingFeedback():
        //   BEFORE (cr-dotnet-0126 / cr-dotnet-0045 violation, Line 21):
        //     Session["aID"] = "";  // IIS InProc sticky session
        //   AFTER (cr-dotnet-0126 / cr-dotnet-0045 fix):
        //     HttpContext.Session.Remove("aID");
        //     // Distributed session backed by Amazon ElastiCache for Redis
        //     // No server affinity required; scales horizontally
        //
        //   BEFORE (cr-dotnet-0126 / cr-dotnet-0045 violation, Line 56):
        //     Session["aID"] = aID;  // IIS InProc sticky session
        //   AFTER (cr-dotnet-0126 / cr-dotnet-0045 fix):
        //     HttpContext.Session.SetInt32("aID", aID);
        //     // Distributed session backed by Amazon ElastiCache for Redis
        //
        // Original giveFeedback():
        //   BEFORE (cr-dotnet-0045 violation, Line 79):
        //     int aID = (int)Session["aID"];  // InProc HttpSessionState
        //   AFTER (cr-dotnet-0045 fix):
        //     int aID = HttpContext.Session.GetInt32("aID") ?? 0;
        //     // Distributed session backed by Amazon ElastiCache for Redis
        //
        // cr-dotnet-0126: All IIS sticky session / InProc Session["..."] accesses replaced with
        //   HttpContext.Session distributed session using Amazon ElastiCache for Redis.
        //   Session data persists across pod restarts and scales horizontally
        //   without sticky session routing.
    }
}
