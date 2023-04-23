using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

public partial class eTrust_T_GIF_AMT : System.Web.UI.Page
{
    public int iGvRowCount = 0;
     protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Page.Master.FindControl("lbAPId") != null)
            {
                ((Label)Page.Master.FindControl("lbAPId")).Text = "To Outward Remittance";
                ((Label)Page.Master.FindControl("lbHeaders")).Text = new soUtility().getHeaders(9002);
            }

            if (null != Session["soption"])
            {
                souser su = (souser)Session["soption"];
                //set user info

                lbUserInfo.Text = su.IDNO + " - " + su.UNAME;
                loadData(su.IDNO, su.COMPANY);

                su = null;
            }
            else
            {
                Server.Transfer("SU000.aspx?errors=" + Server.UrlEncode("Session failure"));
            }
        }
    }

    #region --- Method ---

     /// <summary>
    /// 讀取要出金的相關資料
    /// </summary>
     private void loadData(string IDNO, string CompanyID)
    {
        dbClassGo dbGo = new dbClassGo();
        DataTable dt = new DataTable();
        souser su = (souser)Session["soption"];
        string strErrors = string.Empty;
        string ddlCURRENCY = string.Empty;
                
      try
      {
          string sql = "select a.*,b.EMPNAME,(case when (a.real_amt-a.fee) > 0 then a.real_amt-a.fee else 0 end) as out_amts ";
          sql += " from GIF_AMT a Join GIF_EMP b on a.idno=b.IDNO ";          
          //modify 2014/02/10
          string stockno = new soUtility().CompanyCode2StockNo(su.StockNo);
          string companyid = new soUtility().CompanyCode2StockNo(su.COMPANY);
          sql += " where a.STOCK_NO ='" + (stockno.Length > 0 ? stockno : su.StockNo) + "'";
          sql += " and b.CMPID='" + (companyid.Length > 0 ? companyid : su.COMPANY) + "'";
          //sql += " where a.STOCK_NO ='" + su.StockNo + "'";
          //sql += " and b.CMPID='" + su.COMPANY + "'";
          sql += " and b.IDNO='" + su.IDNO + "'";
          sql += " and a.inv_type='0' and a.amt>0 and a.islock='0' ";
          new soUtility().CompanyCode2StockNo(su.StockNo);
          //寫log
          new soUtility().AuditLog(su.COMPANY, su.IDNO, "T_GIF_AMT", sql, "Q");
        dbGo.execQuery(sql, ref dt);
        //確認有無資料
            if (null != dt)
            {
                //hidacname.Value = dt.Rows[0]["EPD07"] != null ? dt.Rows[0]["EPD07"].ToString().Substring(0, 30) : "";
                gv_Outward.DataSource = dt;
                gv_Outward.DataBind();
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

    protected string updateGIF(string sidno, string sbroker, string saccount, int iRow)
    {
        souser su = (souser)Session["soption"];
        string errors = string.Empty;
        string sql = string.Empty;
        Decimal dhidamt = (null != gv_Outward.Rows[iRow].Cells[2].FindControl("txtOUTAMT") ? Convert.ToDecimal(((TextBox)gv_Outward.Rows[iRow].Cells[2].FindControl("txtOUTAMT")).Text) : 0);
        Decimal dhidfee = (null != gv_Outward.Rows[iRow].Cells[8].FindControl("hideFee") ? Convert.ToDecimal(((HiddenField)gv_Outward.Rows[iRow].Cells[8].FindControl("hideFee")).Value) : 0);
        string strCurrency = (null != gv_Outward.Rows[iRow].Cells[3].FindControl("ddlCURRENCY") ? ((DropDownList)gv_Outward.Rows[iRow].Cells[3].FindControl("ddlCURRENCY")).SelectedValue : string.Empty);
        string strCountry = (null != gv_Outward.Rows[iRow].Cells[4].FindControl("txtCOUNTRY") ? ((TextBox)gv_Outward.Rows[iRow].Cells[4].FindControl("txtCOUNTRY")).Text : string.Empty);
        string strBank = (null != gv_Outward.Rows[iRow].Cells[5].FindControl("txtBANK") ? ((TextBox)gv_Outward.Rows[iRow].Cells[5].FindControl("txtBANK")).Text.TrimEnd() : string.Empty);
        string strAcNumber = (null != gv_Outward.Rows[iRow].Cells[6].FindControl("txtACNUMBER") ? ((TextBox)gv_Outward.Rows[iRow].Cells[6].FindControl("txtACNUMBER")).Text : string.Empty);
        strAcNumber = strAcNumber.PadLeft(14, '0');
        string strInst = (null != gv_Outward.Rows[iRow].Cells[7].FindControl("txtINST") ? ((TextBox)gv_Outward.Rows[iRow].Cells[7].FindControl("txtINST")).Text : string.Empty);
        //判斷帶員工姓名或者受款戶名
        string strAcName = string.Empty;
        if (su.StockNo.Equals("8069") && ((DropDownList)gv_Outward.Rows[iRow].Cells[3].FindControl("ddlCURRENCY")).Enabled == false)
        {
            strAcName = (null != gv_Outward.Rows[iRow].Cells[8].FindControl("hideAcct_Name") ? ((HiddenField)gv_Outward.Rows[iRow].Cells[2].FindControl("hideAcct_Name")).Value.TrimEnd() : string.Empty);
        }
        else
        {
            strAcName = (null != gv_Outward.Rows[iRow].Cells[8].FindControl("hideAccount_Name") ? ((HiddenField)gv_Outward.Rows[iRow].Cells[2].FindControl("hideAccount_Name")).Value.TrimEnd() : string.Empty);
     
        }

        dbClassGo dbGo = new dbClassGo();
        DataTable dt = new DataTable();

        try
        {
            dt = new DataTable();
            sql = "select status from  GIF_AMT_ORDER where idno='" + sidno + "' and broker_id='" + sbroker + "' and account='" + saccount + "' and status='N' and inv_type='0' ";
            errors = dbGo.execQuery(sql, ref dt);

            if (!errors.Equals(string.Empty))
            {
                return errors;
            }
            if (null != dt)
            {
                if (dt.Rows.Count > 0)
                {
                    errors = "One outward remittance transaction per day";
                    return errors;
                }
            }

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
                    sql = "insert  into GIF_AMT_ORDER (idno,broker_id,account,tdate,obank,obank_account,amt,OutCuencyID,submit_time,ip,status,country,msg,stock_no,fee,account_name) values";
                    sql += "('" + sidno + "','" + sbroker + "','" + saccount + "','" + DateTime.Now.ToString("yyyyMMdd") + "','" + strBank + "','" + strAcNumber + "'," + dhidamt + ",'" + strCurrency + "','";
                    sql += DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','" + Request.UserHostAddress + "','N','" + strCountry + "',N'" + strInst + "','" + su.StockNo + "'," + dhidfee + ",'" + strAcName + "'" + ")";

                    dbCommand.CommandText = sql;
                    dbCommand.ExecuteNonQuery();

                    //更新 AMT
                    sql = "update GIF_AMT set amt=amt-" + dhidamt + "-" + dhidfee + ",real_amt=real_amt-" + dhidamt + "-" + dhidfee + ",inhand_amt=inhand_amt-" + dhidamt + "-" + dhidfee + "  where idno='" + sidno + "' and broker_id='" + sbroker + "' and account='" + saccount + "' and inv_type='0'";

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
            sql = string.Empty;
        }
        if (errors.Equals(string.Empty))
        {
            #region 發送信件給 股東
            dbClassGo dbGoS = new dbClassGo();
            try
            {

                string sTo_mail = string.Empty; //email

                //取 Submit time
                //2014/03/11 Modify 成霖公司代碼問題
                sql = "Select A.submit_time as SUBTIME , B.EMAIL From GIF_AMT_ORDER A , GIF_EMP B ";
                sql += " Where A.IDNO = B.IDNO AND A.IDNO='" + su.IDNO + "' and A.stock_no='" + su.StockNo + "'";
                sql += " AND B.CMPID ='" + su.StockNo + "' and status='N' and inv_type='0' ";
                //sql = "Select A.submit_time as SUBTIME , B.EMAIL From GIF_AMT_ORDER A , GIF_EMP B Where A.IDNO = B.IDNO AND A.IDNO='" + su.IDNO + "' and A.stock_no='" + su.StockNo + "' AND B.CMPID ='" + su.COMPANY + "' and status='N' and inv_type='0' ";
                dt = null;
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
                        sbBody.AppendLine("Employee " + su.UID + " has submitted an instruction on SinoPac eTrust to exercise outward remittance on ");
                        sbBody.AppendLine(dtSubtime.ToString("yyyy/MM/dd") + " at " + dtSubtime.ToString("HH:mm:ss"));
                        sbBody.AppendLine("The details of outward remittance are as follows： ");
                        sbBody.AppendLine("Beneficiary Bank：" + strBank);
                        sbBody.AppendLine("Beneficiary Account Number：" + strAcNumber);
                        sbBody.AppendLine("Country：" + strCountry);
                        sbBody.AppendLine("Wire Amount：NTD $" + dhidamt.ToString("###,###,###0.##"));
                        sbBody.AppendLine("Fee：NTD $" + dhidfee.ToString("###,###,###0.##"));    
                        sbBody.AppendLine("Please visit the SinoPac eTrust website to check the status of the transaction. ");
                        sbBody.AppendLine("If you have any questions regarding to the submission of instruction, please contact ESOP administrator. Thank you.");
                        sbBody.AppendLine("");
                        sbBody.AppendLine("");
                        sbBody.AppendLine("Yours Sincerely,");
                        sbBody.AppendLine("");
                        sbBody.AppendLine("Bank SinoPac");
                        System.Net.Mail.MailMessage oMessage = new System.Net.Mail.MailMessage();
                        oMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                        oMessage.BodyEncoding = System.Text.Encoding.UTF8;
                        oMessage.IsBodyHtml = false;
                        oMessage.Subject = "Employee Stock Option Transaction completed notification –Outward Remittance Reserved"; //信件標題
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
        }
        return errors;  
    }

    #endregion

    #region --- Behavior ---

    #region --- GridView ---

    protected void gv_Outward_DataBound(object sender, EventArgs e)
    {
        souser su = (souser)Session["soption"];
        string strStock = su.COMPANY;
        string name = string.Empty;
                      
        foreach (GridViewRow gvr in gv_Outward.Rows)
        {
            if (gvr.RowType == DataControlRowType.DataRow)
            {
                if (null != gvr.Cells[8].FindControl("hideOutAmts"))
                {
                    if (Convert.ToInt32(((HiddenField)gvr.Cells[8].FindControl("hideOutAmts")).Value) > 0)
                    {
                        ((Button)gvr.Cells[8].FindControl("btConfirmSubmit")).Visible = true;
                        //加入鎖定效果
                        ((Button)gvr.Cells[8].FindControl("btConfirmSubmit")).Attributes.Add("onclick", "this.disabled=true;" + ClientScript.GetPostBackEventReference(((Button)gvr.Cells[8].FindControl("btConfirmSubmit")), ""));
                    }
                }
             
                ((TextBox)gvr.Cells[4].FindControl("txtCOUNTRY")).Text = ((TextBox)gvr.Cells[4].FindControl("txtCOUNTRY")).Text.Trim();
                ((TextBox)gvr.Cells[5].FindControl("txtBANK")).Text = ((TextBox)gvr.Cells[5].FindControl("txtBANK")).Text.Trim();
                ((TextBox)gvr.Cells[6].FindControl("txtACNUMBER")).Text = ((TextBox)gvr.Cells[6].FindControl("txtACNUMBER")).Text.Trim();
                ((TextBox)gvr.Cells[7].FindControl("txtINST")).Text=((TextBox)gvr.Cells[7].FindControl("txtINST")).Text.Trim();

                if (null != gvr.Cells[2].FindControl("txtOUTAMT"))
                {
                    ((TextBox)gvr.Cells[2].FindControl("txtOUTAMT")).Attributes.Add("onkeyup", "this.value=this.value.replace(/\\D/g,'')");
                    ((TextBox)gvr.Cells[2].FindControl("txtOUTAMT")).Attributes.Add("onafterpaste", "this.value=this.value.replace(/\\D/g,'')");
                }
                //hideOutAmts
                
                //if (null != gvr.Cells[6].FindControl("txtACNUMBER") && null != gvr.Cells[7].FindControl("txtINST"))
                //{
                //    if (((TextBox)gvr.Cells[6].FindControl("txtACNUMBER")).Text.Equals(string.Empty))
                //    {
                //        ((TextBox)gvr.Cells[7].FindControl("txtINST")).Text = string.Empty;
                //    }
                //    if (((TextBox)gvr.Cells[7].FindControl("txtINST")).Text.Equals(string.Empty))
                //    {
                //        ((TextBox)gvr.Cells[6].FindControl("txtACNUMBER")).Text = string.Empty;
                //    }
                //}
                //((TextBox)gvr.Cells[6].FindControl("txtACNUMBER")).Text.Trim();
                //元太出金時不可選擇入款帳號
                if (su.StockNo.Equals("8069") && (!((TextBox)gvr.Cells[4].FindControl("txtCOUNTRY")).Text.Trim().Equals("") || !((TextBox)gvr.Cells[5].FindControl("txtBANK")).Text.Trim().Equals("") || !((TextBox)gvr.Cells[6].FindControl("txtACNUMBER")).Text.Trim().Equals("")))
                {
                    ((DropDownList)gvr.Cells[3].FindControl("ddlCURRENCY")).Enabled=false;
                    ((TextBox)gvr.Cells[4].FindControl("txtCOUNTRY")).Enabled = false;
                    ((TextBox)gvr.Cells[5].FindControl("txtBANK")).Enabled = false;
                    ((TextBox)gvr.Cells[6].FindControl("txtACNUMBER")).Enabled = false;
                    ((TextBox)gvr.Cells[7].FindControl("txtINST")).Enabled = false;

                }
            }
        }
    }
   
    #endregion

    // <summary>
    /// 鎖定送出按鈕
    /// </summary>
    /// <param name="islock"></param>
    //protected void setButton(bool islock)
    //{
    //    btCancel.Enabled = islock;
    //}  
 
    #region

    protected void showAlert(string message)
    {
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), DateTime.Now.ToString("HHmmss") + "errorAlrt", "alert('" + message + "')", true);
    }
    
    protected void btConfirmSubmit_Click(object sender, EventArgs e)
    {
        string strErrors = String.Empty;
        string temp = ((Button)sender).CommandArgument;
        string[] split = temp.Split(new Char[] { ',' }, 4);
        int iRow = Convert.ToInt32(split[3]);
        string strBank =(null != gv_Outward.Rows[iRow].Cells[5].FindControl("txtBANK") ? ((TextBox)gv_Outward.Rows[iRow].Cells[5].FindControl("txtBANK")).Text : string.Empty);
        string strAcNumber =(null != gv_Outward.Rows[iRow].Cells[6].FindControl("txtACNUMBER") ? ((TextBox)gv_Outward.Rows[iRow].Cells[6].FindControl("txtACNUMBER")).Text : string.Empty);
        

        ////檢核輸入資料

        Decimal dfee = (null != gv_Outward.Rows[iRow].Cells[8].FindControl("hideFee") ? Convert.ToDecimal(((HiddenField)gv_Outward.Rows[iRow].Cells[8].FindControl("hideFee")).Value) : 0);
        Decimal drealamt = (null != gv_Outward.Rows[iRow].Cells[8].FindControl("hideAmt_Limit") ? Convert.ToDecimal(((HiddenField)gv_Outward.Rows[iRow].Cells[8].FindControl("hideAmt_Limit")).Value) : 0);
        Decimal doutamt = (null != gv_Outward.Rows[iRow].Cells[2].FindControl("txtOUTAMT") ? Convert.ToDecimal(((TextBox)gv_Outward.Rows[iRow].Cells[2].FindControl("txtOUTAMT")).Text) : 0);

        if (doutamt.Equals("0"))
        {
            strErrors += "Please enter an amount for outward remittance.\\n";
        }

        if (dfee + doutamt > drealamt)
        {
            strErrors += "Insufficient account balance.\\n";
        }

        if (strBank.Trim().Equals("") || strAcNumber.Trim().Equals(""))
        {
            strErrors += "Please enter the Benificiary\\'s bank account name and number.\\n";
        }

        if (!strErrors.Equals(string.Empty))
        {
            showAlert(strErrors);
            return;
        }

        //將Click BindValue 分割成3個;讀取GIF_AMT的資料
        if (split.Length > 2)
        {
            strErrors = updateGIF(split[0].Trim(), split[1].Trim(), split[2].Trim(), iRow);
            if (!strErrors.Equals(string.Empty))
            {
                showAlert(strErrors);
            }
            else
            {
                Response.Redirect("T_GIF_AMT_ORDER.aspx");
            }
        }
    }
}
    #endregion
    

    #endregion

