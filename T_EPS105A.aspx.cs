using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class eTrust_T_EPS105A : System.Web.UI.Page
{
    //protected static souser su;

    protected void Page_Load(object sender, EventArgs e)
    { 
        if (!Page.IsPostBack)
        {
            if (Page.Master.FindControl("lbAPId") != null)
            {
                ((Label)Page.Master.FindControl("lbAPId")).Text = "Grant Information";
                ((Label)Page.Master.FindControl("lbHeaders")).Text = new soUtility().getHeaders(9001);
            }
           
            if (null != Session["soption"])
            {
                souser su;
                su = (souser)Session["soption"];
                //set user info
                
                string strLevel = new soUtility().getAdmin_Level(su.IDNO, su.COMPANY);
                if (strLevel.Equals("S") || strLevel.Equals("A"))
                {
                    loadCompany();
                    MultiView1.SetActiveView(View4);
                }
                else
                {
                    lbUserInfo.Text = su.IDNO + " - " + su.UNAME;
                    loadGrant(su.IDNO, su.COMPANY);
                    MultiView1.SetActiveView(View2);
                }
                su = null;
            }
            else
            {
                Server.Transfer("SU000.aspx?errors=" + Server.UrlEncode("Session failure"));
            }
        }
    }
    #region --- Method ---

    public void loadCompany()
    {
        souser su = (souser)Session["soption"];
        new soUtility().getCompanyCode(ref ddlCompany, su.GroupID, su.COMPANY,su.IDNO, false);
    }
    private void loadGrant(string IDNO, string CompanyID)
    {
        dbClassGo dbGo = new dbClassGo();
        DataTable dt = new DataTable();
        string strErrors = string.Empty;
        string strCountry = new soUtility().getUserCountry(CompanyID, IDNO);
       
        string sql = "Select DISTINCT A.EPJ01,A.EPJ02,A.EPJ03,A.EPJ04,A.EPJ12,A.EPJ28,(Select C.EPJ06 from EPS100 C where A.EPJ01=C.EPJ01 and A.EPJ02=C.EPJ02 and A.EPJ03=C.EPJ03 and A.EPJ04=C.EPJ04) as aa, " +
                    "((Select C.EPJ06 from EPS100 C where A.EPJ01=C.EPJ01 and A.EPJ02=C.EPJ02 and A.EPJ03=C.EPJ03 and A.EPJ04=C.EPJ04) - A.EPJ08) as bb, A.EPJ08, " +
                    "A.EPJ09,(A.EPJ08-A.EPJ09) as Eshares,'0' as CPrice,'0' as mValue,A.EPJ11,(Select D.EPD07 from EPS040 D where A.EPJ04=D.EPD02) as name, '0' as Column0, " +
                    "(Select filepath from filepath where filecom=A.EPJ01 and fileoption=A.EPJ02 and filecountry='" + strCountry + "' and filetype='P') as PlanFile, " +
                    "(Select filepath from filepath where filecom=A.EPJ01 and fileoption=A.EPJ02 and filecountry='" + strCountry + "' and filetype='G') as GuideFile " +
                    "from EPS105 A join EPS020 B on A.EPJ01=B.EPB01 and A.EPJ02=B.EPB02 and A.EPJ03=B.EPB03 where A.EPJ10 in('Y','U') and  EPJ04='" + IDNO + "' and EPJ01='" + CompanyID + "' ";

        //寫log
        new soUtility().AuditLog(CompanyID, IDNO, "T_EPS105A", sql, "Q");

        try
        {
            strErrors = dbGo.execQuery(sql, ref dt);

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
                    //gv_Main.Columns[0].HeaderText = "简体";
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
        souser su;
        su = (souser)Session["soption"];
        string strStock = su.StockNo;
        string name = string.Empty;
        su = null;
        decimal sum5 = 0;
        decimal sum0 = 0;
        decimal sum1 = 0;
        decimal sum8 = 0;
        decimal sum9 = 0;
        decimal sum4 = 0;
        

        foreach (GridViewRow gvr in gv_Main.Rows)
        {
            if (gvr.RowType == DataControlRowType.DataRow)
            {
                gvr.Cells[7].Text = gvr.Cells[7].Text.Trim().Equals("&nbsp;") ? "N/A" : gvr.Cells[7].Text;
                gvr.Cells[9].Text = gvr.Cells[9].Text.Trim().Equals("&nbsp;") ? "N/A" : gvr.Cells[9].Text;

                decimal dCP = 0;
                decimal dStocks = 0;
                decimal mValue = 0;
                decimal dVested = 0;
                decimal dExercised = 0;
                decimal dGranted = 0;
                //decimal dEPJ12 = 0;

                dGranted = gvr.Cells[2].Text.Equals("&nbsp;") ? 0 : Convert.ToDecimal(gvr.Cells[2].Text.Trim().Replace(",", ""));
                Label lbVested = gvr.Cells[4].FindControl("lbVest") != null ? (Label)gvr.Cells[4].FindControl("lbVest") : new Label();
                dVested = lbVested.Text.Equals(string.Empty) ? 0 : Convert.ToDecimal(lbVested.Text);
                dExercised = gvr.Cells[5].Text.Equals("&nbsp;") ? 0 : Convert.ToDecimal(gvr.Cells[5].Text.Trim().Replace(",", ""));
                dStocks = gvr.Cells[6].Text.Equals("&nbsp;") ? 0 : Convert.ToDecimal(gvr.Cells[6].Text.Trim().Replace(",", ""));
                dCP = new soUtility().seekPrice(strStock,"PC");
                gvr.Cells[7].Text = dCP.ToString("###,###,###0.00");
                mValue = dStocks * dCP;
                //dEPJ12 = gvr.Cells[1].Text.Equals("&nbsp;") ? 0 : Convert.ToDecimal(gvr.Cells[1].Text.Trim().Replace(",", ""));
                
                //目前僅聯發科的部分有,
                //if (dEPJ12 > dCP)
                //{
                //    mValue = 0;
                //}
                sum5 += mValue;
                sum8 += dVested;
                sum9 += dExercised;
                sum0 += dGranted;
                sum1 += (dGranted - dVested);
                sum4 += (dVested - dExercised);
                             
                gvr.Cells[8].Text = mValue.ToString("###,###,###0.####");

                //加入Plan / Guide 的hide判斷 ibtGuideDownload
                if (null != gvr.Cells[10].FindControl("ibtPlanDownload"))
                {
                    ((ImageButton)gvr.Cells[10].FindControl("ibtPlanDownload")).Visible = !((ImageButton)gvr.Cells[10].FindControl("ibtPlanDownload")).CommandArgument.Equals(string.Empty);
                }
                if (null != gvr.Cells[10].FindControl("ibtGuideDownload"))
                {
                    ((ImageButton)gvr.Cells[10].FindControl("ibtGuideDownload")).Visible = !((ImageButton)gvr.Cells[10].FindControl("ibtGuideDownload")).CommandArgument.Equals(string.Empty);
                }
            }
        }
        //show the total sum
        gv_Main.FooterRow.Cells[0].Text = "Total:";
        gv_Main.FooterRow.Cells[2].Text = sum0.ToString("###,###,###0");
        gv_Main.FooterRow.Cells[3].Text = sum1.ToString("###,###,###0");
        gv_Main.FooterRow.Cells[4].Text = sum8.ToString("###,###,###0");
        gv_Main.FooterRow.Cells[5].Text = sum9.ToString("###,###,###0");
        gv_Main.FooterRow.Cells[6].Text = sum4.ToString("###,###,###0");
        gv_Main.FooterRow.Cells[8].Text = sum5.ToString("###,###,###0");


    }

    protected void ExPrice_load(string sEPJ01, string sEPJ02, string sEPJ03)
    {
        dbClassGo dbGo = new dbClassGo();
        DataTable dt = new DataTable();
        string strErrors = string.Empty;

        string sql = "select cast(EPB03 as char(10))+'(Grant Date)' as DT,EPB14 as EP from EPS020 where EPB01=@EPB01 and EPB02=@EPB02 and EPB03=@EPB03 union all " +
                     "select cast(EPH04 as char(10))+'(Adjust Date)' as DT,EPH05 as EP from EPS080 where EPH01=@EPH01 and EPH02=@EPH02 and EPH03=@EPH03 " +
                     "order by DT";
  
        SqlParameter oEPB01 = new SqlParameter("@EPB01", sEPJ01);
        SqlParameter oEPB02 = new SqlParameter("@EPB02", sEPJ02);
        SqlParameter oEPB03 = new SqlParameter("@EPB03", sEPJ03);
        SqlParameter oEPH01 = new SqlParameter("@EPH01", sEPJ01);
        SqlParameter oEPH02 = new SqlParameter("@EPH02", sEPJ02);
        SqlParameter oEPH03 = new SqlParameter("@EPH03", sEPJ03);
        SqlParameter[] oParams = new SqlParameter[] { oEPB01, oEPB02, oEPB03, oEPH01, oEPH02, oEPH03 };

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

                    gv_ExPrice.DataSource = dt;
                    gv_ExPrice.DataBind();
                }
            }
        }
        finally
        {
            dbGo = null;
        }
    }

    protected void Vest_load(string sEPJ01, string sEPJ02, string sEPJ03, string sEPJ04)
    {

        dbClassGo dbGo = new dbClassGo();
        DataTable dt = new DataTable();
        string strErrors = string.Empty;

        string sql = "select EPJ05,EPJ06,EPJ24 from EPS105 where EPJ01=@EPJ01 and EPJ02=@EPJ02 and EPJ03=@EPJ03 and EPJ04=@EPJ04 ";

        SqlParameter oEPJ01 = new SqlParameter("@EPJ01", sEPJ01);
        SqlParameter oEPJ02 = new SqlParameter("@EPJ02", sEPJ02);
        SqlParameter oEPJ03 = new SqlParameter("@EPJ03", sEPJ03);
        SqlParameter oEPJ04 = new SqlParameter("@EPJ04", sEPJ04);
        SqlParameter[] oParams = new SqlParameter[] { oEPJ01, oEPJ02, oEPJ03, oEPJ04 };

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

                    gv_Vest.DataSource = dt;
                    gv_Vest.DataBind();
                }
            }
        }
        finally
        {
            dbGo = null;
        }
    }

    protected void btnVest_Click(object sender, EventArgs e)
    {

        string temp = ((Button)sender).CommandArgument;
        string[] split = temp.Split(new Char[] { ',' }, 4);

        //將Click BindValue 分割成3個;傳給Vest_load讀取EPS105的資料
        if (split.Length > 3)
        {
            Vest_load(split[0].Trim(), split[1].Trim(), split[2].Trim(), split[3].Trim());
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ERRalert", "alert('Data Load Error(4321)');", true);
        }
        MultiView1.SetActiveView(View3);
    }

    protected void btnExPrice_Click(object sender, EventArgs e)
    {
        
        string temp = ((Button)sender).CommandArgument;
        string [] split = temp.Split(new Char [] {','},3);

        //將Click BindValue 分割成3個;傳給ExPrice_load讀取EPS080的資料
        if (split.Length > 2)
        {
            ExPrice_load(split[0].Trim(), split[1].Trim(), split[2].Trim());
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ERRalert", "alert('Data Load Error(1234)');", true);
        }
        MultiView1.SetActiveView(View1);
        
    }

    protected void btV1Back_Click(object sender, EventArgs e)
    {
        MultiView1.SetActiveView(View2);
    }

    protected void btV2Back_Click(object sender, EventArgs e)
    {
        MultiView1.SetActiveView(View2);
    }

    protected void ibtPlanDownload_Click(object sender, ImageClickEventArgs e)
    {
        string filepath = ((ImageButton)sender).CommandArgument;
        string strURL = System.Configuration.ConfigurationManager.AppSettings["ApURL"];
        string uuu = strURL + "modules/download.aspx?fileInfos=" + Server.UrlEncode(filepath);
        string script = "window.open('" + uuu + "')";
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "download", script, true);

       
    }

    protected void ibtGuideDownload_Click(object sender, ImageClickEventArgs e)
    {
        string filepath = ((ImageButton)sender).CommandArgument;
        string strURL = System.Configuration.ConfigurationManager.AppSettings["ApURL"];
        string uuu = strURL + "modules/download.aspx?fileInfos=" + Server.UrlEncode(filepath);
        string script = "window.open('" + uuu + "')";
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "download", script, true);
    }

    protected void btQry_Click(object sender, EventArgs e)
    {
        loadGrant(Idno.Text, ddlCompany.SelectedValue);
        MultiView1.SetActiveView(View2);
    }

}

