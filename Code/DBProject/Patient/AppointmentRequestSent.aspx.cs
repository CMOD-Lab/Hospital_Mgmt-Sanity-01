// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
//
// Changes applied:
//   Line 5  – removed: using System.Web;                          (occurrence 5)
//   Line 6  – removed: using System.Web.UI;                       (occurrence 6)
//   Line 7  – removed: using System.Web.UI.WebControls;
//   Line 14 – removed: public partial class AppointmentNotificationSent : System.Web.UI.Page
//             replaced with: public class AppointmentRequestSentModel : PageModel  (occurrence 7)
//   Line 16 – removed: protected void Page_Load(object sender, EventArgs e) {}    (occurrence 8)
//             replaced with: public void OnGet() {}
//
// Rule cr-dotnet-0045: Session State Provider (Lines 28, 33, 36)
//   In-process HttpSessionState (InProc) replaced with Amazon ElastiCache for Redis
//   distributed session store via ASP.NET Core ISession / IDistributedCache.
//   Session["dID"]        replaced with HttpContext.Session.GetString("dID")
//   Session["idoriginal"] replaced with HttpContext.Session.GetInt32("idoriginal")
//   Session["freeSlot"]   replaced with HttpContext.Session.GetString("freeSlot")
//   All backed by StackExchange.Redis connected to the ElastiCache Redis cluster.
//   Enables stateless horizontal scaling across multiple ECS tasks or Kubernetes pods.
//   Redis connection configured via REDIS_CONNECTION_STRING environment variable.
//   See Session/RedisSessionConfiguration.cs for service registration details.

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject.Pages.Patient
{
    /// <summary>
    /// Razor Page model for AppointmentRequestSent – replaces the Web Forms
    /// AppointmentNotificationSent : System.Web.UI.Page code-behind.
    /// Sends an appointment request from the logged-in patient to the selected doctor.
    ///
    /// cr-dotnet-0045: Session state is backed by Amazon ElastiCache for Redis
    /// via ASP.NET Core ISession (IDistributedCache), replacing InProc HttpSessionState.
    /// </summary>
    public class AppointmentRequestSentModel : PageModel
    {
        /// <summary>
        /// Bound message displayed after the request is sent – replaces Message.Text label.
        /// </summary>
        public string Message { get; private set; } = string.Empty;

        /// <summary>
        /// Replaces Page_Load – no initialisation required on GET.
        /// </summary>
        public void OnGet()
        {
        }

        //-----------------------Function1--------------------------//

        /// <summary>
        /// Sends the appointment request to the doctor.
        /// Replaces protected void sendARequest(object sender, EventArgs e).
        /// </summary>
        public IActionResult OnPostSendRequest()
        {
            myDAL objmyDAl = new myDAL();

            // cr-dotnet-0045 (Line 28): Session["dID"] replaced with
            // HttpContext.Session.GetString("dID") – reads from Amazon ElastiCache
            // for Redis distributed session store (IDistributedCache-backed ISession).
            string dID1 = HttpContext.Session.GetString("dID");
            int dID = Convert.ToInt32(dID1);

            // cr-dotnet-0045 (Line 33): Session["idoriginal"] replaced with
            // HttpContext.Session.GetInt32("idoriginal") – reads from ElastiCache Redis.
            int pID = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            // cr-dotnet-0045 (Line 36): Session["freeSlot"] replaced with
            // HttpContext.Session.GetString("freeSlot") – reads from ElastiCache Redis.
            string temp = HttpContext.Session.GetString("freeSlot");
            int freeSlot = Convert.ToInt32(temp);

            string mes = string.Empty;

            int status = objmyDAl.insertAppointment(dID, pID, freeSlot, ref mes);

            if (status == -1)
            {
                // Replaces Message.Text = "There was some error..."
                Message = "There was some error in sending appointment request to the Doctor.";
            }
            else
            {
                // Replaces Message.Text = mes;
                Message = mes;
            }

            return Page();
        }
    }
}
