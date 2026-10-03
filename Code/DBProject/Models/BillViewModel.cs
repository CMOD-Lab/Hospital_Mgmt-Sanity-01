// ViewModel for the Bill (Generate Bill) page.
// Replaces the Web Forms code-behind data binding for Bill.aspx (cr-dotnet-0026).

namespace DBProject.Models
{
    /// <summary>
    /// View model that carries all data required by the Bill Razor View.
    /// Replaces the direct Web Forms server-control data binding
    /// (asp:Label Label1, asp:Button Bill, asp:Button Button1).
    /// </summary>
    public class BillViewModel
    {
        /// <summary>
        /// The bill amount for the current appointment
        /// (replaces asp:Label Label1 bound to dt.Rows[0][0]).
        /// </summary>
        public string BillAmount { get; set; } = "0";

        /// <summary>Error message displayed when bill generation fails.</summary>
        public string ErrorMessage { get; set; }
    }
}
