// ASP.NET Core MVC Controller for Patient Notifications (cr-dotnet-0026).
// Migrated from PatientNotifications.aspx.cs (Web Forms code-behind) to
// PatientNotificationsController.cs.
//
// Web Forms patterns replaced:
//   Line 5  – using System.Web;                → removed (not available in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not available in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not available in ASP.NET Core)
//   Line 15 – public partial class PatientNotifications : System.Web.UI.Page
//             → PatientNotificationsController (MVC Controller)
//   Line 17 – Page_Load → Notifications()     → Index() GET action
//   Notify.Text = "..."                        → PatientNotificationsViewModel.NotifyMessage
//   NDoctor.Text = "..."                       → PatientNotificationsViewModel.DoctorMessage
//   NTimings.Text = "..."                      → PatientNotificationsViewModel.TimingsMessage
//   Session["idoriginal"]                      → HttpContext.Session.GetInt32
//
// All business logic from the original code-behind is preserved.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DBProject.DAL;
using DBProject.Models;

namespace DBProject.Controllers
{
    /// <summary>
    /// Handles the Patient Notifications page.
    /// Replaces PatientNotifications.aspx + PatientNotifications.aspx.cs (Web Forms).
    /// </summary>
    public class PatientNotificationsController : Controller
    {
        // -----------------------------------------------------------------------
        // GET /PatientNotifications
        // Replaces Page_Load → Notifications() in PatientNotifications.aspx.cs
        // -----------------------------------------------------------------------
        [HttpGet]
        public IActionResult Index()
        {
            var vm = new PatientNotificationsViewModel();
            LoadNotifications(vm);
            return View(vm);
        }

        // -----------------------------------------------------------------------
        // Helper: loads patient notifications from DAL into the view model.
        // Replaces Notifications() method in PatientNotifications.aspx.cs.
        // -----------------------------------------------------------------------
        private void LoadNotifications(PatientNotificationsViewModel vm)
        {
            var objmyDAl = new myDAL();

            // Replaces: int pid = (int)Session["idoriginal"];
            int pid = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            string dName   = "";
            string timings = "";

            // Replaces: int status = objmyDAl.getNotifications(pid, ref dName, ref timings);
            int status = objmyDAl.getNotifications(pid, ref dName, ref timings);

            if (status == -1)
            {
                // Replaces: Notify.Text = "There was some error in retrieving the Patient's notifications.";
                vm.NotifyMessage = "There was some error in retrieving the Patient's notifications.";
            }
            else if (status == 0)
            {
                // Replaces: Notify.Text = "There are no new notifications :)";
                vm.NotifyMessage = "There are no new notifications :)";
            }
            else
            {
                if (status == 1)
                {
                    // Replaces: NDoctor.Text = "Your requested appointment with Doctor " + dName + " has been accepted by him! :)";
                    //           NTimings.Text = "The Appointment Timings are : " + timings;
                    vm.DoctorMessage  = "Your requested appointment with Doctor " + dName + " has been accepted by him! :)";
                    vm.TimingsMessage = "The Appointment Timings are : " + timings;
                    return;
                }
                else if (status == 2)
                {
                    // Replaces: NDoctor.Text = "Your requested appointment with Doctor " + dName + " has been rejected by him! :(";
                    //           NTimings.Text = "The Appointment Timings were : " + timings;
                    vm.DoctorMessage  = "Your requested appointment with Doctor " + dName + " has been rejected by him! :(";
                    vm.TimingsMessage = "The Appointment Timings were : " + timings;
                    return;
                }
                else if (status == 3)
                {
                    // Replaces: NDoctor.Text = "Your appointment with Doctor " + dName + " has been completed now. We hope you are feeling better now!";
                    //           NTimings.Text = "The Appointment Timings were : " + timings;
                    vm.DoctorMessage  = "Your appointment with Doctor " + dName + " has been completed now. We hope you are feeling better now!";
                    vm.TimingsMessage = "The Appointment Timings were : " + timings;
                    return;
                }
            }
        }
    }
}
