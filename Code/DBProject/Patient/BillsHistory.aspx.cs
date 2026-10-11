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

namespace DBProject
{
    public partial class BillsHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            billHistory(sender, e);
        }


        //-----------------------Function1--------------------------//

        protected void billHistory(object sender, EventArgs e)
        {
            myDAL objmyDAl = new myDAL();

            DataTable DT = new DataTable();

            // cz-dotnet-0022: Session backed by distributed Redis cache (ElastiCache) via RedisSessionStateProvider
            int id = (int)Session["idoriginal"];


            int status = objmyDAl.getBillHistory(id, ref DT);


            if (status == -1)
            {
                BHistory.Text = "There was some error in retrieving the Patient's Bill History.";
            }

            else if (status == 0)
            {
                BHistory.Text = "There is currently no bill history of yours.";
            }

            else
            {
                BHistory.Text = status + " Bill(s) are found: ";
                BHistoryGrid.DataSource = DT;
                BHistoryGrid.DataBind();
            }

            return;
        }


        //-----------------------Add a new function here------------------//


    }

}
