// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages.
// Rule cr-dotnet-0026: Web Forms Usage
//
// Changes applied:
//   Line 5  – removed: using System.Web;
//   Line 6  – removed: using System.Web.UI;
//   Line 7  – removed: using System.Web.UI.WebControls;
//   Line 15 – removed: public partial class PatientNotifications : System.Web.UI.Page
//             replaced with: public class PatientNotificationsModel : PageModel
//   Line 17 – removed: protected void Page_Load(object sender, EventArgs e)
//             replaced with: public void OnGet()
//
// Rule cr-dotnet-0045: Session State Provider
//   In-process (InProc) HttpSessionState replaced with Amazon ElastiCache for Redis
//   distributed session store via ASP.NET Core ISession (IDistributedCache-backed).
//   Session["idoriginal"] replaced with HttpContext.Session.GetInt32("idoriginal") ?? 0
//   using the ASP.NET Core ISession extension methods (Microsoft.AspNetCore.Http).
//   The unsafe direct cast (int)HttpContext.Session.GetInt32("idoriginal") has been
//   replaced with the null-safe ?? 0 pattern to prevent NullReferenceException when
//   the session key is absent in stateless cloud deployments.
//   Redis session is registered via AddRedisDistributedSession() in
//   Session/RedisSessionConfiguration.cs, reading REDIS_CONNECTION_STRING from the
//   ECS task definition / Elastic Beanstalk environment / AWS Systems Manager.
//   Enables stateless horizontal scaling across multiple ECS tasks or Kubernetes pods.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBProject.DAL;

namespace DBProject.Pages.Patient
{
    public class PatientNotificationsModel : PageModel
    {
        // Bound properties replace the Web Forms <asp:Label> server controls
        // (Notify, NDoctor, NTimings) that were declared in the designer file.
        public string NotifyMessage { get; private set; } = string.Empty;
        public string NDoctorMessage { get; private set; } = string.Empty;
        public string NTimingsMessage { get; private set; } = string.Empty;

        // Replaces protected void Page_Load(object sender, EventArgs e)
        public void OnGet()
        {
            Notifications();
        }

        //-----------------------Function1--------------------------//

        // Replaces protected void Notifications(object sender, EventArgs e)
        private void Notifications()
        {
            myDAL objmyDAl = new myDAL();

            // cr-dotnet-0045: Replaces (int)Session["idoriginal"]
            // HttpContext.Session is backed by Amazon ElastiCache for Redis via
            // IDistributedCache (registered in Session/RedisSessionConfiguration.cs).
            // The null-coalescing operator ?? 0 replaces the previous unsafe direct cast
            // (int)HttpContext.Session.GetInt32("idoriginal") which would throw a
            // NullReferenceException when the session key is absent in stateless
            // cloud deployments across multiple ECS tasks or Kubernetes pods.
            int pid = HttpContext.Session.GetInt32("idoriginal") ?? 0;

            string dName = "";
            string timings = "";

            int status = objmyDAl.getNotifications(pid, ref dName, ref timings);

            if (status == -1)
            {
                NotifyMessage = "There was some error in retrieving the Patient's notifications.";
            }
            else if (status == 0)
            {
                NotifyMessage = "There are no new notifications :)";
            }
            else
            {
                if (status == 1)
                {
                    NDoctorMessage = "Your requested appointment with Doctor " + dName + " has been accepted by him! :)";
                    NTimingsMessage = "The Appointment Timings are : " + timings;
                    return;
                }
                else if (status == 2)
                {
                    NDoctorMessage = "Your requested appointment with Doctor " + dName + " has been rejected by him! :(";
                    NTimingsMessage = "The Appointment Timings were : " + timings;
                    return;
                }
                else if (status == 3)
                {
                    NDoctorMessage = "Your appointment with Doctor " + dName + " has been completed now. We hope you are feeling better now!";
                    NTimingsMessage = "The Appointment Timings were : " + timings;
                    return;
                }

                return;
            }
        }

        //-----------------------Add a new function here------------------//
    }
}
