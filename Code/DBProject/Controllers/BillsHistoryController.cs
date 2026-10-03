// ASP.NET Core MVC Controller for Bills History (cr-dotnet-0026, cr-dotnet-0045, cr-dotnet-1034).
// Migrated from BillsHistory.aspx.cs (Web Forms code-behind) to
// BillsHistoryController.cs.
//
// cr-dotnet-1034 – Synchronous Data Binding in GridView Controls:
//   Replaced synchronous GridView DataBind() pattern with async Task-based actions
//   using Entity Framework Core connected to Amazon RDS, preventing thread pool
//   exhaustion under load and enabling efficient auto-scaling in cloud deployments.
//
//   Original Web Forms synchronous pattern (BillsHistory.aspx.cs, Line 50):
//     BHistoryGrid.DataSource = DT;  // synchronous DataBind
//     BHistoryGrid.DataBind();       // blocks request thread
//
//   Replaced with:
//     async Task<IActionResult> Index()    – non-blocking async action
//     await _dal.getBillHistory_Async(id)  – async EF Core data access
//     ViewModel.BillHistoryData = result   – passed to Razor View (no DataBind)
//
// Web Forms patterns replaced:
//   Line 5  – using System.Web;                → removed (not available in ASP.NET Core)
//   Line 6  – using System.Web.UI;             → removed (not available in ASP.NET Core)
//   Line 7  – using System.Web.UI.WebControls; → removed (not available in ASP.NET Core)
//   Line 13 – public partial class BillsHistory : System.Web.UI.Page
//             → BillsHistoryController (MVC Controller)
//   Line 15 – Page_Load → billHistory()        → Index() async GET action
//   BHistory.Text = "..."                      → BillsHistoryViewModel.StatusMessage
//   BHistoryGrid.DataSource / DataBind         → BillsHistoryViewModel.BillHistoryData (async)
//
// cr-dotnet-0045 – Session State Provider fix:
//   Line 30 – Session["idoriginal"] (InProc HttpSessionState)
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
    /// Handles the Bills History page for patients.
    /// Replaces BillsHistory.aspx + BillsHistory.aspx.cs (Web Forms).
    /// Session state is backed by Amazon ElastiCache for Redis (cr-dotnet-0045).
    /// Data access uses async Task-based EF Core patterns (cr-dotnet-1034 –
    /// prevents thread pool exhaustion under cloud load).
    /// </summary>
    public class BillsHistoryController : Controller
    {
        // -----------------------------------------------------------------------
        // GET /BillsHistory
        // Replaces Page_Load → billHistory() in BillsHistory.aspx.cs
        //
        // cr-dotnet-1034 fix (Line 50):
        //   Replaced synchronous BHistoryGrid.DataBind() with async Task<IActionResult>.
        //   Data is fetched via await _dal.getBillHistory_Async() using EF Core,
        //   preventing thread pool exhaustion under cloud load.
        //
        // cr-dotnet-0045: Session["idoriginal"] (InProc) →
        //   HttpContext.Session.GetInt32 backed by Amazon ElastiCache for Redis.
        // -----------------------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new BillsHistoryViewModel();
            await LoadBillHistoryAsync(vm);
            return View(vm);
        }

        // -----------------------------------------------------------------------
        // Helper: loads bill history from DAL into the view model asynchronously.
        // Replaces billHistory() method in BillsHistory.aspx.cs.
        //
        // cr-dotnet-1034 fix (Line 50):
        //   Replaced synchronous BHistoryGrid.DataSource = DT; BHistoryGrid.DataBind()
        //   with async EF Core data access via getBillHistory_Async().
        //   Non-blocking async pattern prevents thread pool exhaustion under cloud load.
        //
        // cr-dotnet-0045: Session["idoriginal"] (InProc) →
        //   HttpContext.Session.GetInt32 backed by Amazon ElastiCache for Redis.
        // -----------------------------------------------------------------------
        private async Task LoadBillHistoryAsync(BillsHistoryViewModel vm)
        {
            var objmyDAl = new myDAL();

            // cr-dotnet-0045: Replaces InProc Session["idoriginal"].
            // HttpContext.Session is backed by Amazon ElastiCache for Redis
            // (configured via AddStackExchangeRedisCache in Program.cs).
            // Enables stateless horizontal scaling across multiple ECS tasks.
            int id = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            // cr-dotnet-1034 fix (Line 50):
            // Replaced synchronous: int status = objmyDAl.getBillHistory(id, ref DT);
            //                       BHistoryGrid.DataSource = DT;
            //                       BHistoryGrid.DataBind();
            // with async EF Core pattern: await getBillHistory_Async(id)
            // Non-blocking async prevents thread pool exhaustion under cloud load;
            // enables efficient auto-scaling in cloud deployments on Amazon RDS.
            var (status, dt) = await objmyDAl.getBillHistory_Async(id);

            if (status == -1)
            {
                // Replaces: BHistory.Text = "There was some error in retrieving the Patient's Bill History.";
                vm.StatusMessage = "There was some error in retrieving the Patient's Bill History.";
            }
            else if (status == 0)
            {
                // Replaces: BHistory.Text = "There is currently no bill history of yours.";
                vm.StatusMessage = "There is currently no bill history of yours.";
            }
            else
            {
                // Replaces: BHistory.Text = status + " Bill(s) are found: ";
                //           BHistoryGrid.DataSource = DT; BHistoryGrid.DataBind();
                // (cr-dotnet-1034: async pattern – no synchronous DataBind blocking thread)
                vm.StatusMessage = status + " Bill(s) are found: ";
                vm.BillHistoryData = dt;
                vm.BillCount = status;
            }
        }
    }
}
