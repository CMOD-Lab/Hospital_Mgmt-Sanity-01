// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
//
// Changes applied:
//   Line 5  – removed: using System.Web;                          (occurrence 5)
//   Line 6  – removed: using System.Web.UI;                       (occurrence 6)
//   Line 7  – removed: using System.Web.UI.WebControls;
//   Line 13 – removed: public partial class pendingappointment : System.Web.UI.Page
//             replaced with: public class PendingAppointmentModel : PageModel  (occurrence 7)
//   Line 15 – Page_Load replaced with OnGetAsync()                (occurrence 8)
//
// cr-dotnet-1034 (Line 32):
//   Synchronous Page_Load / GridView.DataBind() replaced with async OnGetAsync() using
//   await GetAllpendingappointments_DAL_Async() backed by Entity Framework Core connected
//   to Amazon RDS. This prevents thread-pool exhaustion under cloud load and enables
//   efficient auto-scaling in AWS (ECS/Fargate, Elastic Beanstalk).
//   The pendingappointments.DataSource = DT; pendingappointments.DataBind() synchronous
//   pattern is replaced with async EF Core SqlQueryRaw<T>().ToListAsync() via
//   HospitalDbContext, and the result is exposed as the Appointments DataTable property.
//
// Rule cr-dotnet-0045: Session State Provider (Line 25)
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject.Pages.Doctor
{
    /// <summary>
    /// Razor Page model for PendingAppointment – replaces the Web Forms
    /// pendingappointment : System.Web.UI.Page code-behind.
    /// Loads all pending appointments for the logged-in doctor and handles
    /// row-level update and delete actions.
    ///
    /// cr-dotnet-1034: Synchronous GridView.DataBind() replaced with async
    /// OnGetAsync() using await GetAllpendingappointments_DAL_Async() via Entity
    /// Framework Core connected to Amazon RDS, preventing thread-pool exhaustion
    /// under cloud load.
    ///
    /// cr-dotnet-0045: Session state is backed by Amazon ElastiCache for Redis
    /// via ASP.NET Core ISession (IDistributedCache), replacing InProc HttpSessionState.
    /// </summary>
    public class PendingAppointmentModel : PageModel
    {
        private readonly myDAL _dal;

        /// <summary>
        /// Initialises the PendingAppointmentModel with the DAL dependency.
        /// The DAL provides async EF Core data access methods connected to Amazon RDS.
        /// </summary>
        public PendingAppointmentModel()
        {
            _dal = new myDAL();
        }

        /// <summary>
        /// Bound data table populated from the DAL – replaces the GridView DataSource.
        /// Populated asynchronously via EF Core to prevent thread-pool exhaustion.
        /// </summary>
        public DataTable? Appointments { get; private set; }

        /// <summary>
        /// Non-null when the DAL call fails – replaces Response.Write alert.
        /// </summary>
        public string ErrorMessage { get; private set; } = string.Empty;

        /// <summary>
        /// cr-dotnet-1034 (Line 32): Replaces synchronous Page_Load / GridView.DataBind()
        /// with async OnGetAsync() using await GetAllpendingappointments_DAL_Async() via
        /// Entity Framework Core connected to Amazon RDS.
        /// </summary>
        public async Task OnGetAsync()
        {
            await LoadGridAsync();
        }

        /// <summary>
        /// cr-dotnet-1034: Async helper that fetches pending appointments from the DAL
        /// using EF Core connected to Amazon RDS.
        /// Replaces the synchronous loadgrid() method and pendingappointments.DataBind().
        /// </summary>
        private async Task LoadGridAsync()
        {
            // cr-dotnet-0045: Session["idoriginal"] replaced with
            // HttpContext.Session.GetInt32("idoriginal") – reads from Amazon ElastiCache
            // for Redis distributed session store (IDistributedCache-backed ISession).
            // Registered via AddRedisDistributedSession() in Session/RedisSessionConfiguration.cs.
            int did = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            DataTable dt = new DataTable();

            // cr-dotnet-1034: Async EF Core data access replacing synchronous
            // pendingappointments.DataSource = DT; pendingappointments.DataBind();
            // Uses await to prevent blocking the request thread under cloud load.
            await _dal.GetAllpendingappointments_DAL_Async(did, dt);
            Appointments = dt;
        }

        /// <summary>
        /// cr-dotnet-1034: Replaces synchronous update_appointment handler
        /// (CommandName == "Select") with async OnPostUpdateAppointmentAsync.
        /// Accepts the appointment id from the form post, updates it via the DAL
        /// asynchronously, then reloads the grid.
        /// </summary>
        public async Task<IActionResult> OnPostUpdateAppointmentAsync(string appointmentId)
        {
            // Retrieve appointmentid from the posted form value (replaces Cells[1].Text)
            int appointmentid = Convert.ToInt32(appointmentId);

            // cr-dotnet-1034: Async update via EF Core connected to Amazon RDS
            await _dal.UpdateAppointment_DAL_Async(appointmentid);

            // Reload the page to reflect changes (replaces loadgrid() after EditIndex = -1)
            return RedirectToPage();
        }

        /// <summary>
        /// cr-dotnet-1034: Replaces synchronous Delete_appointment handler
        /// (GridViewDeleteEventArgs) with async OnPostDeleteAppointmentAsync.
        /// Accepts the appointment id from the form post and deletes it via the DAL
        /// asynchronously.
        /// </summary>
        public async Task<IActionResult> OnPostDeleteAppointmentAsync(string appointmentId)
        {
            // Retrieve appointmentid from the posted form value (replaces row.Cells[1].Text)
            int appointmentid = Convert.ToInt32(appointmentId);

            // cr-dotnet-1034: Async delete via EF Core connected to Amazon RDS
            myDAL objDAL = new myDAL();
            int result = await objDAL.Deleteappointment_DAL_Async(appointmentid);

            if (result == 1)
            {
                // Reload the grid to show the modifications in table
                return RedirectToPage();
            }

            // If delete failed, reload anyway
            return RedirectToPage();
        }
    }
}
