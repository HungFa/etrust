using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class etrust : System.Web.UI.MasterPage
{
    #region 共用變數

    //程式標題
    private string strAPTitle = string.Empty;

    public string ApTitle
    {
        get { return strAPTitle; }
        set { strAPTitle = value; }
    }

    #endregion
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    public string APPDESC
    {
        get { return lbAPId.Text; }
        set { lbAPId.Text = value; } 
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        setHeader();

        //開發用
        //souser su = new souser();
        //su.IDNO = "30303030AA";
        //su.UNAME = "Srikanth Kannan";
        //su.COMPANY = "8069";
        //su.StockNo = "8069";
        //su.GroupID = "ABROAD";
        //su.ADMIN_LEVEL = "U";
        //su.ACCOUNT = "55109901022";
        //Session["soption"] = su;

        if (!Page.IsPostBack)
        {
            if (null == Session["soption"])
            {
                Response.Redirect("SU000.aspx?errors=Login authentication has expired");
            }
        }
    }

    public void setHeader()
    {
        //
        lbtime.Text = "System Time: " + DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss") + " (GMT+8)";

    }

}
