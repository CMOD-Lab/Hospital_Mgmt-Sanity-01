// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
//
// Changes applied:
//   - removed: global::System.Web.UI.WebControls.Label Notify
//   - removed: global::System.Web.UI.WebControls.Label NDoctor
//   - removed: global::System.Web.UI.WebControls.Label NTimings
//
// These Web Forms server-control field declarations are no longer required.
// The corresponding UI output is now handled by bound properties on
// PatientNotificationsModel (NotifyMessage, NDoctorMessage, NTimingsMessage)
// rendered directly in the PatientNotifications.aspx Razor Page view.
// This file is retained as a placeholder to preserve project file references
// during the migration transition.

namespace DBProject.Pages.Patient
{
    // Placeholder: Web Forms designer file superseded by Razor PageModel.
    // All server-control declarations have been removed.
}
