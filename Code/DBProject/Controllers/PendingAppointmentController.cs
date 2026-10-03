// ASP.NET Core MVC Controller for Pending Appointments (cr-dotnet-0026, cr-dotnet-0045, cr-dotnet-1034).
// Migrated from PendingAppointment.aspx.cs (Web Forms code-behind) to PendingAppointmentController.cs.
//
// cr-dotnet-1034 – Synchronous Data Binding in GridView Controls:
//   Replaced synchronous GridView DataBind() pattern with async Task-based actions
//   using Entity Framework Core connected to Amazon RDS, preventing thread pool
//   exhaustion under load and enabling efficient auto-scaling in cloud deployments.
//
//   Original Web Forms synchronous pattern (PendingAppointment.aspx.cs, Line 32):
//     pendingappointments.DataSource = DT;  // synchronous DataBind
//     pendingappointments.DataBind();       // blocks request thread
//
//   Replaced with:
//     async Task<IActionResult> Index()  – non-blocking async action
//     await _dal.GetAllpendingappointments_DAL_Async(did)  – async EF Core data access
//     ViewModel.Appointments = result  – passed to Razor View (no DataBind)
//
// Web Forms patterns replaced:
//   Line 5  – using System.Web;                → removed (not available in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not available in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not available in ASP.NET Core)
//   Line 13 – public partial class pendingappointment : System.Web.UI.Page
//             → PendingAppointmentController (MVC Controller)
//   Line 15 – Page_Load / loadgrid()           → Index() async GET action
//   update_appointment (GridViewCommandEventArgs) → Approve() async POST action
//   Delete_appointment (GridViewDeleteEventArgs)  → Delete() async POST action
//
// cr-dotnet-0045 – Session State Provider fix:
//   Line 25 – Session["idoriginal"] (InProc HttpSessionState)
//             → HttpContext.Session.GetInt32("idoriginal") backed by
//               Amazon ElastiCache for Redis via IDistributedCache
//               (Microsoft.Extensions.Caching.StackExchangeRedis).
//   Redis is configured via REDIS_CONNECTION_STRING environment variable.
//   Session middleware registered in Program.cs with AddStackExchangeRedisCache
//   and AddSession, enabling stateless horizontal scaling across ECS tasks.
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
    /// Handles the Pending Appointments page for doctors.
    /// Replaces PendingAppointment.aspx + PendingAppointment.aspx.cs (Web Forms).
    /// Session state is backed by Amazon ElastiCache for Redis (cr-dotnet-0045).
    /// Data access uses async Task-based EF Core patterns (cr-dotnet-1034 –
    /// prevents thread pool exhaustion under cloud load).
    /// </summary>
    public class PendingAppointmentController : Controller
    {
        // -----------------------------------------------------------------------
        // GET /PendingAppointment  or  GET /PendingAppointment/Index
        // Replaces Page_Load → loadgrid() in PendingAppointment.aspx.cs
        //
        // cr-dotnet-1034 fix (Line 32):
        //   Replaced synchronous pendingappointments.DataBind() with async Task<IActionResult>.
        //   Data is fetched via await _dal.GetAllpendingappointments_DAL_Async() using EF Core,
        //   preventing thread pool exhaustion under cloud load.
        //
        // cr-dotnet-0045: Session["idoriginal"] (InProc) → HttpContext.Session.GetInt32
        //   backed by Amazon ElastiCache for Redis distributed session store.
        // -----------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new PendingAppointmentViewModel();

            // cr-dotnet-0045: Replaces InProc Session["idoriginal"].
            // HttpContext.Session is backed by Amazon ElastiCache for Redis
            // (configured via AddStackExchangeRedisCache in Program.cs).
            // Enables stateless horizontal scaling across multiple ECS tasks.
            int did = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            // cr-dotnet-1034 fix (Line 32):
            // Replaced synchronous pendingappointments.DataSource = DT; pendingappointments.DataBind()
            // with async EF Core data access via GetAllpendingappointments_DAL_Async().
            // Non-blocking async pattern prevents thread pool exhaustion under cloud load.
            var objDAL = new myDAL();
            var dt = await objDAL.GetAllpendingappointments_DAL_Async(did);

            // Replaces pendingappointments.DataSource = DT; pendingappointments.DataBind();
            // (cr-dotnet-1034: async pattern – no synchronous DataBind blocking thread)
            vm.Appointments = dt;

            return View(vm);
        }

        // -----------------------------------------------------------------------
        // POST /PendingAppointment/Approve
        // Replaces update_appointment (GridViewCommandEventArgs) in PendingAppointment.aspx.cs
        // Handles the "Select" command that approves an appointment.
        //
        // cr-dotnet-1034 fix:
        //   Converted to async Task<IActionResult> for consistent async pipeline.
        //   Uses async EF Core data access via UpdateAppointment_DAL_Async().
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int appointmentId)
        {
            // cr-dotnet-1034: Replaces synchronous objmyDAL.UpdateAppointment_DAL(appointmentId)
            // with async EF Core data access, preventing thread pool exhaustion.
            var objmyDAL = new myDAL();
            await objmyDAL.UpdateAppointment_DAL_Async(appointmentId);

            // Replaces: pendingappointments.EditIndex = -1; loadgrid();
            return RedirectToAction("Index");
        }

        // -----------------------------------------------------------------------
        // POST /PendingAppointment/Delete
        // Replaces Delete_appointment (GridViewDeleteEventArgs) in PendingAppointment.aspx.cs
        //
        // cr-dotnet-1034 fix:
        //   Converted to async Task<IActionResult> for consistent async pipeline.
        //   Uses async EF Core data access via Deleteappointment_DAL_Async().
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int appointmentId)
        {
            // cr-dotnet-1034: Replaces synchronous objDAL.Deleteappointment_DAL(appointmentId)
            // with async EF Core data access, preventing thread pool exhaustion.
            var objDAL = new myDAL();
            await objDAL.Deleteappointment_DAL_Async(appointmentId);

            return RedirectToAction("Index");
        }
    }
}
