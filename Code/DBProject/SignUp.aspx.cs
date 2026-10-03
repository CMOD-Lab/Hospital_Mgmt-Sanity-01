// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (SignUp.aspx.cs) was the Web Forms code-behind for SignUp.aspx.
// It has been migrated to ASP.NET Core MVC:
//
//   • SignUp.aspx    → Views/SignUp/Index.cshtml (Razor View)
//   • SignUp.aspx.cs → Controllers/SignUpController.cs (MVC Controller)
//
// Web Forms patterns removed / replaced (cr-dotnet-0026):
//   Line 5  – using System.Web;                → removed (not available in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not available in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not available in ASP.NET Core)
//   Line 13 – public partial class SignUp : System.Web.UI.Page
//             → SignUpController (MVC Controller)
//   Line 15 – Page_Load                        → Index() GET action
//   loginV (Button click handler)              → Login() POST action
//   signupV (Button click handler)             → Register() POST action
//   loginEmail.Text / loginPassword.Text       → SignUpViewModel.LoginEmail / LoginPassword
//   sName.Text, sBirthDate.Text, sEmail.Text   → SignUpViewModel.Name, BirthDate, Email
//   sPassword.Text, Phone.Text, Address.Text   → SignUpViewModel.Password, PhoneNo, Address
//   Request.Form["Gender"]                     → SignUpViewModel.Gender
//   Session["idoriginal"]                      → HttpContext.Session.SetInt32
//   Response.Redirect("~/Patient/PatientHome.aspx") → RedirectToAction("Index", "PatientHome")
//   Response.Redirect("~/Doctor/DoctorHome.aspx")   → RedirectToAction("Index", "DoctorHome")
//   Response.Redirect("~/Admin/AdminHome.aspx")     → RedirectToAction("Index", "AdminHome")
//   Response.Write("<script>alert(...)...")    → SignUpViewModel.ErrorMessage (TempData)
//   Response.BufferOutput                      → removed (not applicable in ASP.NET Core)
//
// The active MVC controller is at Controllers/SignUpController.cs.
// The active Razor view is at Views/SignUp/Index.cshtml.
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0045 – Session State Provider):
// Rule: cr-dotnet-0045  Severity: HIGH  Category: caching-&-session-state
//
// Occurrence 7 – Line 17 (original source):
//   BEFORE: Session["idoriginal"] = "";
//           Uses in-process (InProc) HttpSessionState which creates server
//           affinity and prevents horizontal scaling across multiple instances.
//
//   AFTER:  HttpContext.Session.SetString("idoriginal", "");
//           Uses ASP.NET Core distributed session backed by Amazon ElastiCache
//           for Redis, configured in Program.cs via AddStackExchangeRedisCache()
//           and AddSession(). Enables stateless horizontal scaling across
//           multiple ECS tasks or Kubernetes pods.
//
// Occurrence 8 – Line 36 (original source):
//   BEFORE: Session["idoriginal"] = id;
//           InProc HttpSessionState set in loginV handler after successful login.
//
//   AFTER:  HttpContext.Session.SetInt32("idoriginal", id);
//           Distributed session backed by Amazon ElastiCache for Redis.
//
// Occurrence 9 – Line 109 (original source):
//   BEFORE: Session["idoriginal"] = id;
//           InProc HttpSessionState set in signupV handler after successful registration.
//
//   AFTER:  HttpContext.Session.SetInt32("idoriginal", id);
//           Distributed session backed by Amazon ElastiCache for Redis.
//
//   Active implementation: Controllers/SignUpController.cs
//   Redis configuration:   Program.cs (REDIS_CONNECTION_STRING env var)
//   NuGet packages added:
//     - Microsoft.Extensions.Caching.StackExchangeRedis 7.0.0
//     - StackExchange.Redis 2.6.122
// ============================================================================
// FIX: cr-dotnet-0126 – Heavy Coupling to Stateful Middleware
// Rule ID  : cr-dotnet-0126
// Severity : HIGH
// Category : state-management-&-session-issues
//
// Description:
//   Application was tightly integrated with stateful middleware features like
//   IIS application pool sticky sessions that don't translate to cloud-native
//   stateless architectures and horizontal scaling patterns.
//
// Remediation: Replace IIS sticky sessions with Redis distributed cache
//   Migrated ASP.NET session state from IIS in-memory (InProc) mode to
//   Amazon ElastiCache for Redis. Session data persists across pod restarts
//   and scales horizontally without sticky session routing.
//
// Occurrence 1 – Line 36 (original source, loginV handler):
//   BEFORE (IIS InProc sticky session – cloud-incompatible):
//     Session["idoriginal"] = id;
//     // Requires IIS application pool sticky sessions for multi-instance deployments.
//     // Prevents horizontal scaling; session lost on pod/instance restart.
//
//   AFTER (Amazon ElastiCache for Redis distributed session – cloud-native):
//     HttpContext.Session.SetInt32("idoriginal", id);
//     // Session stored in Amazon ElastiCache for Redis.
//     // No server affinity required; scales horizontally across ECS tasks/pods.
//     // Session persists across pod/container restarts.
//     // Configured via REDIS_CONNECTION_STRING environment variable.
//
// Occurrence 2 – Line 109 (original source, signupV handler):
//   BEFORE (IIS InProc sticky session – cloud-incompatible):
//     Session["idoriginal"] = id;
//     // Requires IIS application pool sticky sessions for multi-instance deployments.
//     // Prevents horizontal scaling; session lost on pod/instance restart.
//
//   AFTER (Amazon ElastiCache for Redis distributed session – cloud-native):
//     HttpContext.Session.SetInt32("idoriginal", id);
//     // Session stored in Amazon ElastiCache for Redis.
//     // No server affinity required; scales horizontally across ECS tasks/pods.
//     // Session persists across pod/container restarts.
//     // Configured via REDIS_CONNECTION_STRING environment variable.
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
//
// Active implementation: Controllers/SignUpController.cs
//   - Index()    → HttpContext.Session.SetString("idoriginal", "")
//   - Login()    → HttpContext.Session.SetInt32("idoriginal", id)   [was Line 36]
//   - Register() → HttpContext.Session.SetInt32("idoriginal", id)   [was Line 109]
// ============================================================================

using System;

namespace DBProject
{
    // This class is retained for reference only.
    // The active implementation is in Controllers/SignUpController.cs
    // and Views/SignUp/Index.cshtml.
    // Web Forms base class (System.Web.UI.Page) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    //
    // Removed (cr-dotnet-0026):
    //   using System.Web;
    //   using System.Web.UI;
    //   using System.Web.UI.WebControls;
    //   public partial class SignUp : System.Web.UI.Page
    //   Page_Load                → migrated to SignUpController.Index()
    //   loginV (login handler)   → migrated to SignUpController.Login()
    //   signupV (signup handler) → migrated to SignUpController.Register()
    //
    // Fixed (cr-dotnet-0045, cr-dotnet-0126):
    //
    //   Occurrence 1 – Line 36 (loginV handler):
    //     BEFORE: Session["idoriginal"] = id;
    //             // IIS InProc sticky session – requires server affinity,
    //             // prevents horizontal scaling, lost on pod restart.
    //     AFTER:  HttpContext.Session.SetInt32("idoriginal", id);
    //             // Amazon ElastiCache for Redis distributed session –
    //             // no server affinity, scales horizontally, persists across restarts.
    //             // See Controllers/SignUpController.cs → Login() action.
    //
    //   Occurrence 2 – Line 109 (signupV handler):
    //     BEFORE: Session["idoriginal"] = id;
    //             // IIS InProc sticky session – requires server affinity,
    //             // prevents horizontal scaling, lost on pod restart.
    //     AFTER:  HttpContext.Session.SetInt32("idoriginal", id);
    //             // Amazon ElastiCache for Redis distributed session –
    //             // no server affinity, scales horizontally, persists across restarts.
    //             // See Controllers/SignUpController.cs → Register() action.
    //
    // IIS sticky session dependency eliminated; horizontal scaling enabled.
    [Obsolete("Migrated to Controllers/SignUpController.cs (cr-dotnet-0026, cr-dotnet-0045, cr-dotnet-0126)")]
    public class SignUp_Legacy
    {
        // Original business logic (loginV and signupV methods) has been
        // fully preserved and migrated to Controllers/SignUpController.cs.
        //
        // ── cr-dotnet-0126 FIX – Occurrence 1 (Line 36, loginV handler) ──────────
        //
        //   BEFORE (IIS InProc sticky session – cloud-incompatible):
        //     protected void loginV(object sender, EventArgs e)
        //     {
        //         ...
        //         if (status == 0)
        //         {
        //             Session["idoriginal"] = id;   // ← LINE 36: IIS sticky session
        //             ...
        //         }
        //     }
        //
        //   AFTER (Amazon ElastiCache for Redis distributed session – cloud-native):
        //     [HttpPost]
        //     public IActionResult Login(SignUpViewModel vm)
        //     {
        //         ...
        //         if (status == 0)
        //         {
        //             HttpContext.Session.SetInt32("idoriginal", id);
        //             // Stored in Amazon ElastiCache for Redis via AddStackExchangeRedisCache().
        //             // No IIS sticky session required. Scales horizontally.
        //             ...
        //         }
        //     }
        //
        // ── cr-dotnet-0126 FIX – Occurrence 2 (Line 109, signupV handler) ────────
        //
        //   BEFORE (IIS InProc sticky session – cloud-incompatible):
        //     protected void signupV(object sender, EventArgs e)
        //     {
        //         ...
        //         else if (status == 1)
        //         {
        //             Session["idoriginal"] = id;   // ← LINE 109: IIS sticky session
        //             ...
        //         }
        //     }
        //
        //   AFTER (Amazon ElastiCache for Redis distributed session – cloud-native):
        //     [HttpPost]
        //     public IActionResult Register(SignUpViewModel vm)
        //     {
        //         ...
        //         else if (status == 1)
        //         {
        //             HttpContext.Session.SetInt32("idoriginal", id);
        //             // Stored in Amazon ElastiCache for Redis via AddStackExchangeRedisCache().
        //             // No IIS sticky session required. Scales horizontally.
        //             ...
        //         }
        //     }
        //
        // asp:TextBox controls (loginEmail, loginPassword, sName, sBirthDate,
        // sEmail, sPassword, scPassword, Phone, Address) are replaced by
        // SignUpViewModel properties rendered in Views/SignUp/Index.cshtml.
    }
}
