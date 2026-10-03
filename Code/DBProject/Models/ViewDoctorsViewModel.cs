// ViewModel for the View Doctors page.
// Replaces the Web Forms code-behind data binding for ViewDoctors.aspx (cr-dotnet-0026).

using System.Data;

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the ViewDoctors Razor View.
    /// Replaces the direct Web Forms server-control data binding
    /// (asp:GridView ID="TDoctorGrid" with DataSource/DataBind, AutoGenerateSelectButton,
    ///  OnRowCommand="TDoctorGrid_RowCommand" and asp:Label ID="TDoctor" for status messages).
    /// </summary>
    public class ViewDoctorsViewModel
    {
        /// <summary>
        /// Status or error message displayed when loading doctor information.
        /// Replaces asp:Label ID="TDoctor" used for status text.
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// DataTable containing the doctors for the selected department.
        /// Replaces TDoctorGrid.DataSource = DT; TDoctorGrid.DataBind();
        /// </summary>
        public DataTable DoctorData { get; set; }

        /// <summary>
        /// The currently selected department name, read from session.
        /// Replaces Session["deptOriginal"] used in the status message.
        /// </summary>
        public string DepartmentName { get; set; }
    }
}
