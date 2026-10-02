// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
// Rule cr-dotnet-0045: Session State Provider – replaced InProc HttpSessionState
//   with Amazon ElastiCache for Redis distributed session store.
//
// Changes applied (cr-dotnet-0026):
//   Line 5  – removed: using System.Web;
//   Line 6  – removed: using System.Web.UI;
//   Line 12 – removed: public partial class Historyupdate : System.Web.UI.Page
//             replaced with: public class HistoryUpdateModel : PageModel
//   Line 14 – removed: protected void Page_Load(object sender, EventArgs e)
//             replaced with: public void OnGet()
//   Response.Write("<script>alert(...)") replaced with model-bound StatusMessage property.
//   Response.Redirect replaced with RedirectToPage.
//
// Changes applied (cr-dotnet-0045):
//   Line 23 – Session["idoriginal"] (InProc HttpSessionState) replaced with
//             HttpContext.Session.GetInt32("idoriginal") backed by
//             Amazon ElastiCache for Redis distributed session, enabling
//             stateless horizontal scaling across multiple ECS tasks or pods.
//   Line 28 – Session["appointid"] (InProc HttpSessionState) replaced with
//             HttpContext.Session.GetInt32("appointid") backed by
//             Amazon ElastiCache for Redis distributed session.
//
// The PageModel pattern (ASP.NET Core Razor Pages) + Redis distributed session
// replaces the Web Forms / InProc session model, enabling stateless, cloud-native
// deployment on AWS (Linux containers, Elastic Beanstalk, ECS/Fargate).

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Distributed;
using DBProject.DAL;
using DBProject.Session;

namespace DBProject.Pages.Doctor
{
    /// <summary>
    /// Razor Page model for HistoryUpdate – replaces the Web Forms
    /// Historyupdate : System.Web.UI.Page code-behind.
    /// Handles saving prescription updates and redirecting to bill generation.
    /// Session state is backed by Amazon ElastiCache for Redis via
    /// IDistributedCache (Microsoft.Extensions.Caching.StackExchangeRedis).
    /// </summary>
    public class HistoryUpdateModel : PageModel
    {
        [BindProperty]
        public string Disease { get; set; } = string.Empty;

        [BindProperty]
        public string Progress { get; set; } = string.Empty;

        [BindProperty]
        public string Prescription { get; set; } = string.Empty;

        public string StatusMessage { get; set; } = string.Empty;
        public bool IsError { get; set; } = false;

        /// <summary>
        /// Replaces Page_Load – no initialisation logic was present in the original.
        /// </summary>
        public void OnGet()
        {
            // Intentionally empty – no load logic in original Page_Load.
        }

        /// <summary>
        /// Replaces saveindatabase event handler.
        /// Reads doctor id and appointment id from Redis-backed distributed session,
        /// then persists the update.
        /// </summary>
        public IActionResult OnPostSaveInDatabase()
        {
            myDAL objmyDAL = new myDAL();

            // cr-dotnet-0045 (Line 23): Read "idoriginal" from Redis-backed
            // distributed session instead of InProc HttpSessionState.
            int did = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            // cr-dotnet-0045 (Line 28): Read "appointid" from Redis-backed
            // distributed session instead of InProc HttpSessionState.
            int appid = HttpContext.Session.GetInt32("appointid") ?? 0;

            int found = objmyDAL.update_prescription_DAL(did, appid, Disease, Progress, Prescription);

            if (found != 1)
            {
                IsError = true;
                StatusMessage = "There was some error";
                return Page();
            }

            StatusMessage = "Information Successfully Updated";
            return Page();
        }

        /// <summary>
        /// Replaces generate_bill event handler.
        /// Redirects to the Bill Razor Page.
        /// </summary>
        public IActionResult OnPostGenerateBill()
        {
            return RedirectToPage("/Doctor/Bill");
        }
    }
}
