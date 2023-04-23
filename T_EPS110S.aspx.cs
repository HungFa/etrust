using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;

public partial class eTrust_T_EPS110S : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            //加入按鈕鎖定效果
            //btSubscription.Attributes.Add("onclick","this.disabled=true;"+ClientScript.GetPostBackEventReference(btSubscription,""));

            if (Page.Master.FindControl("lbAPId") != null)
            {
                ((Label)Page.Master.FindControl("lbAPId")).Text = "Exercise History / How to pay";
                ((Label)Page.Master.FindControl("lbHeaders")).Text = new soUtility().getHeaders(9001);
            }

            if (null != Session["soption"])
            {
                souser su = (souser)Session["soption"];
                lbUserInfo.Text = su.IDNO + " - " + su.UNAME;
                loadDataList();
                mainViews.SetActiveView(viewDataList);
                su = null;
            }
            else
            {
                Server.Transfer("SU000.aspx?errors=" + Server.UrlEncode("Session failure"));
            }
        }
    }

    #region --- Method ---

    public souser checkSession()
    {
        if (null != Session["soption"])
        {
            return (souser)Session["soption"];
        }
        else
        {
            return null;
        }
    }

   /// <summary>
   /// 讀取資料
   /// </summary>
    protected void loadDataList()
    {
        souser su = checkSession();
        if (null == su)
        {
            Server.Transfer("SU000.aspx?errors=" + Server.UrlEncode("Session failure"));
        }
        string sql = string.Empty;
        string errors = string.Empty;
        dbClassGo dbGo = new dbClassGo();
        DataTable dt = new DataTable();

        try
        {
            //取國籍
            sql = "select EPD04B,EPD07B from EPS042 b where EPD04B='" + su.IDNO + "' and EPD01B='" + su.COMPANY + "'";
            dbGo.execQuery(sql, ref dt);
            if (null != dt)
            {
                if (dt.Rows.Count > 0)
                {
                    hideEPD07B.Value = dt.Rows[0]["EPD07B"]!=null?dt.Rows[0]["EPD07B"].ToString():"";
                }
            }
            
            //取行使的歷史資料
            dt = new DataTable();
            sql = "select E110.*,E105.EPJ22,CASE E110.EPK27 WHEN 'Y' THEN CAST(E110.EPK10 AS CHAR(20)) ELSE 'N/A'  END AS GIVEUP "+
                "From EPS110 E110 Left Join EPS105 E105 ON E110.EPK01=E105.EPJ01 And E110.EPK02=E105.EPJ02 And E110.EPK03=E105.EPJ03 And E110.EPK04=E105.EPJ04 " +
                "Where E110.EPK04='" + su.IDNO + "' and E110.EPK01='" + su.COMPANY + "' And E105.EPJ06=100 order by EPK08 DESC ";
            //寫log
            new soUtility().AuditLog(su.COMPANY, su.IDNO, "T_EPS110S", sql, "Q");
            errors = dbGo.execQuery(sql, ref dt);

            if (null != dt)
            {
                if (dt.Rows.Count > 0)
                {
                    gvMain.DataSource = dt;
                    gvMain.DataBind();
                }
                //else
                //{
                //    showAlert("Data is empty!");
                //}
            }
        }
        catch (Exception ex)
        {
            showAlert(ex.Message);
        }
        finally
        {
            dbGo = null;
            dt = null;
        }
    }

    /// <summary>
    /// javascript alert windows
    /// </summary>
    /// <param name="message"></param>
    protected void showAlert(string message)
    {
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), DateTime.Now.ToString("HHmmss") + "errorAlrt", "alert('" + message + "')", true);
    }

    protected void cancelExercise(string sEPK01,string sEPK02,string sEPK03,string sEPK04,string sEPK05,string sEPK10)
    {
        string errors = string.Empty;
        dbClassGo dbGo = new dbClassGo();
        try
        {
            //更新EPS110
            string sql = "update EPS110 set status='A' where EPK03=" + sEPK03 + " and EPK02='" + sEPK02 + "' and EPK05=" + sEPK05 + " and EPK01='" + sEPK01 + "' and EPK04='" + sEPK04 + "'";
            errors = dbGo.execNonQuery(sql);
            if (!errors.Equals(string.Empty))
            {
                showAlert("Can not cancel this record(0)!");
                return;
            }
            
            //將已扣除的股數加回去
            
            if ((Convert.ToInt32(sEPK10) % 1000) > 0)
            {
                sql = "update EPS105 set EPJ20=EPJ20+" + sEPK10 + ",EPJ09=EPJ09 - " + sEPK10 + ",EPJ23=0  where EPJ04='" + sEPK04  + "' and EPJ01='" + sEPK01  + "' and EPJ02='" + sEPK02 + "' and EPJ03=" + sEPK03 + " and EPJ10='Y' ";
            }
            else
            {
                sql = "update EPS105 set EPJ20=EPJ20+" + sEPK10 + ",EPJ09=EPJ09 - " + sEPK10 + "  where EPJ04='" + sEPK04 + "' and EPJ01='" + sEPK01 + "' and EPJ02='" + sEPK02 + "' and EPJ03=" + sEPK03 + " and EPJ10='Y' ";
            }
            errors = dbGo.execNonQuery(sql);
            if (!errors.Equals(string.Empty))
            {
                showAlert("Can not cancel this record(1)!");
            }
            else
            {
                showAlert("This record has be canceled!");
            }
        }
        catch (Exception ex)
        {
            showAlert(ex.Message);
        }
        finally
        {
            dbGo = null;
        }
    }

    #endregion

    #region --- Behavior ---

    /// <summary>
    /// gvMain_DataBound
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvMain_DataBound(object sender, EventArgs e)
    {
        foreach (GridViewRow gvr in gvMain.Rows)
        {
            if (gvr.RowType == DataControlRowType.DataRow)
            {
                bool blEPK38 = Convert.ToInt32(((HiddenField)gvr.Cells[0].FindControl("hideEPK38")).Value) >= Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
                bool blEPK26 = ((HiddenField)gvr.Cells[0].FindControl("hideEPK26")).Value.Equals("0") || ((HiddenField)gvr.Cells[0].FindControl("hideEPK26")).Value.Equals("");
                bool blEPK37 = ((HiddenField)gvr.Cells[0].FindControl("hideEPK37")).Value.Equals("A");
                bool blOnlinePay = ((HiddenField)gvr.Cells[0].FindControl("hideOnlinePay")).Value.Equals("N");
                string strStatus = ((HiddenField)gvr.Cells[0].FindControl("hideStatus")).Value;
                string strEPK02 = ((HiddenField)gvr.Cells[0].FindControl("hideEPK02")).Value;
                string strEPK03 = ((HiddenField)gvr.Cells[0].FindControl("hideEPK03")).Value;
                string strEPK05 = ((HiddenField)gvr.Cells[0].FindControl("hideEPK05")).Value;
                string strEPK10 = ((HiddenField)gvr.Cells[0].FindControl("hideEPK10")).Value;
                
                if (blEPK38 && blEPK26 && blEPK37 && blOnlinePay)
                {
                    if (strStatus.Equals("N"))
                    {
                        ((Button)gvr.Cells[0].FindControl("btGVcancel")).Visible = true;
                        ((Button)gvr.Cells[0].FindControl("btGVcancel")).CommandArgument = strEPK02 + "," + strEPK03 + "," + strEPK05 + "," + strEPK10;
                    }
                    if (!strStatus.Equals("A"))
                    {
                        ((Button)gvr.Cells[1].FindControl("btGVhowtopay")).Visible = true;
                        //((Button)gvr.Cells[1].FindControl("btGVhowtopay")).CommandArgument = strEPK02 + "," + strEPK03 + "," + strEPK05 + "," + strEPK10;
                    }
                }
                
                
                gvr.Cells[5].Text = new soUtility().getExerciseMethod(gvr.Cells[5].Text.Trim());
                gvr.Cells[11].Text = gvr.Cells[11].Text.Trim().Equals("//") ? "N/A" : gvr.Cells[11].Text;
                gvr.Cells[13].Text = gvr.Cells[13].Text.Trim().Equals("//") ? "N/A" : gvr.Cells[13].Text;
                gvr.Cells[14].Text = gvr.Cells[14].Text.Trim().Equals("&nbsp;") ? "0.00" : gvr.Cells[14].Text;
            }
        }
    }

    protected void btGVcancel_Click(object sender, EventArgs e)
    {
        souser su = checkSession();
        if (null == su)
        {
            Server.Transfer("SU000.aspx?errors=" + Server.UrlEncode("Session failure"));
        }
        //0:EPK02, 1:EPK03, 2:EPK05, 3:EPK10
        string[] strArgs = ((Button)sender).CommandArgument.Split(',');
        if (null == Session["soption"])
        {
            Response.Redirect("SU000.aspx?errors=" + Server.UrlEncode("Login authentication has expired"));
        }
        if (strArgs.Length < 4)
        {
            Response.Redirect("SU000.aspx?errors=" + Server.UrlEncode("Parameter error"));
        }
        cancelExercise(su.COMPANY, strArgs[0].Trim(), strArgs[1].Trim(), su.IDNO, strArgs[2].Trim(), strArgs[3].Trim());
        loadDataList();
        su = null;
        mainViews.SetActiveView(viewDataList);
        
    }
    protected void btGVhowtopay_Click(object sender, EventArgs e)
    {
        souser su = checkSession();
        if (null == su)
        {
            Server.Transfer("SU000.aspx?errors=" + Server.UrlEncode("Session failure"));
        }
        btSubscription.CommandArgument = ((Button)sender).CommandArgument;
        mainViews.SetActiveView(viewHowToPay);
    }

    #endregion

    protected void btSubscription_Click(object sender, EventArgs e)
    {
        //EPK02,EPK03,EPK05,EPK10
        string[] strArray = ((Button)sender).CommandArgument.Split(',');
        Session["printvalues"] = ((Button)sender).CommandArgument;
        //Server.Transfer("SUEXPOR.aspx");
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "print", " location.href='SUEXPOR.aspx'", true);
    }
    protected void btPaymentBack_Click(object sender, EventArgs e)
    {
        Session["printvalues"] = "";
        mainViews.SetActiveView(viewDataList);
    }
}