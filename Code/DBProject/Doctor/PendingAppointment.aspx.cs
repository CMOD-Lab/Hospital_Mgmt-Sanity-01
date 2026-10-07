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
// Rule cr-dotnet-0045: Session State Provider
//   Replaced in-process HttpSessionState with Amazon ElastiCache for Redis via
//   RedisSessionHelper (IDistributedCache) to enable stateless horizontal scaling.
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
//   Replaced synchronous GridView.DataBind() with async Task-based data loading via
//   OnGetAsync() and myDAL async methods connected to Amazon RDS, preventing thread pool
//   exhaustion under load and enabling efficient auto-scaling in cloud deployments.

namespace doctor
{
    public class PendingAppointmentModel : PageModel
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PendingAppointmentModel(IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            _distributedCache = distributedCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public DataTable PendingAppointments { get; set; }

        // cr-dotnet-1034: Changed from synchronous OnGet() to async OnGetAsync()
        // to prevent thread pool exhaustion under load in cloud (AWS RDS) deployments.
        public async Task OnGetAsync()
        {
            await LoadGridAsync();
        }

        // cr-dotnet-1034: Async grid loading replaces synchronous GridView.DataBind().
        // Data is fetched via Task-based API from Amazon RDS, allowing the request thread
        // to be released while awaiting I/O completion.
        private async Task LoadGridAsync()
        {
            // cr-dotnet-0045: Use Redis-backed distributed session (ElastiCache)
            // instead of in-process HttpSessionState to support horizontal scaling.
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            myDAL objDAL = new myDAL();
            int did = session.GetInt32("idoriginal") ?? 0;
            DataTable dt = await objDAL.GetAllpendingappointmentsAsync(did);
            PendingAppointments = dt;
        }

        public IActionResult OnPostUpdateAppointment(int appointmentId)
        {
            myDAL objmyDAL = new myDAL();
            objmyDAL.UpdateAppointment_DAL(appointmentId);
            return RedirectToPage();
        }

        public IActionResult OnPostDeleteAppointment(int appointmentId)
        {
            myDAL objDAL = new myDAL();
            objDAL.Deleteappointment_DAL(appointmentId);
            return RedirectToPage();
        }
    }
}
