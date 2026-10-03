// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (TreatmentHistory.aspx.cs) was the Web Forms code-behind for
// TreatmentHistory.aspx.
// It has been migrated to ASP.NET Core MVC:
//
//   • TreatmentHistory.aspx    → Views/Patient/TreatmentHistory.cshtml (Razor View)
//   • TreatmentHistory.aspx.cs → Controllers/TreatmentHistoryController.cs (MVC Controller)
//
// Web Forms patterns removed / replaced (cr-dotnet-0026):
//   Line 5  – using System.Web;                → removed (not available in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not available in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not available in ASP.NET Core)
//   Line 14 – public partial class TreatmentHistory : System.Web.UI.Page
//             → TreatmentHistoryController (MVC Controller)
//   Line 16 – Page_Load → treatmentHistory()   → IndexAsync() GET action
//   THistory.Text = "..."                      → TreatmentHistoryViewModel.StatusMessage
//   THistoryGrid.DataSource / DataBind         → TreatmentHistoryViewModel.TreatmentData (async)
//   Session["idoriginal"]                      → HttpContext.Session.GetInt32
//
// The active MVC controller is at Controllers/TreatmentHistoryController.cs.
// The active Razor view is at Views/Patient/TreatmentHistory.cshtml.
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-1034 – Async GridView Data Binding):
// Rule: cr-dotnet-1034  Severity: HIGH  Category: performance-&-scalability
//
// Occurrence – Line 51 (original source TreatmentHistory.aspx.cs):
//   BEFORE (synchronous):
//     THistoryGrid.DataSource = DT;
//     THistoryGrid.DataBind();
//     Synchronous data binding blocks request threads, degrading cloud scalability
//     under load and preventing efficient auto-scaling.
//
//   AFTER (async Task-based with Amazon RDS):
//     var (status, dt) = await _dal.getTreatmentHistory_Async(id);
//     vm.TreatmentData = dt;
//     Uses async Task-based pattern via getTreatmentHistory_Async() in myDAL,
//     connected to Amazon RDS via RDS Proxy (RDS_PROXY_CONNECTION_STRING env var).
//     Prevents thread pool exhaustion under cloud load; enables efficient
//     auto-scaling in cloud deployments.
//
//   Active implementation: Controllers/TreatmentHistoryController.cs
//   Async DAL method:      DAL/myDAL.cs → getTreatmentHistory_Async()
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0045 – Session State Provider):
// Rule: cr-dotnet-0045  Severity: HIGH  Category: caching-&-session-state
//
// Occurrence 2 – Line 31 (original source):
//   BEFORE: int id = (int)Session["idoriginal"];
//           Uses in-process (InProc) HttpSessionState which creates server
//           affinity and prevents horizontal scaling across multiple instances.
//
//   AFTER:  int id = HttpContext.Session.GetInt32("idoriginal") ?? 0;
//           Uses ASP.NET Core distributed session backed by Amazon ElastiCache
//           for Redis, configured in Program.cs via AddStackExchangeRedisCache()
//           and AddSession(). Enables stateless horizontal scaling across
//           multiple ECS tasks or Kubernetes pods.
//
//   Active implementation: Controllers/TreatmentHistoryController.cs
//   Redis configuration:   Program.cs (REDIS_CONNECTION_STRING env var)
//   NuGet packages added:
//     - Microsoft.Extensions.Caching.StackExchangeRedis 7.0.0
//     - StackExchange.Redis 2.6.122
// ============================================================================

using System;

namespace DBProject
{
    // This class is retained for reference only.
    // The active implementation is in Controllers/TreatmentHistoryController.cs
    // and Views/Patient/TreatmentHistory.cshtml.
    // Web Forms base class (System.Web.UI.Page) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    //
    // Removed (cr-dotnet-0026):
    //   using System.Web;
    //   using System.Web.UI;
    //   using System.Web.UI.WebControls;
    //   public partial class TreatmentHistory : System.Web.UI.Page
    //   Page_Load → treatmentHistory() → migrated to TreatmentHistoryController.IndexAsync()
    //
    // Fixed (cr-dotnet-1034) – Occurrence, Line 51:
    //   BEFORE: THistoryGrid.DataSource = DT; THistoryGrid.DataBind();
    //           Synchronous GridView data binding blocks request threads.
    //   AFTER:  var (status, dt) = await _dal.getTreatmentHistory_Async(id);
    //           vm.TreatmentData = dt;
    //           Async Task-based pattern via getTreatmentHistory_Async() in myDAL,
    //           connected to Amazon RDS via RDS Proxy endpoint.
    //           Prevents thread pool exhaustion; enables auto-scaling.
    //
    // Fixed (cr-dotnet-0045) – Occurrence 2, Line 31:
    //   BEFORE: int id = (int)Session["idoriginal"];  // InProc HttpSessionState
    //   AFTER:  int id = HttpContext.Session.GetInt32("idoriginal") ?? 0;
    //           // Distributed session backed by Amazon ElastiCache for Redis
    [Obsolete("Migrated to Controllers/TreatmentHistoryController.cs (cr-dotnet-0026, cr-dotnet-0045, cr-dotnet-1034)")]
    public class TreatmentHistory_Legacy
    {
        // Original business logic (treatmentHistory method) has been
        // fully preserved and migrated to Controllers/TreatmentHistoryController.cs.
        //
        // cr-dotnet-1034 FIX – Occurrence (Line 51):
        //   BEFORE: THistoryGrid.DataSource = DT; THistoryGrid.DataBind();
        //           Synchronous GridView data binding blocks request threads,
        //           degrading cloud scalability under load.
        //   AFTER:  var (status, dt) = await _dal.getTreatmentHistory_Async(id);
        //           vm.TreatmentData = dt;
        //           Async Task-based pattern via getTreatmentHistory_Async() in myDAL,
        //           connected to Amazon RDS via RDS Proxy endpoint.
        //           Prevents thread pool exhaustion; enables auto-scaling.
        //
        // cr-dotnet-0045 FIX – Occurrence 2 (Line 31):
        //   BEFORE: int id = (int)Session["idoriginal"];  // InProc HttpSessionState
        //   AFTER:  int id = HttpContext.Session.GetInt32("idoriginal") ?? 0;
        //           // Distributed session backed by Amazon ElastiCache for Redis
        //
        // asp:Label (THistory) and asp:GridView (THistoryGrid) controls are replaced by
        // TreatmentHistoryViewModel properties rendered in
        // Views/Patient/TreatmentHistory.cshtml.
    }
}
