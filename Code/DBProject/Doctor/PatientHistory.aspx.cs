// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
// Rule cr-dotnet-0045: Session State Provider – replaced InProc HttpSessionState
//   with Amazon ElastiCache for Redis distributed session store.
// Rule cr-dotnet-0126: Heavy Coupling to Stateful Middleware – replaced IIS sticky
//   sessions / in-process session state with Redis distributed cache (Amazon ElastiCache),
//   enabling stateless horizontal scaling without sticky session routing.
//
// Changes applied (cr-dotnet-0026):
//   Line 5  – removed: using System.Web;
//   Line 6  – removed: using System.Web.UI;
//   Line 7  – removed: using System.Web.UI.WebControls;
//   Line 14 – removed: public partial class patienthistory : System.Web.UI.Page
//             replaced with: public class PatientHistoryModel : PageModel
//   Page_Load replaced with OnGetAsync(); GridViewCommandEventArgs replaced with
//   OnPostSelectAppointmentAsync handler using model binding.
//   Response.Write("<script>alert(...)") replaced with model-bound ErrorMessage property.
//   Response.Redirect replaced with RedirectToPage.
//
// Changes applied (cr-dotnet-1034, Line 29):
//   Synchronous Page_Load / GridView.DataBind() replaced with async OnGetAsync() using
//   await search_patient_DAL_Async() backed by Entity Framework Core connected to
//   Amazon RDS. This prevents thread-pool exhaustion under cloud load and enables
//   efficient auto-scaling in AWS (ECS/Fargate, Elastic Beanstalk).
//   The patientsgrid.DataSource = dt; patientsgrid.DataBind() synchronous pattern is
//   replaced with async EF Core SqlQueryRaw<T>().ToListAsync() via HospitalDbContext,
//   and the result is exposed as the Patients DataTable property on the PageModel.
//
// Changes applied (cr-dotnet-0045 / cr-dotnet-0126):
//   Line 21 – Session["idoriginal"] (InProc HttpSessionState / IIS sticky session) replaced
//             with HttpContext.Session.GetInt32("idoriginal") backed by Amazon ElastiCache
//             for Redis distributed session, enabling stateless horizontal scaling across
//             multiple ECS tasks or pods without sticky session routing.
//   Line 46 – Session["appointid"] = appointmentId (InProc HttpSessionState write / IIS
//             sticky session) replaced with HttpContext.Session.SetInt32("appointid", ...)
//             backed by Amazon ElastiCache for Redis distributed session.
//
// The PageModel pattern (ASP.NET Core Razor Pages) + Redis distributed session
// replaces the Web Forms / InProc session model, enabling stateless, cloud-native
// deployment on AWS (Linux containers, Elastic Beanstalk, ECS/Fargate).

using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using DBProject.DAL;
using DBProject.Session;

namespace DBProject.Pages.Doctor
{
    /// <summary>
    /// Razor Page model for PatientHistory – replaces the Web Forms
    /// patienthistory : System.Web.UI.Page code-behind.
    /// Loads today's appointments for the logged-in doctor and handles
    /// row selection to navigate to the HistoryUpdate page.
    ///
    /// cr-dotnet-1034: Synchronous GridView.DataBind() replaced with async
    /// OnGetAsync() using await search_patient_DAL_Async() via Entity Framework Core
    /// connected to Amazon RDS, preventing thread-pool exhaustion under cloud load.
    ///
    /// cr-dotnet-0126: IIS sticky session / stateful middleware coupling removed.
    /// Session state is backed by Amazon ElastiCache for Redis via
    /// IDistributedCache (Microsoft.Extensions.Caching.StackExchangeRedis),
    /// allowing the application to scale horizontally across multiple instances
    /// without requiring sticky session routing at the load balancer.
    /// </summary>
    public class PatientHistoryModel : PageModel
    {
        private readonly myDAL _dal;

        /// <summary>
        /// Initialises the PatientHistoryModel with the DAL dependency.
        /// The DAL provides async EF Core data access methods connected to Amazon RDS.
        /// </summary>
        public PatientHistoryModel()
        {
            _dal = new myDAL();
        }

        /// <summary>
        /// Bound data table populated from the DAL – replaces the GridView DataSource.
        /// Populated asynchronously via EF Core to prevent thread-pool exhaustion.
        /// </summary>
        public DataTable? Patients { get; private set; }

        /// <summary>
        /// Non-null when the DAL call fails – replaces Response.Write alert.
        /// </summary>
        public string ErrorMessage { get; private set; } = string.Empty;

        /// <summary>
        /// cr-dotnet-1034 (Line 29): Replaces synchronous Page_Load / GridView.DataBind()
        /// with async OnGetAsync() using await search_patient_DAL_Async() via Entity
        /// Framework Core connected to Amazon RDS.
        ///
        /// The original synchronous pattern:
        ///   patientsgrid.DataSource = dt;
        ///   patientsgrid.DataBind();
        /// is replaced with async EF Core data access, preventing thread-pool exhaustion
        /// under cloud load and enabling efficient auto-scaling in AWS deployments.
        /// </summary>
        public async Task OnGetAsync()
        {
            DataTable dt = new DataTable();

            // cr-dotnet-0126 (Line 46): Read "idoriginal" from Redis-backed distributed
            // session (Amazon ElastiCache) instead of IIS in-process / sticky session.
            // Replaces: int did = (int)Session["idoriginal"];
            // HttpContext.Session is backed by Amazon ElastiCache for Redis via
            // IDistributedCache, registered in Session/RedisSessionConfiguration.cs.
            int did = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            // cr-dotnet-1034 (Line 29): Async EF Core data access replacing synchronous
            // patientsgrid.DataSource = dt; patientsgrid.DataBind();
            // Uses await to prevent blocking the request thread under cloud load.
            int found = await _dal.search_patient_DAL_Async(did, dt);
            if (found != 1)
            {
                ErrorMessage = "There was some error";
            }
            else
            {
                Patients = dt;
            }
        }

        /// <summary>
        /// cr-dotnet-1034: Replaces synchronous patientsgrid_RowCommand with async
        /// OnPostSelectAppointmentAsync using model binding.
        /// Stores the selected appointment id in Redis-backed distributed session
        /// and redirects to HistoryUpdate.
        ///
        /// cr-dotnet-0126 (Line 46): Session["appointid"] = appointmentId (IIS sticky
        /// session write) replaced with HttpContext.Session.SetInt32("appointid", ...)
        /// backed by Amazon ElastiCache for Redis distributed session store.
        /// </summary>
        public async Task<IActionResult> OnPostSelectAppointmentAsync(string appointmentId)
        {
            int appointmentIdInt = Convert.ToInt32(appointmentId);

            // cr-dotnet-0126 (Line 46): Write "appointid" to Redis-backed distributed
            // session (Amazon ElastiCache) instead of IIS in-process / sticky session.
            // Replaces: Session["appointid"] = appointmentid;
            HttpContext.Session.SetInt32("appointid", appointmentIdInt);

            // Commit session asynchronously to Redis (Amazon ElastiCache)
            await HttpContext.Session.CommitAsync();

            return RedirectToPage("/Doctor/HistoryUpdate");
        }
    }
}
