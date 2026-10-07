using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using DBProject.DAL;
using DBProject.Infrastructure;

// Migrated from ASP.NET Web Forms (System.Web.UI.Page) to ASP.NET Core Razor Pages (PageModel)
// Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages
// Rule cr-dotnet-0126: Heavy Coupling to Stateful Middleware
//   Replaced IIS in-process HttpSessionState (sticky-session) with Amazon ElastiCache for Redis
//   via RedisSessionHelper (IDistributedCache) to enable stateless horizontal scaling.
//   Session data now persists across pod restarts and scales horizontally without sticky routing.
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
//   Replaced synchronous GridView.DataBind() with async Task-based data loading via
//   OnGetAsync() and myDAL async methods connected to Amazon RDS, preventing thread pool
//   exhaustion under load and enabling efficient auto-scaling in cloud deployments.

namespace DBProject.Patient
{
    public class AppointmentTakerModel : PageModel
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppointmentTakerModel(IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            _distributedCache = distributedCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public string AppointmentMessage { get; set; }
        public DataTable FreeSlots { get; set; }
        public int SlotCount { get; set; }

        // cr-dotnet-1034: Changed from synchronous OnGet() to async OnGetAsync()
        // to prevent thread pool exhaustion under load in cloud (AWS RDS) deployments.
        public async Task OnGetAsync()
        {
            // cr-dotnet-0126: Use Redis-backed distributed session (Amazon ElastiCache)
            // instead of IIS in-process HttpSessionState to support horizontal scaling.
            // Original: Session["freeSlot"] = "";
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            session.SetString("freeSlot", "");
            await LoadFreeSlotsAsync(session);
        }

        // Handler for selecting a free slot row (POST)
        public IActionResult OnPostSelectSlot(string slotValue)
        {
            // cr-dotnet-0126: Use Redis-backed distributed session (Amazon ElastiCache)
            // instead of IIS in-process HttpSessionState to support horizontal scaling.
            // Original: Session["freeSlot"] = tokens[0];
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            session.SetString("freeSlot", slotValue ?? "");
            return RedirectToPage("/Patient/AppointmentRequestSent");
        }

        // cr-dotnet-1034: Async data loading replaces synchronous GridView.DataBind().
        // Data is fetched via Task-based API from Amazon RDS, allowing the request thread
        // to be released while awaiting I/O completion.
        private async Task LoadFreeSlotsAsync(RedisSessionHelper session)
        {
            myDAL objmyDAl = new myDAL();

            string dID1 = session.GetString("dID");
            int dID = Convert.ToInt32(dID1);

            int pID = session.GetInt32("idoriginal") ?? 0;

            var (status, dt) = await objmyDAl.getFreeSlotsAsync(dID, pID);

            if (status == -1)
            {
                AppointmentMessage = "There was some error in retrieving the Doctors's Free Slots.";
            }
            else if (status == 0)
            {
                AppointmentMessage = "There is currently no free slot of this doctor.";
            }
            else if (status > 0)
            {
                AppointmentMessage = "The following are the " + status + " free slots of this doctor for today :";
                FreeSlots = dt;
                SlotCount = status;
            }
        }
    }
}
