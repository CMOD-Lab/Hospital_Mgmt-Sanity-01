// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
//
// Changes applied:
//   Line 5  – removed: using System.Web;                          (occurrence 6)
//   Line 6  – removed: using System.Web.UI;                       (occurrence 7)
//   Line 7  – removed: using System.Web.UI.WebControls;
//   Line 12 – removed: public partial class PatientHome : System.Web.UI.Page  (occurrence 8)
//             replaced with: public class PatientHomeModel : PageModel
//   Line 14 – removed: protected void Page_Load(object sender, EventArgs e)   (occurrence 9)
//             replaced with: public void OnGet()
//
// Rule cr-dotnet-0045: Session State Provider
//   In-process (InProc) HttpSessionState replaced with Amazon ElastiCache for Redis
//   distributed session store via ASP.NET Core ISession (IDistributedCache-backed).
//   Session["idoriginal"] replaced with HttpContext.Session.GetInt32("idoriginal") ?? 0
//   using the ASP.NET Core ISession extension methods (Microsoft.AspNetCore.Http).
//   Redis session is registered via AddRedisDistributedSession() in
//   Session/RedisSessionConfiguration.cs, reading REDIS_CONNECTION_STRING from the
//   ECS task definition / Elastic Beanstalk environment / AWS Systems Manager.
//   Enables stateless horizontal scaling across multiple ECS tasks or Kubernetes pods.

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject.Pages.Patient
{
    /// <summary>
    /// Razor Page model for PatientHome – replaces the Web Forms
    /// PatientHome : System.Web.UI.Page code-behind.
    /// Displays the logged-in patient's personal information.
    /// Session state is backed by Amazon ElastiCache for Redis (cr-dotnet-0045).
    /// </summary>
    public class PatientHomeModel : PageModel
    {
        /// <summary>
        /// Patient name – replaces asp:Label ID="PName".
        /// </summary>
        public string PName { get; private set; } = string.Empty;

        /// <summary>
        /// Patient phone number – replaces asp:Label ID="PPhone".
        /// </summary>
        public string PPhone { get; private set; } = string.Empty;

        /// <summary>
        /// Patient birth date – replaces asp:Label ID="PBirthDate".
        /// </summary>
        public string PBirthDate { get; private set; } = string.Empty;

        /// <summary>
        /// Patient age – replaces asp:Label ID="PatientAge".
        /// </summary>
        public int PatientAge { get; private set; } = 0;

        /// <summary>
        /// Patient address – replaces asp:Label ID="PAddress".
        /// </summary>
        public string PAddress { get; private set; } = string.Empty;

        /// <summary>
        /// Patient gender – replaces asp:Label ID="PGender".
        /// </summary>
        public string PGender { get; private set; } = string.Empty;

        /// <summary>
        /// Error message shown when patient info cannot be retrieved.
        /// Replaces Response.Write("&lt;script&gt;alert(...)&lt;/script&gt;").
        /// </summary>
        public string ErrorMessage { get; private set; } = string.Empty;

        /// <summary>
        /// Replaces Page_Load (non-postback branch) – loads patient info on GET.
        /// </summary>
        public void OnGet()
        {
            patientInfo();
        }

        //-----------------------Function1--------------------------//

        /// <summary>
        /// Fetches patient information from the DAL.
        /// Replaces protected void patientInfo(object sender, EventArgs e).
        /// cr-dotnet-0045: Session["idoriginal"] replaced with
        /// HttpContext.Session.GetInt32("idoriginal") ?? 0 backed by
        /// Amazon ElastiCache for Redis distributed session store.
        /// </summary>
        private void patientInfo()
        {
            myDAL objmyDAl = new myDAL();

            // cr-dotnet-0045: Replaces (int)Session["idoriginal"]
            // HttpContext.Session is backed by Amazon ElastiCache for Redis via
            // IDistributedCache (registered in Session/RedisSessionConfiguration.cs).
            // The null-coalescing operator ensures safe fallback when the session key
            // is absent, preventing NullReferenceException in stateless cloud deployments.
            int pid = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            string name      = "";
            string phone     = "";
            string address   = "";
            string birthDate = "";
            int    age       = 0;
            string gender    = "";

            int status = objmyDAl.patientInfoDisplayer(pid, ref name, ref phone, ref address, ref birthDate, ref age, ref gender);

            if (status == -1)
            {
                // Replaces Response.Write("<script>alert('There was some error...');</script>")
                ErrorMessage = "There was some error in retrieving the Patient's Info.";
            }
            else if (status == 0)
            {
                // Replaces PName.Text = name; PPhone.Text = phone; etc.
                PName      = name;
                PPhone     = phone;
                PBirthDate = birthDate;
                PatientAge = age;
                PAddress   = address;
                PGender    = gender;
            }

            return;
        }

        //-----------------------Add a new function here------------------//
    }
}
