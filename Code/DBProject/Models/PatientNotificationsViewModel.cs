// ViewModel for the Patient Notifications page.
// Replaces the Web Forms code-behind data binding for PatientNotifications.aspx (cr-dotnet-0026).

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the PatientNotifications Razor View.
    /// Replaces the direct Web Forms server-control data binding
    /// (asp:Label ID="Notify", "NDoctor", "NTimings").
    /// </summary>
    public class PatientNotificationsViewModel
    {
        /// <summary>
        /// General notification status message (e.g., error or "no new notifications").
        /// Replaces asp:Label ID="Notify" Font-Bold="true" Font-Size="Medium".
        /// </summary>
        public string NotifyMessage { get; set; }

        /// <summary>
        /// Doctor-specific notification message (appointment accepted/rejected/completed).
        /// Replaces asp:Label ID="NDoctor" Font-Bold="true" Font-Size="Medium".
        /// </summary>
        public string DoctorMessage { get; set; }

        /// <summary>
        /// Appointment timings message.
        /// Replaces asp:Label ID="NTimings" Font-Bold="true" Font-Size="Medium".
        /// </summary>
        public string TimingsMessage { get; set; }
    }
}
