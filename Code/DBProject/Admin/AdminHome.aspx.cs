using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

// Migrated from ASP.NET Web Forms (System.Web.UI.Page) to ASP.NET Core Razor Pages (PageModel)
// Rule cr-dotnet-0026: Web Forms Usage - Migrate to ASP.NET Core MVC/Razor Pages
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls
//   Replaced synchronous GridView.DataBind() with async Task-based data loading via
//   OnGetAsync() and myDAL async methods connected to Amazon RDS, preventing thread pool
//   exhaustion under load and enabling efficient auto-scaling in cloud deployments.

namespace DBProject.Admin
{
    public class AdminHomeModel : PageModel
    {
        public string TotalDoctors { get; set; }
        public string TotalPatients { get; set; }
        public string TotalIncome { get; set; }
        public DataTable DepartmentData { get; set; }
        public DataTable AppointmentData { get; set; }

        // cr-dotnet-1034: Changed from synchronous OnGet() to async OnGetAsync()
        // to prevent thread pool exhaustion under load in cloud (AWS RDS) deployments.
        public async Task OnGetAsync()
        {
            await GetAdminHomeInformationAsync();
        }

        // cr-dotnet-1034: Async data loading replaces synchronous GridView.DataBind()
        // pattern. Data is fetched via Task-based API from Amazon RDS, allowing the
        // request thread to be released while awaiting I/O completion.
        public async Task GetAdminHomeInformationAsync()
        {
            myDAL objmyDAL = new myDAL();

            DataTable[] arrTable = new DataTable[5];
            for (int i = 0; i < 5; i++)
            {
                arrTable[i] = new DataTable();
            }

            await objmyDAL.GetAdminHomeInformationAsync(arrTable);

            TotalDoctors = arrTable[1].Rows.Count > 0 ? arrTable[1].Rows[0][0].ToString() : "0";
            TotalPatients = arrTable[0].Rows.Count > 0 ? arrTable[0].Rows[0][0].ToString() : "0";
            TotalIncome = arrTable[2].Rows.Count > 0 ? arrTable[2].Rows[0][0].ToString() : "0";

            DepartmentData = arrTable[3];
            AppointmentData = arrTable[4];
        }
    }
}
