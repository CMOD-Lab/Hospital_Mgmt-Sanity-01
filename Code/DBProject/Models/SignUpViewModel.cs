// ViewModel for the Sign Up / Login page.
// Replaces the Web Forms code-behind data binding for SignUp.aspx (cr-dotnet-0026).

using System.ComponentModel.DataAnnotations;

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the SignUp Razor View.
    /// Replaces the direct Web Forms server-control data binding
    /// (asp:TextBox controls for loginEmail, loginPassword, sName, sBirthDate,
    ///  sEmail, sPassword, scPassword, Phone, Address and asp:button controls
    ///  for loginV and signupV event handlers).
    /// </summary>
    public class SignUpViewModel
    {
        // -----------------------------------------------------------------------
        // Login section – replaces asp:TextBox ID="loginEmail" and
        // asp:TextBox ID="loginPassword" in SignUp.aspx
        // -----------------------------------------------------------------------

        /// <summary>
        /// Login email address.
        /// Replaces asp:TextBox ID="loginEmail" in SignUp.aspx.
        /// </summary>
        [EmailAddress]
        public string LoginEmail { get; set; }

        /// <summary>
        /// Login password.
        /// Replaces asp:TextBox ID="loginPassword" in SignUp.aspx.
        /// </summary>
        public string LoginPassword { get; set; }

        // -----------------------------------------------------------------------
        // Registration section – replaces asp:TextBox controls in SignUp.aspx
        // -----------------------------------------------------------------------

        /// <summary>
        /// Patient full name.
        /// Replaces asp:TextBox ID="sName" in SignUp.aspx.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Patient birth date (dd-mm-yyyy format).
        /// Replaces asp:TextBox ID="sBirthDate" in SignUp.aspx.
        /// </summary>
        public string BirthDate { get; set; }

        /// <summary>
        /// Patient email address.
        /// Replaces asp:TextBox ID="sEmail" in SignUp.aspx.
        /// </summary>
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Patient password.
        /// Replaces asp:TextBox ID="sPassword" in SignUp.aspx.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Confirm password (client-side validation only).
        /// Replaces asp:TextBox ID="scPassword" in SignUp.aspx.
        /// </summary>
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Patient phone number (11 digits).
        /// Replaces asp:TextBox ID="Phone" in SignUp.aspx.
        /// </summary>
        public string PhoneNo { get; set; }

        /// <summary>
        /// Patient gender ("M" or "F").
        /// Replaces radio button name="Gender" in SignUp.aspx.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Patient address.
        /// Replaces asp:TextBox ID="Address" TextMode="multiline" in SignUp.aspx.
        /// </summary>
        public string Address { get; set; }

        // -----------------------------------------------------------------------
        // Feedback / error messages – replaces Response.Write("<script>alert(...)...")
        // -----------------------------------------------------------------------

        /// <summary>
        /// Error or status message to display to the user.
        /// Replaces Response.Write("&lt;script&gt;alert(...)...&lt;/script&gt;") in SignUp.aspx.cs.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
