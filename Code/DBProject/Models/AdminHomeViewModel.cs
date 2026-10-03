// ViewModel for the Admin Home page.
// Replaces the Web Forms code-behind data binding for AdminHome.aspx (cr-dotnet-0026).

using System.Data;

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the Admin Home Razor View.
    /// Replaces the direct Web Forms server-control data binding (asp:Label, asp:GridView).
    /// </summary>
    public class AdminHomeViewModel
    {
        /// <summary>Total number of registered doctors.</summary>
        public string TotalDoctors { get; set; } = "0";

        /// <summary>Total number of registered patients.</summary>
        public string TotalPatients { get; set; } = "0";

        /// <summary>Total clinic income.</summary>
        public string TotalIncome { get; set; } = "0";

        /// <summary>Department information table (replaces asp:GridView department_View).</summary>
        public DataTable Departments { get; set; }

        /// <summary>Current appointments table (replaces asp:GridView Appointment_view).</summary>
        public DataTable Appointments { get; set; }
    }
}
