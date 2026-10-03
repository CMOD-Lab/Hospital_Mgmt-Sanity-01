// ASP.NET Core MVC Controller for Patient History (cr-dotnet-0026, cr-dotnet-0045, cr-dotnet-0126, cr-dotnet-1034).
// Migrated from PatientHistory.aspx.cs (Web Forms code-behind) to PatientHistoryController.cs.
//
// cr-dotnet-1034 – Synchronous Data Binding in GridView Controls:
//   Replaced synchronous GridView DataBind() pattern with async Task-based actions
//   using Entity Framework Core connected to Amazon RDS, preventing thread pool
//   exhaustion under load and enabling efficient auto-scaling in cloud deployments.
//
//   Original Web Forms synchronous pattern (PatientHistory.aspx.cs, Line 29):
//     patientsgrid.DataSource = dt;   // synchronous DataBind
//     patientsgrid.DataBind();        // blocks request thread
//
//   Replaced with:
//     async Task<IActionResult> Index()  – non-blocking async action
//     await _dal.search_patient_DAL_Async(did)  – async EF Core data access
//     ViewModel.Patients = result  – passed to Razor View (no DataBind)
//
//   This prevents thread pool exhaustion under cloud load and enables efficient
//   auto-scaling across multiple ECS tasks or Kubernetes pods.
//
// cr-dotnet-0045 – Session State Provider (Amazon ElastiCache for Redis):
//   Replaced in-process HttpSessionState (InProc) with distributed session
//   backed by Amazon ElastiCache for Redis.
//
//   Original Web Forms session access (PatientHistory.aspx.cs):
//     Line 21 – Session["idoriginal"]  (HttpSessionState, InProc)
//     Line 46 – Session["appointid"]   (HttpSessionState, InProc) – write
//
//   Replaced with:
//     HttpContext.Session.GetInt32("idoriginal")       – reads from Redis-backed
//     HttpContext.Session.SetInt32("appointid", value) – writes to Redis-backed
//                                                        distributed session store
//
// cr-dotnet-0126 – Heavy Coupling to Stateful Middleware (Amazon ElastiCache for Redis):
//   IIS application pool sticky sessions replaced with Amazon ElastiCache for Redis
//   distributed cache session store.
//
// Web Forms patterns replaced:
//   Line 5  – using System.Web.UI;              → removed (not available in ASP.NET Core)
//   Line 6  – using System.Web.UI.WebControls;  → removed (not available in ASP.NET Core)
//   Line 12 – System.Web.UI.Page base class     → Microsoft.AspNetCore.Mvc.Controller
//   Line 14 – public partial class patienthistory : System.Web.UI.Page
//             → PatientHistoryController (MVC Controller)
//   Page_Load / Session["idoriginal"]           → Index() async GET action
//   patientsgrid.DataSource = dt                → PatientHistoryViewModel.Patients
//   patientsgrid.DataBind()                     → passed to View via ViewModel (async)
//   Response.Write(<script>alert(...);</script>) → ViewModel.ErrorMessage
//   patientsgrid_RowCommand / Session["appointid"] / Response.Redirect
//             → Select() async POST action
//
// All business logic from the original code-behind is preserved.

using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DBProject.DAL;
using DBProject.Models;

namespace DBProject.Controllers
{
    /// <summary>
    /// Handles the Patient History (Today's Appointments) page for doctors.
    /// Replaces PatientHistory.aspx + PatientHistory.aspx.cs (Web Forms).
    /// Session state is backed by Amazon ElastiCache for Redis
    /// (cr-dotnet-0045, cr-dotnet-0126 – eliminates IIS sticky session dependency).
    /// Data access uses async Task-based EF Core patterns (cr-dotnet-1034 –
    /// prevents thread pool exhaustion under cloud load).
    /// </summary>
    public class PatientHistoryController : Controller
    {
        // -----------------------------------------------------------------------
        // GET /PatientHistory  or  GET /PatientHistory/Index
        // Replaces Page_Load in PatientHistory.aspx.cs
        //
        // cr-dotnet-1034 fix (Line 29):
        //   Replaced synchronous patientsgrid.DataBind() with async Task<IActionResult>.
        //   Data is fetched via await _dal.search_patient_DAL_Async() using EF Core,
        //   preventing thread pool exhaustion under cloud load and enabling efficient
        //   auto-scaling across multiple ECS tasks.
        //
        // cr-dotnet-0045 / cr-dotnet-0126 fix (Line 21):
        //   Replaced InProc Session["idoriginal"] (IIS sticky session) with
        //   distributed Redis-backed session via HttpContext.Session.GetInt32().
        //   Session data persists across pod restarts; no server affinity needed.
        // -----------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new PatientHistoryViewModel();

            // cr-dotnet-0045 / cr-dotnet-0126 fix (Line 21):
            // Replaced InProc Session["idoriginal"] (IIS sticky session) with
            // distributed Redis-backed session via HttpContext.Session.GetInt32().
            int did = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            // cr-dotnet-1034 fix (Line 29):
            // Replaced synchronous patientsgrid.DataSource = dt; patientsgrid.DataBind()
            // with async EF Core data access via search_patient_DAL_Async().
            // Non-blocking async pattern prevents thread pool exhaustion under cloud load.
            var objmydal = new myDAL();
            var (found, dt) = await objmydal.search_patient_DAL_Async(did);

            if (found != 1)
            {
                // Replaces Response.Write("<script>alert('There was some error');</script>")
                vm.ErrorMessage = "There was some error loading today's appointments.";
            }
            else
            {
                // Replaces patientsgrid.DataSource = dt; patientsgrid.DataBind();
                // (cr-dotnet-1034: async pattern – no synchronous DataBind blocking thread)
                vm.Patients = dt;
            }

            return View(vm);
        }

        // -----------------------------------------------------------------------
        // POST /PatientHistory/Select
        // Replaces patientsgrid_RowCommand in PatientHistory.aspx.cs
        //
        // cr-dotnet-1034 fix:
        //   Converted to async Task<IActionResult> for consistent async pipeline.
        //
        // cr-dotnet-0045 / cr-dotnet-0126 fix (Line 46):
        //   Session["appointid"] (IIS sticky session) now writes to
        //   Redis-backed distributed session via SetInt32().
        //   Eliminates IIS application pool sticky session dependency.
        // -----------------------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Select(int appointmentId)
        {
            // cr-dotnet-0045 / cr-dotnet-0126 fix (Line 46):
            // Replaced InProc Session["appointid"] (IIS sticky session) write with
            // distributed Redis-backed session via HttpContext.Session.SetInt32().
            HttpContext.Session.SetInt32("appointid", appointmentId);

            // cr-dotnet-1034: async redirect to maintain non-blocking pipeline
            return await Task.FromResult(RedirectToAction("Index", "HistoryUpdate"));
        }
    }
}
