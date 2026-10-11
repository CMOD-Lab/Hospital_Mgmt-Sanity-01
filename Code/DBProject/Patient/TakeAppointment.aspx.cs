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
    public partial class TakeAppointment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // cz-dotnet-0022: Session backed by distributed Redis cache (ElastiCache) via RedisSessionStateProvider
            Session["deptOriginal"] = "";
            deptInfo(sender, e);

        }

        //---------------Function Called whenever a Department is selected from the Grid View----//
        protected void TDeptGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                Int16 num = Convert.ToInt16(e.CommandArgument);

                string deptName = TDeptGrid.Rows[num].Cells[2].Text;

                // cz-dotnet-0022: Session backed by distributed Redis cache (ElastiCache) via RedisSessionStateProvider
                Session["deptOriginal"] = deptName;

                Response.BufferOutput = true;
                Response.Redirect("ViewDoctors.aspx");

                return;
            }
        }



        //-----------------------Function1--------------------------//

        protected void deptInfo(object sender, EventArgs e)
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();


            int status = objmyDAl.getdeptInfo(ref DT);


            if (status == -1)
            {
                TDept.Text = "There was some error in retrieving the Departments Information.";
            }

            else
            {
                TDept.Text = "Following are the departments available at our Clinic : ";
                TDeptGrid.DataSource = DT;
                TDeptGrid.DataBind();
            }

            return;
        }


        //-----------------------Add a new function here------------------//


    }
}
