// ViewModel for the Pending Appointments page.
// Replaces the Web Forms code-behind data binding for PendingAppointment.aspx (cr-dotnet-0026).

using System.Data;

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the PendingAppointment Razor View.
    /// Replaces the direct Web Forms server-control data binding
    /// (asp:GridView ID="pendingappointments" with DataSource/DataBind,
    ///  OnRowCommand="update_appointment", OnRowDeleting="Delete_appointment").
    /// </summary>
    public class PendingAppointmentViewModel
    {
        /// <summary>
        /// DataTable containing pending appointments for the logged-in doctor.
        /// Replaces pendingappointments.DataSource = DT; pendingappointments.DataBind();
        /// </summary>
        public DataTable Appointments { get; set; }

        /// <summary>Error message displayed when data loading fails.</summary>
        public string ErrorMessage { get; set; }
    }
}
