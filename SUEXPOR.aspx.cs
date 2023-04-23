using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Text;

public partial class eTrust_SUEXPOR : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (null != Session["printvalues"])
        {
            if (null != Session["soption"])
            {
                souser su = (souser)Session["soption"];
                string [] strArray = Session["printvalues"].ToString().Split(',');
                if (strArray.Length > 3)
                {
                    WritePayForm(su.IDNO, su.COMPANY, strArray[0], strArray[1], strArray[2]);
                    Session["printvalues"] = "";
                }
                else
                {
                    Literal1.Text = "Print key was not currect, Please try again.";
                }

            }
        }
        else
        {
            Literal1.Text = "Print key was lost, Please try again.";
            Response.End();
        }


    }

    public void WritePayForm(string IDNO, string Company, string EPK02, string EPK03, string EPK05)
    {
        //程式用變數宣告
        dbClassGo dbGo = new dbClassGo();
        DataTable dt = new DataTable();
        string sql = string.Empty;
        string strMessage = string.Empty;
        //資料用變數宣告
        string strTitle = string.Empty;
        //銀行資訊
        string Bank_Account = string.Empty;
        string Bank_Name = string.Empty;
        string Company_Name = string.Empty;

        string strEPD05B = string.Empty;
        string strEPD07B = string.Empty;

        string strEPD19 = string.Empty;

        string strEPJ22 = string.Empty;

        decimal dRate = 0;
        decimal dUSD = 0;
        decimal dTWD = 0;

        //E{D05B:GFI 集保帳號, EPD07B:國籍
        sql = "select EPD05B,EPD07B from EPS042 where EPD04B='" + IDNO + "' and EPD01B='" + Company + "' and EPD02B='" + EPK02 + "' ";
        dbGo.execQuery(sql, ref dt);
        if (null != dt)
        {
            if (dt.Rows.Count > 0)
            {
                strEPD05B = dt.Rows[0]["EPD05B"] != null ? dt.Rows[0]["EPD05B"].ToString() : string.Empty;
                strEPD07B = dt.Rows[0]["EPD07B"] != null ? dt.Rows[0]["EPD07B"].ToString() : string.Empty;
            }
        }
        //交換匯率
        sql = "select rate from exchange_rate where  base_id='USD' and pair_id='TWD' order by tdate desc";
        dbGo.execQuery(sql, ref dt);
        if (null != dt)
        {
            if (dt.Rows.Count > 0)
            {
                dRate = dt.Rows[0]["rate"] != null ? Convert.ToDecimal(dt.Rows[0]["rate"]) - 1 : 0;

            }
        }
        //信託銀行檔,EP003: , EP004, EP005 
        sql = "Select * From EPS000 Where EP001='" + Company + "' AND EP002='" + strEPD05B + "'";
        dbGo.execQuery(sql, ref dt);
        if (null != dt)
        {
            if (dt.Rows.Count > 0)
            {
                Bank_Account = dt.Rows[0]["EP003"] != null ? dt.Rows[0]["EP003"].ToString() : string.Empty;
                Bank_Name = dt.Rows[0]["EP004"] != null ? dt.Rows[0]["EP004"].ToString() : string.Empty;
                Company_Name = new soUtility().CompanyCode2StockNo(Company);
                strTitle = dt.Rows[0]["EP005"] != null ? dt.Rows[0]["EP005"].ToString() : string.Empty;
            }
            else
            {
                Bank_Account = string.Empty;
                Bank_Name = string.Empty;
                Company_Name = string.Empty;
                strTitle = string.Empty;
            }
        }

        sql = "select a.*,b.EPD07,b.EPD08,b.EPD06,b.EPD19,c.EPL02,c.EPL03,d.EPA03,b.EPD18,e.EPJ22,h.EPX02 from EPS110 a,EPS040 b,EPS071 c,EPS010 d,EPS105 e,EPS070 g ,EPS251 h where a.EPK02=d.EPA02 and  a.EPK01=b.EPD01  and   a.EPK04=b.EPD02 and a.EPK18=c.EPL01 and  a.EPK04='" + IDNO + "' and a.EPK02='" + EPK02 + "' and  a.EPK03=" + EPK03 + "   and a.EPK05=" + EPK05 + " and EPJ01=EPK01 and EPJ02=EPK02 and  EPJ04= EPK04 and EPG01='" + Company + "' and EPA01='" + Company + "' and EPX01='" + Company + "' and EPG06=EPL02";

        dbGo.execQuery(sql, ref dt);
        if (null != dt)
        {
            if (dt.Rows.Count > 0)
            {
                string strText= string.Empty;
                strEPJ22 = dt.Rows[0]["EPJ22"] != null ? dt.Rows[0]["EPJ22"].ToString() : string.Empty;
                strEPD19 = dt.Rows[0]["EPD19"] != null ? dt.Rows[0]["EPD19"].ToString() : string.Empty;
                dUSD = dt.Rows[0]["EPK14"] != null ? (Convert.ToDecimal(dt.Rows[0]["EPK14"]) / dRate) : 0;

                try
                {
                    if (strEPJ22.Trim().Equals("Y") && strEPD07B.Trim().Equals("A")) //判斷是海外,國內
                    {
                        #region --- 海外-繳款單 ---
                        lbHeader.Text = Company_Name;
                        
                        lbStockNo.Text = Company_Name;
                        lbSeriaNo.Text = "No." + EPK05.PadLeft(8, '0');
                        
                        lbPrintDate.Text = new soUtility().TransDate(dt.Rows[0]["EPK08"] != null ? dt.Rows[0]["EPK08"].ToString() : string.Empty);
                        
                        lbCompanyName.Text =  (dt.Rows[0]["EPD07"] != null ? dt.Rows[0]["EPD07"].ToString() : string.Empty);
                        
                        lbOptions.Text=(dt.Rows[0]["EPK09"] != null ? Convert.ToDecimal(dt.Rows[0]["EPK09"]).ToString("###,###,###,###,##0.##") : string.Empty);
                        lbShares.Text = (dt.Rows[0]["EPK10"] != null ? Convert.ToDecimal(dt.Rows[0]["EPK10"]).ToString("###,###,###,###,##0.##") : string.Empty);
                        lbExercisePrice.Text ="TWD " + (dt.Rows[0]["EPK11"] != null ? Convert.ToDecimal(dt.Rows[0]["EPK11"]).ToString("###,###,###,###,##0.##") : string.Empty);
                        
                        dTWD = dt.Rows[0]["EPK14"] != null ? Convert.ToDecimal(dt.Rows[0]["EPK14"]) : 0;
                        dUSD = dt.Rows[0]["EPK14"] != null ? Convert.ToDecimal(dt.Rows[0]["EPK14"]) / dRate : 0;

                        lbTWD.Text = "TWD " + dTWD.ToString("###,###,###,###,##0.##");
                        lbForginDollars.Text ="USD " + dUSD.ToString("###,###,###,###,##0.##");

                        lbCertificateNo.Text =(dt.Rows[0]["EPK02"] != null ? dt.Rows[0]["EPK02"].ToString():string.Empty);
                        lbDateofIssue.Text =new soUtility().TransDate((dt.Rows[0]["EPK03"] != null ? dt.Rows[0]["EPK03"].ToString():string.Empty));
                        lbEmployeeID.Text = (dt.Rows[0]["EPD06"] != null ? dt.Rows[0]["EPD06"].ToString():string.Empty);
                        lbFXRate.Text = dRate.ToString("###,##0.00");

                        lbInformations1.Text = " USD " + dUSD.ToString("###,###,###,###,##0.##") + " ";
                        lbInformations2.Text = new soUtility().TransDate(dt.Rows[0]["EPK38"] != null ? dt.Rows[0]["EPK38"].ToString() : string.Empty);

                        lbBeneficiaryAC.Text = Bank_Account;
                        lbBeneficiaryName.Text = Bank_Name + strTitle;
                        strText = string.Empty;
                        strText = (dt.Rows[0]["EPK18"] != null ? dt.Rows[0]["EPK18"].ToString() : string.Empty) + "-";
                        strText += (dt.Rows[0]["EPK19"] != null ? dt.Rows[0]["EPK19"].ToString() : string.Empty);
                        lbBankECollectionNo.Text = strText;
                        lbBankIdNo.Text = IDNO;


                        
                        #endregion
                    }
                }
                catch //(Exception ex)
                {
                    strMessage = "Print error";
                }


            }
            else
            {
                strMessage = "no data to print !!!";
            }
        }
    }
}