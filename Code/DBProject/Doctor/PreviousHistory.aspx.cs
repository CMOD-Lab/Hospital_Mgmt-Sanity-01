using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DBProject.DAL;
using System.Data;

// Session state is backed by Amazon ElastiCache Redis via Microsoft.Web.RedisSessionStateProvider
// configured in Web.config. Session["key"] calls use distributed Redis session (cz-dotnet-0022).


namespace DBProject.Doctor
{
    public partial class PreviousHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PatHistory(sender, e);
        }


        //-----------------------Function1--------------------------//

        protected void PatHistory(object sender, EventArgs e)
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();

            // cz-dotnet-0022: Session backed by distributed Redis cache (ElastiCache) via RedisSessionStateProvider
            int id = (int)Session["idoriginal"];


            int status = objmyDAl.getPHistory(id, ref DT);


            if (status == -1)
            {
                PHistory.Text = "There was some error in retrieving the Patients History.";
            }

          
            else
            {
                PHistoryGrid.DataSource = DT;
                PHistoryGrid.DataBind();
            }

            return;
        }


        //-----------------------Add a new function here------------------//


    }
}
