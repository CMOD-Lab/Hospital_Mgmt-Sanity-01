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

namespace DBProject.Patient
{
    public class CurrentAppointmentModel : PageModel
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentAppointmentModel(IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            _distributedCache = distributedCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public string AppointmentStatus { get; set; }
        public string DoctorInfo { get; set; }
        public string TimingInfo { get; set; }

        public void OnGet()
        {
            LoadAppointmentToday();
        }

        private void LoadAppointmentToday()
        {
            // cr-dotnet-0045: Use Redis-backed distributed session (ElastiCache)
            // instead of in-process HttpSessionState to support horizontal scaling.
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            myDAL objmyDAl = new myDAL();

            int pid = session.GetInt32("idoriginal") ?? 0;

            string dName = "";
            string timings = "";

            int status = objmyDAl.appointmentTodayDisplayer(pid, ref dName, ref timings);

            if (status == -1)
            {
                AppointmentStatus = "There was some error in retrieving the Patient's appointment.";
            }
            else if (status == 0)
            {
                AppointmentStatus = "You have no appointment today with any doctor.";
            }
            else
            {
                if (status == 3)
                {
                    DoctorInfo = "You had an outdated appointment with Doctor " + dName + " to which he didn't respond. So that appointment is discarded.";
                    TimingInfo = "The Appointment Timings were : " + timings;
                    return;
                }
                else if (status == 2)
                {
                    DoctorInfo = "You have sent an appointment request to Doctor " + dName + " which isn't approved by him yet.";
                }
                else
                {
                    DoctorInfo = "Today you have an appointment with Doctor " + dName;
                }

                TimingInfo = "The Appointment Timings are : " + timings;
            }
        }
    }
}
