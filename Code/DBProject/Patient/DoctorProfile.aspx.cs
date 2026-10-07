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
    public class DoctorProfileModel : PageModel
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DoctorProfileModel(IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            _distributedCache = distributedCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public string DName { get; set; }
        public string DPhone { get; set; }
        public string DQualification { get; set; }
        public string DSpecialization { get; set; }
        public string DWork { get; set; }
        public string DAge { get; set; }
        public string DGender { get; set; }
        public string DDept { get; set; }
        public string DCharges { get; set; }
        public string DRI { get; set; }
        public string DPT { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            LoadDoctorInfo();
        }

        public IActionResult OnPostTakeAppointment()
        {
            return RedirectToPage("/Patient/AppointmentTaker");
        }

        private void LoadDoctorInfo()
        {
            // cr-dotnet-0045: Use Redis-backed distributed session (ElastiCache)
            // instead of in-process HttpSessionState to support horizontal scaling.
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            myDAL objmyDAl = new myDAL();

            string dID1 = session.GetString("dID");
            int dID = Convert.ToInt32(dID1);

            string name = "";
            string phone = "";
            string gender = "";

            float charges_Per_Visit = 0;
            float ReputeIndex = 0;
            int PatientsTreated = 0;
            string qualification = "";
            string specialization = "";
            int workE = 0;
            int age = 0;

            string deptName = session.GetString("deptOriginal");

            int status = objmyDAl.doctorInfoDisplayer(dID, ref name, ref phone, ref gender, ref charges_Per_Visit, ref ReputeIndex, ref PatientsTreated, ref qualification, ref specialization, ref workE, ref age);

            if (status == -1)
            {
                ErrorMessage = "There was some error in retrieving the Doctor's Info.";
            }
            else
            {
                DName = name;
                DPhone = phone;
                DQualification = qualification;
                DSpecialization = specialization;
                DWork = workE.ToString();
                DAge = age.ToString();
                DGender = gender;
                DDept = deptName;
                DCharges = charges_Per_Visit.ToString();
                DRI = ReputeIndex.ToString();
                DPT = PatientsTreated.ToString();
            }
        }
    }
}
