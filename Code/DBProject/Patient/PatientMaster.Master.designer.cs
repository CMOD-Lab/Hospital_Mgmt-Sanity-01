// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
//
// Changes applied:
//   - removed: global::System.Web.UI.WebControls.ContentPlaceHolder head
//   - removed: global::System.Web.UI.HtmlControls.HtmlForm form1
//   - removed: global::System.Web.UI.WebControls.ContentPlaceHolder ContentPlaceHolder1
//
// These Web Forms server-control field declarations are no longer required.
// The Master Page has been superseded by the ASP.NET Core Razor Layout file
// _PatientLayout.cshtml in the same folder.
// ContentPlaceHolder controls are replaced by @RenderBody() and
// @RenderSection() in the Razor Layout.
// This file is retained as a placeholder to preserve project file references
// during the migration transition.

namespace DBProject.Pages.Patient
{
    // Placeholder: Web Forms Master Page designer file superseded by _PatientLayout.cshtml.
    // All server-control declarations have been removed.
}
