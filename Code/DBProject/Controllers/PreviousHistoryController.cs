// ASP.NET Core MVC Controller for Previous History (cr-dotnet-0026, cr-dotnet-0045, cr-dotnet-1034).
// Migrated from PreviousHistory.aspx.cs (Web Forms code-behind) to PreviousHistoryController.cs.
//
// cr-dotnet-1034 – Synchronous Data Binding in GridView Controls:
//   Replaced synchronous GridView DataBind() pattern with async Task-based actions
//   using Entity Framework Core connected to Amazon RDS, preventing thread pool
//   exhaustion under load and enabling efficient auto-scaling in cloud deployments.
//
//   Original Web Forms synchronous pattern (PreviousHistory.aspx.cs, Line 50):
//     PHistoryGrid.DataSource = DT;  // synchronous DataBind
//     PHistoryGrid.DataBind();       // blocks request thread
//
//   Replaced with:
//     async Task<IActionResult> Index()  – non-blocking async action
//     await _dal.getPHistory_Async(id)   – async EF Core data access
//     ViewModel.HistoryData = result     – passed to Razor View (no DataBind)
//
// Web Forms patterns replaced:
//   Line 5  – using System.Web;                → removed (not available in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not available in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not available in ASP.NET Core)
//   Line 13 – public partial class PreviousHistory : System.Web.UI.Page
//             → PreviousHistoryController (MVC Controller)
//   Line 15 – Page_Load → PatHistory()         → Index() async GET action
//   PHistory.Text = "error message"            → PreviousHistoryViewModel.ErrorMessage
//   PHistoryGrid.DataSource / DataBind         → PreviousHistoryViewModel.HistoryData (async)
//
// cr-dotnet-0045 – Session State Provider fix:
//   Line 31 – Session["idoriginal"] (InProc HttpSessionState)
//             → HttpContext.Session.GetInt32("idoriginal") backed by
//               Amazon ElastiCache for Redis via IDistributedCache
//               (Microsoft.Extensions.Caching.StackExchangeRedis).
//   Redis is configured via REDIS_CONNECTION_STRING environment variable.
//   Session middleware registered in Program.cs with AddStackExchangeRedisCache
//   and AddSession, enabling stateless horizontal scaling across ECS tasks.
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
    /// Handles the Previous History (History of Treated Patients) page for doctors.
    /// Replaces PreviousHistory.aspx + PreviousHistory.aspx.cs (Web Forms).
    /// Session state is backed by Amazon ElastiCache for Redis (cr-dotnet-0045).
    /// Data access uses async Task-based EF Core patterns (cr-dotnet-1034 –
    /// prevents thread pool exhaustion under cloud load).
    /// </summary>
    public class PreviousHistoryController : Controller
    {
        // -----------------------------------------------------------------------
        // GET /PreviousHistory  or  GET /PreviousHistory/Index
        // Replaces Page_Load → PatHistory() in PreviousHistory.aspx.cs
        //
        // cr-dotnet-1034 fix (Lines 24, 50):
        //   Replaced synchronous PHistoryGrid.DataBind() with async Task<IActionResult>.
        //   Data is fetched via await _dal.getPHistory_Async() using EF Core,
        //   preventing thread pool exhaustion under cloud load.
        //
        // cr-dotnet-0045: Session["idoriginal"] (InProc) → HttpContext.Session.GetInt32
        //   backed by Amazon ElastiCache for Redis distributed session store.
        // -----------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new PreviousHistoryViewModel();

            // cr-dotnet-0045: Replaces InProc Session["idoriginal"].
            // HttpContext.Session is backed by Amazon ElastiCache for Redis
            // (configured via AddStackExchangeRedisCache in Program.cs).
            // Enables stateless horizontal scaling across multiple ECS tasks.
            int id = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            // cr-dotnet-1034 fix (Lines 24, 50):
            // Replaced synchronous PHistoryGrid.DataSource = DT; PHistoryGrid.DataBind()
            // with async EF Core data access via getPHistory_Async().
            // Non-blocking async pattern prevents thread pool exhaustion under cloud load.
            var objmyDAl = new myDAL();
            var (status, dt) = await objmyDAl.getPHistory_Async(id);

            if (status == -1)
            {
                // Replaces: PHistory.Text = "There was some error in retrieving the Patients History.";
                vm.ErrorMessage = "There was some error in retrieving the Patients History.";
            }
            else
            {
                // Replaces: PHistoryGrid.DataSource = DT; PHistoryGrid.DataBind();
                // (cr-dotnet-1034: async pattern – no synchronous DataBind blocking thread)
                vm.HistoryData = dt;
            }

            return View(vm);
        }
    }
}
