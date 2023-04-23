using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;


public partial class eTrust_T_GIF_AMT_ORDER : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Page.Master.FindControl("lbAPId") != null)
            {
                ((Label)Page.Master.FindControl("lbAPId")).Text = "Outward Remittance Information";
                ((Label)Page.Master.FindControl("lbHeaders")).Text = new soUtility().getHeaders(9002);
            }

            if (null != Session["soption"])
            {
                souser su = (souser)Session["soption"];
                //set user info

                lbUserInfo.Text = su.IDNO + " - " + su.UNAME;
                loadData(su.IDNO);
                su = null;
            }
            else
            {
                Server.Transfer("SU000.aspx?errors=" + Server.UrlEncode("Session failure"));
            }
        }
    }

#region --- Method ---

    private void loadData(string IDNO)
    {
        dbClassGo dbGo = new dbClassGo();
        DataTable dt = new DataTable();
        souser su = (souser)Session["soption"];     
                
      try
      {
          string sql = "select * from GIF_AMT_ORDER where STOCK_NO ='" + su.StockNo + "' and idno='" + su.IDNO + "' and inv_type='0' ";
        //寫log
        new soUtility().AuditLog(su.COMPANY, su.IDNO, "T_GIF_AMT_ORDER", sql, "Q");
        dbGo.execQuery(sql, ref dt);
        //確認有無資料
            if (null != dt)
            {
                gv_InputConfirm.DataSource = dt;
                gv_InputConfirm.DataBind();   
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

    protected string cancelGIF(string sidno, string sbroker, string saccount,string stdate)
    {
        string errors = string.Empty;
        string sql = string.Empty;
        decimal dhidamt = Convert.ToDecimal(hidoutamt.Value);
        decimal dhidfee = Convert.ToDecimal(hidfee.Value);
        dbClassGo dbGo = new dbClassGo();
        DataTable dt = new DataTable();
        souser su = (souser)Session["soption"];

        try
        {
            using (DbConnection dbConn = dbGo.getConnection())
            {
                dbConn.Open();

                DbTransaction dbTrans = dbConn.BeginTransaction();
                DbCommand dbCommand = dbGo.getDbCommand();
                dbCommand.Connection = dbConn;
                dbCommand.Transaction = dbTrans;
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandTimeout = 20;

                try
                {
                    //更新 GIF_AMT_ORDER
                    sql = "update GIF_AMT_ORDER set status='A' where idno='" + sidno + "' and broker_id='" + sbroker + "' and account='" + saccount + "' and tdate='" + stdate + "' and status='N'  and inv_type='0'";

                    dbCommand.CommandText = sql;
                    dbCommand.ExecuteNonQuery();

                    //更新 GIF_AMT
                    sql = "update GIF_AMT set amt=amt+" + dhidamt + "+" + dhidfee + ",real_amt=real_amt+" + dhidamt + "+" + dhidfee + ",inhand_amt=inhand_amt+" + dhidamt + "+" + dhidfee + "  where idno='" + sidno + "' and broker_id='" + sbroker + "' and account='" + saccount + "' and inv_type='0'";

                    dbCommand.CommandText = sql;
                    dbCommand.ExecuteNonQuery();

                    dbTrans.Commit();

                }
                catch (Exception)
                {
                    dbTrans.Rollback();
                    errors = "Invalid data input. Please go to the previous page and try again";
                    showAlert(errors);
                }
                finally
                {
                    dbTrans.Dispose();
                    dbCommand.Dispose();
                    dbConn.Close();
                    dbConn.Dispose();

                    errors = "The order has been cancelled successfully.";
                    showAlert(errors);
                }
            }

        }
        catch (Exception)
        {
            errors = "Invalid data input. Please go to the previous page and try again";
            showAlert(errors);
        }
        finally
        {
            dbGo = null;
            dt = null;
        }
        return errors;
    }

#endregion

#region --- Behavior ---

    protected void gv_InputConfirm_DataBound(object sender, EventArgs e)
    {
        souser su = (souser)Session["soption"];
        string strStock = su.COMPANY;
        //string name = string.Empty;
       
        foreach (GridViewRow gvr in gv_InputConfirm.Rows)
        {
            if (gvr.RowType == DataControlRowType.DataRow)
            {
                string strSTATUS = string.Empty;
                strSTATUS = gvr.Cells[7].Text;
                switch (strSTATUS)
                {
                    case "N":
                        gvr.Cells[7].Text = "In ordering processing";
                        break;
                    case "Y":
                        gvr.Cells[7].Text = "Submitted";
                        break;
                    case "A":
                        gvr.Cells[7].Text = "Cancelled";
                        break;
                    default:
                        gvr.Cells[7].Text = "undefined";
                        break;
                }
                
                gvr.Cells[7].Text = gvr.Cells[7].Text.Trim().Equals("&nbsp;") ? "N/A" : gvr.Cells[7].Text;
                hidoutamt.Value = gvr.Cells[4].Text;
                hidfee.Value = gvr.Cells[5].Text;

                if (!strSTATUS.Equals("N"))
                {
                    ((Button)gvr.Cells[0].FindControl("btCancel")).Visible = false;
                }
                else
                {
                    //加入鎖定效果
                    ((Button)gvr.Cells[0].FindControl("btCancel")).Attributes.Add("onclick", "this.disabled=true;" + ClientScript.GetPostBackEventReference(((Button)gvr.Cells[0].FindControl("btCancel")), ""));
                }
            }
        }  
    }

    protected void showAlert(string message)
    {
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), DateTime.Now.ToString("HHmmss") + "errorAlrt", "alert('" + message + "')", true);
    }

    protected void btCancel_Click(object sender, EventArgs e)
    {
        string errors = String.Empty;
        string temp = ((Button)sender).CommandArgument;
        string[] split = temp.Split(new Char[] { ',' }, 4);

             
        //將Click BindValue 分割成3個;讀取GIF_AMT的資料
        if (split.Length > 2)
        {
            cancelGIF(split[0].Trim(), split[1].Trim(), split[2].Trim(), split[3].Trim());
            loadData(split[0].Trim());
        }
        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "ERRalert", "alert('Data Load Error(8765)');", true);
        }
   }
}
#endregion