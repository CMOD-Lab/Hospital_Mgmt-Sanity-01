using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBProject.DAL;
using System.Data;

// cz-dotnet-0022: Session state is backed by Amazon ElastiCache Redis via Microsoft.Web.RedisSessionStateProvider
// configured in Web.config. All Session["key"] calls use distributed Redis session (not InProc).

namespace DBProject
{
    public partial class ViewDoctors : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // cz-dotnet-0022: Session backed by distributed Redis cache (ElastiCache) via RedisSessionStateProvider
            Session["dID"] = "";
            deptDoctorInfo(sender, e);
        }

        //---------------Function Called whenever a Doctor is selected from the Grid View----//
        protected void TDoctorGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                Int16 num = Convert.ToInt16(e.CommandArgument);

                string dID = TDoctorGrid.Rows[num].Cells[2].Text;
  
                // cz-dotnet-0022: Session backed by distributed Redis cache (ElastiCache) via RedisSessionStateProvider
                Session["dID"] = dID;

                Response.BufferOutput = true;
                Response.Redirect("DoctorProfile.aspx");

                return;
            }
        }



        //-----------------------Function1--------------------------//

        protected void deptDoctorInfo(object sender, EventArgs e)
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();

            // cz-dotnet-0022: Session backed by distributed Redis cache (ElastiCache) via RedisSessionStateProvider
            string deptName = (string) Session["deptOriginal"];

            int status = objmyDAl.getDeptDoctorInfo(deptName, ref DT);


            if (status == -1)
            {
                TDoctor.Text = "There was some error in retrieving the Doctors Information.";
            }

            else
            {
                // cz-dotnet-0022: Session backed by distributed Redis cache (ElastiCache) via RedisSessionStateProvider
                TDoctor.Text = "Following are our Specialized Doctors of " + Session["deptOriginal"] + " Department:";
                TDoctorGrid.DataSource = DT;
                TDoctorGrid.DataBind();
            }

            return;
        }


        //-----------------------Add a new function here------------------//
    }
}
