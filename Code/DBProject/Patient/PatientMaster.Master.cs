// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
//
// Changes applied:
//   - removed: using System.Web;
//   - removed: using System.Web.UI;
//   - removed: using System.Web.UI.WebControls;
//   - removed: public partial class PatientMaster : System.Web.UI.MasterPage
//   - removed: protected void Page_Load(object sender, EventArgs e)
//
// The PatientMaster Master Page has been superseded by the ASP.NET Core
// Razor Layout file _PatientLayout.cshtml in the same folder.
// This code-behind is retained as a placeholder to preserve project
// file references during the migration transition.
// All layout logic now lives in _PatientLayout.cshtml.

namespace DBProject.Pages.Patient
{
    // Placeholder: Master Page code-behind replaced by _PatientLayout.cshtml
    // No Web Forms MasterPage base class or Page_Load event handler required
    // in ASP.NET Core Razor Pages.
}
