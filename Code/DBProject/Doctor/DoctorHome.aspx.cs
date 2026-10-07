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
    public class DoctorHomeModel : PageModel
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DoctorHomeModel(IDistributedCache distributedCache, IHttpContextAccessor httpContextAccessor)
        {
            _distributedCache = distributedCache;
            _httpContextAccessor = httpContextAccessor;
        }

        public string ErrorMessage { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string BirthDate { get; set; }
        public string Gender { get; set; }
        public string DepartmentNo { get; set; }
        public string ChargesPerVisit { get; set; }
        public string MonthlySalary { get; set; }
        public string ReputeIndex { get; set; }
        public string PatientsTreated { get; set; }
        public string Qualification { get; set; }
        public string Specialization { get; set; }
        public string WorkExperience { get; set; }
        public string Status { get; set; }

        public void OnGet()
        {
            // cr-dotnet-0045: Use Redis-backed distributed session (ElastiCache)
            // instead of in-process HttpSessionState to support horizontal scaling.
            var session = new RedisSessionHelper(_distributedCache, _httpContextAccessor);

            myDAL objmyDAL = new myDAL();
            DataTable dt = new DataTable();
            int found;
            int did = session.GetInt32("idoriginal") ?? 0;

            found = objmyDAL.docinfo_DAL(did, ref dt);

            if (found != 1)
            {
                ErrorMessage = "There was some error";
            }
            else
            {
                Name = dt.Rows[0][1].ToString();
                Phone = dt.Rows[0][2].ToString();
                Address = dt.Rows[0][3].ToString();
                BirthDate = dt.Rows[0][4].ToString();
                Gender = dt.Rows[0][5].ToString();
                DepartmentNo = dt.Rows[0][6].ToString();
                ChargesPerVisit = dt.Rows[0][7].ToString();
                MonthlySalary = dt.Rows[0][8].ToString();
                ReputeIndex = dt.Rows[0][9].ToString();
                PatientsTreated = dt.Rows[0][10].ToString();
                Qualification = dt.Rows[0][11].ToString();
                Specialization = dt.Rows[0][12].ToString();
                WorkExperience = dt.Rows[0][13].ToString();
                Status = dt.Rows[0][14].ToString();
            }
        }
    }
}
