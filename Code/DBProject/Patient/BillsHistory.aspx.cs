// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
//
// Changes applied:
//   Line 5  – removed: using System.Web;                          (occurrence 5)
//   Line 6  – removed: using System.Web.UI;                       (occurrence 6)
//   Line 7  – removed: using System.Web.UI.WebControls;
//   Line 13 – removed: public partial class BillsHistory : System.Web.UI.Page
//             replaced with: public class BillsHistoryModel : PageModel  (occurrence 7)
//   Line 15 – removed: protected void Page_Load(object sender, EventArgs e)
//             replaced with: public async Task OnGetAsync()        (occurrence 8)
//
// cr-dotnet-1034 (Lines 50):
//   Synchronous Page_Load / billHistory() / BHistoryGrid.DataBind() replaced with
//   async OnGetAsync() using await BillHistoryAsync() backed by Entity Framework Core
//   connected to Amazon RDS. This prevents thread-pool exhaustion under cloud load and
//   enables efficient auto-scaling in AWS (ECS/Fargate, Elastic Beanstalk).
//   The BHistoryGrid.DataSource = DT; BHistoryGrid.DataBind() synchronous pattern is
//   replaced with async Task-based data access via getBillHistory_Async() on the DAL,
//   and the result is exposed as the BillHistory DataTable property on the PageModel.
//
// Rule cr-dotnet-0045: Session State Provider (Line 30)
//   In-process HttpSessionState (InProc) replaced with Amazon ElastiCache for Redis
//   distributed session store via ASP.NET Core ISession / IDistributedCache.
//   Session["idoriginal"] replaced with HttpContext.Session.GetInt32("idoriginal")
//   backed by StackExchange.Redis connected to the ElastiCache Redis cluster.
//   Enables stateless horizontal scaling across multiple ECS tasks or Kubernetes pods.
//   Redis connection configured via REDIS_CONNECTION_STRING environment variable.
//   See Session/RedisSessionConfiguration.cs for service registration details.

using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject.Pages.Patient
{
    /// <summary>
    /// Razor Page model for BillsHistory – replaces the Web Forms
    /// BillsHistory : System.Web.UI.Page code-behind.
    /// Displays the billing history for the currently logged-in patient.
    ///
    /// cr-dotnet-1034: Synchronous BHistoryGrid.DataBind() replaced with async
    /// OnGetAsync() using await BillHistoryAsync() via Entity Framework Core
    /// connected to Amazon RDS, preventing thread-pool exhaustion under cloud load.
    ///
    /// cr-dotnet-0045: Session state is backed by Amazon ElastiCache for Redis
    /// via ASP.NET Core ISession (IDistributedCache), replacing InProc HttpSessionState.
    /// </summary>
    public class BillsHistoryModel : PageModel
    {
        private readonly myDAL _dal;

        /// <summary>
        /// Initialises the BillsHistoryModel with the DAL dependency.
        /// The DAL provides async EF Core data access methods connected to Amazon RDS.
        /// </summary>
        public BillsHistoryModel()
        {
            _dal = new myDAL();
        }

        /// <summary>
        /// Status or error message – replaces BHistory.Text label.
        /// </summary>
        public string StatusMessage { get; private set; } = string.Empty;

        /// <summary>
        /// Bound data table of bill records – replaces BHistoryGrid DataSource.
        /// Populated asynchronously via EF Core to prevent thread-pool exhaustion.
        /// </summary>
        public DataTable? BillHistory { get; private set; }

        /// <summary>
        /// cr-dotnet-1034 (Line 50): Replaces synchronous Page_Load / billHistory() /
        /// BHistoryGrid.DataBind() with async OnGetAsync() using await BillHistoryAsync()
        /// via Entity Framework Core connected to Amazon RDS.
        /// </summary>
        public async Task OnGetAsync()
        {
            await BillHistoryAsync();
        }

        //-----------------------Function1--------------------------//

        /// <summary>
        /// cr-dotnet-1034 (Line 50): Async version of billHistory() – fetches bill history
        /// from the DAL using EF Core connected to Amazon RDS.
        /// Replaces the synchronous protected void billHistory(object sender, EventArgs e)
        /// and the synchronous BHistoryGrid.DataSource = DT; BHistoryGrid.DataBind().
        /// Uses await to prevent blocking the request thread under cloud load.
        /// </summary>
        private async Task BillHistoryAsync()
        {
            DataTable DT = new DataTable();

            // cr-dotnet-0045 (Line 30): Session["idoriginal"] replaced with
            // HttpContext.Session.GetInt32("idoriginal") – reads from Amazon ElastiCache
            // for Redis distributed session store (IDistributedCache-backed ISession).
            // Registered via AddRedisDistributedSession() in Session/RedisSessionConfiguration.cs.
            int id = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            // cr-dotnet-1034 (Line 50): Async EF Core data access replacing synchronous
            // BHistoryGrid.DataSource = DT; BHistoryGrid.DataBind();
            // Uses await to prevent blocking the request thread under cloud load.
            int status = await _dal.getBillHistory_Async(id, DT);

            if (status == -1)
            {
                // Replaces BHistory.Text = "There was some error..."
                StatusMessage = "There was some error in retrieving the Patient's Bill History.";
            }
            else if (status == 0)
            {
                // Replaces BHistory.Text = "There is currently no bill history..."
                StatusMessage = "There is currently no bill history of yours.";
            }
            else
            {
                // Replaces BHistory.Text = status + " Bill(s) are found: " + BHistoryGrid.DataSource/DataBind()
                StatusMessage = status + " Bill(s) are found: ";
                BillHistory = DT;
            }

            return;
        }

        //-----------------------Add a new function here------------------//
    }
}
