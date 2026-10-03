// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (PatientHome.aspx.cs) was the Web Forms code-behind for
// PatientHome.aspx.
// It has been migrated to ASP.NET Core MVC:
//
//   • PatientHome.aspx    → Views/Patient/PatientHome.cshtml  (Razor View)
//   • PatientHome.aspx.cs → Controllers/PatientHomeController.cs (MVC Controller)
//   • (new)               → Models/PatientHomeViewModel.cs (ViewModel)
//
// Web Forms patterns removed / replaced (cr-dotnet-0026):
//   Line 5  – using System.Web;                → removed (not in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not in ASP.NET Core)
//   Line 12 – public partial class PatientHome : System.Web.UI.Page
//             → PatientHomeController (MVC Controller)
//   Line 14 – Page_Load → patientInfo()        → PatientHomeController.Index() [HttpGet]
//   PName.Text = name                          → PatientHomeViewModel.Name
//   PPhone.Text = phone                        → PatientHomeViewModel.Phone
//   PBirthDate.Text = birthDate                → PatientHomeViewModel.BirthDate
//   PatientAge.Text = age.ToString()           → PatientHomeViewModel.Age
//   PAddress.Text = address                    → PatientHomeViewModel.Address
//   PGender.Text = gender                      → PatientHomeViewModel.Gender
//   Response.Write("<script>alert(...)...")    → PatientHomeViewModel.ErrorMessage
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
//   Active implementation: Controllers/PatientHomeController.cs
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
    // The active implementation is in Controllers/PatientHomeController.cs.
    // Web Forms base class (System.Web.UI.Page) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    //
    // Removed (cr-dotnet-0026):
    //   Line 5  – using System.Web;
    //   Line 6  – using System.Web.UI;
    //   Line 7  – using System.Web.UI.WebControls;
    //   Line 12 – public partial class PatientHome : System.Web.UI.Page
    //   Line 14 – Page_Load → patientInfo() → PatientHomeController.Index() [HttpGet]
    //
    // Fixed (cr-dotnet-0045):
    //   Line 28 – Session["idoriginal"] (InProc) →
    //             HttpContext.Session.GetInt32("idoriginal") backed by
    //             Amazon ElastiCache for Redis (see PatientHomeController.cs)
    [Obsolete("Migrated to Controllers/PatientHomeController.cs (cr-dotnet-0026, cr-dotnet-0045)")]
    public class PatientHome_Legacy
    {
        // Original Page_Load → patientInfo():
        //   BEFORE (cr-dotnet-0045 violation, Line 28):
        //     int pid = (int)Session["idoriginal"];  // InProc HttpSessionState
        //
        //   AFTER (cr-dotnet-0045 fix):
        //     int pid = HttpContext.Session.GetInt32("idoriginal") ?? 0;
        //     // Distributed session backed by Amazon ElastiCache for Redis
        //
        //   int status = objmyDAl.patientInfoDisplayer(pid, ref name, ref phone,
        //                    ref address, ref birthDate, ref age, ref gender);
        //   PName.Text = name; PPhone.Text = phone; PBirthDate.Text = birthDate;
        //   PatientAge.Text = age.ToString(); PAddress.Text = address; PGender.Text = gender;
        //   → PatientHomeController.Index() [HttpGet]
    }
}
