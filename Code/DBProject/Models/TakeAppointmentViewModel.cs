// ViewModel for the Take Appointment (Department Selection) page.
// Replaces the Web Forms code-behind data binding for TakeAppointment.aspx (cr-dotnet-0026).

using System.Data;

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the TakeAppointment Razor View.
    /// Replaces the direct Web Forms server-control data binding
    /// (asp:GridView ID="TDeptGrid" with DataSource/DataBind and
    ///  asp:Label ID="TDept" for status messages).
    /// </summary>
    public class TakeAppointmentViewModel
    {
        /// <summary>
        /// Status or error message displayed when loading department information.
        /// Replaces asp:Label ID="TDept" used for status text.
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// DataTable containing the department information.
        /// Replaces TDeptGrid.DataSource = DT; TDeptGrid.DataBind();
        /// </summary>
        public DataTable DeptData { get; set; }
    }
}
