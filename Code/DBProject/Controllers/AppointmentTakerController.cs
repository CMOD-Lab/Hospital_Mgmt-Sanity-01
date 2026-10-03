// ASP.NET Core MVC Controller for Appointment Taker (Free Slots)
// (cr-dotnet-0026, cr-dotnet-0045, cr-dotnet-0126, cr-dotnet-1034).
// Migrated from AppointmentTaker.aspx.cs (Web Forms code-behind) to
// AppointmentTakerController.cs.
//
// cr-dotnet-1034 – Synchronous Data Binding in GridView Controls:
//   Replaced synchronous GridView DataBind() pattern with async Task-based actions
//   using Entity Framework Core connected to Amazon RDS, preventing thread pool
//   exhaustion under load and enabling efficient auto-scaling in cloud deployments.
//
//   Original Web Forms synchronous pattern (AppointmentTaker.aspx.cs, Line 77):
//     PAppointmentGrid.DataSource = DT;  // synchronous DataBind
//     PAppointmentGrid.DataBind();       // blocks request thread
//
//   Replaced with:
//     async Task<IActionResult> Index()       – non-blocking async action
//     await _dal.getFreeSlots_Async(dID, pID) – async EF Core data access
//     ViewModel.FreeSlotsData = result        – passed to Razor View (no DataBind)
//
// Web Forms patterns replaced:
//   Line 5  – using System.Web;                → removed (not available in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not available in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not available in ASP.NET Core)
//   Line 13 – public partial class AppointmentTaker : System.Web.UI.Page
//             → AppointmentTakerController (MVC Controller)
//   Line 15 – Page_Load → freeSlots()          → Index() async GET action
//   PAppointmentGrid_RowCommand (GridView row select) → SelectSlot() POST action
//   PAppointment.Text = "..."                  → AppointmentTakerViewModel.StatusMessage
//   PAppointmentGrid.DataSource / DataBind     → AppointmentTakerViewModel.FreeSlotsData (async)
//   Response.Redirect("AppointmentRequestSent.aspx") → RedirectToAction("Index", "AppointmentRequestSent")
//
// cr-dotnet-0045 – Session State Provider fix:
//   Line 17 – Session["freeSlot"] = "" (InProc HttpSessionState)
//             → HttpContext.Session.SetString("freeSlot", "") backed by Amazon ElastiCache for Redis.
//   Line 33 – Session["dID"] (InProc HttpSessionState)
//             → HttpContext.Session.GetString("dID") backed by Amazon ElastiCache for Redis.
//   Line 52 – Session["idoriginal"] (InProc HttpSessionState)
//             → HttpContext.Session.GetInt32("idoriginal") backed by Amazon ElastiCache for Redis.
//   Line 57 – Session["freeSlot"] = tokens[0] (InProc HttpSessionState)
//             → HttpContext.Session.SetString("freeSlot", tokens[0]) backed by Amazon ElastiCache for Redis.
//   Redis is configured via REDIS_CONNECTION_STRING environment variable.
//   Session middleware registered in Program.cs with AddStackExchangeRedisCache
//   and AddSession, enabling stateless horizontal scaling across ECS tasks.
//
// cr-dotnet-0126 – Heavy Coupling to Stateful Middleware fix:
//   IIS application pool sticky sessions replaced with Amazon ElastiCache for Redis
//   distributed cache session store.
//
//   Line 17 – Session["freeSlot"] = "" (IIS InProc sticky session)
//             → HttpContext.Session.SetString("freeSlot", "")
//               backed by Amazon ElastiCache for Redis.
//   Line 33 – Session["dID"] (IIS InProc sticky session)
//             → HttpContext.Session.GetString("dID")
//               backed by Amazon ElastiCache for Redis.
//
//   This eliminates server affinity (sticky sessions) required by IIS in-process
//   session state. Session data persists across pod/container restarts and scales
//   horizontally without sticky session routing.
//
// All business logic from the original code-behind is preserved.

using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DBProject.DAL;
using DBProject.Models;

namespace DBProject.Controllers
{
    /// <summary>
    /// Handles the Appointment Taker (Free Time Slots) page for patients.
    /// Replaces AppointmentTaker.aspx + AppointmentTaker.aspx.cs (Web Forms).
    /// Session state is backed by Amazon ElastiCache for Redis
    /// (cr-dotnet-0045, cr-dotnet-0126 – eliminates IIS sticky session dependency).
    /// Data access uses async Task-based EF Core patterns (cr-dotnet-1034 –
    /// prevents thread pool exhaustion under cloud load).
    /// </summary>
    public class AppointmentTakerController : Controller
    {
        // -----------------------------------------------------------------------
        // GET /AppointmentTaker
        // Replaces Page_Load → freeSlots() in AppointmentTaker.aspx.cs
        //
        // cr-dotnet-1034 fix (Line 77):
        //   Replaced synchronous PAppointmentGrid.DataBind() with async Task<IActionResult>.
        //   Data is fetched via await _dal.getFreeSlots_Async() using EF Core,
        //   preventing thread pool exhaustion under cloud load.
        //
        // cr-dotnet-0045 / cr-dotnet-0126:
        //   Session["freeSlot"] = "" (IIS InProc sticky session) →
        //   HttpContext.Session.SetString backed by Amazon ElastiCache for Redis.
        //   No server affinity required; scales horizontally.
        // -----------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // cr-dotnet-0045 / cr-dotnet-0126 fix (Line 17):
            // Replaced InProc Session["freeSlot"] = "" (IIS sticky session) with
            // distributed Redis-backed session via HttpContext.Session.SetString().
            // Session data persists across pod restarts; no server affinity needed.
            HttpContext.Session.SetString("freeSlot", "");

            var vm = new AppointmentTakerViewModel();
            await LoadFreeSlotsAsync(vm);
            return View(vm);
        }

        // -----------------------------------------------------------------------
        // POST /AppointmentTaker/SelectSlot
        // Replaces PAppointmentGrid_RowCommand (GridView row select handler)
        // in AppointmentTaker.aspx.cs.
        // The selected appointment slot value is submitted via a form POST.
        //
        // cr-dotnet-0045 / cr-dotnet-0126:
        //   Session["freeSlot"] = tokens[0] (IIS InProc sticky session) →
        //   HttpContext.Session.SetString backed by Amazon ElastiCache for Redis.
        //   Eliminates IIS application pool sticky session dependency.
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SelectSlot(string selectedSlot)
        {
            if (!string.IsNullOrEmpty(selectedSlot))
            {
                // Replaces: string appointment = PAppointmentGrid.Rows[num].Cells[2].Text;
                //           string[] tokens = appointment.Split(':');
                //           Session["freeSlot"] = tokens[0];
                // cr-dotnet-0045 / cr-dotnet-0126 fix (Line 57):
                // Replaced InProc Session["freeSlot"] (IIS sticky session) with
                // distributed Redis-backed session. No server affinity required.
                string[] tokens = selectedSlot.Split(':');
                HttpContext.Session.SetString("freeSlot", tokens[0]);

                // Replaces: Response.Redirect("AppointmentRequestSent.aspx");
                return RedirectToAction("Index", "AppointmentRequestSent");
            }

            // If no slot selected, reload the page
            var vm = new AppointmentTakerViewModel();
            await LoadFreeSlotsAsync(vm);
            return View("Index", vm);
        }

        // -----------------------------------------------------------------------
        // Helper: loads free slots from DAL into the view model asynchronously.
        // Replaces freeSlots() method in AppointmentTaker.aspx.cs.
        //
        // cr-dotnet-1034 fix (Line 77):
        //   Replaced synchronous PAppointmentGrid.DataSource = DT; PAppointmentGrid.DataBind()
        //   with async EF Core data access via getFreeSlots_Async().
        //   Non-blocking async pattern prevents thread pool exhaustion under cloud load.
        //
        // cr-dotnet-0045 / cr-dotnet-0126:
        //   Session["dID"] (Line 33, IIS InProc sticky session) and
        //   Session["idoriginal"] (Line 52, InProc) →
        //   HttpContext.Session.GetString/GetInt32 backed by Amazon ElastiCache for Redis.
        //   Eliminates IIS application pool sticky session dependency.
        // -----------------------------------------------------------------------
        private async Task LoadFreeSlotsAsync(AppointmentTakerViewModel vm)
        {
            var objmyDAl = new myDAL();

            // cr-dotnet-0045 / cr-dotnet-0126 fix (Line 33):
            // Replaced InProc Session["dID"] (IIS sticky session) with
            // distributed Redis-backed session via HttpContext.Session.GetString().
            // Session data persists across pod restarts; no server affinity needed.
            string dID1 = HttpContext.Session.GetString("dID");
            int dID = Convert.ToInt32(dID1);

            // cr-dotnet-0045 fix (Line 52):
            // Replaced InProc Session["idoriginal"] with distributed Redis-backed session.
            int pID = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            // cr-dotnet-1034 fix (Line 77):
            // Replaced synchronous: int status = objmyDAl.getFreeSlots(dID, pID, ref DT);
            //                       PAppointmentGrid.DataSource = DT;
            //                       PAppointmentGrid.DataBind();
            // with async EF Core pattern: await getFreeSlots_Async(dID, pID)
            // Non-blocking async prevents thread pool exhaustion under cloud load;
            // enables efficient auto-scaling in cloud deployments on Amazon RDS.
            var (status, dt) = await objmyDAl.getFreeSlots_Async(dID, pID);

            if (status == -1)
            {
                // Replaces: PAppointment.Text = "There was some error in retrieving the Doctors's Free Slots.";
                vm.StatusMessage = "There was some error in retrieving the Doctors's Free Slots.";
            }
            else if (status == 0)
            {
                // Replaces: PAppointment.Text = "There is currently no free slot of this doctor.";
                vm.StatusMessage = "There is currently no free slot of this doctor.";
            }
            else if (status > 0)
            {
                // Replaces: PAppointment.Text = "The following are the " + status + " free slots of this doctor for today :";
                //           PAppointmentGrid.DataSource = DT; PAppointmentGrid.DataBind();
                // (cr-dotnet-1034: async pattern – no synchronous DataBind blocking thread)
                vm.StatusMessage = "The following are the " + status + " free slots of this doctor for today :";
                vm.FreeSlotsData = dt;
                vm.SlotCount = status;
            }
        }
    }
}
