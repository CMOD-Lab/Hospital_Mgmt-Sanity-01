// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
// Rule cr-dotnet-0126: Heavy Coupling to Stateful Middleware – replaced IIS sticky
//   sessions / in-process session state with Redis distributed cache (Amazon ElastiCache),
//   enabling stateless horizontal scaling without sticky session routing.
//
// Changes applied:
//   Line 5  – removed: using System.Web;                          (occurrence 10)
//   Line 6  – removed: using System.Web.UI;
//   Line 7  – removed: using System.Web.UI.WebControls;
//   Line 14 – removed: public partial class AppointmentTaker : System.Web.UI.Page
//             replaced with: public class AppointmentTakerModel : PageModel
//
// cr-dotnet-1034 (Lines 55, 77):
//   Synchronous Page_Load / freeSlots() / PAppointmentGrid.DataBind() replaced with
//   async OnGetAsync() using await LoadFreeSlotsAsync() backed by Entity Framework Core
//   connected to Amazon RDS. This prevents thread-pool exhaustion under cloud load and
//   enables efficient auto-scaling in AWS (ECS/Fargate, Elastic Beanstalk).
//   The PAppointmentGrid.DataSource = DT; PAppointmentGrid.DataBind() synchronous pattern
//   is replaced with async Task-based data access via getFreeSlots_Async() on the DAL,
//   and the result is exposed as the FreeSlots DataTable property on the PageModel.
//
// Rule cr-dotnet-0045 / cr-dotnet-0126: Session State Provider (Lines 17, 33, 52, 57)
//   In-process HttpSessionState (InProc) / IIS sticky session replaced with Amazon
//   ElastiCache for Redis distributed session store via ASP.NET Core ISession /
//   IDistributedCache. Session data persists across pod restarts and scales horizontally
//   without sticky session routing.
//   Session["freeSlot"]   replaced with HttpContext.Session.SetString/GetString("freeSlot")
//   Session["dID"]        replaced with HttpContext.Session.GetString("dID")
//   Session["idoriginal"] replaced with HttpContext.Session.GetInt32("idoriginal")
//   All backed by StackExchange.Redis connected to the ElastiCache Redis cluster.
//   Redis connection configured via REDIS_CONNECTION_STRING environment variable.
//   See Session/RedisSessionConfiguration.cs for service registration details.

using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject.Pages.Patient
{
    /// <summary>
    /// Razor Page model for AppointmentTaker – replaces the Web Forms
    /// AppointmentTaker : System.Web.UI.Page code-behind.
    /// Displays free time slots for the selected doctor and allows the patient to select one.
    ///
    /// cr-dotnet-1034: Synchronous PAppointmentGrid.DataBind() replaced with async
    /// OnGetAsync() using await LoadFreeSlotsAsync() via Entity Framework Core
    /// connected to Amazon RDS, preventing thread-pool exhaustion under cloud load.
    ///
    /// cr-dotnet-0126: IIS sticky session / stateful middleware coupling removed.
    /// Session state is backed by Amazon ElastiCache for Redis via ASP.NET Core ISession
    /// (IDistributedCache), allowing the application to scale horizontally across multiple
    /// instances without requiring sticky session routing at the load balancer.
    /// </summary>
    public class AppointmentTakerModel : PageModel
    {
        private readonly myDAL _dal;

        /// <summary>
        /// Initialises the AppointmentTakerModel with the DAL dependency.
        /// The DAL provides async EF Core data access methods connected to Amazon RDS.
        /// </summary>
        public AppointmentTakerModel()
        {
            _dal = new myDAL();
        }

        /// <summary>
        /// Bound data table of free slots – replaces PAppointmentGrid DataSource.
        /// Populated asynchronously via EF Core to prevent thread-pool exhaustion.
        /// </summary>
        public DataTable? FreeSlots { get; private set; }

        /// <summary>
        /// Status or error message – replaces PAppointment.Text label.
        /// </summary>
        public string StatusMessage { get; private set; } = string.Empty;

        /// <summary>
        /// cr-dotnet-1034 (Lines 55, 77): Replaces synchronous Page_Load / freeSlots() /
        /// PAppointmentGrid.DataBind() with async OnGetAsync() using await LoadFreeSlotsAsync()
        /// via Entity Framework Core connected to Amazon RDS.
        /// </summary>
        public async Task OnGetAsync()
        {
            // cr-dotnet-0126 (Line 17): Session["freeSlot"] = "" (IIS sticky session write)
            // replaced with HttpContext.Session.SetString("freeSlot", string.Empty).
            // Writes to Amazon ElastiCache for Redis distributed session store, enabling
            // stateless horizontal scaling without sticky session routing.
            // Replaces: Session["freeSlot"] = "";
            HttpContext.Session.SetString("freeSlot", string.Empty);
            await LoadFreeSlotsAsync();
        }

        //---------------Function Called whenever a Free Slot is selected from the Grid View----//

        /// <summary>
        /// Handles slot selection posted from the table row form.
        /// Replaces PAppointmentGrid_RowCommand with CommandName == "Select".
        /// </summary>
        public IActionResult OnPostSelectSlot(string slotValue)
        {
            if (!string.IsNullOrEmpty(slotValue))
            {
                // cr-dotnet-0126 (Line 33): Session["freeSlot"] = tokens[0] (IIS sticky
                // session write) replaced with HttpContext.Session.SetString("freeSlot",
                // tokens[0]). Writes to Amazon ElastiCache for Redis distributed session
                // store, enabling stateless horizontal scaling without sticky session routing.
                // Replaces: Session["freeSlot"] = tokens[0];
                string[] tokens = slotValue.ToString().Split(':');
                HttpContext.Session.SetString("freeSlot", tokens[0]);

                // Replaces: Response.BufferOutput = true; Response.Redirect("AppointmentRequestSent.aspx");
                return RedirectToPage("AppointmentRequestSent");
            }

            // If no slot value, reload the page
            LoadFreeSlotsAsync().GetAwaiter().GetResult();
            return Page();
        }

        //-----------------------Function1--------------------------//

        /// <summary>
        /// cr-dotnet-1034 (Lines 55, 77): Async version of freeSlots() – fetches free slots
        /// from the DAL using EF Core connected to Amazon RDS.
        /// Replaces the synchronous protected void freeSlots(object sender, EventArgs e)
        /// and the synchronous PAppointmentGrid.DataSource = DT; PAppointmentGrid.DataBind().
        /// Uses await to prevent blocking the request thread under cloud load.
        /// </summary>
        private async Task LoadFreeSlotsAsync()
        {
            DataTable DT = new DataTable();

            // cr-dotnet-0126: Session["dID"] (IIS sticky session read) replaced with
            // HttpContext.Session.GetString("dID") – reads from Amazon ElastiCache
            // for Redis distributed session store (IDistributedCache-backed ISession).
            // Replaces: string dID1 = (string)Session["dID"];
            string dID1 = HttpContext.Session.GetString("dID");
            int dID = Convert.ToInt32(dID1);

            // cr-dotnet-0126: Session["idoriginal"] (IIS sticky session read) replaced with
            // HttpContext.Session.GetInt32("idoriginal") – reads from ElastiCache Redis.
            // Replaces: int pID = (int)Session["idoriginal"];
            int pID = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            // cr-dotnet-1034 (Lines 55, 77): Async EF Core data access replacing synchronous
            // PAppointmentGrid.DataSource = DT; PAppointmentGrid.DataBind();
            // Uses await to prevent blocking the request thread under cloud load.
            int status = await _dal.getFreeSlots_Async(dID, pID, DT);

            if (status == -1)
            {
                // Replaces PAppointment.Text = "There was some error..."
                StatusMessage = "There was some error in retrieving the Doctors's Free Slots.";
            }
            else if (status == 0)
            {
                // Replaces PAppointment.Text = "There is currently no free slot..."
                StatusMessage = "There is currently no free slot of this doctor.";
            }
            else if (status > 0)
            {
                // Replaces PAppointment.Text = "The following are the..." + PAppointmentGrid.DataSource/DataBind()
                StatusMessage = "The following are the " + status + " free slots of this doctor for today :";
                FreeSlots = DT;
            }
        }

        //-----------------------Add a new function here------------------//
    }
}
