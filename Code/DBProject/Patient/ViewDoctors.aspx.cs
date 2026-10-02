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
//   - removed: public partial class ViewDoctors : System.Web.UI.Page
//             replaced with: public class ViewDoctorsModel : PageModel
//   - removed: protected void Page_Load(object sender, EventArgs e)
//             replaced with: public async Task OnGetAsync()
//   - removed: protected void TDoctorGrid_RowCommand(object sender, GridViewCommandEventArgs e)
//             replaced with: doctor selection handled via query-string redirect in the Razor view
//   - removed: TDoctor.Text = "..." (server control property assignment)
//             replaced with: public string TDoctorMessage { get; private set; }
//   - removed: TDoctorGrid.DataSource = DT; TDoctorGrid.DataBind();
//             (cr-dotnet-1034 Lines 63/63: synchronous GridView DataBind() replaced with
//              async Task-based data access via getDeptDoctorInfo_Async())
//             replaced with: public DataTable Doctors { get; private set; }
//
// Rule cr-dotnet-0045 / cr-dotnet-0126: Session State Provider
//   In-process (InProc) HttpSessionState / IIS sticky session replaced with Amazon
//   ElastiCache for Redis distributed session store via ASP.NET Core ISession
//   (IDistributedCache-backed). Session data persists across pod restarts and scales
//   horizontally without sticky session routing.
//   HttpContext.Session.SetString() / HttpContext.Session.GetString() use the ASP.NET Core
//   ISession extension methods (Microsoft.AspNetCore.Http) backed by Amazon ElastiCache for Redis.
//   Redis session is registered via AddRedisDistributedSession() in
//   Session/RedisSessionConfiguration.cs, reading REDIS_CONNECTION_STRING from the
//   ECS task definition / Elastic Beanstalk environment / AWS Systems Manager.
//   Replaces Session["deptOriginal"] and Session["dID"] from the original InProc / IIS session.
//   Enables stateless horizontal scaling across multiple ECS tasks or Kubernetes pods.
//
// The Web Forms Page lifecycle (Page_Load, GridView RowCommand, server controls,
// ViewState) has been replaced with an ASP.NET Core Razor PageModel.
// Session access uses the PageModel's HttpContext (ASP.NET Core).
// The dept query-string parameter replaces Session["deptOriginal"] for the
// department name lookup, and doctor selection navigates to DoctorProfile
// via query-string dID parameter instead of Session["dID"].
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
    /// <summary>
    /// Razor Page model for ViewDoctors – replaces the Web Forms
    /// ViewDoctors : System.Web.UI.Page code-behind.
    ///
    /// cr-dotnet-1034 (Line 63): Synchronous GridView DataBind() replaced with async
    /// Task-based OnGetAsync() using getDeptDoctorInfo_Async() connected to Amazon RDS
    /// via RDS Proxy, preventing thread-pool exhaustion under cloud load.
    ///
    /// cr-dotnet-0126: IIS sticky session / stateful middleware coupling removed.
    /// Session state is backed by Amazon ElastiCache for Redis, allowing the application
    /// to scale horizontally across multiple instances without sticky session routing.
    /// </summary>
    public class ViewDoctorsModel : PageModel
    {
        // Replaces the Web Forms <asp:Label ID="TDoctor"> server control
        public string TDoctorMessage { get; private set; } = string.Empty;

        // Replaces the Web Forms <asp:GridView ID="TDoctorGrid"> server control
        // cr-dotnet-1034 (Line 63): Data is now populated via async Task-based OnGetAsync()
        // instead of synchronous TDoctorGrid.DataSource = DT; TDoctorGrid.DataBind()
        public DataTable Doctors { get; private set; } = new DataTable();

        // cr-dotnet-1034 (Line 63): Replaces synchronous Page_Load /
        // TDoctorGrid.DataBind() with async OnGetAsync() to prevent thread-pool
        // exhaustion under cloud load. Uses getDeptDoctorInfo_Async() connected to
        // Amazon RDS via RDS Proxy.
        public async Task OnGetAsync(string dept)
        {
            // cr-dotnet-0126: Session["dID"] = "" (IIS sticky session write /
            // InProc HttpSessionState) replaced with HttpContext.Session.SetString("dID", "")
            // backed by Amazon ElastiCache for Redis distributed session store.
            // Session data persists across pod restarts and scales horizontally without
            // sticky session routing at the load balancer.
            // Replaces: Session["dID"] = "";
            if (!string.IsNullOrEmpty(dept))
            {
                // cr-dotnet-0126: Store the department name in Redis-backed distributed
                // session for downstream pages (e.g. DoctorProfile).
                // Replaces: Session["deptOriginal"] = deptName (IIS sticky session write).
                HttpContext.Session.SetString("deptOriginal", dept);
            }

            // cr-dotnet-0126: Session["dID"] = "" (IIS sticky session write)
            // replaced with HttpContext.Session.SetString("dID", "") backed by Amazon
            // ElastiCache for Redis distributed session store.
            // Replaces: Session["dID"] = "";
            HttpContext.Session.SetString("dID", "");

            await DeptDoctorInfoAsync();
        }

        // cr-dotnet-0126: Doctor selection is now handled via a hyperlink in the Razor view
        // that navigates to /Patient/DoctorProfile?dID=<doctorId>, replacing the
        // Web Forms TDoctorGrid_RowCommand GridViewCommandEventArgs handler which used
        // Session["dID"] = dID (IIS sticky session write).
        // The DoctorProfile page reads the dID query-string parameter and stores
        // it in Session["dID"] (Redis-backed) before displaying the doctor profile.

        //-----------------------Function1--------------------------//

        // cr-dotnet-1034 (Line 63): Replaces synchronous deptDoctorInfo() /
        // TDoctorGrid.DataSource = DT; TDoctorGrid.DataBind() with async Task-based
        // DeptDoctorInfoAsync() using getDeptDoctorInfo_Async() connected to Amazon RDS
        // via RDS Proxy. Prevents thread-pool exhaustion under cloud load and enables
        // efficient auto-scaling in AWS (ECS/Fargate, Elastic Beanstalk).
        private async Task DeptDoctorInfoAsync()
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();

            // cr-dotnet-0126: Session["deptOriginal"] (IIS sticky session read) replaced
            // with HttpContext.Session.GetString("deptOriginal") backed by Amazon ElastiCache
            // for Redis distributed session store.
            // Replaces: string deptName = (string)Session["deptOriginal"];
            string deptName = HttpContext.Session.GetString("deptOriginal") ?? string.Empty;

            // cr-dotnet-1034 (Line 63): getDeptDoctorInfo_Async() replaces synchronous
            // getDeptDoctorInfo() / TDoctorGrid.DataBind() to prevent thread-pool exhaustion.
            // Connected to Amazon RDS via RDS Proxy.
            int status = await objmyDAl.getDeptDoctorInfo_Async(deptName, DT);

            if (status == -1)
            {
                TDoctorMessage = "There was some error in retrieving the Doctors Information.";
            }
            else
            {
                TDoctorMessage = "Following are our Specialized Doctors of " + deptName + " Department:";
                Doctors = DT;
            }

            return;
        }

        //-----------------------Add a new function here------------------//
    }
}
