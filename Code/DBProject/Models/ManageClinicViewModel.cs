// ViewModel for the Manage Clinic page.
// Replaces the Web Forms code-behind data binding for ManageClinic.aspx (cr-dotnet-0026).

using System.Data;

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the ManageClinic Razor View.
    /// Replaces the direct Web Forms server-control data binding
    /// (asp:RadioButton, asp:TextBox, asp:GridView, asp:Label, HtmlGenericControl).
    /// </summary>
    public class ManageClinicViewModel
    {
        /// <summary>Currently selected category: "DOCTOR", "PATIENT", or "OTHERSTAFF".</summary>
        public string Category { get; set; } = "DOCTOR";

        /// <summary>Current search query text.</summary>
        public string SearchQuery { get; set; } = "";

        /// <summary>Data table bound to the grid (replaces asp:GridView Manage).</summary>
        public DataTable GridData { get; set; }

        /// <summary>Status / error message (replaces asp:Label Msg).</summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// HTML fragment for the selected record detail panel
        /// (replaces HtmlGenericControl mydiv.InnerHtml).
        /// </summary>
        public string SelectedRecord { get; set; }
    }
}
