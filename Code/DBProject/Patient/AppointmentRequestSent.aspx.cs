using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBProject.DAL;
using System.Data;



namespace DBProject
{
    public partial class AppointmentNotificationSent : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        //-----------------------Function1--------------------------//
        protected void sendARequest (object sender, EventArgs e)
        {
            myDAL objmyDAl = new myDAL();

            
            // cz-dotnet-0022: Replaced InProc Session["dID"] with distributed Redis-backed session (ElastiCache on EKS)
            string dID1 = (string)HttpContext.Current.Session["dID"];

            int dID = Convert.ToInt32(dID1);


            // cz-dotnet-0022: Replaced InProc Session["idoriginal"] with distributed Redis-backed session (ElastiCache on EKS)
            int pID = (int)HttpContext.Current.Session["idoriginal"];


            // cz-dotnet-0022: Replaced InProc Session["freeSlot"] with distributed Redis-backed session (ElastiCache on EKS)
            string temp = (string)HttpContext.Current.Session["freeSlot"];

            int freeSlot = Convert.ToInt32(temp);

            string mes = "";

            int status = objmyDAl.insertAppointment(dID, pID, freeSlot, ref mes);


            if (status == -1)
            {
                Message.Text = "There was some error in sending appointment request to the Doctor.";
            }

            else
            {
                Message.Text = mes;
            }
            
            return;
        }



    }
    
}
