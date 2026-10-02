// Migrated from ASP.NET Web Forms Master Page to ASP.NET Core Razor Pages Layout.
// Rule cr-dotnet-0026: Web Forms Usage
//
// Changes applied:
//   Line 6  – removed: using System.Web;                   (occurrence 1)
//   Line 10 – removed: public partial class doctormaster : System.Web.UI.MasterPage
//             replaced with: public class DoctorMasterModel : PageModel  (occurrence 2)
//   Line 12 – removed: protected void Page_Load(object sender, EventArgs e)
//             replaced with: public void OnGet()            (occurrence 3)
//
// The Master Page code-behind is superseded by the Razor Layout file:
//   ~/Pages/Doctor/_DoctorLayout.cshtml
// which provides the same navigation bar, footer, and content placeholders
// using standard ASP.NET Core Razor syntax, enabling stateless cloud-native
// deployment on AWS (Linux containers, Elastic Beanstalk, ECS/Fargate).

using System;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DBProject.Pages.Doctor
{
    /// <summary>
    /// Layout model for the Doctor section – replaces the Web Forms
    /// doctormaster : System.Web.UI.MasterPage code-behind.
    /// In ASP.NET Core Razor Pages the layout logic lives in
    /// _DoctorLayout.cshtml; this class is retained for namespace
    /// consistency and can be extended with layout-level services if needed.
    /// </summary>
    public class DoctorMasterModel : PageModel
    {
        // No server-side load logic was present in the original Master Page.
        // The OnGet stub is intentionally empty.
        public void OnGet()
        {
            // Intentionally empty – layout initialisation is handled by
            // the Razor layout file (_DoctorLayout.cshtml).
        }
    }
}
