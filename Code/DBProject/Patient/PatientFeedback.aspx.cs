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
// Rule cr-dotnet-0126: Heavy Coupling to Stateful Middleware
//   Replaced IIS in-process HttpSessionState (sticky-session) with Amazon ElastiCache for Redis
//   via RedisSessionHelper (IDistributedCache) to enable stateless horizontal scaling.
//   Session data now persists across pod restarts and scales horizontally without sticky routing.

namespace DBProject.Patient
{
    public class PatientFeedbackModel : PageModel
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientFeedbackModel(IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            _distributedCache = distributedCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public string FeedbackStatus { get; set; }
        public string FDoctorInfo { get; set; }
        public string FTimings { get; set; }
        public bool ShowFeedbackForm { get; set; }
        public string FeedbackResult { get; set; }

        [BindProperty]
        public int SelectedRating { get; set; }

        public void OnGet()
        {
            // cr-dotnet-0126: Use Redis-backed distributed session (Amazon ElastiCache)
            // instead of IIS in-process HttpSessionState to support horizontal scaling.
            // Original: Session["aID"] = "";
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            session.SetString("aID", "");
            LoadPendingFeedback(session);
        }

        public IActionResult OnPostGiveFeedback()
        {
            // cr-dotnet-0126: Use Redis-backed distributed session (Amazon ElastiCache)
            // instead of IIS in-process HttpSessionState to support horizontal scaling.
            // Original: int aID = (int)Session["aID"];
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            myDAL objmyDAl = new myDAL();

            int aID = session.GetInt32("aID") ?? 0;

            int status = objmyDAl.givePendingFeedback(aID);

            if (status == -1)
            {
                FeedbackResult = "There was some error.";
            }
            else if (status == 0)
            {
                FeedbackResult = "Thank you for your feedback :)";
            }

            // Reload pending feedback state after submission
            LoadPendingFeedback(session);
            return Page();
        }

        private void LoadPendingFeedback(RedisSessionHelper session)
        {
            myDAL objmyDAl = new myDAL();

            int pid = session.GetInt32("idoriginal") ?? 0;

            string dName = "";
            string timings = "";
            int aID = 0;

            int status = objmyDAl.isFeedbackPending(pid, ref dName, ref timings, ref aID);

            if (status == -1)
            {
                FeedbackStatus = "There was some error in retrieving the Pending Feedbacks.";
            }
            else if (status == 0)
            {
                FeedbackStatus = "There are no pending feedbacks :)";
            }
            else
            {
                // cr-dotnet-0126: Store aID in Redis-backed distributed session (Amazon ElastiCache)
                // instead of IIS in-process HttpSessionState.
                // Original: Session["aID"] = aID;
                session.SetInt32("aID", aID);

                FDoctorInfo = "Your feedback for the appointment with Doctor " + dName + " is pending. Kindly give it.";
                FTimings = "The Appointment Timings were : " + timings;
                ShowFeedbackForm = true;
            }
        }
    }
}
