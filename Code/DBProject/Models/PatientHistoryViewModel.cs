// ViewModel for the Patient History page.
// Replaces the Web Forms code-behind data binding for PatientHistory.aspx (cr-dotnet-0026).

using System.Data;

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the PatientHistory Razor View.
    /// Replaces the direct Web Forms server-control data binding
    /// (asp:GridView ID="patientsgrid" with DataSource/DataBind and OnRowCommand).
    /// </summary>
    public class PatientHistoryViewModel
    {
        /// <summary>
        /// DataTable containing today's appointments for the logged-in doctor.
        /// Replaces patientsgrid.DataSource = dt; patientsgrid.DataBind();
        /// </summary>
        public DataTable Patients { get; set; }

        /// <summary>Error message displayed when data loading fails.</summary>
        public string ErrorMessage { get; set; }
    }
}
