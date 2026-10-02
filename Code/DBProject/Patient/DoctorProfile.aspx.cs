// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
//
// Changes applied:
//   Line 5  – removed: using System.Web;                          (occurrence 5)
//   Line 6  – removed: using System.Web.UI;                       (occurrence 6)
//   Line 7  – removed: using System.Web.UI.WebControls;
//   Line 14 – removed: public partial class DoctorProfile : System.Web.UI.Page
//             replaced with: public class DoctorProfileModel : PageModel  (occurrence 7)
//   Line 16 – removed: protected void Page_Load(object sender, EventArgs e)
//             replaced with: public void OnGet()                   (occurrence 8)
//
// Rule cr-dotnet-0045: Session State Provider
//   In-process (InProc) HttpSessionState replaced with Amazon ElastiCache for Redis
//   distributed session store via ASP.NET Core ISession (IDistributedCache-backed).
//   Session["dID"] replaced with HttpContext.Session.GetString("dID") ?? "0"
//   Session["deptOriginal"] replaced with HttpContext.Session.GetString("deptOriginal") ?? string.Empty
//   using the ASP.NET Core ISession extension methods (Microsoft.AspNetCore.Http).
//   Redis session is registered via AddRedisDistributedSession() in
//   Session/RedisSessionConfiguration.cs, reading REDIS_CONNECTION_STRING from the
//   ECS task definition / Elastic Beanstalk environment / AWS Systems Manager.
//   Enables stateless horizontal scaling across multiple ECS tasks or Kubernetes pods.

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject.Pages.Patient
{
    /// <summary>
    /// Razor Page model for DoctorProfile – replaces the Web Forms
    /// DoctorProfile : System.Web.UI.Page code-behind.
    /// Displays the profile of the selected doctor.
    /// Session state is backed by Amazon ElastiCache for Redis (cr-dotnet-0045).
    /// </summary>
    public class DoctorProfileModel : PageModel
    {
        /// <summary>Replaces asp:Label ID="DName".</summary>
        public string DName { get; private set; } = string.Empty;

        /// <summary>Replaces asp:Label ID="DPhone".</summary>
        public string DPhone { get; private set; } = string.Empty;

        /// <summary>Replaces asp:Label ID="DQualification".</summary>
        public string DQualification { get; private set; } = string.Empty;

        /// <summary>Replaces asp:Label ID="DSpecialization".</summary>
        public string DSpecialization { get; private set; } = string.Empty;

        /// <summary>Replaces asp:Label ID="DWork".</summary>
        public string DWork { get; private set; } = string.Empty;

        /// <summary>Replaces asp:Label ID="DAge".</summary>
        public string DAge { get; private set; } = string.Empty;

        /// <summary>Replaces asp:Label ID="DGender".</summary>
        public string DGender { get; private set; } = string.Empty;

        /// <summary>Replaces asp:Label ID="DDept".</summary>
        public string DDept { get; private set; } = string.Empty;

        /// <summary>Replaces asp:Label ID="DCharges".</summary>
        public string DCharges { get; private set; } = string.Empty;

        /// <summary>Replaces asp:Label ID="DRI".</summary>
        public string DRI { get; private set; } = string.Empty;

        /// <summary>Replaces asp:Label ID="DPT".</summary>
        public string DPT { get; private set; } = string.Empty;

        /// <summary>Error message shown when doctor info retrieval fails.</summary>
        public string ErrorMessage { get; private set; } = string.Empty;

        /// <summary>
        /// Replaces Page_Load – loads doctor profile on GET.
        /// </summary>
        public void OnGet()
        {
            doctorInfo();
        }

        //-----------------------Function1--------------------------//

        /// <summary>
        /// Fetches doctor information from the DAL.
        /// Replaces protected void doctorInfo(object sender, EventArgs e).
        /// cr-dotnet-0045: Session["dID"] and Session["deptOriginal"] replaced with
        /// HttpContext.Session.GetString() backed by Amazon ElastiCache for Redis
        /// distributed session store.
        /// </summary>
        private void doctorInfo()
        {
            myDAL objmyDAl = new myDAL();

            // cr-dotnet-0045: Replaces (string)Session["dID"] + Convert.ToInt32(...)
            // HttpContext.Session is backed by Amazon ElastiCache for Redis via
            // IDistributedCache (registered in Session/RedisSessionConfiguration.cs).
            // The null-coalescing operator ensures safe fallback when the session key
            // is absent, preventing NullReferenceException in stateless cloud deployments.
            string dID1 = HttpContext.Session.GetString("dID") ?? "0";
            int dID = Convert.ToInt32(dID1);

            string name = "";
            string phone = "";
            string gender = "";

            float charges_Per_Visit = 0;
            float ReputeIndex = 0;
            int PatientsTreated = 0;
            string qualification = "";
            string specialization = "";
            int workE = 0;
            int age = 0;

            // cr-dotnet-0045: Replaces (string)Session["deptOriginal"]
            // HttpContext.Session is backed by Amazon ElastiCache for Redis via
            // IDistributedCache (registered in Session/RedisSessionConfiguration.cs).
            string deptName = HttpContext.Session.GetString("deptOriginal") ?? string.Empty;

            int status = objmyDAl.doctorInfoDisplayer(dID, ref name, ref phone, ref gender,
                ref charges_Per_Visit, ref ReputeIndex, ref PatientsTreated,
                ref qualification, ref specialization, ref workE, ref age);

            if (status == -1)
            {
                // Replaces Response.Write("<script>alert('There was some error...');</script>")
                ErrorMessage = "There was some error in retrieving the Doctor's Info.";
            }
            else if (status == 0)
            {
                // Replaces DName.Text = name; DPhone.Text = phone; etc.
                DName = name;
                DPhone = phone;
                DQualification = qualification;
                DSpecialization = specialization;
                DWork = workE.ToString();
                DAge = age.ToString();
                DGender = gender;
                DDept = deptName;
                DCharges = charges_Per_Visit.ToString();
                DRI = ReputeIndex.ToString();
                DPT = PatientsTreated.ToString();
            }

            return;
        }

        //-----------------------Function2------------------//
        // RedirectToAppointmentTaker has been replaced by an HTML anchor link
        // in the Razor view (DoctorProfile.aspx), pointing to /Patient/AppointmentTaker.
        // No server-side postback or Response.Redirect is needed in Razor Pages.

        //-----------------------Add a new function here------------------//
    }
}
