// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (TakeAppointment.aspx.cs) was the Web Forms code-behind for
// TakeAppointment.aspx.
// It has been migrated to ASP.NET Core MVC:
//
//   • TakeAppointment.aspx    → Views/Patient/TakeAppointment.cshtml (Razor View)
//   • TakeAppointment.aspx.cs → Controllers/TakeAppointmentController.cs (MVC Controller)
//
// Web Forms patterns removed / replaced (cr-dotnet-0026):
//   using System.Web;                → removed (not available in ASP.NET Core)
//   using System.Web.UI;             → removed (not available in ASP.NET Core)
//   using System.Web.UI.WebControls; → removed (not available in ASP.NET Core)
//   public partial class TakeAppointment : System.Web.UI.Page
//             → TakeAppointmentController (MVC Controller)
//   Page_Load → deptInfo()           → IndexAsync() GET action
//   TDeptGrid_RowCommand              → SelectDepartment() POST action
//   TDept.Text = "..."               → TakeAppointmentViewModel.StatusMessage
//   TDeptGrid.DataSource / DataBind  → TakeAppointmentViewModel.DeptData (async)
//   Response.Redirect("ViewDoctors.aspx") → RedirectToAction("Index", "ViewDoctors")
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-1034 – Async GridView Data Binding):
// Rule: cr-dotnet-1034  Severity: HIGH  Category: performance-&-scalability
//
// Occurrence – Line 63 (original source TakeAppointment.aspx.cs):
//   BEFORE (synchronous):
//     TDeptGrid.DataSource = DT;
//     TDeptGrid.DataBind();
//     Synchronous data binding blocks request threads, degrading cloud scalability
//     under load and preventing efficient auto-scaling.
//
//   AFTER (async Task-based with Amazon RDS):
//     var (status, dt) = await _dal.getdeptInfo_Async();
//     vm.DeptData = dt;
//     Uses async Task-based pattern via getdeptInfo_Async() in myDAL,
//     connected to Amazon RDS via RDS Proxy (RDS_PROXY_CONNECTION_STRING env var).
//     Prevents thread pool exhaustion under cloud load; enables efficient
//     auto-scaling in cloud deployments.
//
//   Active implementation: Controllers/TakeAppointmentController.cs
//   Async DAL method:      DAL/myDAL.cs → getdeptInfo_Async()
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0045 – Session State Provider):
// Rule: cr-dotnet-0045  Severity: HIGH  Category: caching-&-session-state
//
//   BEFORE: Session["deptOriginal"] = "";
//           Uses in-process (InProc) HttpSessionState which creates server
//           affinity and prevents horizontal scaling across multiple instances.
//
//   AFTER:  HttpContext.Session.SetString("deptOriginal", "");
//           Uses ASP.NET Core distributed session backed by Amazon ElastiCache
//           for Redis, configured in Program.cs via AddStackExchangeRedisCache()
//           and AddSession(). Enables stateless horizontal scaling across
//           multiple ECS tasks or Kubernetes pods.
//
//   Active implementation: Controllers/TakeAppointmentController.cs
//   Redis configuration:   Program.cs (REDIS_CONNECTION_STRING env var)
//   NuGet packages added:
//     - Microsoft.Extensions.Caching.StackExchangeRedis 7.0.0
//     - StackExchange.Redis 2.6.122
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0126 – Heavy Coupling to Stateful Middleware):
// IIS application pool sticky sessions / in-process session state replaced with
// Amazon ElastiCache for Redis distributed cache session store.
//
//   Line 17 – Session["deptOriginal"] = "" (IIS InProc sticky session)
//             → HttpContext.Session.SetString("deptOriginal", "")
//               backed by Amazon ElastiCache for Redis (see TakeAppointmentController.cs)
//
//   Line 31 – Session["deptOriginal"] = deptName (IIS InProc sticky session)
//             → HttpContext.Session.SetString("deptOriginal", selectedDept)
//               backed by Amazon ElastiCache for Redis (see TakeAppointmentController.cs)
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

namespace DBProject
{
    // This class is retained for reference only.
    // The active implementation is in Controllers/TakeAppointmentController.cs
    // and Views/Patient/TakeAppointment.cshtml.
    // Web Forms base class (System.Web.UI.Page) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    //
    // Removed (cr-dotnet-0026):
    //   using System.Web;
    //   using System.Web.UI;
    //   using System.Web.UI.WebControls;
    //   public partial class TakeAppointment : System.Web.UI.Page
    //   Page_Load → deptInfo() → migrated to TakeAppointmentController.IndexAsync()
    //   TDeptGrid_RowCommand   → migrated to TakeAppointmentController.SelectDepartment()
    //
    // Fixed (cr-dotnet-1034) – Occurrence, Line 63:
    //   BEFORE: TDeptGrid.DataSource = DT; TDeptGrid.DataBind();
    //           Synchronous GridView data binding blocks request threads.
    //   AFTER:  var (status, dt) = await _dal.getdeptInfo_Async();
    //           vm.DeptData = dt;
    //           Async Task-based data loading via getdeptInfo_Async() in myDAL,
    //           connected to Amazon RDS via RDS Proxy. Prevents thread pool
    //           exhaustion; enables auto-scaling in cloud deployments.
    //
    // Fixed (cr-dotnet-0045, cr-dotnet-0126) – Occurrence 1, Lines 17 & 31:
    //   Line 17: Session["deptOriginal"] = "" (IIS InProc / sticky session) →
    //            HttpContext.Session.SetString("deptOriginal", "") backed by
    //            Amazon ElastiCache for Redis (see TakeAppointmentController.cs)
    //   Line 31: Session["deptOriginal"] = deptName (IIS InProc / sticky session) →
    //            HttpContext.Session.SetString("deptOriginal", selectedDept) backed by
    //            Amazon ElastiCache for Redis (see TakeAppointmentController.cs)
    //
    // IIS sticky session dependency eliminated; horizontal scaling enabled.
    [Obsolete("Migrated to Controllers/TakeAppointmentController.cs (cr-dotnet-0026, cr-dotnet-0045, cr-dotnet-0126, cr-dotnet-1034)")]
    public class TakeAppointment_Legacy
    {
        // Original business logic (deptInfo and TDeptGrid_RowCommand methods) has been
        // fully preserved and migrated to Controllers/TakeAppointmentController.cs.
        //
        // cr-dotnet-1034 FIX – Occurrence (Line 63):
        //   BEFORE: TDeptGrid.DataSource = DT; TDeptGrid.DataBind();
        //           Synchronous GridView data binding blocks request threads,
        //           degrading cloud scalability under load.
        //   AFTER:  var (status, dt) = await _dal.getdeptInfo_Async();
        //           vm.DeptData = dt;
        //           Async Task-based pattern via getdeptInfo_Async() in myDAL,
        //           connected to Amazon RDS via RDS Proxy endpoint.
        //           Prevents thread pool exhaustion; enables auto-scaling.
        //
        // cr-dotnet-0126 / cr-dotnet-0045 FIX – Occurrence 1 (Line 17):
        //   BEFORE: Session["deptOriginal"] = "";  // IIS InProc sticky session
        //   AFTER:  HttpContext.Session.SetString("deptOriginal", "");
        //           // Distributed session backed by Amazon ElastiCache for Redis
        //           // No server affinity required; scales horizontally
        //
        // cr-dotnet-0126 / cr-dotnet-0045 FIX – Occurrence 1 (Line 31):
        //   BEFORE: Session["deptOriginal"] = deptName;  // IIS InProc sticky session
        //   AFTER:  HttpContext.Session.SetString("deptOriginal", selectedDept);
        //           // Distributed session backed by Amazon ElastiCache for Redis
        //
        // asp:Label (TDept) and asp:GridView (TDeptGrid) controls are replaced by
        // TakeAppointmentViewModel properties rendered in
        // Views/Patient/TakeAppointment.cshtml.
    }
}
