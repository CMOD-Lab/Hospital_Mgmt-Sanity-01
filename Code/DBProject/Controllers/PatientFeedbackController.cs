// ASP.NET Core MVC Controller for Patient Feedback
// (cr-dotnet-0026, cr-dotnet-0045, cr-dotnet-0126).
// Migrated from PatientFeedback.aspx.cs (Web Forms code-behind) to
// PatientFeedbackController.cs.
//
// Web Forms patterns replaced:
//   Line 5  – using System.Web;                → removed (not available in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not available in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not available in ASP.NET Core)
//   Line 13 – public partial class PatientFeedback : System.Web.UI.Page
//             → PatientFeedbackController (MVC Controller)
//   Page_Load (IsPostBack check)               → GET/POST action separation
//   pendingFeedback()                          → Index() GET action
//   giveFeedback() (Button OnClick)            → GiveFeedback() POST action
//   Feedback.Text = "..."                      → PatientFeedbackViewModel.FeedbackMessage
//   FDoctor.Text = "..."                       → PatientFeedbackViewModel.DoctorMessage
//   FTimings.Text = "..."                      → PatientFeedbackViewModel.TimingsMessage
//   Message.Visible = true                     → PatientFeedbackViewModel.ShowRatingPrompt
//   List.Visible = true                        → PatientFeedbackViewModel.ShowRatingPrompt
//   button1.Visible = true                     → PatientFeedbackViewModel.ShowRatingPrompt
//   F.Text = "..."                             → PatientFeedbackViewModel.ConfirmationMessage
//   Session["idoriginal"]                      → HttpContext.Session.GetInt32
//   Session["aID"]                             → HttpContext.Session.SetInt32 / GetInt32
//
// cr-dotnet-0045 – Session State Provider fix:
//   Line 21 – Session["aID"] = "" (InProc) → HttpContext.Session.Remove("aID")
//   Line 35 – Session["idoriginal"] (InProc) → HttpContext.Session.GetInt32("idoriginal")
//   Line 56 – Session["aID"] = aID (InProc) → HttpContext.Session.SetInt32("aID", aID)
//   Line 79 – Session["aID"] (InProc) → HttpContext.Session.GetInt32("aID")
//   All backed by Amazon ElastiCache for Redis via REDIS_CONNECTION_STRING env var.
//
// cr-dotnet-0126 – Heavy Coupling to Stateful Middleware fix:
//   IIS application pool sticky sessions replaced with Amazon ElastiCache for Redis
//   distributed cache session store.
//
//   Line 21 – Session["aID"] = "" (IIS InProc sticky session)
//             → HttpContext.Session.Remove("aID")
//               backed by Amazon ElastiCache for Redis.
//   Line 56 – Session["aID"] = aID (IIS InProc sticky session)
//             → HttpContext.Session.SetInt32("aID", aID)
//               backed by Amazon ElastiCache for Redis.
//
//   This eliminates server affinity (sticky sessions) required by IIS in-process
//   session state. Session data persists across pod/container restarts and scales
//   horizontally without sticky session routing.
//
// All business logic from the original code-behind is preserved.

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DBProject.DAL;
using DBProject.Models;

namespace DBProject.Controllers
{
    /// <summary>
    /// Handles the Patient Feedback page.
    /// Replaces PatientFeedback.aspx + PatientFeedback.aspx.cs (Web Forms).
    /// Session state is backed by Amazon ElastiCache for Redis
    /// (cr-dotnet-0045, cr-dotnet-0126 – eliminates IIS sticky session dependency).
    /// </summary>
    public class PatientFeedbackController : Controller
    {
        // -----------------------------------------------------------------------
        // GET /PatientFeedback
        // Replaces Page_Load (when !IsPostBack) → pendingFeedback() in PatientFeedback.aspx.cs
        // cr-dotnet-0045 / cr-dotnet-0126:
        //   Session["aID"] = "" (IIS InProc sticky session) →
        //   HttpContext.Session.Remove("aID") backed by Amazon ElastiCache for Redis.
        //   No server affinity required; scales horizontally.
        // -----------------------------------------------------------------------
        [HttpGet]
        public IActionResult Index()
        {
            // cr-dotnet-0045 / cr-dotnet-0126 fix (Line 21):
            // Replaced InProc Session["aID"] = "" (IIS sticky session) with
            // distributed Redis-backed session Remove operation.
            // Session data persists across pod restarts; no server affinity needed.
            HttpContext.Session.Remove("aID");

            var vm = new PatientFeedbackViewModel();
            LoadPendingFeedback(vm);
            return View(vm);
        }

        // -----------------------------------------------------------------------
        // POST /PatientFeedback/GiveFeedback
        // Replaces giveFeedback (Button OnClick handler) in PatientFeedback.aspx.cs
        // cr-dotnet-0045 / cr-dotnet-0126:
        //   Session["aID"] (Line 79, IIS InProc sticky session) →
        //   HttpContext.Session.GetInt32("aID") backed by Amazon ElastiCache for Redis.
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GiveFeedback(int rating)
        {
            var vm = new PatientFeedbackViewModel();

            var objmyDAl = new myDAL();

            // cr-dotnet-0045 / cr-dotnet-0126 fix (Line 79):
            // Replaced InProc Session["aID"] (IIS sticky session) read with
            // distributed Redis-backed session via HttpContext.Session.GetInt32().
            int aID = HttpContext.Session.GetInt32("aID") ?? 0;

            // Replaces: int rating = Convert.ToInt32(List.SelectedItem.Value);
            // rating is now a POST parameter bound from the form's <select name="rating">

            // Replaces: int status = objmyDAl.givePendingFeedback(aID);
            int status = objmyDAl.givePendingFeedback(aID);

            if (status == -1)
            {
                // Replaces: F.Text = "There was some error.";
                vm.ConfirmationMessage = "There was some error.";
            }
            else if (status == 0)
            {
                // Replaces: F.Text = "Thank you for your feedback :)";
                vm.ConfirmationMessage = "Thank you for your feedback :)";
            }

            return View("Index", vm);
        }

        // -----------------------------------------------------------------------
        // Helper: loads pending feedback data from DAL into the view model.
        // Replaces pendingFeedback() method in PatientFeedback.aspx.cs.
        // cr-dotnet-0045 / cr-dotnet-0126:
        //   Session["idoriginal"] (Line 35, InProc) and
        //   Session["aID"] = aID (Line 56, IIS InProc sticky session) →
        //   HttpContext.Session.GetInt32/SetInt32 backed by Amazon ElastiCache for Redis.
        //   Eliminates IIS application pool sticky session dependency.
        // -----------------------------------------------------------------------
        private void LoadPendingFeedback(PatientFeedbackViewModel vm)
        {
            var objmyDAl = new myDAL();

            // cr-dotnet-0045 fix (Line 35):
            // Replaced InProc Session["idoriginal"] with distributed Redis-backed session.
            int pid = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            string dName = "";
            string timings = "";
            int aID = 0;

            // Replaces: int status = objmyDAl.isFeedbackPending(pid, ref dName, ref timings, ref aID);
            int status = objmyDAl.isFeedbackPending(pid, ref dName, ref timings, ref aID);

            if (status == -1)
            {
                // Replaces: Feedback.Text = "There was some error in retrieving the Pending Feedbacks.";
                vm.FeedbackMessage = "There was some error in retrieving the Pending Feedbacks.";
            }
            else if (status == 0)
            {
                // Replaces: Feedback.Text = "There are no pending feedbacks :)";
                vm.FeedbackMessage = "There are no pending feedbacks :)";
            }
            else
            {
                // cr-dotnet-0045 / cr-dotnet-0126 fix (Line 56):
                // Replaced InProc Session["aID"] = aID (IIS sticky session) with
                // distributed Redis-backed session via HttpContext.Session.SetInt32().
                // Session data persists across pod restarts; no server affinity needed.
                HttpContext.Session.SetInt32("aID", aID);

                // Replaces: FDoctor.Text = "Your feedback for the appointment with Doctor " + dName + " is pending. Kindly give it.";
                vm.DoctorMessage = "Your feedback for the appointment with Doctor " + dName + " is pending. Kindly give it.";

                // Replaces: FTimings.Text = "The Appointment Timings were : " + timings;
                vm.TimingsMessage = "The Appointment Timings were : " + timings;

                // Replaces: Message.Visible = true; List.Visible = true; button1.Visible = true;
                vm.ShowRatingPrompt = true;
            }
        }
    }
}
