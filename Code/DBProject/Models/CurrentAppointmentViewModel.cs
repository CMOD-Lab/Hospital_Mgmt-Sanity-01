// ViewModel for the Current Appointment page.
// Replaces the Web Forms code-behind data binding for CurrentAppointment.aspx (cr-dotnet-0026).

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the CurrentAppointment Razor View.
    /// Replaces the direct Web Forms server-control data binding
    /// (asp:Label ID="Appointment", asp:Label ID="ADoctor", asp:Label ID="ATimings").
    /// </summary>
    public class CurrentAppointmentViewModel
    {
        /// <summary>
        /// General appointment status message (e.g., error or "no appointment today").
        /// Replaces asp:Label ID="Appointment" used for status text.
        /// </summary>
        public string AppointmentMessage { get; set; }

        /// <summary>
        /// Doctor-related appointment message (e.g., appointment with Doctor X).
        /// Replaces asp:Label ID="ADoctor" used for doctor info text.
        /// </summary>
        public string DoctorMessage { get; set; }

        /// <summary>
        /// Appointment timings message.
        /// Replaces asp:Label ID="ATimings" used for timing info text.
        /// </summary>
        public string TimingsMessage { get; set; }
    }
}
