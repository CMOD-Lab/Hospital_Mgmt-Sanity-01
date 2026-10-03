// ASP.NET Core MVC Controller for Current Appointment (cr-dotnet-0026).
// Migrated from CurrentAppointment.aspx.cs (Web Forms code-behind) to
// CurrentAppointmentController.cs.
//
// Web Forms patterns replaced:
//   Line 5  – using System.Web;                → removed (not available in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not available in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not available in ASP.NET Core)
//   Line 13 – public partial class CurrentAppointment : System.Web.UI.Page
//             → CurrentAppointmentController (MVC Controller)
//   Line 15 – Page_Load → appointmentToday()   → Index() GET action
//   Appointment.Text = "..."                   → CurrentAppointmentViewModel.AppointmentMessage
//   ADoctor.Text = "..."                       → CurrentAppointmentViewModel.DoctorMessage
//   ATimings.Text = "..."                      → CurrentAppointmentViewModel.TimingsMessage
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
    /// Handles the Current Appointment page for patients.
    /// Replaces CurrentAppointment.aspx + CurrentAppointment.aspx.cs (Web Forms).
    /// </summary>
    public class CurrentAppointmentController : Controller
    {
        // -----------------------------------------------------------------------
        // GET /CurrentAppointment
        // Replaces Page_Load → appointmentToday() in CurrentAppointment.aspx.cs
        // -----------------------------------------------------------------------
        [HttpGet]
        public IActionResult Index()
        {
            var vm = new CurrentAppointmentViewModel();
            LoadCurrentAppointment(vm);
            return View(vm);
        }

        // -----------------------------------------------------------------------
        // Helper: loads today's appointment from DAL into the view model.
        // Replaces appointmentToday() method in CurrentAppointment.aspx.cs.
        // -----------------------------------------------------------------------
        private void LoadCurrentAppointment(CurrentAppointmentViewModel vm)
        {
            var objmyDAl = new myDAL();

            // Replaces: int pid = (int)Session["idoriginal"];
            int pid = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            string dName = "";
            string timings = "";

            // Replaces: int status = objmyDAl.appointmentTodayDisplayer(pid, ref dName, ref timings);
            int status = objmyDAl.appointmentTodayDisplayer(pid, ref dName, ref timings);

            if (status == -1)
            {
                // Replaces: Appointment.Text = "There was some error in retrieving the Patient's appointment.";
                vm.AppointmentMessage = "There was some error in retrieving the Patient's appointment.";
            }
            else if (status == 0)
            {
                // Replaces: Appointment.Text = "You have no appointment today with any doctor.";
                vm.AppointmentMessage = "You have no appointment today with any doctor.";
            }
            else
            {
                if (status == 3)
                {
                    // Replaces: ADoctor.Text = "You had an outdated appointment with Doctor " + dName + " to which he didn't respond. So that appointment is discarded.";
                    vm.DoctorMessage = "You had an outdated appointment with Doctor " + dName + " to which he didn't respond. So that appointment is discarded.";
                    // Replaces: ATimings.Text = "The Appointment Timings were : " + timings;
                    vm.TimingsMessage = "The Appointment Timings were : " + timings;
                }
                else if (status == 2)
                {
                    // Replaces: ADoctor.Text = "You have sent an appointment request to Doctor " + dName + " which isn't approved by him yet.";
                    vm.DoctorMessage = "You have sent an appointment request to Doctor " + dName + " which isn't approved by him yet.";
                    // Replaces: ATimings.Text = "The Appointment Timings are : " + timings;
                    vm.TimingsMessage = "The Appointment Timings are : " + timings;
                }
                else
                {
                    // Replaces: ADoctor.Text = "Today you have an appointment with Doctor " + dName;
                    vm.DoctorMessage = "Today you have an appointment with Doctor " + dName;
                    // Replaces: ATimings.Text = "The Appointment Timings are : " + timings;
                    vm.TimingsMessage = "The Appointment Timings are : " + timings;
                }
            }
        }
    }
}
