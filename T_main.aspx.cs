using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class eTrust_T_main : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ////檢核eTrust,驗證後 所得到的Session值,DEBUG ONLY
        //if (null != Session["soption"])
        //{
        //    souser su = (souser)Session["soption"];
        //    Response.Write("IDNO" + su.IDNO + "<BR>");
        //    Response.Write("Company:" + su.COMPANY + "<BR>");
        //    Response.Write("Stock No:" + su.StockNo + "<BR>");
        //    Response.Write("TRANSID:" + su.TRANSID + "<BR>");
        //    Response.Write("Account:" + su.ACCOUNT + "<BR>");
        //}
    }
}
