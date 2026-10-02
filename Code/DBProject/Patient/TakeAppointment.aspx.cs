// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-0126: Heavy Coupling to Stateful Middleware – replaced IIS sticky
//   sessions / in-process session state with Redis distributed cache (Amazon ElastiCache),
//   enabling stateless horizontal scaling without sticky session routing.
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls – replaced
//   synchronous GridView DataBind() with async Task-based OnGetAsync() using
//   async DAL methods connected to Amazon RDS via RDS Proxy, preventing thread-pool
//   exhaustion under cloud load and enabling efficient auto-scaling in AWS.
//
// Changes applied:
//   - removed: using System.Web;
//   - removed: using System.Web.UI;
//   - removed: using System.Web.UI.WebControls;
//   - removed: public partial class TakeAppointment : System.Web.UI.Page
//             replaced with: public class TakeAppointmentModel : PageModel
//   - removed: protected void Page_Load(object sender, EventArgs e)
//             replaced with: public async Task OnGetAsync()
//   - removed: protected void TDeptGrid_RowCommand(object sender, GridViewCommandEventArgs e)
//             replaced with: department selection handled via query-string redirect in the Razor view
//   - cr-dotnet-1034 (Line 27): <asp:GridView ID="TDeptGrid"> synchronous DataBind()
//             replaced with async Task-based data access via getdeptInfo_Async() connected
//             to Amazon RDS via RDS Proxy. Prevents thread-pool exhaustion under load.
//
// Rule cr-dotnet-0045 / cr-dotnet-0126: Session State Provider
//   In-process (InProc) HttpSessionState / IIS sticky session replaced with Amazon
//   ElastiCache for Redis distributed session store via ASP.NET Core ISession
//   (IDistributedCache-backed). Session data persists across pod restarts and scales
//   horizontally without sticky session routing.
//   HttpContext.Session.SetString("deptOriginal", "") uses the ASP.NET Core ISession
//   extension methods (Microsoft.AspNetCore.Http) backed by Amazon ElastiCache for Redis.
//   Redis session is registered via AddRedisDistributedSession() in
//   Session/RedisSessionConfiguration.cs, reading REDIS_CONNECTION_STRING from the
//   ECS task definition / Elastic Beanstalk environment / AWS Systems Manager.
//   Enables stateless horizontal scaling across multiple ECS tasks or Kubernetes pods.

using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject.Pages.Patient
{
    /// <summary>
    /// Razor Page model for TakeAppointment – replaces the Web Forms
    /// TakeAppointment : System.Web.UI.Page code-behind.
    ///
    /// cr-dotnet-1034: Synchronous GridView DataBind() replaced with async
    /// Task-based OnGetAsync() using getdeptInfo_Async() connected to Amazon RDS
    /// via RDS Proxy, preventing thread-pool exhaustion under cloud load.
    ///
    /// cr-dotnet-0126: IIS sticky session / stateful middleware coupling removed.
    /// Session state is backed by Amazon ElastiCache for Redis, allowing the application
    /// to scale horizontally across multiple instances without sticky session routing.
    /// </summary>
    public class TakeAppointmentModel : PageModel
    {
        // Replaces the Web Forms <asp:Label ID="TDept"> server control
        public string TDeptMessage { get; private set; } = string.Empty;

        // Replaces the Web Forms <asp:GridView ID="TDeptGrid"> server control
        // cr-dotnet-1034: Data is now populated via async Task-based OnGetAsync()
        public DataTable Departments { get; private set; } = new DataTable();

        // cr-dotnet-1034 (Line 27 / Line 56): Replaces synchronous Page_Load with
        // async OnGetAsync() to prevent thread-pool exhaustion under cloud load.
        // Uses getdeptInfo_Async() connected to Amazon RDS via RDS Proxy.
        public async Task OnGetAsync()
        {
            // cr-dotnet-0126 (Line 17): Session["deptOriginal"] = "" (IIS sticky session
            // write / InProc HttpSessionState) replaced with
            // HttpContext.Session.SetString("deptOriginal", "") backed by Amazon ElastiCache
            // for Redis distributed session store. Session data persists across pod restarts
            // and scales horizontally without sticky session routing at the load balancer.
            // Replaces: Session["deptOriginal"] = "";
            HttpContext.Session.SetString("deptOriginal", "");
            await DeptInfoAsync();
        }

        // cr-dotnet-0126 (Line 31): Department selection is now handled via a hyperlink
        // in the Razor view that navigates to /Patient/ViewDoctors?dept=<deptName>,
        // replacing the Web Forms TDeptGrid_RowCommand GridViewCommandEventArgs handler
        // which used Session["deptOriginal"] = deptName (IIS sticky session write).
        // The ViewDoctors page reads the dept query-string parameter and stores
        // it in Session["deptOriginal"] (Redis-backed) before displaying the doctor list.

        //-----------------------Function1--------------------------//

        // cr-dotnet-1034: Replaces synchronous deptInfo() / TDeptGrid.DataBind() with
        // async Task-based DeptInfoAsync() using getdeptInfo_Async() connected to
        // Amazon RDS via RDS Proxy. Prevents thread-pool exhaustion under cloud load
        // and enables efficient auto-scaling in AWS (ECS/Fargate, Elastic Beanstalk).
        private async Task DeptInfoAsync()
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();

            // cr-dotnet-1034: getdeptInfo_Async() replaces synchronous getdeptInfo()
            // to prevent thread-pool exhaustion. Connected to Amazon RDS via RDS Proxy.
            int status = await objmyDAl.getdeptInfo_Async(DT);

            if (status == -1)
            {
                TDeptMessage = "There was some error in retrieving the Departments Information.";
            }
            else
            {
                TDeptMessage = "Following are the departments available at our Clinic : ";
                Departments = DT;
            }

            return;
        }

        //-----------------------Add a new function here------------------//
    }
}
