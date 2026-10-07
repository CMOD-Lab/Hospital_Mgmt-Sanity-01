using System;
using System.Data;
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

namespace doctor
{
    public class HistoryUpdateModel : PageModel
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HistoryUpdateModel(IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            _distributedCache = distributedCache;
            _httpContextAccessor = httpContextAccessor;
        }

        [BindProperty]
        public string Disease { get; set; }

        [BindProperty]
        public string Progress { get; set; }

        [BindProperty]
        public string Prescription { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public void OnGet()
        {
            // Page load - no action needed
        }

        public IActionResult OnPostSaveInDatabase()
        {
            // cr-dotnet-0045: Use Redis-backed distributed session (ElastiCache)
            // instead of in-process HttpSessionState to support horizontal scaling.
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            myDAL objmyDAL = new myDAL();
            int found;
            int did = session.GetInt32("idoriginal") ?? 0;
            int appid = session.GetInt32("appointid") ?? 0;

            found = objmyDAL.update_prescription_DAL(did, appid, Disease, Progress, Prescription);

            if (found != 1)
            {
                ErrorMessage = "There was some error";
            }
            else
            {
                SuccessMessage = "Information Successfully Updated";
            }

            return Page();
        }

        public IActionResult OnPostGenerateBill()
        {
            return RedirectToPage("/Doctor/Bill");
        }
    }
}
