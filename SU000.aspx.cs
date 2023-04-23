using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class eTrust_SU000_1 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (null != Request.QueryString["errors"])
        {
            litErrors.Text = Request.QueryString["errors"].ToString();
            Session.Clear();
        }
        lbtime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
    }
}
