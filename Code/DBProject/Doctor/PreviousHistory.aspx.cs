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

namespace DBProject.Doctor
{
    public class PreviousHistoryModel : PageModel
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PreviousHistoryModel(IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            _distributedCache = distributedCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public string ErrorMessage { get; set; }
        public DataTable HistoryData { get; set; }

        // cr-dotnet-1034: Changed from synchronous OnGet() to async OnGetAsync()
        // to prevent thread pool exhaustion under load in cloud (AWS RDS) deployments.
        public async Task OnGetAsync()
        {
            await PatHistoryAsync();
        }

        // cr-dotnet-1034: Async data loading replaces synchronous GridView.DataBind().
        // Data is fetched via Task-based API from Amazon RDS, allowing the request thread
        // to be released while awaiting I/O completion.
        private async Task PatHistoryAsync()
        {
            // cr-dotnet-0045: Use Redis-backed distributed session (ElastiCache)
            // instead of in-process HttpSessionState to support horizontal scaling.
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            myDAL objmyDAl = new myDAL();

            int id = session.GetInt32("idoriginal") ?? 0;

            var (status, dt) = await objmyDAl.getPHistoryAsync(id);

            if (status == -1)
            {
                ErrorMessage = "There was some error in retrieving the Patients History.";
            }
            else
            {
                HistoryData = dt;
            }
        }
    }
}
