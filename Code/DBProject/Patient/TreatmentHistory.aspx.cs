// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls – replaced
//   synchronous GridView DataBind() with async Task-based OnGetAsync() using
//   async DAL methods connected to Amazon RDS via RDS Proxy, preventing thread-pool
//   exhaustion under cloud load and enabling efficient auto-scaling in AWS.
//
// Changes applied:
//   - removed: using System.Web;
//   - removed: using System.Web.UI;
//   - removed: using System.Web.UI.WebControls;
//   - removed: public partial class TreatmentHistory : System.Web.UI.Page
//             replaced with: public class TreatmentHistoryModel : PageModel
//   - removed: protected void Page_Load(object sender, EventArgs e)
//             replaced with: public async Task OnGetAsync()
//   - removed: THistory.Text = "..." (server control property assignment)
//             replaced with: public string THistoryMessage { get; private set; }
//   - removed: THistoryGrid.DataSource = DT; THistoryGrid.DataBind();
//             (cr-dotnet-1034 Line 51: synchronous GridView DataBind() replaced with
//              async Task-based data access via getTreatmentHistory_Async())
//             replaced with: public DataTable TreatmentHistoryData { get; private set; }
//
// Rule cr-dotnet-0045: Session State Provider
//   In-process (InProc) HttpSessionState replaced with Amazon ElastiCache for Redis
//   distributed session store via ASP.NET Core ISession (IDistributedCache-backed).
//   HttpContext.Session.GetInt32("idoriginal") uses the ASP.NET Core ISession
//   extension methods (Microsoft.AspNetCore.Http) backed by Amazon ElastiCache for Redis.
//   Redis session is registered via AddRedisDistributedSession() in
//   Session/RedisSessionConfiguration.cs, reading REDIS_CONNECTION_STRING from the
//   ECS task definition / Elastic Beanstalk environment / AWS Systems Manager.
//   Replaces (int)Session["idoriginal"] from the original InProc session.
//   Enables stateless horizontal scaling across multiple ECS tasks or Kubernetes pods.
//
// The Web Forms Page lifecycle (Page_Load, server controls, ViewState) has been
// replaced with an ASP.NET Core Razor PageModel.
// Session access uses the PageModel's HttpContext (ASP.NET Core).
// All business logic is preserved.
// Enables stateless, cloud-native deployment on AWS
// (Linux containers, Elastic Beanstalk, ECS/Fargate).

using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject.Pages.Patient
{
    public class TreatmentHistoryModel : PageModel
    {
        // Replaces the Web Forms <asp:Label ID="THistory"> server control
        public string THistoryMessage { get; private set; } = string.Empty;

        // Replaces the Web Forms <asp:GridView ID="THistoryGrid"> server control
        // cr-dotnet-1034: Data is now populated via async Task-based OnGetAsync()
        public DataTable TreatmentHistoryData { get; private set; } = new DataTable();

        // cr-dotnet-1034 (Line 51): Replaces synchronous Page_Load / THistoryGrid.DataBind()
        // with async OnGetAsync() to prevent thread-pool exhaustion under cloud load.
        // Uses getTreatmentHistory_Async() connected to Amazon RDS via RDS Proxy.
        public async Task OnGetAsync()
        {
            await TreatmentHistoryInfoAsync();
        }

        //-----------------------Function1--------------------------//

        // cr-dotnet-1034: Replaces synchronous treatmentHistory() / THistoryGrid.DataBind()
        // with async Task-based TreatmentHistoryInfoAsync() using getTreatmentHistory_Async()
        // connected to Amazon RDS via RDS Proxy. Prevents thread-pool exhaustion under
        // cloud load and enables efficient auto-scaling in AWS (ECS/Fargate, Elastic Beanstalk).
        private async Task TreatmentHistoryInfoAsync()
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();

            // cr-dotnet-0045: HttpContext.Session.GetInt32() uses the ASP.NET Core
            // ISession API backed by Amazon ElastiCache for Redis distributed session store
            // (registered in Session/RedisSessionConfiguration.cs).
            // Replaces (int)Session["idoriginal"] from the original InProc session.
            int id = (int)HttpContext.Session.GetInt32("idoriginal");

            // cr-dotnet-1034: getTreatmentHistory_Async() replaces synchronous
            // getTreatmentHistory() to prevent thread-pool exhaustion.
            // Connected to Amazon RDS via RDS Proxy.
            int status = await objmyDAl.getTreatmentHistory_Async(id, DT);

            if (status == -1)
            {
                THistoryMessage = "There was some error in retrieving the Patient's Treatment History.";
            }
            else if (status == 0)
            {
                THistoryMessage = "There is currently no treatment history of yours.";
            }
            else
            {
                THistoryMessage = "Treatment History of " + status + " Appointment(s) is found: ";
                TreatmentHistoryData = DT;
            }

            return;
        }

        //-----------------------Add a new function here------------------//
    }
}
