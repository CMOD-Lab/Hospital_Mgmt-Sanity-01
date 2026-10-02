using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBProject.DAL;
using System.Data;

namespace doctor
{
    public partial class bill : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            myDAL objmyDAL = new myDAL();
            DataTable dt = new DataTable();
            int found;

            // cz-dotnet-0022: Replaced InProc Session["idoriginal"] with distributed Redis-backed session (ElastiCache on EKS)
            int did = (int)HttpContext.Current.Session["idoriginal"];
            
            found = objmyDAL.generate_bill_DAL(did, ref dt);

            if (found != 1)
            { Response.Write("<script>alert('There was some error');</script>"); }
            else
            {
                Label1.Text = dt.Rows[0][0].ToString();
            }
        }


        public void bill_paid(object sender, EventArgs e)
        {
            myDAL objmyDAL = new myDAL();
            
            // cz-dotnet-0022: Replaced InProc Session["idoriginal"] with distributed Redis-backed session (ElastiCache on EKS)
            int  did = (int)HttpContext.Current.Session["idoriginal"];
            // cz-dotnet-0022: Replaced InProc Session["appointid"] with distributed Redis-backed session (ElastiCache on EKS)
            int appoint = (int)HttpContext.Current.Session["appointid"];
            objmyDAL.paid_bill_DAL(did,appoint);

			Response.BufferOutput = false;
            Response.Redirect("patienthistory.aspx");
        }


        public void bill_Unpaid(object sender, EventArgs e)
        {
            myDAL objmyDAL = new myDAL();

            // cz-dotnet-0022: Replaced InProc Session["idoriginal"] with distributed Redis-backed session (ElastiCache on EKS)
            int did = (int)HttpContext.Current.Session["idoriginal"];
            // cz-dotnet-0022: Replaced InProc Session["appointid"] with distributed Redis-backed session (ElastiCache on EKS)
            int appoint = (int)HttpContext.Current.Session["appointid"];
            objmyDAL.Unpaid_bill_DAL(did, appoint);

            Response.BufferOutput = false;
            Response.Redirect("patienthistory.aspx");
        }
    }
}
