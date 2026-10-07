using System;
using System.Collections.Generic;
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

namespace doctor
{
    public class PatientHistoryModel : PageModel
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientHistoryModel(IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            _distributedCache = distributedCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public string ErrorMessage { get; set; }
        public DataTable PatientsData { get; set; }

        // cr-dotnet-1034: Changed from synchronous OnGet() to async OnGetAsync()
        // to prevent thread pool exhaustion under load in cloud (AWS RDS) deployments.
        public async Task OnGetAsync()
        {
            // cr-dotnet-0126: Use Redis-backed distributed session (Amazon ElastiCache)
            // instead of IIS in-process HttpSessionState to support horizontal scaling.
            // Original: int did = (int)Session["idoriginal"];
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            myDAL objmydal = new myDAL();

            int did = session.GetInt32("idoriginal") ?? 0;

            // cr-dotnet-1034: Async data fetch replaces synchronous GridView.DataBind().
            // Data is fetched via Task-based API from Amazon RDS, allowing the request
            // thread to be released while awaiting I/O completion.
            var (status, dt) = await objmydal.search_patient_DALAsync(did);
            if (status != 1)
            {
                ErrorMessage = "There was some error";
            }
            else
            {
                PatientsData = dt;
            }
        }

        public IActionResult OnPostSelectAppointment(int appointmentId)
        {
            // cr-dotnet-0126: Use Redis-backed distributed session (Amazon ElastiCache)
            // instead of IIS in-process HttpSessionState to support horizontal scaling.
            // Original: Session["appointid"] = appointmentid;
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            session.SetInt32("appointid", appointmentId);
            return RedirectToPage("/Doctor/HistoryUpdate");
        }
    }
}
