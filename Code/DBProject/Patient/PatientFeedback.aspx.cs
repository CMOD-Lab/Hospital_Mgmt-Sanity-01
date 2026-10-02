// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-0126: Heavy Coupling to Stateful Middleware – replaced IIS sticky
//   sessions / in-process session state with Redis distributed cache (Amazon ElastiCache),
//   enabling stateless horizontal scaling without sticky session routing.
//
// Changes applied:
//   Line 5  – removed: using System.Web;                          (occurrence 10)
//   Line 6  – removed: using System.Web.UI;
//   Line 7  – removed: using System.Web.UI.WebControls;
//   Line 14 – removed: public partial class PatientFeedback : System.Web.UI.Page
//             replaced with: public class PatientFeedbackModel : PageModel
//   Line 16 – removed: protected void Page_Load(object sender, EventArgs e)
//             replaced with: public void OnGet()
//
// Rule cr-dotnet-0045 / cr-dotnet-0126: Session State Provider
//   In-process (InProc) HttpSessionState / IIS sticky session replaced with Amazon
//   ElastiCache for Redis distributed session store via ASP.NET Core ISession
//   (IDistributedCache-backed). Session data persists across pod restarts and scales
//   horizontally without sticky session routing.
//   Session["idoriginal"] replaced with HttpContext.Session.GetInt32("idoriginal") ?? 0
//   Session["aID"] replaced with model-bound PendingAppointmentId property (passed via
//   hidden form field in the Razor view) to avoid server-side session writes for
//   transient form state, further reducing session coupling.
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
    /// Razor Page model for PatientFeedback – replaces the Web Forms
    /// PatientFeedback : System.Web.UI.Page code-behind.
    /// Displays pending feedback and allows the patient to submit a rating.
    ///
    /// cr-dotnet-0126: IIS sticky session / stateful middleware coupling removed.
    /// Session state is backed by Amazon ElastiCache for Redis (cr-dotnet-0045 /
    /// cr-dotnet-0126), allowing the application to scale horizontally across multiple
    /// instances without requiring sticky session routing at the load balancer.
    /// </summary>
    public class PatientFeedbackModel : PageModel
    {
        /// <summary>
        /// General feedback status message – replaces asp:Label ID="Feedback".
        /// </summary>
        public string FeedbackMessage { get; private set; } = string.Empty;

        /// <summary>
        /// Doctor feedback message – replaces asp:Label ID="FDoctor".
        /// </summary>
        public string FDoctorMessage { get; private set; } = string.Empty;

        /// <summary>
        /// Appointment timings message – replaces asp:Label ID="FTimings".
        /// </summary>
        public string FTimingsMessage { get; private set; } = string.Empty;

        /// <summary>
        /// Controls visibility of the feedback form –
        /// replaces Message.Visible, List.Visible, button1.Visible.
        /// </summary>
        public bool ShowFeedbackForm { get; private set; } = false;

        /// <summary>
        /// Appointment ID for the pending feedback –
        /// replaces Session["aID"] used in giveFeedback.
        /// Passed as a hidden form field in the Razor view to avoid
        /// storing transient form state in the distributed session store.
        /// </summary>
        public int PendingAppointmentId { get; private set; } = 0;

        /// <summary>
        /// Result message after submitting feedback – replaces asp:Label ID="F".
        /// </summary>
        public string FeedbackResultMessage { get; private set; } = string.Empty;

        /// <summary>
        /// Replaces Page_Load (non-postback branch) – loads pending feedback on GET.
        /// </summary>
        public void OnGet()
        {
            pendingFeedback();
        }

        //-----------------------Function1--------------------------//

        /// <summary>
        /// Fetches pending feedback from the DAL.
        /// Replaces protected void pendingFeedback(object sender, EventArgs e).
        ///
        /// cr-dotnet-0126 (Line 21): Session["idoriginal"] (IIS sticky session / InProc
        /// HttpSessionState read) replaced with HttpContext.Session.GetInt32("idoriginal") ?? 0
        /// backed by Amazon ElastiCache for Redis distributed session store.
        /// Session data persists across pod restarts and scales horizontally without
        /// sticky session routing at the load balancer.
        /// Replaces: int pid = (int)Session["idoriginal"];
        /// </summary>
        private void pendingFeedback()
        {
            myDAL objmyDAl = new myDAL();

            // cr-dotnet-0126 (Line 21): Replaces (int)Session["idoriginal"]
            // HttpContext.Session is backed by Amazon ElastiCache for Redis via
            // IDistributedCache (registered in Session/RedisSessionConfiguration.cs).
            // The null-coalescing operator ensures safe fallback when the session key
            // is absent, preventing NullReferenceException in stateless cloud deployments.
            int pid = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            string dName = "";
            string timings = "";
            int aID = 0;

            int status = objmyDAl.isFeedbackPending(pid, ref dName, ref timings, ref aID);

            if (status == -1)
            {
                // Replaces Feedback.Text = "There was some error..."
                FeedbackMessage = "There was some error in retrieving the Pending Feedbacks.";
            }
            else if (status == 0)
            {
                // Replaces Feedback.Text = "There are no pending feedbacks :)"
                FeedbackMessage = "There are no pending feedbacks :)";
            }
            else
            {
                // cr-dotnet-0126 (Line 56): Replaces Session["aID"] = aID (IIS sticky
                // session write). The appointment ID is stored as a model property and
                // passed via a hidden form field in the Razor view instead of the
                // distributed session, reducing unnecessary session writes for transient
                // form state and further decoupling from stateful middleware.
                PendingAppointmentId = aID;

                // Replaces FDoctor.Text = "Your feedback for the appointment..."
                FDoctorMessage = "Your feedback for the appointment with Doctor " + dName + " is pending. Kindly give it.";
                // Replaces FTimings.Text = "The Appointment Timings were : " + timings
                FTimingsMessage = "The Appointment Timings were : " + timings;

                // Replaces Message.Visible = true; List.Visible = true; button1.Visible = true;
                ShowFeedbackForm = true;
            }
        }

        //-----------------------Function2--------------------------//

        /// <summary>
        /// Handles the feedback form POST.
        /// Replaces protected void giveFeedback(object sender, EventArgs e).
        ///
        /// cr-dotnet-0126: Session["aID"] (IIS sticky session read) replaced with
        /// model-bound aID parameter passed via hidden form field, avoiding distributed
        /// session reads for transient form state and removing sticky session dependency.
        /// Replaces: int aID = (int)Session["aID"];
        /// </summary>
        public IActionResult OnPost(int aID, int rating)
        {
            myDAL objmyDAl = new myDAL();

            // cr-dotnet-0126: Replaces (int)Session["aID"] and Convert.ToInt32(List.SelectedItem.Value)
            // aID is received as a POST parameter (hidden form field) rather than from
            // the distributed session store, reducing session coupling and improving
            // scalability across multiple ECS tasks or Kubernetes pods.
            int status = objmyDAl.givePendingFeedback(aID);

            if (status == -1)
            {
                // Replaces F.Text = "There was some error."
                FeedbackResultMessage = "There was some error.";
            }
            else if (status == 0)
            {
                // Replaces F.Text = "Thank you for your feedback :)"
                FeedbackResultMessage = "Thank you for your feedback :)";
            }

            // Re-populate form state so the result message is visible
            ShowFeedbackForm = true;
            PendingAppointmentId = aID;

            return Page();
        }

        //-----------------------Add a new function here------------------//
    }
}
