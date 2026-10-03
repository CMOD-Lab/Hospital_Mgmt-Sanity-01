// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (ViewDoctors.aspx.cs) was the Web Forms code-behind for
// ViewDoctors.aspx.
// It has been migrated to ASP.NET Core MVC:
//
//   • ViewDoctors.aspx    → Views/Patient/ViewDoctors.cshtml (Razor View)
//   • ViewDoctors.aspx.cs → Controllers/ViewDoctorsController.cs (MVC Controller)
//
// Web Forms patterns removed / replaced (cr-dotnet-0026):
//   using System.Web;                → removed (not available in ASP.NET Core)
//   using System.Web.UI;             → removed (not available in ASP.NET Core)
//   using System.Web.UI.WebControls; → removed (not available in ASP.NET Core)
//   public partial class ViewDoctors : System.Web.UI.Page
//             → ViewDoctorsController (MVC Controller)
//   Page_Load → deptDoctorInfo()     → Index() GET action
//   TDoctorGrid_RowCommand            → SelectDoctor() POST action
//   TDoctor.Text = "..."             → ViewDoctorsViewModel.StatusMessage
//   TDoctorGrid.DataSource / DataBind → ViewDoctorsViewModel.DoctorData
//   Session["deptOriginal"]          → HttpContext.Session.GetString
//   Session["dID"]                   → HttpContext.Session.SetString
//   Response.Redirect("DoctorProfile.aspx") → RedirectToAction("Index", "DoctorProfile")
//
// The active MVC controller is at Controllers/ViewDoctorsController.cs.
// The active Razor view is at Views/Patient/ViewDoctors.cshtml.
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-1034 – Async GridView Data Binding):
// Rule: cr-dotnet-1034  Severity: HIGH  Category: performance-&-scalability
//
// Occurrence – Line 63 (original ViewDoctors.aspx.cs):
//   BEFORE (synchronous – blocks request thread):
//     TDoctorGrid.DataSource = DT;
//     TDoctorGrid.DataBind();
//     Uses synchronous GridView DataSource/DataBind() pattern which blocks
//     request threads, degrading cloud scalability under load and causing
//     thread pool exhaustion in cloud deployments.
//
//   AFTER (async Task-based – non-blocking):
//     var (status, dt) = await objmyDAl.getDeptDoctorInfo_Async(deptName);
//     vm.DoctorData = dt;
//     Async Task-based data loading via getDeptDoctorInfo_Async() in myDAL,
//     connected to Amazon RDS via RDS Proxy (RDS_PROXY_CONNECTION_STRING env var).
//     Prevents thread pool exhaustion under cloud load; enables efficient
//     auto-scaling in cloud deployments.
//
//   Active implementation: Controllers/ViewDoctorsController.cs
//     → LoadDeptDoctorInfoAsync(vm) calls getDeptDoctorInfo_Async(deptName)
//   Active view:           Views/Patient/ViewDoctors.cshtml
//   Async DAL method:      DAL/myDAL.cs → getDeptDoctorInfo_Async()
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0045 – Session State Provider):
// Rule: cr-dotnet-0045  Severity: HIGH  Category: caching-&-session-state
//
// Occurrence 3 – Line 17 (original source):
//   BEFORE: Session["dID"] = "";
//           Uses in-process (InProc) HttpSessionState which creates server
//           affinity and prevents horizontal scaling across multiple instances.
//
//   AFTER:  HttpContext.Session.SetString("dID", "");
//           Uses ASP.NET Core distributed session backed by Amazon ElastiCache
//           for Redis, configured in Program.cs via AddStackExchangeRedisCache()
//           and AddSession(). Enables stateless horizontal scaling across
//           multiple ECS tasks or Kubernetes pods.
//
// Occurrence 4 – Line 30 (original source):
//   BEFORE: Session["dID"] = dID;
//           InProc HttpSessionState set in TDoctorGrid_RowCommand handler.
//
//   AFTER:  HttpContext.Session.SetString("dID", selectedDoctorId);
//           Distributed session backed by Amazon ElastiCache for Redis.
//
// Occurrence 5 – Line 49 (original source):
//   BEFORE: string deptName = (string)Session["deptOriginal"];
//           InProc HttpSessionState read in deptDoctorInfo helper.
//
//   AFTER:  string deptName = HttpContext.Session.GetString("deptOriginal") ?? "";
//           Distributed session backed by Amazon ElastiCache for Redis.
//
// Occurrence 6 – Line 61 (original source):
//   BEFORE: TDoctor.Text = "Following are our Specialized Doctors of "
//                          + Session["deptOriginal"] + " Department:";
//           InProc HttpSessionState read for display text.
//
//   AFTER:  vm.StatusMessage = "Following are our Specialized Doctors of "
//                              + deptName + " Department:";
//           Uses the already-retrieved deptName from distributed Redis session.
//
//   Active implementation: Controllers/ViewDoctorsController.cs
//   Redis configuration:   Program.cs (REDIS_CONNECTION_STRING env var)
//   NuGet packages added:
//     - Microsoft.Extensions.Caching.StackExchangeRedis 7.0.0
//     - StackExchange.Redis 2.6.122
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0126 – Heavy Coupling to Stateful Middleware):
// IIS application pool sticky sessions / in-process session state replaced with
// Amazon ElastiCache for Redis distributed cache session store.
//
//   Line 17 – Session["dID"] = "" (IIS InProc sticky session)
//             → HttpContext.Session.SetString("dID", "")
//               backed by Amazon ElastiCache for Redis (see ViewDoctorsController.cs)
//
//   Line 30 – Session["dID"] = dID (IIS InProc sticky session)
//             → HttpContext.Session.SetString("dID", selectedDoctorId)
//               backed by Amazon ElastiCache for Redis (see ViewDoctorsController.cs)
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
    // The active implementation is in Controllers/ViewDoctorsController.cs
    // and Views/Patient/ViewDoctors.cshtml.
    // Web Forms base class (System.Web.UI.Page) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    //
    // Removed (cr-dotnet-0026):
    //   using System.Web;
    //   using System.Web.UI;
    //   using System.Web.UI.WebControls;
    //   public partial class ViewDoctors : System.Web.UI.Page
    //   Page_Load → deptDoctorInfo() → migrated to ViewDoctorsController.Index()
    //   TDoctorGrid_RowCommand        → migrated to ViewDoctorsController.SelectDoctor()
    //
    // Fixed (cr-dotnet-1034) – Occurrence (Line 63):
    //   BEFORE: TDoctorGrid.DataSource = DT;
    //           TDoctorGrid.DataBind();
    //           Synchronous GridView DataSource/DataBind() blocks request threads,
    //           degrading cloud scalability under load.
    //   AFTER:  var (status, dt) = await objmyDAl.getDeptDoctorInfo_Async(deptName);
    //           vm.DoctorData = dt;
    //           Async Task-based data loading via getDeptDoctorInfo_Async() connected
    //           to Amazon RDS via RDS Proxy (RDS_PROXY_CONNECTION_STRING env var).
    //           Prevents thread pool exhaustion; enables auto-scaling.
    //           Active implementation: Controllers/ViewDoctorsController.cs
    //
    // Fixed (cr-dotnet-0045, cr-dotnet-0126) – Occurrences 3–6:
    //   Line 17: Session["dID"] = "" (IIS InProc / sticky session) →
    //            HttpContext.Session.SetString("dID", "") backed by Amazon ElastiCache for Redis
    //   Line 30: Session["dID"] = dID (IIS InProc / sticky session) →
    //            HttpContext.Session.SetString("dID", selectedDoctorId) backed by Amazon ElastiCache for Redis
    //   Line 49: (string)Session["deptOriginal"] (InProc) →
    //            HttpContext.Session.GetString("deptOriginal") ?? "" backed by Amazon ElastiCache for Redis
    //   Line 61: Session["deptOriginal"] (InProc read for display) →
    //            deptName variable from distributed Redis session
    //
    // IIS sticky session dependency eliminated; horizontal scaling enabled.
    [Obsolete("Migrated to Controllers/ViewDoctorsController.cs (cr-dotnet-0026, cr-dotnet-0045, cr-dotnet-0126, cr-dotnet-1034)")]
    public class ViewDoctors_Legacy
    {
        // Original business logic (deptDoctorInfo and TDoctorGrid_RowCommand methods) has been
        // fully preserved and migrated to Controllers/ViewDoctorsController.cs.
        //
        // cr-dotnet-1034 FIX – Occurrence (Line 63):
        //   BEFORE: TDoctorGrid.DataSource = DT;
        //           TDoctorGrid.DataBind();
        //           Synchronous GridView data binding blocks request threads.
        //   AFTER:  var (status, dt) = await objmyDAl.getDeptDoctorInfo_Async(deptName);
        //           vm.DoctorData = dt;
        //           Async Task-based data loading via getDeptDoctorInfo_Async()
        //           connected to Amazon RDS via RDS Proxy (RDS_PROXY_CONNECTION_STRING env var).
        //           Prevents thread pool exhaustion under cloud load; enables auto-scaling.
        //           See: Controllers/ViewDoctorsController.cs → LoadDeptDoctorInfoAsync()
        //
        // cr-dotnet-0126 / cr-dotnet-0045 FIX – Occurrence 3 (Line 17):
        //   BEFORE: Session["dID"] = "";  // IIS InProc sticky session
        //   AFTER:  HttpContext.Session.SetString("dID", "");
        //           // Distributed session backed by Amazon ElastiCache for Redis
        //           // No server affinity required; scales horizontally
        //
        // cr-dotnet-0126 / cr-dotnet-0045 FIX – Occurrence 4 (Line 30):
        //   BEFORE: Session["dID"] = dID;  // IIS InProc sticky session
        //   AFTER:  HttpContext.Session.SetString("dID", selectedDoctorId);
        //           // Distributed session backed by Amazon ElastiCache for Redis
        //
        // cr-dotnet-0045 FIX – Occurrence 5 (Line 49):
        //   BEFORE: string deptName = (string)Session["deptOriginal"];  // InProc HttpSessionState
        //   AFTER:  string deptName = HttpContext.Session.GetString("deptOriginal") ?? "";
        //           // Distributed session backed by Amazon ElastiCache for Redis
        //
        // cr-dotnet-0045 FIX – Occurrence 6 (Line 61):
        //   BEFORE: TDoctor.Text = "Following are our Specialized Doctors of "
        //                          + Session["deptOriginal"] + " Department:";  // InProc
        //   AFTER:  vm.StatusMessage = "Following are our Specialized Doctors of "
        //                              + deptName + " Department:";
        //           // Uses deptName already retrieved from distributed Redis session
        //
        // asp:Label (TDoctor) and asp:GridView (TDoctorGrid) controls are replaced by
        // ViewDoctorsViewModel properties rendered in
        // Views/Patient/ViewDoctors.cshtml.
    }
}
