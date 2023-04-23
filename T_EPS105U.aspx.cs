using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using IBM.WMQ;
using MQUtility;
public partial class eTrust_T_EPS105U : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            //加入按鈕鎖定效果
            btSubmit.Attributes.Add("onclick", "this.disabled=true;" + ClientScript.GetPostBackEventReference(btSubmit, ""));
            btConfirm.Attributes.Add("onclick", "this.disabled=true;" + ClientScript.GetPostBackEventReference(btConfirm, ""));
            btConfirmSubmit.Attributes.Add("onclick", "this.disabled=true;" + ClientScript.GetPostBackEventReference(btConfirmSubmit, ""));
             

            if (Page.Master.FindControl("lbAPId") != null)
            {
                ((Label)Page.Master.FindControl("lbAPId")).Text = "Exercise Options";
                ((Label)Page.Master.FindControl("lbHeaders")).Text = new soUtility().getHeaders(9001);
            }

            if (null != Session["soption"])
            {
                souser su = (souser)Session["soption"];
                //set user info
                lbUserInfo.Text = su.IDNO + " - " + su.UNAME;
                mainViews.SetActiveView(viewDataList);
                loadData();

                su = null;
            }
            else
            {
                Server.Transfer("SU000.aspx?errors=" + Server.UrlEncode("Session failure"));
            }
        }
    }

    #region --- Behavior ---

    /// <summary>
    /// GridView DataBound
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gvExceriseList_DataBound(object sender, EventArgs e)
    {
        string strExecMethod = string.Empty;
        int iEPD13 = 0;
        string strEPD12 = string.Empty;
        string strEPD28 = string.Empty;
        string strEPA35 = string.Empty;

        foreach (GridViewRow gvr in gvExceriseList.Rows)
        {
            iEPD13 = 0;
            strEPA35 = string.Empty;
            strEPD12 = string.Empty;
            strEPD28 = string.Empty;
            strExecMethod = "X";
            

            //A,A1,A2,"B,B1,C,D

            if (null != gvr.Cells[3].FindControl("HiddenEPD12"))
            {
                strEPD12=((HiddenField)gvr.Cells[3].FindControl("HiddenEPD12")).Value.Trim();
            }
            if (null != gvr.Cells[3].FindControl("HiddenEPD13"))
            {
                iEPD13 = Convert.ToInt32(((HiddenField)gvr.Cells[3].FindControl("HiddenEPD13")).Value.Trim());
            }
            if (null != gvr.Cells[3].FindControl("HiddenEPD28"))
            {
                strEPD28 = ((HiddenField)gvr.Cells[3].FindControl("HiddenEPD28")).Value.Trim();
            }
            if (null != gvr.Cells[3].FindControl("HiddenEPA35"))
            {
                strEPA35 = ((HiddenField)gvr.Cells[3].FindControl("HiddenEPA35")).Value.Trim();
            }


            if (strEPA35.Equals(strEPD28))//如果EPA35,EPD28註記相同,就抓EPA35的
            {
                strExecMethod = strEPA35;
            }
            else//如果EPA35,EPD28註記不同,就抓非空白的那一個的,空白代表 A B C三種都有,取最小集合
            {
                strExecMethod = strEPD28.Equals(string.Empty) ? (strEPA35.Equals(string.Empty)?"X":strEPA35) : strEPD28;
            }

            //判斷 員工狀況碼,當EPS040.EPD12有上註記,且註記是需判斷EPS040.EPD13 狀態生效日,超過的話就只能行使 A 方案,
            //例： EPD12 狀態為離職註記,生效日為 20110715 , 今天是 20110727, 表示早已生效,
            //     該員工已離職,公司不再代為墊款,所以就只能行使A方案不能行使 B C 方案
            if (!strEPD12.Equals(string.Empty))
            {
                if (iEPD13 < Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")) && checkEPD12Status(strEPD12))
                {
                    strExecMethod = "A";
                }
            }

            //檢視可執行方案
            switch (strExecMethod)
            {
                case "A":
                    ((Button)gvr.Cells[3].FindControl("btMethod01")).Visible = true;
                    break;
                case "B":
                    ((Button)gvr.Cells[3].FindControl("btMethod02")).Visible = true;
                    break;
                case "C":
                    ((Button)gvr.Cells[3].FindControl("btMethod03")).Visible = true;
                    break;
                case "":
                    ((Button)gvr.Cells[3].FindControl("btMethod01")).Visible = true;
                    ((Button)gvr.Cells[3].FindControl("btMethod02")).Visible = true;
                    ((Button)gvr.Cells[3].FindControl("btMethod03")).Visible = true;
                    break;
                default:
                    break;

            }
            
        }
    }

    #region --- viewDataList ---

    /// <summary>
    /// 行使方案A
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btMethod01_Click(object sender, EventArgs e)
    {
        string[] strArgs = ((Button)sender).CommandArgument.Split(',');
        hidEPJ02.Value = strArgs[0].Trim().Replace(".", "");
        hidEPJ03.Value = strArgs[1].Trim().Replace(".", "");
        hidEPJ05.Value = strArgs[2].Trim().Replace(".", "");
        hidEPJ37.Value = ((Button)sender).CommandName;
        lbExerciseMethod.Text = new soUtility().getExerciseMethod(hidEPJ37.Value);
        if (wucBoard1.hasData)
        {
            mainViews.SetActiveView(viewConfirm);
        }
        else
        {
            mainViews.SetActiveView(viewInput);
            loadInputData();
        }
    }

    /// <summary>
    /// 行使方案B
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btMethod02_Click(object sender, EventArgs e)
    {
        string[] strArgs = ((Button)sender).CommandArgument.Split(',');
        hidEPJ02.Value = strArgs[0].Trim().Replace(".", "");
        hidEPJ03.Value = strArgs[1].Trim().Replace(".", "");
        hidEPJ05.Value = strArgs[2].Trim().Replace(".", "");
        hidEPJ37.Value = ((Button)sender).CommandName;
        lbExerciseMethod.Text = new soUtility().getExerciseMethod(hidEPJ37.Value);
        if (wucBoard1.hasData)
        {
            mainViews.SetActiveView(viewConfirm);    
        }
        else
        {
            mainViews.SetActiveView(viewInput);
            loadInputData();
        }
    }

    /// <summary>
    /// 行使方案C
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btMethod03_Click(object sender, EventArgs e)
    {
        string[] strArgs = ((Button)sender).CommandArgument.Split(',');
        hidEPJ02.Value = strArgs[0].Trim().Replace(".","");
        hidEPJ03.Value = strArgs[1].Trim().Replace(".", "");
        hidEPJ05.Value = strArgs[2].Trim().Replace(".", "");
        hidEPJ37.Value = ((Button)sender).CommandName;
        lbExerciseMethod.Text = new soUtility().getExerciseMethod(hidEPJ37.Value);
        if (wucBoard1.hasData)
        {
            mainViews.SetActiveView(viewConfirm);
        }
        else
        {
            mainViews.SetActiveView(viewInput);
            loadInputData();
        }
    }

    #endregion

    #region --- viewConfirm ---

    /// <summary>
    /// 行使前再次確認按鈕
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btConfirm_Click(object sender, EventArgs e)
    {
        mainViews.SetActiveView(viewInput);
        //setup the Exercise Method Label
        loadInputData();
    }

    #endregion

    #region --- viewExercise ---

    /// <summary>
    /// 返回行使資料清單
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btBack_Click(object sender, EventArgs e)
    {
        //clear all hidden values
        cleanHideValue();
        mainViews.SetActiveView(viewDataList);
    }

    /// <summary>
    /// 清除輸入的行使股數
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btClear_Click(object sender, EventArgs e)
    {
        txtEPK10a.Text = string.Empty;
    }

    /// <summary>
    /// 行使送出
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btSubmit_Click(object sender, EventArgs e)
    {
        souser su = checkSession();
        if (null == su)
        {
            Server.Transfer("SU000.aspx?errors=" + Server.UrlEncode("Session failure"));
        }
        if (!(new soUtility().VerifyNumber(txtEPK10a.Text)))
        {
            showAlert("You must press number");
            return;
        }
        else
        {
            hidEPK10.Value = txtEPK10a.Text;
            loadConfirmExercise(su.COMPANY, su.IDNO);
            mainViews.SetActiveView(viewInputConfirm);
        }
        su = null;
    }

    #endregion

    #region --- viewExerciseConfirm ---

    /// <summary>
    /// 行使確認-送出
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btConfirmSubmit_Click(object sender, EventArgs e)
    {
        string errors = updateExercise();
        if (!errors.Equals(string.Empty))
        {
            showAlert(errors);
        }
        else
        {
            showAlert("Exercise Successed!");
            Response.Redirect("T_EPS110S.aspx");
        }
    }

    #region 行使確認-取消
    /// <summary>
    /// 行使確認-取消
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btConfirmCancel_Click(object sender, EventArgs e)
    {
        cleanHideValue();
        mainViews.SetActiveView(viewDataList);
    }

    #endregion

    #endregion

    #endregion

    #region --- Method ---

    #region check Session Value

    /// <summary>
    /// 判斷員工狀態註記碼,當回傳為True時,就要判斷EPD13的日期了
    /// </summary>
    /// <param name="temp">EPS040.EPD12</param>
    /// <returns></returns>
    public bool checkEPD12Status(string temp)
    {
        bool bResult = false;
        switch (temp)
        {
            case "A":
            case "A1":
            case "A2":
            case "B":
            case "B1":
            case "C":
            case "D":
                bResult = true;
                break;
            default:
                bResult = false;
                break;
        }

        return bResult;
    }

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
    #endregion

    #region viewDataList

    /// <summary>
    /// 讀取可行使的相關資料
    /// </summary>
    private void loadData()
    {
        dbClassGo dbGo = new dbClassGo();
        DataTable dt = new DataTable();
        souser su = checkSession();
        if (null == su)
        {
            Server.Transfer("SU000.aspx?errors=" + Server.UrlEncode("Session failure"));
        }

        try
        {
            //舊的sql statement
            //string sql = "select a.*,c.EPD12,c.EPD13, c.EPD28 from EPS105 a,EPS020 b ,EPS040 c where a.EPJ04='' and a.EPJ01='' and c.EPD02='' and c.EPD01='' and a.EPJ10='Y' and a.EPJ01=b.EPB01 and a.EPJ02=b.EPB02 and a.EPJ03=b.EPB03 and EPJ20>0 ";
            string sql = "Select E105.*,E040.EPD12,E040.EPD13, E040.EPD28,(E105.EPJ08-E105.EPJ09) as E200921,(RTRIM(E105.EPJ02)+','+CAST(E105.EPJ03 AS CHAR(10))+','+CAST(E105.EPJ05 AS CHAR(10))) AS EPJ235 ";
            sql += ",(Select EPA35 From EPS010 Where EPA01=E020.EPB01 and EPA02=E020.EPB02) as EPA35 ";
            sql += "From EPS105 E105 ";
            sql += "Left Join EPS040 E040 On E105.EPJ01=E040.EPD01 and E105.EPJ04=E040.EPD02 ";
            sql += "Left Join EPS020 E020 On E105.EPJ01=E020.EPB01 and E105.EPJ02=E020.EPB02 and E105.EPJ03=E020.EPB03 ";
            sql += "where E105.EPJ10='Y'  and  EPJ20>0 and E105.EPJ01='" + su.COMPANY + "' and E105.EPJ04='" + su.IDNO + "' ";
            //寫log
            new soUtility().AuditLog(su.COMPANY, su.IDNO, "T_EPS105U", sql, "Q");
            dbGo.execQuery(sql, ref dt);

            if (null != dt)
            {
                gvExceriseList.DataSource = dt;
                gvExceriseList.DataBind();
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

    #endregion

    #region viewInput

    /// <summary>
    /// 鎖定送出按鈕
    /// </summary>
    /// <param name="islock"></param>
    protected void lockInputItems(bool islock)
    {
        txtEPK10a.Enabled = islock;
        btSubmit.Enabled = islock;
    }

    /// <summary>
    /// 讀取輸入頁資料
    /// </summary>
    protected void loadInputData()
    {
        string sql = string.Empty;
        string errors = string.Empty;
        dbClassGo dbGo = new dbClassGo();
        DataTable dt = new DataTable();
        souser su = checkSession();
        if (null == su)
        {
            Server.Transfer("SU000.aspx?errors=" + Server.UrlEncode("Session failure"));
        }
       
        try
        {
            //讀取Input顯示頁面
            sql = "Select E105.*,E010.EPA03,(E105.EPJ08-E105.EPJ09) as EPJ0809 From EPS105 E105 ";
            sql += " Left Join EPS010 E010 On E010.EPA01=E105.EPJ01 And E010.EPA02=E105.EPJ02 ";
            sql += " Where E105.EPJ01='" + su.COMPANY + "' And E105.EPJ02='" + hidEPJ02.Value + "' And E105.EPJ03=" + hidEPJ03.Value;
            sql += " And E105.EPJ04='" + su.IDNO + "' And E105.EPJ05=" + hidEPJ05.Value + " And E105.EPJ15>=" + DateTime.Now.ToString("yyyyMMdd") + " ";
            errors = dbGo.execQuery(sql, ref dt);
            if (null != dt)
            {
                if (dt.Rows.Count > 0)
                {
                    //集保帳號 NULL 判斷
                    if (dt.Rows[0]["EPJ19"].ToString().Trim().ToUpper().Equals("Y"))//是否強制入集保
                    {
                        if (su.ACCOUNT.Trim().Equals(string.Empty))
                        {
                            showAlert("Sorry you must first fill account before exercising the rights");
                            lockInputItems(false);
                        }
                    }
                    lbEPJ03.Text = dt.Rows[0]["EPJ03"] != null ? string.Format("{0:####/##/##}",dt.Rows[0]["EPJ03"]) : string.Empty;
                    lbEPJ12.Text = dt.Rows[0]["EPJ12"]!=null?string.Format("{0:###,###,###0.##}",dt.Rows[0]["EPJ12"]):string.Empty;
                    lbEPJ0809.Text = dt.Rows[0]["EPJ0809"]!=null?string.Format("{0:###,###,###0}",dt.Rows[0]["EPJ0809"]):string.Empty;
                    //讀取Hidden Values : EPS105, EPS010
                    hidLimit.Value = dt.Rows[0]["EPJ0809"] != null ? dt.Rows[0]["EPJ0809"].ToString() : string.Empty;
                    hidEPJ12.Value = dt.Rows[0]["EPJ12"] != null ? dt.Rows[0]["EPJ12"].ToString() : string.Empty;//每股價格
                    hidEPJ13.Value = dt.Rows[0]["EPJ13"] != null ? dt.Rows[0]["EPJ13"].ToString() : string.Empty;
                    hidEPJ17.Value = dt.Rows[0]["EPJ17"] != null ? dt.Rows[0]["EPJ17"].ToString() : string.Empty;
                    hidEPJ22.Value = dt.Rows[0]["EPJ22"] != null ? dt.Rows[0]["EPJ22"].ToString() : string.Empty;
                    hidEPJ23.Value = dt.Rows[0]["EPJ23"] != null ? dt.Rows[0]["EPJ23"].ToString() : string.Empty;
                    hidEPA03.Value = dt.Rows[0]["EPA03"] != null ? dt.Rows[0]["EPA03"].ToString() : string.Empty;
                }
            }
            
            //讀取Hidden Values : EPS070, EPS071
            dt = new DataTable();
            sql = "Select E070.EPG05,E070.EPG04,E071.EPL02,E071.EPL03 From EPS070 E070 ";
            sql += "Left Join EPS071 E071 On E070.EPG06=E071.EPL02 And E070.EPG04=E071.EPL01 ";
            sql += "Where E070.EPG01='" + su.COMPANY + "' and E070.EPG02='" + hidEPJ02.Value + "' and E070.EPG03=" + hidEPJ03.Value + " ";
            errors += dbGo.execQuery(sql, ref dt);
            if (null != dt)
            {
                if (dt.Rows.Count > 0)
                {
                    hidEPG04.Value = dt.Rows[0]["EPG04"] != null ? dt.Rows[0]["EPG04"].ToString() : string.Empty;
                    hidEPL02.Value = dt.Rows[0]["EPL02"] != null ? dt.Rows[0]["EPL02"].ToString() : string.Empty;
                    hidEPL03.Value = dt.Rows[0]["EPL03"] != null ? dt.Rows[0]["EPL03"].ToString() : string.Empty;
                }
            }

            if (!errors.Equals(string.Empty))
            {
                showAlert(errors);
                lockInputItems(false);
            }
        }
        catch (Exception ex)
        {
            showAlert(ex.Message);
            lockInputItems(false);
        }
        finally
        {
            dbGo = null;
            dt = null;
        }

    }

    #endregion

    #region viewConfirmExercise

    /// <summary>
    /// 計算行使相關股數及金額,做二次確認
    /// </summary>
    /// <param name="Company"></param>
    /// <param name="Idno"></param>
    protected void loadConfirmExercise(string Company,string Idno)
    {
        string sql = "select EPA33 from EPS010 where EPA01='" + Company + "' and EPA02='" + hidEPJ02.Value.Trim() + "'";
        dbClassGo dbGo = new dbClassGo();
        try
        {
            lbCirEPJ03.Text = hidEPJ03.Value.Trim().Equals(string.Empty)?DateTime.Now.ToString("yyyy/MM/dd"):string.Format("{0:####/##/##}", Convert.ToDouble(hidEPJ03.Value.Replace(".", "")));
            lbCirEPJ12.Text = hidEPJ12.Value.Trim().Equals(string.Empty) ? "0" : string.Format("{0:###,###,###0.##}", Convert.ToDouble(hidEPJ12.Value));
            lbCirEPK10.Text = hidEPK10.Value.Trim().Equals(string.Empty) ? "0" : string.Format("{0:###,###,###0.##}", Convert.ToDouble(hidEPK10.Value));

            //計算總數
            double dEPJ12 = Convert.ToDouble(hidEPJ12.Value);
            double dEPK10 = Convert.ToDouble(hidEPK10.Value);
            double dEPJ13 = Convert.ToDouble(hidEPJ13.Value);
            double dLimit = Convert.ToDouble(hidLimit.Value);
            double dCurrentEx = dLimit - dEPK10;
            double dTotal = 0;
            dTotal = (dEPJ12 * dEPK10) + dEPJ13;
            object oEPA33 = dbGo.execScalar(sql);
            //計算方式 (null or 1):四捨五入, 2:無條件捨去, 3:無條件進位
            if (null != oEPA33)
            {
                switch (oEPA33.ToString().Trim())
                {
                    case "2":
                        dTotal = Math.Floor(dTotal);
                        break;
                    case "3":
                        dTotal = Math.Ceiling(dTotal);
                        break;
                    default:
                        dTotal = Math.Round(dTotal);
                        break;
                }
            }
            else
            {
                dTotal = Math.Round(dTotal);
            }

            lbCirTotal.Text = string.Format("{0:###,###,###0.##}", dTotal);
            lbCirLimt_EPK10.Text = string.Format("{0:###,###,###0.##}", dCurrentEx);
        }
        catch (Exception ex)
        {
            showAlert(ex.Message);
            btConfirmSubmit.Enabled = false;
            
        }
        finally
        {
            sql = string.Empty;
            dbGo = null;
        }
    }
    #endregion

    #region Utility

    protected string updateExercise()
    {
        souser su = checkSession();
        if (null == su)
        {
            Server.Transfer("SU000.aspx?errors=" + Server.UrlEncode("Session failure"));
        }
        string errors = string.Empty;
        string sql = string.Empty;
        dbClassGo dbGo = new dbClassGo();
        DataTable dt = new DataTable();
        string sEPJ01 = string.Empty;
        string sEPK07 = string.Empty;
        string sEPK15 = string.Empty;
        string sEPK23 = string.Empty;
        string sEPK24 = string.Empty;
        string sEPD26 = string.Empty;
        string sEPK38 = string.Empty;
        string sTo_mail = string.Empty; //email
        string sSubCompany = string.Empty; //子公司代碼
        string sEPG05 = string.Empty;
        string sEPK18 = string.Empty; //銀行代號
        string sEPK04 = string.Empty; //發行次數
        string sEPK19 = string.Empty; //銀行繳款

        double dEPK05 = 0;
        double dEPK09 = 0;
        double dEPK10 = Convert.ToDouble(hidEPK10.Value);
        double dEPK11 = 0;
        double dEPK12 = 0;
        double dEPK13 = 0;
        double dTotal = 0;
        string sEPA33 = string.Empty;

        try
        {
            #region 讀取員工基本資料,可行使股數
            sql = "SELECT E010.EPA33, " +
                    "E105.EPJ01,E105.EPJ12,E105.EPJ13,E105.EPJ16,E105.EPJ09,E105.EPJ17,E105.EPJ18,E105.EPJ19,E105.EPJ20,E105.EPJ29," +
                    "E040.EPD08,E040.EPD18,E040.EPD03,E040.EPD25,E040.EPD26 " +
                    "FROM EPS105 E105 " +
                    "LEFT JOIN EPS040 E040 ON E040.EPD01=E105.EPJ01 AND E040.EPD02=E105.EPJ04 " +
                    "LEFT JOIN EPS010 E010 ON E010.EPA01=E105.EPJ01 " +
                    "WHERE E105.EPJ01='" + su.COMPANY + "' AND E105.EPJ04='" + su.IDNO + "' " +
                    "AND E105.EPJ02='" + hidEPJ02.Value + "' " +
                    "AND E105.EPJ03=" + hidEPJ03.Value + " " +
                    "AND E105.EPJ05=" + hidEPJ05.Value + " ";

            errors = dbGo.execQuery(sql, ref dt);
            if (!errors.Equals(string.Empty))
            {
                return errors;
            }
            if (null != dt)
            {
                if (dt.Rows.Count > 0)
                {
                    dEPK09 = dt.Rows[0]["EPJ20"]!=null?Convert.ToDouble(dt.Rows[0]["EPJ20"]):0;
                    //判斷輸入的股數 是否超過網路限制股數上限
                    if (dEPK10 > dEPK09)
                    {
                        return "Insufficient shares for exercise.";
                    }

                    dEPK11 = dt.Rows[0]["EPJ12"] != null ? Convert.ToDouble(dt.Rows[0]["EPJ12"]) * 1000 : 0;
                    dEPK12 = (dEPK10 * Math.Round(dEPK11)) / 1000;
                    sEPA33 = dt.Rows[0]["EPA33"] != null ? dt.Rows[0]["EPA33"].ToString() : "";
                    switch (sEPA33)
                    {
                        case "2":
                            dEPK12 = Math.Floor(dEPK12);
                            break;
                        case "3":
                            dEPK12 = Math.Ceiling(dEPK12);
                            break;
                        default:
                            dEPK12 = Math.Round(dEPK12);
                            break;
                    }

                    dEPK11 = dt.Rows[0]["EPJ12"] != null ? Convert.ToDouble(dt.Rows[0]["EPJ12"]) : 0; //認購金額 
                    dEPK13 = dt.Rows[0]["EPJ13"] != null ? Convert.ToDouble(dt.Rows[0]["EPJ13"]) : 0; //手續費 
                    sEPK15 = dt.Rows[0]["EPJ16"] != null ? dt.Rows[0]["EPJ16"].ToString() : ""; //最後繳款日 
                    sEPK38 = dt.Rows[0]["EPJ29"] != null ? dt.Rows[0]["EPJ29"].ToString() : ""; //繳款書最後繳款日 
                    dTotal = dEPK12 + dEPK13;
                    sEPK07 = dt.Rows[0]["EPD08"] != null ? dt.Rows[0]["EPD08"].ToString() : ""; //員工代號
                    sEPK23 = dt.Rows[0]["EPD18"] != null ? dt.Rows[0]["EPD18"].ToString() : ""; //集保帳號
                    sEPK24 = dt.Rows[0]["EPJ17"] != null ? dt.Rows[0]["EPJ17"].ToString() : ""; //入集保代號
                    sTo_mail = dt.Rows[0]["EPD25"] != null ? dt.Rows[0]["EPD25"].ToString() : ""; //e-mail
                    sEPJ01 = dt.Rows[0]["EPJ01"] != null ? dt.Rows[0]["EPJ01"].ToString() : ""; //
                    sSubCompany = dt.Rows[0]["EPD03"] != null ? dt.Rows[0]["EPD03"].ToString() : ""; //子公司代碼
                    if (sEPJ01.Trim().Equals("0218"))
                    {
                        sEPD26 = dt.Rows[0]["EPD26"] != null ? dt.Rows[0]["EPD26"].ToString() : ""; //指定帳號
                    }
                    else
                    {
                        sEPD26 = "00000000000000";
                    }
                }
            }
            #endregion

            #region 讀取銀行代號
            dt = new DataTable();
            sql = "select * from EPS070 where EPG01='" + su.COMPANY + "' and EPG02='" + hidEPJ02.Value + "'  and EPG03=" + hidEPJ03.Value + " ";
            errors = dbGo.execQuery(sql, ref dt);
            if (null != dt)
            {
                if (dt.Rows.Count > 0)
                {
                    sEPG05 = dt.Rows[0]["EPG05"] != null ? dt.Rows[0]["EPG05"].ToString() : string.Empty;
                    sEPK18 = dt.Rows[0]["EPG04"] != null ? dt.Rows[0]["EPG04"].ToString() : string.Empty;
                }
                else
                {
                    errors = "Bank Code is Empty";
                }
            }
            else
            {
                errors = "Bank Code is Empty";
            }

            if (!errors.Equals(string.Empty))
            {
                return errors;
            }
            #endregion

            #region --寫入資料庫 with Transaction

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

                    #region 更新繳款書流水號

                    //流水號+1
                    sql = "update EPS020 set EPB09=EPB09+1  where  EPB01='" + su.COMPANY + "' and EPB02='" + hidEPJ02.Value + "'  and EPB03=" + hidEPJ03.Value.Replace(".","") + "  ";
                    //errors = dbGo.execNonQuery(sql);
                    dbCommand.CommandText = sql;
                    dbCommand.ExecuteNonQuery();

                    //if (!errors.Equals(string.Empty))
                    //{
                    //    return errors;
                    //}
                    //取出流水號
                    sql = "select EPB09 from EPS020 where EPB01='" + su.COMPANY + "' and EPB02='" + hidEPJ02.Value + "'  and EPB03=" + hidEPJ03.Value + " ";
                    //dt = new DataTable();
                    //errors = dbGo.execQuery(sql, ref dt);
                    //if (!errors.Equals(string.Empty))
                    //{
                    //    return errors;
                    //}
                    dbCommand.CommandText = sql;
                    object oEPB09 = dbCommand.ExecuteScalar();
                    if (null != oEPB09)
                    {
                        dEPK05 = Convert.ToDouble(oEPB09); //繳款書流水號
                        sEPK04 = "0" + oEPB09.ToString().Trim(); //發行次數
                    }
                    else
                    {
                        errors = "Serial Number error(0)";
                    }

                    if (!errors.Equals(string.Empty))
                    {
                        return errors;
                    }

                    #endregion

                    sEPK19 = sEPG05 + dEPK05.ToString("00000000");//銀行繳款

                    #region 傳送線上認購資料至銀行(Message Queue)
                    sql = "select EPG01 from EPS070 where EPG01='" + su.COMPANY + "' and EPG02='" + hidEPJ02.Value + "' and EPG04='807' ";
                    dbCommand.CommandText = sql;
                    object oEPG01 = dbCommand.ExecuteScalar();
                    if (null != oEPG01)
                    {
                        string ErrMsg = string.Empty;
                        try
                        {
                            string BONAME = "OPTION1             ";
                            string TRG = "SPST      ";
                            string FUNCTIONNAME = "SPSTOPTION1                   ";
                            string MSGNO = "      ";
                            string COUNT = "      ";
                            string ERRMSG = "                                                            ";

                            string TransType = "1";  //1:線上認購 2:線上繳款
                            string MIDTRANSEQ = sEPK19 + DateTime.Now.ToString("HHmmss");
                            string CUSTOMERID = su.IDNO.PadRight(11, '0');
                            string StockNo = sEPK24.PadRight(6, ' ');
                            string EPK11 = (dEPK11 * 100).ToString("000000");
                            string EPK10 = dEPK10.ToString("00000000000");
                            string EPK14 = Math.Round(dTotal * 100).ToString("00000000000");

                            //MqData
                            string MqData = BONAME + TRG + FUNCTIONNAME + MIDTRANSEQ + MSGNO + COUNT + ERRMSG + TransType + CUSTOMERID + StockNo + sEPK15 + EPK11 + EPK10 + EPK14 + sEPK19 + sEPD26;
                            //新增MQ_LOG
                            sql = "insert into MQ_LOG(MIDTRANSEQ,TRANSTYPE,CUSTOMERID,STOCKNO,EXPIRATIONDATE,EXERCISEPRICE,SHARES,TOTALAMOUNT,VIRTUALACCT,CODE,SUBMIT_TIME) values('"
                                + MIDTRANSEQ + "','" + TransType + "','" + su.IDNO + "','" + StockNo + "','" + sEPK15 + "'," + dEPK11 + "," + dEPK10 + "," + dTotal + ",'" + sEPK19 + "','P','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                            errors = dbGo.execNonQuery(sql);

                            string OutData = string.Empty;
                            string OutMsg = string.Empty;
                            //Call MQ Server

                            MQUtility.MQUtility.CallWMQ(MqData, out OutData);
                            //回覆碼
                            OutMsg = OutData.Substring(80, 6).Trim();
                            //拆解下行電文
                            string ExpirationDate = OutData.Substring(170, 8).Trim();
                            decimal ExercisePrice = Convert.ToDecimal(OutData.Substring(178, 6)) / 100;
                            decimal Shares = Convert.ToDecimal(OutData.Substring(184, 11));
                            decimal TotalAmount = Convert.ToDecimal(OutData.Substring(195, 11)) / 100;
                            string Acct = OutData.Substring(206, 14).Trim();

                            //取得錯誤訊息
                            if (!OutMsg.Equals("000000"))
                            {
                                ErrMsg = OutData.Substring(234, 30).Trim();

                            }

                            //新增MQ_LOG
                            sql = "insert into MQ_LOG(MIDTRANSEQ,MSGNO,COUNTER,ERRMSG,TRANSTYPE,CUSTOMERID,STOCKNO,EXPIRATIONDATE,EXERCISEPRICE,SHARES,TOTALAMOUNT,VIRTUALACCT,CODE,SUBMIT_TIME) values('"
                                + MIDTRANSEQ + "','" + OutMsg + "','" + COUNT + "','" + ErrMsg + "','" + TransType + "','" + su.IDNO + "','" + StockNo + "','" + ExpirationDate + "'," + ExercisePrice + "," + Shares + "," + Math.Round(TotalAmount) + ",'" + Acct + "','G','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "') ";
                            errors = dbGo.execNonQuery(sql);

                            if (!OutMsg.Equals("000000"))
                            {
                                return "Exercise fail，ERROR：" + ErrMsg;

                            }

                        }
                        catch (MQException mqex)
                        {
                            ErrMsg = mqex.Message.ToString() + "(" + mqex.ReasonCode.ToString() + ")";
                            sql = "insert into MQ_log_err(date,put_get,body) values('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','G','" + ErrMsg + "') ";
                            errors = dbGo.execNonQuery(sql);
                            dbGo = null;
                            dt = null;
                            dbTrans.Dispose();
                            dbCommand.Dispose();
                            dbConn.Close();
                            dbConn.Dispose();
                            return "Exercise fail，ERROR：Connection failed with banks!!";


                        }
                        catch (Exception ex)
                        {

                            sql = "insert into MQ_log_err(date,put_get,body) values('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','G','" + ex.Message + "') ";
                            errors = dbGo.execNonQuery(sql);
                            dbGo = null;
                            dt = null;
                            dbTrans.Dispose();
                            dbCommand.Dispose();
                            dbConn.Close();
                            dbConn.Dispose();
                            return "Exercise fail，ERROR：Connection failed with banks!!";

                        }
                    }
                    #endregion

                    #region 存入行使異動檔

                    sql = "INSERT INTO EPS110 (EPK01,EPK02,EPK03,EPK04,EPK05,EPK07,EPK08,EPK09,EPK10,EPK11,EPK12,EPK13,EPK14,EPK15,EPK18,EPK19,EPK23,EPK24,PMDAT,PTIME,IP,EPK20,EPK21,EPK22,EPK26,EPK27,STATUS,ONLINE_PAYMENT,EPK37,SUB_COMPANY,EPK38) VALUES ";

                    //EPK01,EPK02,EPK03,EPK04,EPK05,EPK07,EPK08
                    sql += "('" + su.COMPANY + "','" + hidEPJ02.Value + "'," + hidEPJ03.Value + ",'" + su.IDNO + "'," + dEPK05.ToString() + ",'" + sEPK07 + "'," + DateTime.Now.ToString("yyyyMMdd") + ",";
                    //EPK09,EPK10,EPK11,EPK12,EPK13,EPK14
                    sql += dEPK09.ToString() + "," + dEPK10.ToString() + "," + dEPK11.ToString() + "," + Math.Round(dEPK12).ToString() + "," + dEPK13.ToString() + "," + Math.Round(dTotal).ToString() + ",";
                    //EPK15,EPK18,EPK19,EPK23,EPK24,PMDAT,PTIME,IP,EPK20,EPK21,EPK22,EPK26,EPK27,STATUS,ONLINE_PAYMENT,EPK37,SUB_COMPANY,EPK38
                    sql += sEPK15 + ",'" + sEPK18 + "','" + sEPK19 + "','" + sEPK23 + "','" + sEPK24 + "'," + DateTime.Now.ToString("yyyyMMdd") + "," + DateTime.Now.ToString("HHmmss") + ",'" + Request.UserHostAddress + "',0,0,0,0,'','N','N','" + hidEPJ37.Value + "','" + sSubCompany + "'," + sEPK38 + ")";

                    dbCommand.CommandText = sql;
                    dbCommand.ExecuteNonQuery();

                    #region -- 存入行使異動檔(old version) --
                    //if (hidEPJ22.Value.Equals("Y")) //海外
                    //{
                    //    //EPK01,EPK02,EPK03,EPK04,EPK05,EPK07,EPK08
                    //    sql += "('" + su.COMPANY + "','"+hidEPJ02.Value +"',"+hidEPJ03.Value +",'"+su.IDNO +"',"+dEPK05.ToString()+",'"+sEPK07 +"',"+DateTime.Now.ToString("yyyyMMdd")+",";
                    //    //EPK09,EPK10,EPK11,EPK12,EPK13,EPK14
                    //    sql += dEPK09.ToString() + "," + dEPK10.ToString() + "," + dEPK11.ToString() + "," + Math.Round(dEPK12).ToString() + "," + dEPK13.ToString() + "," + Math.Round(dTotal).ToString() + ",";
                    //    //EPK15,EPK18,EPK19,EPK23,EPK24,PMDAT,PTIME,IP,EPK20,EPK21,EPK22,EPK26,EPK27,STATUS,ONLINE_PAYMENT,EPK37,SUB_COMPANY,EPK38
                    //    sql += sEPK15 + ",'" + sEPK18 + "','" + sEPK19 + "','" + sEPK23 + "','" + sEPK24 + "'," + DateTime.Now.ToString("yyyyMMdd") + "," + DateTime.Now.ToString("HHmmss") + ",'"+Request.UserHostAddress +"',0,0,0,0,'','N','N','" + hidEPJ37.Value + "','" + sSubCompany + "'," + sEPK38 + ")";
                    //}
                    //else //國內
                    //{
                    //    //SQLstr = "insert  into EPS110 (EPK01,EPK02,EPK03,EPK04,EPK05,EPK07,EPK08,EPK09,EPK10,EPK11,EPK12,EPK13,EPK14,EPK15,EPK18,EPK19,EPK23,EPK24,PMDAT,PTIME,IP,EPK20,EPK21,EPK22,EPK26,EPK27,status,online_payment,EPK37,SUB_COMPANY,EPK38) values";
                    //    //SQLstr = SQLstr + "('" + (String)session.getAttribute("COMPANY") + "','" + EPK02 + "'," + EPK03 + ",'" + (String)session.getAttribute("IDNO") + "'," + EPK05 + ",'" + EPK07 + "'," + SimpleDateFormat(new Date()) + "," + EPK09 + "," + EPK10 + "," + EPK11 + "," + (Math.round(EPK12)) + "," + EPK13 + "," + (Math.round(TOTAL)) + "," + EPK15 + ",'" + EPK18 + "','" + EPK19 + "','" + EPK23 + "','" + EPK24 + "'," + SimpleDateFormat(new Date()) + "," + PTIME + ",'" + request.getRemoteAddr() + "',0,0,0,0,'','N','N','" + EPK37 + "','" + SUB_COMPANY + "'," + EPK38 + ")";
                    //    sql += "(";

                    //}
                    #endregion

                    //扣除已行使的股數(剩餘股數) EPJ20:網路剩餘可行使股數, EPJ09:已行使股數

                    sql = "update EPS105 set EPJ20=EPJ20-" + dEPK10.ToString() + ",EPJ09=EPJ09 + " + dEPK10.ToString() + "  where EPJ04='" + su.IDNO + "' and EPJ01='" + su.COMPANY + "' and EPJ02='" + hidEPJ02.Value + "' and EPJ03=" + hidEPJ03.Value + " ";
                    dbCommand.CommandText = sql;
                    dbCommand.ExecuteNonQuery();

                    //errors = dbGo.execNonQuery(sql);

                    //if (!errors.Equals(string.Empty))
                    //{
                    //    return errors;
                    //}



                    #endregion

                    dbTrans.Commit();

                }
                catch// (Exception ex)
                {
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

            #endregion

            #region 發送信件給 股東

            //取 Submit time
            sql = "Select submit_time as SUBTIME From EPS110 Where EPK04='" + su.IDNO + "' and EPK01='" + su.COMPANY + "' and EPK02='" + hidEPJ02.Value + "' and EPK03=" + hidEPJ03.Value + " and EPK05=" + dEPK05 + "";
            object oExecTime = dbGo.execScalar(sql);

            if (null == oExecTime)
            {
                return "Submit Time Error";
            }
            DateTime dtSubtime = Convert.ToDateTime(oExecTime);
            System.Text.StringBuilder sbBody = new System.Text.StringBuilder();
            sbBody.AppendLine("Dear SinoPac Customer：");
            sbBody.AppendLine("");
            sbBody.AppendLine("Employee " + su.UID + " has submitted an instruction on SinoPac eTrust to exercise the employee stock option on ");
            sbBody.AppendLine(dtSubtime.ToString("yyyy/MM/dd") + " at " + dtSubtime.ToString("HH:mm:ss"));
            sbBody.AppendLine("The details of instruction are as follows： ");
            sbBody.AppendLine("．E-Collection No.：" + sEPK19);
            sbBody.AppendLine("．Number of exercised shares： " + dEPK10.ToString("###,###,###0.##") + " shares");
            sbBody.AppendLine("．Exercise Method：" + new soUtility().getExerciseMethod(hidEPJ37.Value));
            sbBody.AppendLine("．Exercise price： NTD $ " + dEPK11.ToString("###,###,###0.##"));
            sbBody.AppendLine("．Due date of Payment：" + Convert.ToInt32(sEPK38).ToString("####/##/##"));
            sbBody.AppendLine("Please visit the SinoPac eTrust website to check the status of the transaction. ");
            sbBody.AppendLine("If you have any questions regarding to the submission of instruction, please contact ESOP administrator. Thank you. ");
            sbBody.AppendLine("");
            sbBody.AppendLine("");
            sbBody.AppendLine("Yours Sincerely,");
            sbBody.AppendLine("");
            sbBody.AppendLine("Bank SinoPac");
            sbBody.AppendLine("");
            sbBody.AppendLine("");
            //
            sbBody.AppendLine("亲爱的永丰金证券客户，您好：");
            sbBody.AppendLine("");
            sbBody.AppendLine("您(员工编号:" + su.UID + ")于 " + dtSubtime.ToString("yyyy/MM/dd") + " " + dtSubtime.ToString("HH:mm:ss") + " 于员工认股权凭证系统行使认股权 " + dEPK10.ToString("###,###,###0.##") + " 股");
            sbBody.AppendLine("认股资料如下： ");
            sbBody.AppendLine("．缴款银行帐号： " + sEPK19);
            sbBody.AppendLine("．行使认股股数： " + dEPK10.ToString("###,###,###0.##") + " 股");
            //sbBody.AppendLine("．Exercise Method：" + new soUtility().getExerciseMethod(hidEPJ37.Value));
            //2013/03/03 Modify 目前只有聯發科有不同的認股行使方式,聯發科沒有簡體版本
            //所以就直接固定給[行使认股权利]
            sbBody.AppendLine("．行使方法：行使认股权利");
            sbBody.AppendLine("．每股金额(台币)： " + dEPK11.ToString("###,###,###0.##"));
            sbBody.AppendLine("．最后缴款日： " + Convert.ToInt32(sEPK38).ToString("####/##/##"));
            sbBody.AppendLine("请依据系统中列印之缴款书，在   " + Convert.ToInt32(sEPK38).ToString("####/##/##") + "  下午3:30前完成缴款。 ");
            sbBody.AppendLine("");
            sbBody.AppendLine("");
            sbBody.AppendLine("敬祝 健康快乐");
            System.Net.Mail.MailMessage oMessage = new System.Net.Mail.MailMessage();
            oMessage.SubjectEncoding = System.Text.Encoding.UTF8;
            oMessage.BodyEncoding = System.Text.Encoding.UTF8;
            oMessage.IsBodyHtml = false;
            oMessage.Subject = "Employee Stock Option Transaction completed notification – Exercise Stock Option  永丰信托网 行使认股权利通知"; //信件標題
            oMessage.Body = sbBody.ToString(); //信件內容
            oMessage.From = new System.Net.Mail.MailAddress("esop@sinopac.com", "ESOP");
            oMessage.To.Add(new System.Net.Mail.MailAddress(sTo_mail, su.UNAME)); //設定主要寄送信箱
            //System.Net.Mail.MailMessage oMessage = new System.Net.Mail.MailMessage();
            //oMessage.SubjectEncoding = System.Text.Encoding.UTF8;
            //oMessage.BodyEncoding = System.Text.Encoding.UTF8;
            //oMessage.IsBodyHtml = false;
            //oMessage.Subject = "Employee Stock Option Transaction completed notification – Exercise Stock Option"; //信件標題
            //oMessage.Body = sbBody.ToString(); //信件內容
            //oMessage.From = new System.Net.Mail.MailAddress("esop@sinopac.com","ESOP");
            //oMessage.To.Add(new System.Net.Mail.MailAddress(sTo_mail , su.UNAME)); //設定主要寄送信箱
            //設定其他收信信箱
            sql = "select PRIEMAIL from EPS042_ADDIT  where EMPID='" + su.IDNO + "' and CMPID='" + su.COMPANY + "'";
            dt = new DataTable();
            dbGo.execQuery(sql, ref dt);
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

            #endregion
        }
        catch (Exception ex)
        {
            errors = ex.Message;
        }
        finally
        {
            dbGo = null;
            sql = string.Empty;
        }


        return errors;
    }

    /// <summary>
    /// 清除隱藏欄位
    /// </summary>
    protected void cleanHideValue()
    {
        hidEPA03.Value = string.Empty;
        hidEPG04.Value = string.Empty;
        hidEPJ02.Value = string.Empty;
        hidEPJ03.Value = string.Empty;
        hidEPJ05.Value = string.Empty;
        hidEPJ12.Value = string.Empty; //每股價格
        hidEPJ13.Value = string.Empty; //手續費
        hidEPJ17.Value = string.Empty;
        hidEPJ22.Value = string.Empty; //國外GFI 
        hidEPJ23.Value = string.Empty; //零股股數 
        hidEPJ37.Value = string.Empty;
        hidEPK10.Value = string.Empty;
        hidEPL02.Value = string.Empty;
        hidEPL03.Value = string.Empty;
        hidLimit.Value = string.Empty;
        txtEPK10a.Text = string.Empty; //行使股數
        lbExerciseMethod.Text = string.Empty;
    }

    /// <summary>
    /// javascript alert windows
    /// </summary>
    /// <param name="message"></param>
    protected void showAlert(string message)
    {
        message = message.Replace("'", "\\'");
        //message = message.Replace("\\","\\\\");
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), DateTime.Now.ToString("HHmmss") + "errorAlrt", "alert('" + message + "')", true);
    }

    #endregion

    #endregion




}
