using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;

public partial class eTrust_T_GIF_ORDER : System.Web.UI.Page
{

    #region -- 共用變數宣告 --

    //賣股用的變數結構
    public struct SellStockVar
    {
        public string Broker_ID;
        public string Account;
        public string Stock_No;
        public string Inv_Type;
        public string Share;
        public string Submit_Time;
        public string EcollectionNo;
        public int Quantity;        
    }


    #endregion

    #region -- Page Load --
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (Page.Master.FindControl("lbAPId") != null)
            {
                ((Label)Page.Master.FindControl("lbAPId")).Text = "Stock Selling Information";
                ((Label)Page.Master.FindControl("lbHeaders")).Text = new soUtility().getHeaders(9001);
            }

            if (null != Session["soption"])
            {
                souser su;
                su = (souser)Session["soption"];
                lbUserInfo.Text = su.IDNO + " - " + su.UNAME;
                loadData(su);
                su = null;
            }
            else
            {
                Server.Transfer("SU000.aspx?errors=" + Server.UrlEncode("Session failure"));
            }
        }
    }
    #endregion

    #region -- Method --
    
    #region -- 讀取資料 --
    public void loadData(souser su)
    {
        if (null == su)
        {
            return;
        }
        dbClassGo dbGo = new dbClassGo();
        DataTable dt = new DataTable();
        string sql = "select inv_type,ecollectionno,idno,status,use_market_price,counter,price,broker_id,account,stock_no,quantity,submit_time from GIF_ORDER where STOCK_NO ='" + su.StockNo + "' and idno='" + su.IDNO + "'";
        //寫log
        new soUtility().AuditLog(su.COMPANY, su.IDNO, "T_GIF_ORDER", sql, "Q");
        dbGo.execQuery(sql, ref dt);
        gvMain.DataSource = dt;
        gvMain.DataBind();
    }

    #endregion

    #region -- 取消賣股 --

    protected string CancelSell(SellStockVar ssVar)
    {
        string strErr = string.Empty;
        dbClassGo dbGo = new dbClassGo();

        try
        {
            // --- 資料庫變數 ---
            string sql = string.Empty;

            souser su = (souser)Session["soption"];

            using (DbConnection dbConn = dbGo.getConnection())
            {
                double dQuantity = 0;
                object oQuantity;
                DbCommand dbCommand = dbGo.getDbCommand();
                dbCommand.Connection = dbConn;
                dbConn.Open();
                DbTransaction dbTrans = dbConn.BeginTransaction();
                dbCommand.Transaction = dbTrans;
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandTimeout = 20;
                try
                {
                    sql = string.Empty;
                    sql = "select sum(quantity) as sumquantity from gif_order where idno='" + su.IDNO  + "' and broker_id='" + ssVar.Broker_ID + "' and account='" + ssVar.Account + "' and stock_no='" + ssVar.Stock_No + "' and submit_time='" + ssVar.Submit_Time + "'";
                    dbCommand.CommandText = sql;
                    oQuantity = dbCommand.ExecuteScalar();
                    dQuantity = Convert.ToDouble(oQuantity !=null?oQuantity:0);

                    

                    ////比對計算出來的股數總額
                    if (dQuantity < Convert.ToDouble(ssVar.Quantity))
                    {
                        dQuantity = ssVar.Quantity;
                    }

                    //更改下單狀態
                    sql = "update GIF_ORDER set status='A' where idno='" + su.IDNO + "' and broker_id='" + ssVar.Broker_ID + "' and account='" + ssVar.Account;
                    sql += "' and stock_no='" + ssVar.Stock_No + "' and submit_time='" + ssVar.Submit_Time + "' ";
                    dbCommand.CommandText = sql;
                    dbCommand.ExecuteNonQuery();

                    sql = "";

                    //加回取消股數
                    sql = "update GIF_INV set quantity=quantity+" + dQuantity + " where idno='" + su.IDNO + "'  and broker_id='" + ssVar.Broker_ID;
                    sql += "' and account='" + ssVar.Account + "'and stock_no='" + ssVar.Stock_No + "' and ecollectionno='" + ssVar.EcollectionNo + "'";
                    dbCommand.CommandText = sql;
                    dbCommand.ExecuteNonQuery();
                    //ViewState["vKey"] = "v" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    dbTrans.Commit();
                }
                catch(Exception ex)
                {
                    strErr = ex.Message;
                    dbTrans.Rollback();
                }
                finally
                {
                    dbTrans.Dispose();
                    dbCommand.Dispose();
                    dbConn.Close();
                    dbConn.Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            showAlert(ex.Message);
            dbGo = null;
        }
        strErr = strErr.Equals(string.Empty) ? "Cancel successful .." : strErr;
        return strErr;
    }

    #endregion

    #region -- Show Alert --

    protected void showAlert(string sMessage)
    {
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + sMessage + "')", true);
    }

    #endregion

    #endregion

    #region -- Behavior --

    #region -- GridView DataBound --

    protected void gvMain_DataBound(object sender, EventArgs e)
    {
        int iCount = 0;
        string strTmp = string.Empty;
        foreach (GridViewRow gvr in gvMain.Rows)
        {
            //刪除按鈕參數
            ((Button)gvr.Cells[0].FindControl("btDelete")).CommandArgument = iCount.ToString();
            //加入鎖定效果
            ((Button)gvr.Cells[0].FindControl("btDelete")).Attributes.Add("onclick", "this.disabled=true;" + ClientScript.GetPostBackEventReference(((Button)gvr.Cells[0].FindControl("btDelete")), ""));
            
            //狀態更新
            if (null != gvr.Cells[5].FindControl("lbStatus"))
            {
                strTmp = ((Label)gvr.Cells[5].FindControl("lbStatus")).Text;
                switch (strTmp.ToUpper())
                {
                    case "N":
                        ((Label)gvr.Cells[5].FindControl("lbStatus")).Text = "Waiting to be sent";
                        ((Button)gvr.Cells[0].FindControl("btDelete")).Visible = true;
                        break;
                    case "Y":
                        ((Label)gvr.Cells[5].FindControl("lbStatus")).Text = "Sent";
                        break;
                    case "A":
                        ((Label)gvr.Cells[5].FindControl("lbStatus")).Text = "Cancelled";
                        break;
                    case "S":
                        ((Label)gvr.Cells[5].FindControl("lbStatus")).Text = "Order Recieved";
                        break;
                    case "R":
                        ((Label)gvr.Cells[5].FindControl("lbStatus")).Text = "Order Recieved";
                        break;
                    case "E":
                        ((Label)gvr.Cells[5].FindControl("lbStatus")).Text = "Reject Order";
                        break;
                    default:
                        ((Label)gvr.Cells[5].FindControl("lbStatus")).Text = "undefined";
                        ((Button)gvr.Cells[0].FindControl("btDelete")).Visible = false;
                        break;
                }
                strTmp = string.Empty;
            }
            //價格顯示
            if (null != gvr.Cells[3].FindControl("lbPrice"))
            {
                strTmp = ((Label)gvr.Cells[3].FindControl("lbPrice")).Text;
                switch (strTmp.ToUpper())
                {
                    case "2":
                        ((Label)gvr.Cells[3].FindControl("lbPrice")).Text = "Market price";
                        break;
                    case "3":
                        ((Label)gvr.Cells[3].FindControl("lbPrice")).Text = "Market price";
                        break;
                    case "Y":
                        ((Label)gvr.Cells[3].FindControl("lbPrice")).Text = "Market price";
                        break;
                    default:
                        ((Label)gvr.Cells[3].FindControl("lbPrice")).Text = ((HiddenField)gvr.Cells[0].FindControl("hidePrice")).Value;
                        break;
                }
                strTmp = string.Empty;
            }
            //日期格式化
            if (null != gvr.Cells[6].FindControl("lbSubmitTime"))
            {
                ((Label)gvr.Cells[6].FindControl("lbSubmitTime")).Text = Convert.ToDateTime(((Label)gvr.Cells[6].FindControl("lbSubmitTime")).Text).ToString("yyyy/MM/dd HH:mm:ss");
            }
            iCount++;
        }
    }

    #endregion

    #endregion

    protected void btDelete_Click(object sender, EventArgs e)
    {
        try
        {
            //取得相關變數值
            int rowidx = Convert.ToInt32(((Button)sender).CommandArgument);
            SellStockVar CancelStock = new SellStockVar();
            string strTmp = string.Empty;
            //Ecollection No
            CancelStock.EcollectionNo = ((Label)gvMain.Rows[rowidx].Cells[1].FindControl("lbEcollectionNo")).Text;
            //Stock NO
            CancelStock.Stock_No = ((Label)gvMain.Rows[rowidx].Cells[2].FindControl("lbSecurityCode")).Text;
            //Broker_ID
            CancelStock.Broker_ID = ((HiddenField)gvMain.Rows[rowidx].Cells[0].FindControl("hideBrokerID")).Value;
            //Account
            CancelStock.Account = ((HiddenField)gvMain.Rows[rowidx].Cells[0].FindControl("hideAccount")).Value;
            //Quantity
            CancelStock.Quantity = Convert.ToInt32(((Label)gvMain.Rows[rowidx].Cells[4].FindControl("lbShare")).Text);
            //Inv_Type
            CancelStock.Inv_Type = ((HiddenField)gvMain.Rows[rowidx].Cells[1].FindControl("hideInv_type")).Value;
            //Submit Time
            CancelStock.Submit_Time =Convert.ToDateTime(((HiddenField)gvMain.Rows[rowidx].Cells[1].FindControl("hideSubmitTime")).Value).ToString("yyyy-MM-dd HH:mm:ss");
            //CancelStock.Submit_Time = ((HiddenField)gvMain.Rows[rowidx].Cells[1].FindControl("hideSubmitTime")).Value;
            
            strTmp=CancelSell(CancelStock);
            //顯示訊息
            showAlert(strTmp);
            //重整畫面
            if (null != Session["soption"])
            {
                souser su;
                su = (souser)Session["soption"];
                lbUserInfo.Text = su.IDNO + " - " + su.UNAME;
                loadData(su);
                su = null;
            }
            else
            {
                Server.Transfer("SU000.aspx?errors=" + Server.UrlEncode("Session failure"));
            }
        }
        catch (Exception ex)
        {
            showAlert("System Alert:" + ex.Message);
        }
    }
}