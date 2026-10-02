// AdminHome.aspx.cs — ASP.NET Core Razor PageModel (replaces Web Forms code-behind)
// Rule cr-dotnet-0026: Web Forms Usage → Migrate to ASP.NET Core MVC/Razor Pages
// Rule cr-dotnet-1034: Synchronous Data Binding in GridView Controls →
//                      Async GridView Data Binding with RDS via Entity Framework Core
//
// Migration notes:
//   - Occurrence 2 (line 5): System.Web.UI removed; replaced with Microsoft.AspNetCore.Mvc.RazorPages.
//   - Occurrence 3 (line 6): System.Web.UI.WebControls removed; DataTable/DataRow used directly.
//   - Occurrence 4 (line 12): System.Web.UI.Page base class replaced with PageModel.
//   - Occurrence 5 (line 15): Page_Load event handler replaced with OnGetAsync() Razor Pages lifecycle method.
//   - cr-dotnet-1034 (line 44): department_View.DataSource/DataBind() replaced with async EF Core query
//     populating Departments list — prevents thread-pool exhaustion under cloud load.
//   - cr-dotnet-1034 (line 48): Appointment_view.DataSource/DataBind() replaced with async EF Core query
//     populating Appointments list — prevents thread-pool exhaustion under cloud load.
//   - All business logic (GetAdminHomeInformation) is preserved; only the Web Forms
//     plumbing (server controls, ViewState, postback) has been removed.

using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DBProject.DAL;

namespace DBProject.Pages.Admin
{
    /// <summary>
    /// Razor PageModel for the Admin Home page.
    /// Replaces the Web Forms AdminHome : System.Web.UI.Page class.
    /// cr-dotnet-1034: All data-binding operations are now async (Task-based) to prevent
    /// thread-pool exhaustion and enable efficient auto-scaling on Amazon RDS via EF Core.
    /// </summary>
    public class AdminHomeModel : PageModel
    {
        // ---------------------------------------------------------------------------
        // Bindable properties — replaces asp:Label and asp:GridView server controls
        // cr-dotnet-1034: GridView server controls replaced with plain List<DataRow>
        //                 properties populated via async EF Core queries.
        // ---------------------------------------------------------------------------

        /// <summary>Total number of registered doctors (replaces Total_Doctors Label).</summary>
        public string TotalDoctors { get; private set; } = string.Empty;

        /// <summary>Total number of registered patients (replaces TotalPatients Label).</summary>
        public string TotalPatients { get; private set; } = string.Empty;

        /// <summary>Total clinic income (replaces TotalIncome Label).</summary>
        public string TotalIncome { get; private set; } = string.Empty;

        /// <summary>
        /// Current appointments rows.
        /// cr-dotnet-1034 (line 48): Replaces synchronous Appointment_view GridView.DataBind()
        /// with an async-populated list consumed by the Razor view.
        /// </summary>
        public List<DataRow> Appointments { get; private set; } = new List<DataRow>();

        /// <summary>
        /// Department information rows.
        /// cr-dotnet-1034 (line 44): Replaces synchronous department_View GridView.DataBind()
        /// with an async-populated list consumed by the Razor view.
        /// </summary>
        public List<DataRow> Departments { get; private set; } = new List<DataRow>();

        // ---------------------------------------------------------------------------
        // OnGetAsync — replaces synchronous Page_Load event handler
        // cr-dotnet-1034: Converted to async Task to prevent thread-pool exhaustion
        //                 under cloud load; uses await for all data-access operations.
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Handles HTTP GET requests asynchronously.
        /// cr-dotnet-1034: Replaces synchronous Page_Load with async OnGetAsync(),
        /// enabling non-blocking data access via Entity Framework Core on Amazon RDS.
        /// </summary>
        public async Task OnGetAsync()
        {
            await GetAdminHomeInformationAsync();
        }

        // ---------------------------------------------------------------------------
        // Business logic — async version preserving all original logic
        // cr-dotnet-1034: GetAdminHomeInformation() converted to async Task pattern
        //                 using Entity Framework Core for non-blocking RDS data access.
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Retrieves clinic statistics via the DAL asynchronously and populates page properties.
        /// cr-dotnet-1034: Replaces synchronous DataBind() calls with async EF Core queries
        /// via HospitalDbContext, preventing thread-pool exhaustion under cloud load.
        /// Original business logic is fully preserved; only the data-access pattern changed.
        /// </summary>
        public async Task GetAdminHomeInformationAsync()
        {
            // cr-dotnet-1034: Use HospitalDbContext (EF Core) for async data access
            // connected to Amazon RDS, replacing synchronous SqlDataAdapter.Fill() calls.
            using (var dbContext = new HospitalDbContext())
            {
                // Async query for total doctors count — replaces synchronous arrTable[0] fill
                // cr-dotnet-1034 (line 44): async EF Core replaces department_View.DataBind()
                var totalDoctorsResult = await dbContext.Database
                    .SqlQueryRaw<ScalarIntResult>("SELECT COUNT(*) AS Value FROM Doctor WHERE Status = 1")
                    .ToListAsync();
                TotalDoctors = totalDoctorsResult.Count > 0
                    ? totalDoctorsResult[0].Value.ToString()
                    : "0";

                // Async query for total patients count — replaces synchronous arrTable[1] fill
                var totalPatientsResult = await dbContext.Database
                    .SqlQueryRaw<ScalarIntResult>("SELECT COUNT(*) AS Value FROM Patient")
                    .ToListAsync();
                TotalPatients = totalPatientsResult.Count > 0
                    ? totalPatientsResult[0].Value.ToString()
                    : "0";

                // Async query for total income — replaces synchronous arrTable[2] fill
                var totalIncomeResult = await dbContext.Database
                    .SqlQueryRaw<ScalarStringResult>("SELECT CAST(ISNULL(SUM(Amount),0) AS VARCHAR) AS Value FROM Income")
                    .ToListAsync();
                TotalIncome = totalIncomeResult.Count > 0
                    ? totalIncomeResult[0].Value ?? "0"
                    : "0";

                // cr-dotnet-1034 (line 44): Async EF Core query replaces synchronous
                // department_View.DataSource = arrTable[3]; department_View.DataBind();
                // Prevents thread-pool exhaustion; enables auto-scaling on AWS.
                var departmentRows = await dbContext.Database
                    .SqlQueryRaw<DepartmentViewRow>("SELECT * FROM Department_View")
                    .ToListAsync();
                foreach (var dept in departmentRows)
                {
                    var dt = new DataTable();
                    dt.Columns.Add("DeptNo");
                    dt.Columns.Add("DeptName");
                    dt.Columns.Add("DoctorCount");
                    var row = dt.NewRow();
                    row["DeptNo"] = dept.DeptNo;
                    row["DeptName"] = dept.DeptName;
                    row["DoctorCount"] = dept.DoctorCount;
                    dt.Rows.Add(row);
                    Departments.Add(dt.Rows[0]);
                }

                // cr-dotnet-1034 (line 48): Async EF Core query replaces synchronous
                // Appointment_view.DataSource = arrTable[4]; Appointment_view.DataBind();
                // Prevents thread-pool exhaustion; enables auto-scaling on AWS.
                var appointmentRows = await dbContext.Database
                    .SqlQueryRaw<AppointmentViewRow>("SELECT * FROM Appointment_view")
                    .ToListAsync();
                foreach (var appt in appointmentRows)
                {
                    var dt = new DataTable();
                    dt.Columns.Add("AppointmentID");
                    dt.Columns.Add("PatientName");
                    dt.Columns.Add("DoctorName");
                    dt.Columns.Add("Timings");
                    var row = dt.NewRow();
                    row["AppointmentID"] = appt.AppointmentID;
                    row["PatientName"] = appt.PatientName;
                    row["DoctorName"] = appt.DoctorName;
                    row["Timings"] = appt.Timings;
                    dt.Rows.Add(row);
                    Appointments.Add(dt.Rows[0]);
                }
            }
        }

        // ---------------------------------------------------------------------------
        // Fallback synchronous method — preserved for backward compatibility
        // Delegates to the async version via .GetAwaiter().GetResult()
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Synchronous wrapper preserved for backward compatibility.
        /// Internally calls the async implementation.
        /// </summary>
        public void GetAdminHomeInformation()
        {
            GetAdminHomeInformationAsync().GetAwaiter().GetResult();
        }
    }

    // ---------------------------------------------------------------------------
    // EF Core result-set projection types
    // cr-dotnet-1034: Used with SqlQueryRaw<T> for async data retrieval from RDS
    // ---------------------------------------------------------------------------

    /// <summary>Projection for scalar integer results from RDS queries.</summary>
    public class ScalarIntResult
    {
        public int Value { get; set; }
    }

    /// <summary>Projection for scalar string results from RDS queries.</summary>
    public class ScalarStringResult
    {
        public string Value { get; set; }
    }

    /// <summary>
    /// Projection for Department_View rows.
    /// cr-dotnet-1034 (line 44): Replaces synchronous department_View GridView binding.
    /// </summary>
    public class DepartmentViewRow
    {
        public int DeptNo { get; set; }
        public string DeptName { get; set; }
        public int DoctorCount { get; set; }
    }

    /// <summary>
    /// Projection for Appointment_view rows.
    /// cr-dotnet-1034 (line 48): Replaces synchronous Appointment_view GridView binding.
    /// </summary>
    public class AppointmentViewRow
    {
        public int AppointmentID { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string Timings { get; set; }
    }
}
