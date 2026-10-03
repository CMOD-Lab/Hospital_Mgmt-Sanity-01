// ViewModel for the Appointment Taker (Free Slots) page.
// Replaces the Web Forms code-behind data binding for AppointmentTaker.aspx (cr-dotnet-0026).

using System.Data;

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the AppointmentTaker Razor View.
    /// Replaces the direct Web Forms server-control data binding
    /// (asp:GridView ID="PAppointmentGrid" with DataSource/DataBind and
    ///  asp:Label ID="PAppointment" for status messages).
    /// </summary>
    public class AppointmentTakerViewModel
    {
        /// <summary>
        /// Status or error message displayed when loading free slots.
        /// Replaces asp:Label ID="PAppointment" used for status text.
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// DataTable containing the free time slots for the selected doctor.
        /// Replaces PAppointmentGrid.DataSource = DT; PAppointmentGrid.DataBind();
        /// </summary>
        public DataTable FreeSlotsData { get; set; }

        /// <summary>
        /// Number of free slots returned by the DAL.
        /// Used to display the count in the status message.
        /// </summary>
        public int SlotCount { get; set; }
    }
}
