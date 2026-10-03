// ViewModel for the Appointment Request Sent page.
// Replaces the Web Forms code-behind data binding for AppointmentRequestSent.aspx (cr-dotnet-0026).

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the AppointmentRequestSent Razor View.
    /// Replaces the direct Web Forms server-control data binding
    /// (asp:Label ID="Message" for status messages and
    ///  asp:Button OnClick="sendARequest" for form submission).
    /// </summary>
    public class AppointmentRequestSentViewModel
    {
        /// <summary>
        /// Status message displayed after attempting to send an appointment request.
        /// Replaces asp:Label ID="Message" used for result text.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Indicates whether the request was submitted (POST) and a message is available.
        /// </summary>
        public bool RequestSubmitted { get; set; }
    }
}
