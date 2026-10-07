using System;
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

namespace DBProject.Patient
{
    public class PatientNotificationsModel : PageModel
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientNotificationsModel(IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            _distributedCache = distributedCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public string Notify { get; set; }
        public string NDoctor { get; set; }
        public string NTimings { get; set; }

        public void OnGet()
        {
            Notifications();
        }

        //-----------------------Function1--------------------------//

        private void Notifications()
        {
            // cr-dotnet-0045: Use Redis-backed distributed session (ElastiCache)
            // instead of in-process HttpSessionState to support horizontal scaling.
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            myDAL objmyDAl = new myDAL();

            int pid = session.GetInt32("idoriginal") ?? 0;

            string dName   = "";
            string timings = "";

            int status = objmyDAl.getNotifications(pid, ref dName, ref timings);

            if (status == -1)
            {
                Notify = "There was some error in retrieving the Patient's notifications.";
            }
            else if (status == 0)
            {
                Notify = "There are no new notifications :)";
            }
            else
            {
                if (status == 1)
                {
                    NDoctor  = "Your requested appointment with Doctor " + dName + " has been accepted by him! :)";
                    NTimings = "The Appointment Timings are : " + timings;
                    return;
                }
                else if (status == 2)
                {
                    NDoctor  = "Your requested appointment with Doctor " + dName + " has been rejected by him! :(";
                    NTimings = "The Appointment Timings were : " + timings;
                    return;
                }
                else if (status == 3)
                {
                    NDoctor  = "Your appointment with Doctor " + dName + " has been completed now. We hope you are feeling better now!";
                    NTimings = "The Appointment Timings were : " + timings;
                    return;
                }
            }
        }

        //-----------------------Add a new function here------------------//
    }
}
