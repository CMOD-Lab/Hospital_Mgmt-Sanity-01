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
    public class PatientHomeModel : PageModel
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientHomeModel(IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            _distributedCache = distributedCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public string PName { get; set; }
        public string PPhone { get; set; }
        public string PBirthDate { get; set; }
        public string PatientAge { get; set; }
        public string PGender { get; set; }
        public string PAddress { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            PatientInfo();
        }

        //-----------------------Function1--------------------------//

        private void PatientInfo()
        {
            // cr-dotnet-0045: Use Redis-backed distributed session (ElastiCache)
            // instead of in-process HttpSessionState to support horizontal scaling.
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            myDAL objmyDAl = new myDAL();

            int pid = session.GetInt32("idoriginal") ?? 0;

            string name      = "";
            string phone     = "";
            string address   = "";
            string birthDate = "";
            int    age       = 0;
            string gender    = "";

            int status = objmyDAl.patientInfoDisplayer(pid, ref name, ref phone, ref address, ref birthDate, ref age, ref gender);

            if (status == -1)
            {
                ErrorMessage = "There was some error in retrieving the Patient's Info.";
            }
            else if (status == 0)
            {
                PName      = name;
                PPhone     = phone;
                PBirthDate = birthDate;
                PatientAge = age.ToString();
                PAddress   = address;
                PGender    = gender;
            }
        }

        //-----------------------Add a new function here------------------//
    }
}
