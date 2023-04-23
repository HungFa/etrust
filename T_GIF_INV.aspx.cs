using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;

public partial class eTrust_T_GIF_INV : System.Web.UI.Page
{
    #region -- 共用變數宣告 --

    //賣股用的變數結構
    public struct SellStockVar
    {
        public string Broker_ID;
        public string Account;
        public string Stock_No;
        public string Sh;
        public string Shs;
        public string sqty;
        public string Rtnbkstr;
        public string Type;
        public string Price;
        public string Use_Market_Price;
        public string Share;
        public string EcollectionNo;
        public int Quantity;
        public int Quantity1;
    }


    #endregion

    #region -- Page Load --

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (Page.Master.FindControl("lbAPId") != null)
            {
                ((Label)Page.Master.FindControl("lbAPId")).Text = "Custody Account Inventory / Sell";
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


    #region --- Method ---

    #region -- 讀取資料 --
    public void loadData(souser su)
    {
        if (null == su)
        {
            return;
        }
        dbClassGo dbGo = new dbClassGo();
        DataTable dt = new DataTable();        
        string sql = "select GIF_INV.*,quantity as quantity1,0 as Cprice,QUANTITY % 1000 as qty from GIF_INV  where STOCK_NO ='" + su.StockNo + "' and idno='" + su.IDNO + "' and quantity<>0 and INV_TYPE='0' and islock='0'";
        //string sql = "select GIF_INV.*,quantity as quantity1,0 as Cprice,QUANTITY % 1000 as qty ";
        //sql +=" from GIF_INV  ";
        //sql +=" where STOCK_NO ='2454' ";
        //sql +=" and idno='MTK30104' ";
        //sql +=" and quantity<>0 ";
        //sql +=" and INV_TYPE='0' ";
        //sql += " and islock='0'";
        //寫log
        new soUtility().AuditLog(su.COMPANY, su.IDNO, "T_GIF_INV", sql, "Q");
        dbGo.execQuery(sql, ref dt);
        gvMain.DataSource = dt;
        gvMain.DataBind();
    }
    #endregion
    
    #region -- 賣股 --

    public void SellStock(SellStockVar sellVar)
    {
        dbClassGo dbGo = new dbClassGo();
        //dbClassGo dbGoTrim = new dbClassGo("soptionTrans");
        bool isDone = false;
        // --- 資料庫變數 ---
        string sql = string.Empty;

        // --- 流程變數 ---
        string strBroker_ID = sellVar.Broker_ID.Trim();
        string strAccount = sellVar.Account;
        string strStock_No = sellVar.Stock_No.Trim();
        string strQuantity = sellVar.Quantity.ToString();
        string strQuantity1 = sellVar.Quantity1.ToString();
        string strSh = sellVar.Sh;
        string strSqty = sellVar.sqty;
        string strShs = sellVar.Shs;
        string strRtnbkstr = sellVar.Rtnbkstr;
        string strType = sellVar.Type;
        string strPrice = sellVar.Price;
        string strUse_Market_Price = sellVar.Use_Market_Price;
        string strShare = sellVar.Share;
        string strEcollectionNo = sellVar.EcollectionNo.Trim();

        souser su = (souser)Session["soption"];
        int iCounter = 1;
        int iQuantity_Tmp = sellVar.Quantity;
        try
        {


            //get the counter !!
            sql = "select  PRIEMAIL  From EPS042_ADDIT where CMPID='GiCtr' and  EMPID='GifCounter'";
            object oCounter = dbGo.execScalar(sql);

            if (null != oCounter)
            {
                iCounter = Convert.ToInt32(oCounter);
            }
            else
            {
                sql = "insert  into EPS042_ADDIT (CMPID,EMPID,PRIEMAIL) values ('GiCtr','GifCounter','" + iCounter.ToString() + "')";
                //dbGo.execNonQuery(sql);
            }

            sql = "select  counter from gif_order where counter=" + iCounter.ToString() + "";

            oCounter = null;
            oCounter = dbGo.execScalar(sql);
            if (null == oCounter)
            {
                showAlert("Sell Error(Counter Error)");
                return;
            }


            #region -- no transaction ---

            //整股
            if (!sellVar.Quantity.Equals(0))
            {
                //更新counter
                iCounter++;
                sql = "update EPS042_ADDIT set PRIEMAIL='" + iCounter.ToString() + "' where CMPID='GiCtr' and  EMPID='GifCounter'";
                dbGo.execNonQuery(sql);

                iQuantity_Tmp = iQuantity_Tmp * 1000;
                sql = "insert  into GIF_ORDER (idno,broker_id,account,stock_no,quantity,price,submit_time,status,ip,use_market_price,type,counter,tdate,buy_sell,inv_type,ecollectionno) values";
                sql += "('" + su.IDNO + "','" + strBroker_ID + "','" + strAccount + "','" + strStock_No + "'," + iQuantity_Tmp.ToString() + "," + strPrice + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                sql += ",'N','" + getClientIP() + "','" + strUse_Market_Price + "','0'," + iCounter.ToString() + ",'" + getTDate(false, "0") + "','S','0','" + strEcollectionNo + "') ";
                dbGo.execNonQuery(sql);

            }
            //零股
            if (strShs.Equals("YES"))
            {
                
                //更新counter
                iCounter++;
                sql = "update EPS042_ADDIT set PRIEMAIL='" + iCounter.ToString() + "' where CMPID='GiCtr' and  EMPID='GifCounter'";
                dbGo.execNonQuery(sql);

                iQuantity_Tmp += Convert.ToInt32(strSqty);
                sql = "insert  into GIF_ORDER (idno,broker_id,account,stock_no,quantity,price,submit_time,status,ip,use_market_price,type,counter,tdate,buy_sell,inv_type,ecollectionno) values";
                sql += "('" + su.IDNO + "','" + strBroker_ID + "','" + strAccount + "','" + strStock_No + "'," + strSqty + "," + strPrice + ",'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',";
                sql += "'N','" + getClientIP() + "','" + strUse_Market_Price + "','1'," + iCounter.ToString() + ",'" + getTDate(false, "1") + "','S','0','" + strEcollectionNo + "')";
                dbGo.execNonQuery(sql);
            }

            //扣除已賣出股數
            sql = "update GIF_INV set quantity=quantity-" + iQuantity_Tmp.ToString() + " where idno='" + su.IDNO + "' and broker_id='" + strBroker_ID + "' and account='" + strAccount + "' ";
            sql += "and stock_no='" + strStock_No + "' and inv_type='0' and ecollectionno='" + strEcollectionNo + "'";
            dbGo.execNonQuery(sql);
            ViewState["vKey"] = "v" + DateTime.Now.ToString("yyyyMMddHHmmss");
            isDone = true;
            #endregion
           

        }
        catch (Exception ex)
        {
            showAlert(ex.Message);
        }
        finally
        {
            dbGo = null;
        }
        showAlert("Sell Successful ..");
        if (isDone)
        {
            #region 發送信件給 股東
            dbClassGo dbGoS = new dbClassGo();
            try
            {
              
            string sTo_mail = string.Empty; //email
            
            //取 Submit time
            //2014/03/11 Modify 成霖公司代碼問題
            sql = "Select A.submit_time as SUBTIME , B.EMAIL From GIF_ORDER A , GIF_EMP B Where A.IDNO = B.IDNO AND A.IDNO='" + su.IDNO + "' ";
            sql += " and A.stock_no='" + strStock_No + "' and A.counter=" + iCounter.ToString() + " AND B.CMPID ='" + su.StockNo + "'";
            //sql = "Select A.submit_time as SUBTIME , B.EMAIL From GIF_ORDER A , GIF_EMP B Where A.IDNO = B.IDNO AND A.IDNO='" + su.IDNO + "' and A.stock_no='" + strStock_No + "' and A.counter=" + iCounter.ToString() + " AND B.CMPID ='" + su.COMPANY + "'";
            DataTable dt = new DataTable();
            dbGoS.execQuery(sql, ref dt);
            if (null != dt)
            {
                if (dt.Rows.Count > 0)
                {

                    DateTime dtSubtime = Convert.ToDateTime(dt.Rows[0]["SUBTIME"].ToString());
                    sTo_mail = dt.Rows[0]["EMAIL"].ToString();
                    System.Text.StringBuilder sbBody = new System.Text.StringBuilder();
                    sbBody.AppendLine("Dear SinoPac Customer：");
                    sbBody.AppendLine("");
                    sbBody.AppendLine("Employee " + su.UID + " has submitted an instruction on SinoPac eTrust to sell stocks on ");
                    sbBody.AppendLine(dtSubtime.ToString("yyyy/MM/dd") + " at " + dtSubtime.ToString("HH:mm:ss"));
                    sbBody.AppendLine("The details of stock sell order are as follows： ");
                    sbBody.AppendLine("The shares of stock sell order： " + iQuantity_Tmp.ToString("###,###,###0.##") + " shares");
                    if (sellVar.Use_Market_Price.Trim().Equals("N"))
                    {
                        sbBody.AppendLine("Price： NTD $ " + Convert.ToDouble(sellVar.Price).ToString("###,###,###0.##"));
                    }
                    else
                    {
                        sbBody.AppendLine("Price： Market Price ");
                    }

                    sbBody.AppendLine("Please visit the SinoPac eTrust website to check the status of the transaction. ");
                    sbBody.AppendLine("If you have any questions regarding to the submission of instruction, please contact ESOP administrator. Thank you.");
                    sbBody.AppendLine("");
                    sbBody.AppendLine("");
                    sbBody.AppendLine("Yours Sincerely,");
                    sbBody.AppendLine("");
                    sbBody.AppendLine("Bank SinoPac");
                    //以下是簡體
                    sbBody.AppendLine("");
                    sbBody.AppendLine("");
                    sbBody.AppendLine("");
                    sbBody.AppendLine("");
                    sbBody.AppendLine("");
                    sbBody.AppendLine("亲爱的永丰商业银行客户，您好：");
                    sbBody.AppendLine("");
                    sbBody.AppendLine("本行已于 " + dtSubtime.ToString("yyyy") + "年" + dtSubtime.ToString("MM") + "月" + dtSubtime.ToString("dd") + "日" + dtSubtime.ToString("HH") + "点" + dtSubtime.ToString("mm") + "分收到您﹝员工身分证号:" + su.UID + "﹞于永丰信托网委托卖股交易，委托卖股明细如下:");
                    sbBody.AppendLine("委托股数: " + iQuantity_Tmp.ToString("###,###,###0.##") + "股");
                    //sbBody.AppendLine("委托价: 市价 / 限价");
                    if (sellVar.Use_Market_Price.Trim().Equals("N"))
                        sbBody.AppendLine("委托价: 限价 NTD $ " + Convert.ToDouble(sellVar.Price).ToString("###,###,###0.##"));
                    //sbBody.AppendLine("委托价: 限价");
                    else
                        sbBody.AppendLine("委托价: 市价");
                    sbBody.AppendLine("");
                    sbBody.AppendLine("若您对本讯息所揭信息有任何问题，请联络 贵公司相关业务窗口协助处理。");
                    sbBody.AppendLine("");
                    //sbBody.AppendLine("本讯息为系统自动发送，欢迎您上永丰信托网查询账户相关数据");
                    //sbBody.AppendLine("永丰信托网 (https://etrust.sinopac.com)");
                    //sbBody.AppendLine("");
                    sbBody.AppendLine("★本服务如因电子邮件系统服务器、个人计算机设定等非可归责本行之原因被判定为垃圾邮件，");
                    sbBody.AppendLine("将可能导致邮件无法或延迟送达，本行不负有保证之义务★");
                    sbBody.AppendLine("");
                    sbBody.AppendLine("");
                    sbBody.AppendLine("敬祝 健康快乐 ");
                    sbBody.AppendLine("永丰商业银行 敬上");
                    //
                    System.Net.Mail.MailMessage oMessage = new System.Net.Mail.MailMessage();
                    oMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                    oMessage.BodyEncoding = System.Text.Encoding.UTF8;
                    oMessage.IsBodyHtml = false;
                    oMessage.Subject = "Employee Stock Option Transaction completed notification – Sales of Stcocks Reserved"; //信件標題
                    oMessage.Body = sbBody.ToString(); //信件內容
                    oMessage.From = new System.Net.Mail.MailAddress("esop@sinopac.com", "ESOP");
                    oMessage.To.Add(new System.Net.Mail.MailAddress(sTo_mail, su.UNAME)); //設定主要寄送信箱

                    //設定其他收信信箱
                    sql = "select PRIEMAIL from EPS042_ADDIT  where EMPID='" + su.IDNO + "' and CMPID='" + su.COMPANY + "'";
                    dt = null;
                    dbGoS.execQuery(sql, ref dt);
                    if (null != dt)
                    {
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                oMessage.To.Add(new System.Net.Mail.MailAddress(dr["PRIEMAIL"].ToString(), su.UNAME));
                            }
                        }
                    }

                    //建立Smtp Server 連線
                    System.Net.Mail.SmtpClient oMailCleint = new System.Net.Mail.SmtpClient();
                    oMailCleint.Host = System.Configuration.ConfigurationManager.AppSettings["mServer"].ToString();
                    oMailCleint.Send(oMessage);//送出信件

                    }
                }
            }
            catch (Exception ex)
            {
              
               
            }
            finally
            {
                dbGoS = null;
              
            }
            #endregion
            Response.Redirect("T_GIF_ORDER.aspx");
        }
    }

    #endregion

    #region -- 取得交易日期-- 
    /// <summary>
    /// 取得日期
    /// </summary>
    /// <param name="inMTKZone"></param>
    /// <param name="strType"></param>
    /// <returns></returns>
    public string getTDate(bool inMTKZone, string strType)
    {
        string sql = string.Empty;
        dbClassGo dbGo = new dbClassGo();

        string strTDate = DateTime.Now.ToString("yyyyMMdd");

        int iHH = DateTime.Now.Hour;
        int imm = DateTime.Now.Minute;
        int iDay = DateTime.Now.Day;
        int iChk = 0;
       
        int iDayOfMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
        string strDayf = string.Empty;

        //Modify 2014/02/18 整/零股統一調整成08:30決定當日單或次日單
        if (iHH > 8 || (iHH == 8 && imm > 30))
        {
            strTDate = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
        }        
        //if (strType.Equals("0"))//整股
        //{
        //    if (iHH > 8 ||( iHH == 8 && imm > 30))
        //    {
        //        strTDate = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
        //    }
        //}
        //else //零股
        //{
            
        //    if (iHH > 13 || (iHH == 13 && imm > 40))
        //    {
        //        strTDate = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
        //    }
        //}      
        try
        {

            while (iChk < 2)
            {

                sql = "select  *  from mhld where country='TWN' and year='" + strTDate.Substring(0, 4) + "' and month='" + strTDate.Substring(4, 2) + "' ";
                iDay = Convert.ToInt16(strTDate.Substring(6, 2));
                DataTable dt = new DataTable();
                dbGo.execQuery(sql, ref dt);
                if (null != dt)
                {
                    if (dt.Rows.Count > 0)
                    {
                        strDayf = dt.Rows[0]["DAYS"].ToString();

                        for (int idex = iDay; idex <= dt.Rows[0]["DAYS"].ToString().Trim().Length; idex++)
                        {
                            //string test = strDayf.Substring(idex - 1, 1);
                            if (strDayf.Substring(idex - 1, 1).Equals("0"))
                            {
                                iChk++;
                                strTDate = strTDate.Substring(0, 6) + iDay.ToString("00");
                                break;
                            }
                            else
                            {
                                iDay++;
                                strTDate = DateTime.ParseExact(strTDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces).AddDays(1).ToString("yyyyMMdd");
                            }
                        }                       
                    }
                }
                iChk++;                
            }// While End 
        }
        catch (Exception ex)
        {
            throw new Exception("Error:" + ex.Message);
        }
        finally
        {
            dbGo = null;
        }
        return strTDate;
    }
    #endregion

    #region -- 取得使用者端IP --
    public string getClientIP()
    {
        string strClientIP = string.Empty;
        try
        {
            if (string.IsNullOrEmpty(Request.ServerVariables["HTTP_X_FORWARDED_FOR"]) || Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString().IndexOf("unknown") > 0)
            {
                strClientIP = Request.ServerVariables["REMOTE_ADDR"];
            }
            else if (Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString().IndexOf(',') > 0)
            {
                strClientIP = Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Substring(1, Request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf(",") - 1);
            }
            else if (Request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf(";") > 0)
            {
                strClientIP = Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Substring(1, Request.ServerVariables["HTTP_X_FORWARDED_FOR"].IndexOf(";") - 1);
            }
            else
            {
                strClientIP = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
        }
        catch
        {
            strClientIP = "Unknow";
        }
        return strClientIP;
    }
    #endregion

    #region -- 賣股前檢核 --

    protected string checkSellData(SellStockVar ssVar)
    {
        string strErrors = string.Empty;
        string strTmp = string.Empty;
        try
        {
            if (ssVar.Quantity.Equals(null) && !ssVar.Shs.ToUpper().Equals("YES"))
            {
                strErrors += "\\nplease enter the quantity!!\\n";
            }

            if (ssVar.Quantity.Equals(0) && !ssVar.Shs.ToUpper().Equals("YES"))
            {
                strErrors += "\\nplease enter the quantity!!\\n";
            }

            if (ssVar.Quantity.Equals(null) && !ssVar.Shs.ToUpper().Equals("YES"))
            {
                strErrors += "\\nplease enter the quantity or odd lots!!\\n";
            }

            if (!ssVar.Share.ToUpper().Equals("YES") && (ssVar.Quantity * 1000 > ssVar.Quantity1))
            {
                strErrors += "\\nOvertake the quantity!!\\n";
            }
            if (ssVar.Shs.ToUpper().Equals("YES") && Convert.ToInt32(ssVar.sqty) > ssVar.Quantity1 % 1000)
            {
                strErrors += "\\nOvertake the odd lots!!\\n";
            }
            if (!ssVar.Price.Equals(string.Empty) && Convert.ToDouble(ssVar.Price) > 10000)
            {
                strErrors += "\\nOvertake the price!!\\n";
            }
            if (ssVar.Use_Market_Price.ToUpper().Equals("N") && (ssVar.Price.Equals("0")||ssVar.Price.Equals(string.Empty)))
            {
                strErrors += "\\nplease enter the price!!\\n";
            }

            return strErrors;

        }
        catch (Exception ex)
        {
            strErrors = ex.Message;
        }

        return strErrors;
    }

    #endregion 

    #region -- Show Alert --

    protected void showAlert(string sMessage)
    {
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('" + sMessage + "');location.reload(true);", true);
    }



    #endregion

    #endregion


    #region --- Behavior ---

    #region -- GridView DataBound --
    protected void gvMain_DataBound(object sender, EventArgs e)
    {
        int iCount=0;
        foreach (GridViewRow gvr in gvMain.Rows)
        {
            ((RadioButton)gvr.Cells[6].FindControl("rbSellPriceN")).Attributes.Add("onClick", "javascript:document.getElementById('" + ((TextBox)gvr.Cells[6].FindControl("txtLimitPrice")).ClientID + "').value='';");
            ((Button)gvr.Cells[7].FindControl("btSubmit")).CommandArgument = iCount.ToString();
            //加入鎖定效果
            ((Button)gvr.Cells[7].FindControl("btSubmit")).Attributes.Add("onclick", "this.disabled=true;" + ClientScript.GetPostBackEventReference(((Button)gvr.Cells[7].FindControl("btSubmit")), ""));
            //取得股票代號
            string stockNo = ((Label)gvr.Cells[0].FindControl("lbSecuCode")).Text;
            //取得市場目前價格
            ((Label)gvr.Cells[3].FindControl("lbRefPrice")).Text = new soUtility().seekPrice(stockNo, "PC").ToString("###,###,###,##0.00");
            //
            if (Convert.ToInt32(((HiddenField)gvr.Cells[7].FindControl("hideQty")).Value) > 0)
            {
                ((HiddenField)gvr.Cells[7].FindControl("hideShs")).Value = "Yes";
                //有零股的話就將值填入,並顯示輸入欄位
                ((TextBox)gvr.Cells[5].FindControl("txtOddLotShares")).Visible = true;
                ((RadioButtonList)gvr.Cells[5].FindControl("rbOddList")).Visible = true;
                ((TextBox)gvr.Cells[5].FindControl("txtOddLotShares")).Text = ((HiddenField)gvr.Cells[7].FindControl("hideQty")).Value;
            }
            //如果股數有大於1000(即有1張的話)顯示整股輸入欄位
            if (Convert.ToInt32(((HiddenField)gvr.Cells[7].FindControl("hideQuantity1")).Value) > 999)
            {
                ((TextBox)gvr.Cells[4].FindControl("txtLotsShares")).Visible = true;
                ((TextBox)gvr.Cells[4].FindControl("txtLotsShares")).Text = "1";
                ((Label)gvr.Cells[4].FindControl("lbLotsShares")).Visible = true;
            }
            iCount++;
        }
    }
    #endregion

    #region -- Sell Submin --

    protected void btSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            ((Button)sender).Enabled = false;
            //取得相關變數值
            int rowidx = Convert.ToInt32(((Button)sender).CommandArgument);
            SellStockVar sStock;
            string strTmp=string.Empty;
            // Ecollection No 快易收編號
            sStock.EcollectionNo = ((HiddenField)gvMain.Rows[rowidx].Cells[7].FindControl("hideEcollectionNo")).Value;
            //Broker ID
            sStock.Broker_ID = ((HiddenField)gvMain.Rows[rowidx].Cells[7].FindControl("hideBroker_Id")).Value;
            //Account
            sStock.Account = ((HiddenField)gvMain.Rows[rowidx].Cells[7].FindControl("hideAccount")).Value;
            // Stock No
            sStock.Stock_No = ((HiddenField)gvMain.Rows[rowidx].Cells[7].FindControl("hideStock_No")).Value;
            // 全部的股數量
            sStock.Quantity1 = Convert.ToInt32(((HiddenField)gvMain.Rows[rowidx].Cells[7].FindControl("hideQuantity1")).Value); 
            //整股股數
            sStock.Quantity = (sStock.Quantity1 < 1000) ? 0 : Convert.ToInt32(((TextBox)gvMain.Rows[rowidx].Cells[4].FindControl("txtLotsShares")).Text);
            // 是否賣零股
            sStock.Sh = ((RadioButtonList)gvMain.Rows[rowidx].Cells[5].FindControl("rbOddList")).SelectedValue.ToUpper(); 
            // 零股股數
            sStock.sqty = ((TextBox)gvMain.Rows[rowidx].Cells[5].FindControl("txtOddLotShares")).Text;
            sStock.sqty = sStock.sqty.Equals(string.Empty) ? "0" : sStock.sqty;
            //是否有零股
            sStock.Shs = Convert.ToInt32(((HiddenField)gvMain.Rows[rowidx].Cells[7].FindControl("hideQty")).Value) > 0 ? "YES" : "NO";
            sStock.Shs = (sStock.Sh.ToUpper().Equals("N") || sStock.Sh.Equals(string.Empty)) ? "NO" : "YES";
            //Rtnbkstr
            sStock.Rtnbkstr = "?sqty1=" + sStock.sqty + "&sh=" + sStock.Sh + "&quantity=" + sStock.Quantity.ToString() + "";
            //類型 1：零股   0：整股
            if (sStock.Shs.Equals("YES"))
            {
                sStock.Type = "1";
            }
            else
            {
                sStock.Type = "0";
            }
            //賣股是否依照市價
            strTmp = string.Empty;
            if (((RadioButton)gvMain.Rows[rowidx].Cells[6].FindControl("rbSellPriceY")).Checked)
            {
                strTmp = "N";
            }
            if (((RadioButton)gvMain.Rows[rowidx].Cells[6].FindControl("rbSellPriceN")).Checked)
            {
                strTmp = "3";
            }
            sStock.Use_Market_Price = strTmp;
            //限定價格
            if (sStock.Use_Market_Price.ToUpper().Equals("N"))
            {
                sStock.Price = ((TextBox)gvMain.Rows[rowidx].Cells[6].FindControl("txtLimitPrice")).Text;
            }
            else
            {
                sStock.Price = "0";
            }

            sStock.Share = "NO";
            strTmp = string.Empty;
            strTmp = checkSellData(sStock);
            if (!strTmp.Equals(string.Empty))
            {
                showAlert(strTmp);
                return;
            }
            SellStock(sStock);
            ((Button)sender).Enabled = true;
        }
        catch (Exception ex)
        {
            showAlert("System Alert:"+ex.Message);
        }

    }
    #endregion

    #endregion

}