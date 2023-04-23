using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class eTrust_T_GIF_transaction : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (Page.Master.FindControl("lbAPId") != null)
            {
                ((Label)Page.Master.FindControl("lbAPId")).Text = "Stocks Transaction Query";
                ((Label)Page.Master.FindControl("lbHeaders")).Text = new soUtility().getHeaders(9001);
            }

            if (null != Session["soption"])
            {
                souser su = (souser)Session["soption"];
                lbIDNO.Text = su.IDNO + " - " + su.UNAME;
                loadFilled(su.COMPANY,su.StockNo,su.IDNO);
                //MultiView1.SetActiveView(View1);
            }
        }
    }

    #region --- Method ---

    private void loadFilled(string COMPANY,string STOCKNO, string IDNO)
    {
        dbClassGo dbGo = new dbClassGo();
        DataTable dt = new DataTable();
        string strErrors = string.Empty;

        string sql = "Select * from FILLED Where inv_type='0' and STOCK_NO ='" + STOCKNO + "' and IDNO='" + IDNO + "' ";
        //寫log
        new soUtility().AuditLog(COMPANY, IDNO, "T_GIF_transaction", sql, "Q");
        SqlParameter oIDNO = new SqlParameter("@IDNO", IDNO);
        SqlParameter[] oParams = new SqlParameter[] { oIDNO };

        try
        {
            strErrors = dbGo.execQuery(sql, oParams, ref dt);

            //確認資料庫處理有無錯誤
            if (!strErrors.Equals(string.Empty))
            {
                Response.Redirect("SU000.aspx?errors=" + strErrors);
            }

            //確認有無資料
            if (null != dt)
            {
                if (dt.Rows.Count > 0)
                {
                    gv_Main.DataSource = dt;
                    gv_Main.DataBind();
                }
            }
        }
        finally
        {
            dbGo = null;
        }

    }

    #endregion

    protected void gv_Main_DataBound(object sender, EventArgs e)
    {
        souser su = (souser)Session["soption"];
        string strStock = su.COMPANY;
        su = null;

        foreach (GridViewRow gvr in gv_Main.Rows)
        {
            if (gvr.RowType == DataControlRowType.DataRow)
            {
                
            }
        }

    }
    protected void gv_Main_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}
