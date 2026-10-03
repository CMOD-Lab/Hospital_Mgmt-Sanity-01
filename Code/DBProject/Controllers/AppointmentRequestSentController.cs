// ASP.NET Core MVC Controller for Appointment Request Sent (cr-dotnet-0026, cr-dotnet-0045).
// Migrated from AppointmentRequestSent.aspx.cs (Web Forms code-behind) to
// AppointmentRequestSentController.cs.
//
// Web Forms patterns replaced:
//   Line 5  – using System.Web;                → removed (not available in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not available in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not available in ASP.NET Core)
//   Line 14 – public partial class AppointmentNotificationSent : System.Web.UI.Page
//             → AppointmentRequestSentController (MVC Controller)
//   Line 16 – Page_Load                        → Index() GET action
//   sendARequest (Button OnClick handler)      → SendRequest() POST action
//   Message.Text = "..."                       → AppointmentRequestSentViewModel.Message
//
// cr-dotnet-0045 – Session State Provider fix:
//   Line 28 – Session["dID"] (InProc HttpSessionState)
//             → HttpContext.Session.GetString("dID") backed by Amazon ElastiCache for Redis.
//   Line 33 – Session["idoriginal"] (InProc HttpSessionState)
//             → HttpContext.Session.GetInt32("idoriginal") backed by Amazon ElastiCache for Redis.
//   Line 36 – Session["freeSlot"] (InProc HttpSessionState)
//             → HttpContext.Session.GetString("freeSlot") backed by Amazon ElastiCache for Redis.
//   Redis is configured via REDIS_CONNECTION_STRING environment variable.
//   Session middleware registered in Program.cs with AddStackExchangeRedisCache
//   and AddSession, enabling stateless horizontal scaling across ECS tasks.
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
    /// Handles the Appointment Request Sent page for patients.
    /// Replaces AppointmentRequestSent.aspx + AppointmentRequestSent.aspx.cs (Web Forms).
    /// Session state is backed by Amazon ElastiCache for Redis (cr-dotnet-0045).
    /// </summary>
    public class AppointmentRequestSentController : Controller
    {
        // -----------------------------------------------------------------------
        // GET /AppointmentRequestSent
        // Replaces Page_Load in AppointmentRequestSent.aspx.cs
        // -----------------------------------------------------------------------
        [HttpGet]
        public IActionResult Index()
        {
            var vm = new AppointmentRequestSentViewModel();
            return View(vm);
        }

        // -----------------------------------------------------------------------
        // POST /AppointmentRequestSent/SendRequest
        // Replaces sendARequest (Button OnClick handler) in AppointmentRequestSent.aspx.cs
        //
        // cr-dotnet-0045: All Session["..."] (InProc) accesses replaced with
        //   HttpContext.Session.GetString/GetInt32 backed by Amazon ElastiCache for Redis.
        //   Configured via AddStackExchangeRedisCache in Program.cs using
        //   REDIS_CONNECTION_STRING environment variable.
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendRequest()
        {
            var vm = new AppointmentRequestSentViewModel { RequestSubmitted = true };

            var objmyDAl = new myDAL();

            // cr-dotnet-0045: Replaces InProc Session["dID"].
            // HttpContext.Session is backed by Amazon ElastiCache for Redis.
            string dID1 = HttpContext.Session.GetString("dID");
            int dID = Convert.ToInt32(dID1);

            // cr-dotnet-0045: Replaces InProc Session["idoriginal"].
            // HttpContext.Session is backed by Amazon ElastiCache for Redis.
            int pID = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            // cr-dotnet-0045: Replaces InProc Session["freeSlot"].
            // HttpContext.Session is backed by Amazon ElastiCache for Redis.
            string temp = HttpContext.Session.GetString("freeSlot");
            int freeSlot = Convert.ToInt32(temp);

            string mes = "";

            // Replaces: int status = objmyDAl.insertAppointment(dID, pID, freeSlot, ref mes);
            int status = objmyDAl.insertAppointment(dID, pID, freeSlot, ref mes);

            if (status == -1)
            {
                // Replaces: Message.Text = "There was some error in sending appointment request to the Doctor.";
                vm.Message = "There was some error in sending appointment request to the Doctor.";
            }
            else
            {
                // Replaces: Message.Text = mes;
                vm.Message = mes;
            }

            return View("Index", vm);
        }
    }
}
