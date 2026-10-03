// ViewModel for the Patient Feedback page.
// Replaces the Web Forms code-behind data binding for PatientFeedback.aspx (cr-dotnet-0026).

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the PatientFeedback Razor View.
    /// Replaces the direct Web Forms server-control data binding
    /// (asp:Label ID="Feedback", "FDoctor", "FTimings", "Message", "F",
    ///  asp:DropDownList ID="List", asp:Button ID="button1").
    /// </summary>
    public class PatientFeedbackViewModel
    {
        /// <summary>
        /// General feedback status message (e.g., error or "no pending feedbacks").
        /// Replaces asp:Label ID="Feedback".
        /// </summary>
        public string FeedbackMessage { get; set; }

        /// <summary>
        /// Doctor-related feedback message (e.g., "Your feedback for Doctor X is pending").
        /// Replaces asp:Label ID="FDoctor".
        /// </summary>
        public string DoctorMessage { get; set; }

        /// <summary>
        /// Appointment timings message.
        /// Replaces asp:Label ID="FTimings".
        /// </summary>
        public string TimingsMessage { get; set; }

        /// <summary>
        /// Controls visibility of the rating prompt, dropdown, and submit button.
        /// Replaces Message.Visible, List.Visible, and button1.Visible = true.
        /// </summary>
        public bool ShowRatingPrompt { get; set; }

        /// <summary>
        /// Confirmation message after feedback is submitted.
        /// Replaces asp:Label ID="F".
        /// </summary>
        public string ConfirmationMessage { get; set; }
    }
}
