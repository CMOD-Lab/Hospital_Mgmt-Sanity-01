// ============================================================================
// MIGRATION NOTE (cr-dotnet-0026 – Web Forms Usage):
// This file (DoctorProfile.aspx.cs) was the Web Forms code-behind for
// DoctorProfile.aspx.
// It has been migrated to ASP.NET Core MVC:
//
//   • DoctorProfile.aspx    → Views/Patient/DoctorProfile.cshtml  (Razor View)
//   • DoctorProfile.aspx.cs → Controllers/DoctorProfileController.cs (MVC Controller)
//   • (new)                 → Models/DoctorProfileViewModel.cs (ViewModel)
//
// Web Forms patterns removed / replaced (cr-dotnet-0026):
//   Line 5  – using System.Web;                → removed (not in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not in ASP.NET Core)
//   Line 14 – public partial class DoctorProfile : System.Web.UI.Page
//             → DoctorProfileController (MVC Controller)
//   Line 16 – Page_Load → doctorInfo()         → DoctorProfileController.Index() [HttpGet]
//   DName.Text = name                          → DoctorProfileViewModel.Name
//   DPhone.Text = phone                        → DoctorProfileViewModel.Phone
//   DQualification.Text = qualification        → DoctorProfileViewModel.Qualification
//   DSpecialization.Text = specialization      → DoctorProfileViewModel.Specialization
//   DWork.Text = workE.ToString()              → DoctorProfileViewModel.WorkExperience
//   DAge.Text = age.ToString()                 → DoctorProfileViewModel.Age
//   DGender.Text = gender                      → DoctorProfileViewModel.Gender
//   DDept.Text = deptName                      → DoctorProfileViewModel.Department
//   DCharges.Text = charges_Per_Visit.ToString() → DoctorProfileViewModel.ChargesPerVisit
//   DRI.Text = ReputeIndex.ToString()          → DoctorProfileViewModel.ReputeIndex
//   DPT.Text = PatientsTreated.ToString()      → DoctorProfileViewModel.PatientsTreated
//   Response.Redirect("AppointmentTaker.aspx") → RedirectToAction("Index","AppointmentTaker")
//
// ============================================================================
// MIGRATION NOTE (cr-dotnet-0045 – Session State Provider):
// Line 29 – Session["dID"] (InProc HttpSessionState)
//   BEFORE: string dID1 = (string)Session["dID"];
//           Uses in-process (InProc) HttpSessionState which creates server
//           affinity and prevents horizontal scaling across multiple instances.
//
//   AFTER:  string dID1 = HttpContext.Session.GetString("dID");
//           Uses ASP.NET Core distributed session backed by Amazon ElastiCache
//           for Redis.
//
// Line 45 – Session["deptOriginal"] (InProc HttpSessionState)
//   BEFORE: string deptName = (string)Session["deptOriginal"];
//           Uses in-process (InProc) HttpSessionState.
//
//   AFTER:  string deptName = HttpContext.Session.GetString("deptOriginal") ?? "";
//           Uses ASP.NET Core distributed session backed by Amazon ElastiCache
//           for Redis.
//
//   Active implementation: Controllers/DoctorProfileController.cs
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
    // The active implementation is in Controllers/DoctorProfileController.cs.
    // Web Forms base class (System.Web.UI.Page) and all server-control
    // references have been removed as part of the ASP.NET Core MVC migration.
    //
    // Removed (cr-dotnet-0026):
    //   Line 5  – using System.Web;
    //   Line 6  – using System.Web.UI;
    //   Line 7  – using System.Web.UI.WebControls;
    //   Line 14 – public partial class DoctorProfile : System.Web.UI.Page
    //   Line 16 – Page_Load → doctorInfo() → DoctorProfileController.Index() [HttpGet]
    //
    // Fixed (cr-dotnet-0045):
    //   Line 29 – Session["dID"] (InProc) →
    //             HttpContext.Session.GetString("dID") backed by
    //             Amazon ElastiCache for Redis (see DoctorProfileController.cs)
    //   Line 45 – Session["deptOriginal"] (InProc) →
    //             HttpContext.Session.GetString("deptOriginal") backed by
    //             Amazon ElastiCache for Redis (see DoctorProfileController.cs)
    [Obsolete("Migrated to Controllers/DoctorProfileController.cs (cr-dotnet-0026, cr-dotnet-0045)")]
    public class DoctorProfile_Legacy
    {
        // Original Page_Load → doctorInfo():
        //   BEFORE (cr-dotnet-0045 violation, Line 29):
        //     string dID1 = (string)Session["dID"];  // InProc HttpSessionState
        //
        //   AFTER (cr-dotnet-0045 fix):
        //     string dID1 = HttpContext.Session.GetString("dID");
        //     // Distributed session backed by Amazon ElastiCache for Redis
        //
        //   BEFORE (cr-dotnet-0045 violation, Line 45):
        //     string deptName = (string)Session["deptOriginal"];  // InProc HttpSessionState
        //
        //   AFTER (cr-dotnet-0045 fix):
        //     string deptName = HttpContext.Session.GetString("deptOriginal") ?? "";
        //     // Distributed session backed by Amazon ElastiCache for Redis
        //
        //   → DoctorProfileController.Index() [HttpGet]
        //
        // Original RedirectToAppointmentTaker():
        //   Response.Redirect("AppointmentTaker.aspx")
        //   → DoctorProfileController.TakeAppointment() [HttpPost]
        //     → RedirectToAction("Index", "AppointmentTaker")
    }
}
