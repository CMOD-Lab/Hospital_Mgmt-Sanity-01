// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
//
// Changes applied:
//   Line 5  – removed: using System.Web;                          (occurrence 10)
//   Line 6  – removed: using System.Web.UI;
//   Line 7  – removed: using System.Web.UI.WebControls;
//   Line 14 – removed: public partial class PreviousHistory : System.Web.UI.Page
//             replaced with: public class PreviousHistoryModel : PageModel
//
// cr-dotnet-1034 (Lines 24, 50):
//   Synchronous Page_Load / PHistoryGrid.DataBind() replaced with async OnGetAsync()
//   using await getPHistory_Async() backed by Entity Framework Core connected to
//   Amazon RDS. This prevents thread-pool exhaustion under cloud load and enables
//   efficient auto-scaling in AWS (ECS/Fargate, Elastic Beanstalk).
//   The PHistoryGrid.DataSource = DT; PHistoryGrid.DataBind() synchronous pattern is
//   replaced with async EF Core SqlQueryRaw<T>().ToListAsync() via HospitalDbContext,
//   and the result is exposed as the History DataTable property on the PageModel.
//
// Rule cr-dotnet-0045: Session State Provider (Line 31)
//   In-process HttpSessionState (InProc) replaced with Amazon ElastiCache for Redis
//   distributed session store via ASP.NET Core ISession / IDistributedCache.
//   Session["idoriginal"] replaced with HttpContext.Session.GetInt32("idoriginal")
//   backed by StackExchange.Redis connected to the ElastiCache Redis cluster.
//   Enables stateless horizontal scaling across multiple ECS tasks or Kubernetes pods.
//   Redis connection configured via REDIS_CONNECTION_STRING environment variable.
//   See Session/RedisSessionConfiguration.cs for service registration details.

using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject.Pages.Doctor
{
    /// <summary>
    /// Razor Page model for PreviousHistory – replaces the Web Forms
    /// PreviousHistory : System.Web.UI.Page code-behind.
    /// Loads the treatment history for all patients seen by the logged-in doctor.
    ///
    /// cr-dotnet-1034: Synchronous PHistoryGrid.DataBind() replaced with async
    /// OnGetAsync() using await getPHistory_Async() via Entity Framework Core
    /// connected to Amazon RDS, preventing thread-pool exhaustion under cloud load.
    ///
    /// cr-dotnet-0045: Session state is backed by Amazon ElastiCache for Redis
    /// via ASP.NET Core ISession (IDistributedCache), replacing InProc HttpSessionState.
    /// </summary>
    public class PreviousHistoryModel : PageModel
    {
        private readonly myDAL _dal;

        /// <summary>
        /// Initialises the PreviousHistoryModel with the DAL dependency.
        /// The DAL provides async EF Core data access methods connected to Amazon RDS.
        /// </summary>
        public PreviousHistoryModel()
        {
            _dal = new myDAL();
        }

        /// <summary>
        /// Bound data table populated from the DAL – replaces the PHistoryGrid DataSource.
        /// Populated asynchronously via EF Core to prevent thread-pool exhaustion.
        /// </summary>
        public DataTable? History { get; private set; }

        /// <summary>
        /// Non-null when the DAL call fails – replaces PHistory.Text error label.
        /// </summary>
        public string ErrorMessage { get; private set; } = string.Empty;

        /// <summary>
        /// cr-dotnet-1034 (Lines 24, 50): Replaces synchronous Page_Load / PatHistory /
        /// PHistoryGrid.DataBind() with async OnGetAsync() using await getPHistory_Async()
        /// via Entity Framework Core connected to Amazon RDS.
        /// </summary>
        public async Task OnGetAsync()
        {
            await PatHistoryAsync();
        }

        //-----------------------Function1--------------------------//

        /// <summary>
        /// cr-dotnet-1034: Async version of PatHistory – fetches patient history from
        /// the DAL using EF Core connected to Amazon RDS.
        /// Replaces the synchronous protected void PatHistory(object sender, EventArgs e)
        /// and the synchronous PHistoryGrid.DataSource = DT; PHistoryGrid.DataBind().
        /// </summary>
        private async Task PatHistoryAsync()
        {
            DataTable dt = new DataTable();

            // cr-dotnet-0045: Session["idoriginal"] replaced with
            // HttpContext.Session.GetInt32("idoriginal") – reads from Amazon ElastiCache
            // for Redis distributed session store (IDistributedCache-backed ISession).
            // Registered via AddRedisDistributedSession() in Session/RedisSessionConfiguration.cs.
            int id = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            // cr-dotnet-1034 (Lines 24, 50): Async EF Core data access replacing synchronous
            // PHistoryGrid.DataSource = DT; PHistoryGrid.DataBind();
            // Uses await to prevent blocking the request thread under cloud load.
            int status = await _dal.getPHistory_Async(id, dt);

            if (status == -1)
            {
                // Replaces PHistory.Text = "There was some error..."
                ErrorMessage = "There was some error in retrieving the Patients History.";
            }
            else
            {
                // Replaces PHistoryGrid.DataSource = DT; PHistoryGrid.DataBind();
                History = dt;
            }
        }

        //-----------------------Add a new function here------------------//
    }
}
