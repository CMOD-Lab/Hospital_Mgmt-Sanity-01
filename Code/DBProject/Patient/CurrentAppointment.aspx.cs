// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
//
// Changes applied:
//   Line 5  – removed: using System.Web;                          (occurrence 10)
//   Line 6  – removed: using System.Web.UI;
//   Line 7  – removed: using System.Web.UI.WebControls;
//   Line 13 – removed: public partial class CurrentAppointment : System.Web.UI.Page
//             replaced with: public class CurrentAppointmentModel : PageModel
//   Line 15 – removed: protected void Page_Load(object sender, EventArgs e)
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
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject.Pages.Patient
{
    /// <summary>
    /// Razor Page model for CurrentAppointment – replaces the Web Forms
    /// CurrentAppointment : System.Web.UI.Page code-behind.
    /// Displays the current appointment for the logged-in patient.
    /// Session state is backed by Amazon ElastiCache for Redis (cr-dotnet-0045).
    /// </summary>
    public class CurrentAppointmentModel : PageModel
    {
        /// <summary>
        /// General appointment status message – replaces Appointment.Text label.
        /// </summary>
        public string AppointmentMessage { get; private set; } = string.Empty;

        /// <summary>
        /// Doctor information message – replaces ADoctor.Text label.
        /// </summary>
        public string DoctorMessage { get; private set; } = string.Empty;

        /// <summary>
        /// Appointment timings message – replaces ATimings.Text label.
        /// </summary>
        public string TimingsMessage { get; private set; } = string.Empty;

        /// <summary>
        /// Replaces Page_Load – loads today's appointment on GET.
        /// </summary>
        public void OnGet()
        {
            appointmentToday();
        }

        //-----------------------Function1--------------------------//

        /// <summary>
        /// Fetches today's appointment from the DAL.
        /// Replaces protected void appointmentToday(object sender, EventArgs e).
        /// cr-dotnet-0045: Session["idoriginal"] replaced with
        /// HttpContext.Session.GetInt32("idoriginal") ?? 0 backed by
        /// Amazon ElastiCache for Redis distributed session store.
        /// </summary>
        private void appointmentToday()
        {
            myDAL objmyDAl = new myDAL();

            // cr-dotnet-0045: Replaces (int)Session["idoriginal"]
            // HttpContext.Session is backed by Amazon ElastiCache for Redis via
            // IDistributedCache (registered in Session/RedisSessionConfiguration.cs).
            // The null-coalescing operator ensures safe fallback when the session key
            // is absent, preventing NullReferenceException in stateless cloud deployments.
            int pid = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            string dName = "";
            string timings = "";

            int status = objmyDAl.appointmentTodayDisplayer(pid, ref dName, ref timings);

            if (status == -1)
            {
                // Replaces Appointment.Text = "There was some error..."
                AppointmentMessage = "There was some error in retrieving the Patient's appointment.";
            }
            else if (status == 0)
            {
                // Replaces Appointment.Text = "You have no appointment today..."
                AppointmentMessage = "You have no appointment today with any doctor.";
            }
            else
            {
                if (status == 3)
                {
                    // Replaces ADoctor.Text = "You had an outdated appointment..."
                    DoctorMessage = "You had an outdated appointment with Doctor " + dName + " to which he didn't respond. So that appointment is discarded.";
                    // Replaces ATimings.Text = "The Appointment Timings were : " + timings
                    TimingsMessage = "The Appointment Timings were : " + timings;
                    return;
                }
                else if (status == 2)
                {
                    // Replaces ADoctor.Text = "You have sent an appointment request..."
                    DoctorMessage = "You have sent an appointment request to Doctor " + dName + " which isn't approved by him yet.";
                }
                else
                {
                    // Replaces ADoctor.Text = "Today you have an appointment with Doctor " + dName
                    DoctorMessage = "Today you have an appointment with Doctor " + dName;
                }

                // Replaces ATimings.Text = "The Appointment Timings are : " + timings
                TimingsMessage = "The Appointment Timings are : " + timings;
            }

            return;
        }

        //-----------------------Add a new function here------------------//
    }
}
