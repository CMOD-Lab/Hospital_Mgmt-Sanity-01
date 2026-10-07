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
//   Replaced synchronous OnGet() with async Task OnGetAsync() using getTreatmentHistoryAsync
//   connected to Amazon RDS via Dapper async APIs, preventing thread pool exhaustion under load
//   and enabling efficient auto-scaling in cloud deployments.

namespace DBProject.Patient
{
    public class TreatmentHistoryModel : PageModel
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TreatmentHistoryModel(IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            _distributedCache = distributedCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public string THistory { get; set; }
        public DataTable TreatmentData { get; set; }

        // cr-dotnet-1034: Converted from synchronous OnGet() to async Task OnGetAsync()
        // Uses async DAL method to prevent thread pool exhaustion under cloud load.
        public async Task OnGetAsync()
        {
            await TreatmentHistoryLoadAsync();
        }

        //-----------------------Function1--------------------------//

        // cr-dotnet-1034: Converted from synchronous TreatmentHistoryLoad() to async Task TreatmentHistoryLoadAsync()
        // Calls getTreatmentHistoryAsync on the DAL to avoid blocking the thread pool on I/O.
        private async Task TreatmentHistoryLoadAsync()
        {
            myDAL objmyDAl = new myDAL();

            // cr-dotnet-0045: Use Redis-backed distributed session (ElastiCache)
            // instead of in-process HttpSessionState to support horizontal scaling.
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            int id = session.GetInt32("idoriginal") ?? 0;

            var (status, DT) = await objmyDAl.getTreatmentHistoryAsync(id);

            if (status == -1)
            {
                THistory = "There was some error in retrieving the Patient's Treatment History.";
            }
            else if (status == 0)
            {
                THistory = "There is currently no treatment history of yours.";
            }
            else
            {
                THistory       = "Treatment History of " + status + " Appointment(s) is found: ";
                TreatmentData  = DT;
            }
        }

        //-----------------------Add a new function here------------------//
    }
}
