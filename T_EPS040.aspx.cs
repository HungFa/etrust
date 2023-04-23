using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class eTrust_T_EPS040 : System.Web.UI.Page
{
    //protected static souser su;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (Page.Master.FindControl("lbAPId") != null)
            {
                ((Label)Page.Master.FindControl("lbAPId")).Text = "Personal Information";
                ((Label)Page.Master.FindControl("lbHeaders")).Text = "";
            }

            if (null != Session["soption"])
            {
                souser su;
                su = (souser)Session["soption"];
                lbUserInfo.Text = su.IDNO + " - " + su.UNAME;
                loadEPS040(su.IDNO, su.COMPANY);
                su = null;
            }
            else
            {
                Server.Transfer("SU000.aspx?errors=" + Server.UrlEncode("Session failure"));
            }
        }
    }
    #region --- Method ---

    private void loadEPS040(string IDNO,string CompanyID)
    {
        dbClassGo dbGo = new dbClassGo();
        DataTable dt = new DataTable();
        string strErrors = string.Empty;
        // Modify 2014/03/06 
        string sql = "Select ISNULL(EPX01, CMPID) CMPID, EMPID, IDNO, EMAIL, EMPNAME ";
        sql += " From GIF_EMP a left join EPS251 b on a.CMPID = b.EPX03";
        sql += " Where ISNULL(EPX01, CMPID)='" + CompanyID + "' AND EMPID ='" + IDNO + "' ";
        //string sql = "Select * From GIF_EMP Where CMPID='" + CompanyID + "' AND EMPID ='" + IDNO + "' ";
        //寫log
        new soUtility().AuditLog(CompanyID, IDNO, "T_EPS040", sql, "Q");

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
                    lbName.Text = dt.Rows[0]["EMPNAME"] != null ? dt.Rows[0]["EMPNAME"].ToString() : "N/A"; // Name
                    lbemail.Text = dt.Rows[0]["EMAIL"] != null ? dt.Rows[0]["EMAIL"].ToString() : "N/A"; // e-mail
                    //bCountry.Text = new soUtility().QueryCountryFullName(dt.Rows[0]["EPD19"].ToString().Trim());//Full Country Name
                    txtPemail.Text = QueryPersionalEmail(IDNO, CompanyID);
                    txtPemail.ToolTip = CountPersionalEmail(IDNO, CompanyID); 
                    setButton(txtPemail.Text.Trim().Equals(string.Empty));
                }
            }
        }
        finally
        {
            dbGo = null;
        }

    }

    /// <summary>
    /// 取得個人用e-mail
    /// </summary>
    /// <param name="IDNO">帳號</param>
    /// <param name="Company">公司別</param>
    /// <returns></returns>
    public string QueryPersionalEmail(string IDNO, string Company)
    {
        dbClassGo dbGo = new dbClassGo();
        string sql = "select PRIEMAIL from EPS042_ADDIT b where EMPID=@EMPID and CMPID=@CMPID ";
        string pEmail = string.Empty;

        SqlParameter oEPD04B = new SqlParameter("@EMPID", SqlDbType.VarChar, 21);
        SqlParameter oEPD07B = new SqlParameter("@CMPID", SqlDbType.VarChar, 6);
        oEPD04B.Value = IDNO;
        oEPD07B.Value = Company;
        SqlParameter [] oParams = new SqlParameter [] {oEPD04B,oEPD07B};
        try
        {
            object oEmail = dbGo.execScalar(sql, oParams);
            pEmail = oEmail != null ? oEmail.ToString() : string.Empty;
        }
        finally
        {
            dbGo = null;
        }

        return pEmail;
    }

    public string CountPersionalEmail(string IDNO, string Company)
    {
        dbClassGo dbGo = new dbClassGo();
        string sql = "select count(*) from EPS042_ADDIT b where EMPID=@EMPID and CMPID=@CMPID ";
        string cpEmail = string.Empty;

        SqlParameter oEPD04B = new SqlParameter("@EMPID", SqlDbType.VarChar, 21);
        SqlParameter oEPD07B = new SqlParameter("@CMPID", SqlDbType.VarChar, 6);
        oEPD04B.Value = IDNO;
        oEPD07B.Value = Company;
        SqlParameter[] oParams = new SqlParameter[] { oEPD04B, oEPD07B };
        try
        {
            object oPemail = dbGo.execScalar(sql, oParams);
            cpEmail = oPemail != null ? oPemail.ToString() : string.Empty;
        }
        finally
        {
            dbGo = null;
        }

        return cpEmail;
    }

    /// <summary>
    /// 按紐鎖
    /// </summary>
    /// <param name="islock">當Psersional E-mail有資料 就設定false</param>
    public void setButton(bool islock)
    {
        btAddPEmail.Enabled = islock;
        btChangePEmail.Enabled = !islock;
        btDelPEmail.Enabled = !islock;
    }

    public void EditPersionalEmail(string IDNO, string CompanyID,string P_Email,string Counts)
    {
        string sql = string.Empty;
        dbClassGo dbGo = new dbClassGo();
        SqlParameter oPRIEMAIL = new SqlParameter("PRIEMAIL", SqlDbType.VarChar, 45);
        SqlParameter oEMPID = new SqlParameter("@EMPID", SqlDbType.Char ,10);
        SqlParameter oCMPID = new SqlParameter("@CMPID", SqlDbType.Char, 5);

        oPRIEMAIL.Value = P_Email;
        oEMPID.Value = IDNO;
        oCMPID.Value = CompanyID;
        SqlParameter [] oParams = new SqlParameter[] {oPRIEMAIL,oEMPID,oCMPID};
        if (!Counts.Trim().Equals(string.Empty))
        {
            if (Counts.Trim().Equals("0"))
            {
                sql = "insert into  EPS042_ADDIT  (PRIEMAIL,EMPID,CMPID) values (@PRIEMAIL,@EMPID,@CMPID)";
            }
            else
            {
                sql = "update EPS042_ADDIT set PRIEMAIL=@PRIEMAIL where EMPID=@EMPID and CMPID=@CMPID ";
            }
        }

        try
        {
            dbGo.execNonQuery(sql, oParams);
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditAlert", "alert('Save Successed!!');", true);
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "EditAlert", "alert('Save Failed :" + ex.Message + "');", true);
        }
        finally
        {
            dbGo = null;
        }

    }

    #endregion

    #region --- Behavior ---

    /// <summary>
    /// 新增 Persional E-mail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btAddPEmail_Click(object sender, EventArgs e)
    {
        if (null != Session["soption"])
        {
            souser su;
            su = (souser)Session["soption"];
            EditPersionalEmail(su.IDNO, su.COMPANY, txtPemail.Text, txtPemail.ToolTip);
            loadEPS040(su.IDNO, su.COMPANY);
            su = null;
        }
    }

    /// <summary>
    /// 修改 Persional E-mail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btChangePEmail_Click(object sender, EventArgs e)
    {
        if (null != Session["soption"])
        {
            souser su;
            su = (souser)Session["soption"];
            EditPersionalEmail(su.IDNO, su.COMPANY, txtPemail.Text, txtPemail.ToolTip);
            loadEPS040(su.IDNO, su.COMPANY);
            su = null;
        }
    }

    /// <summary>
    /// 刪除 Persional E-mail
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btDelPEmail_Click(object sender, EventArgs e)
    {
        if (null != Session["soption"])
        {
            souser su;
            su = (souser)Session["soption"];
            EditPersionalEmail(su.IDNO, su.COMPANY, "", txtPemail.ToolTip);
            loadEPS040(su.IDNO, su.COMPANY);
            su = null;
        }
    }

    #endregion

}
