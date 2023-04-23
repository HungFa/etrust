using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using NPOI.HSSF.UserModel;
using System.IO;

public partial class eTrust_SU020 : System.Web.UI.Page
{
    string[] strHeaders = new string[] { "Subsidiary", "EID#", "Name", "Grant Date", "E-Collection No.", "Exercise Type", "Shares Exercised", "Purchase Price", "Amount Paid", "Book-entry Date", "Closing Price", "Payment Date", "Closing Price", "" };

    protected void Page_Load(object sender, EventArgs e)
    {
        retisterPageFile();
        if (!Page.IsPostBack)
        {
            //加入按鈕鎖定效果
            btn_query.Attributes.Add("onclick", "this.disabled=true;" + ClientScript.GetPostBackEventReference(btn_query, ""));
            //btn_print.Attributes.Add("onclick", "this.disabled=true;" + ClientScript.GetPostBackEventReference(btn_print, ""));
            
            
            if (Page.Master.FindControl("lbAPId") != null)
            {
                ((Label)Page.Master.FindControl("lbAPId")).Text = "The daily/monthly detail of Employee Exercise";
            }

            if (null == Session["soption"])
            {
                Server.Transfer("SU000.aspx?errors=" + Server.UrlEncode("Session failure|PCODE:ox020"));
            }
        }
    }
    


    #region --- Method ---

    #region - 註冊此頁面相關js檔 -

    public void retisterPageFile()
    {
        string script = "Showcalendar('" + tb_BeginDate.ClientID + "','" + tb_EndDate.ClientID + "');";
        ScriptManager.RegisterStartupScript(this, this.GetType(), "loadCalendar", script, true);
        tb_BeginDate.Attributes.Add("readonly", "readonly");
        tb_EndDate.Attributes.Add("readonly", "readonly");
    }

    #endregion

    #region - 找尋最近收盤價 -
    /// <summary>
    /// - 找尋最近收盤價 -
    /// </summary>
    /// <param name="inDate"></param>
    /// <returns></returns>
    public decimal seekPrice(string inDate)
    {
        string sql = "SELECT top (1) CP_Price FROM E_CLOSING_PRICE WHERE CP_STOCK_ID='2454'  AND CAST(CP_DATE AS DECIMAL(8,0)) >=" + inDate + " ORDER BY CP_DATE ";
        dbClassGo dbGo = new dbClassGo();
        try
        {
            object o = dbGo.execScalar(sql);
            return Convert.ToDecimal(o);
        }
        catch
        {
            return 0;
        }

        
    }
    #endregion

    #region - BindData -
    /// <summary>
    /// BindData
    /// </summary>
    /// <param name="sorts"></param>
    public void BindData(string sorts,string sortstype)
    {
        souser su = (souser)Session["soption"];
        dbClassGo dbGo = new dbClassGo();
        string sql = getSqlStatement(dateTransFormat(tb_BeginDate.Text, 1), dateTransFormat(tb_EndDate.Text, 1), sorts, sortstype);
        //寫log
        new soUtility().AuditLog(su.COMPANY, su.IDNO, "SU020", sql, "Q");
        DataTable dt = new DataTable();
        dbGo.execQuery(sql, ref dt);
        Label3.Text = dateTransFormat(tb_BeginDate.Text, 2);
        Label5.Text = dateTransFormat(tb_EndDate.Text, 2);
        lb_s_pdate.Text = DateTime.Now.ToString("yyyy/MM/dd");
        lb_s_pemp_id.Text = su.UNAME;
        gv_list.DataSource = dt;
        gv_list.DataBind();
        divIsEmpty.Visible = false;
        su = null;
    }
 
    #endregion

    #region - 組SQL Statement -
    /// <summary>
    /// 組SQL Statement
    /// </summary>
    /// <returns></returns>
    public string getSqlStatement(string strF_DATE,string strE_DATE, string strSort,string strSortType)
    {
        string sql = string.Empty;
        //------------Statement 主體 -----------
        sql = "select (E040.EPD19+'-'+E040.EPD03) as EPD1903, " //子公司別謹慎
            + "E110.EPK04, " //員工號碼
            + "E040.EPD07, " //姓名
            + "E110.EPK03, " //授與日期
            + "E110.EPK19, " //繳款書編號
            + "CASE E110.EPK37 "//認股型別
            + "WHEN 'A' THEN 'EH' "
            + "WHEN 'B' THEN 'ES' "
            + "WHEN 'C' THEN 'ESC' "
            + "ELSE E110.EPK37 "
            + "END AS EPK37C,"
            + "E110.EPK10, " //認股數
            + "E110.EPK11, " //認購價
            + "E110.EPK14, " //認股繳款金額
            + "TDATE, "//行使出帳日,2010/08/04前抓EPK33-經反應查詢 配合出帳日期 及銀行運作時間 以轉檔日為主
            + "(SELECT CP_Price From E_CLOSING_PRICE WHERE CAST(CP_DATE AS DECIMAL(8,0))=CAST(E110.TDATE AS DECIMAL(8,0)) AND CP_STOCK_ID='2454') AS EOD33, " //行使出帳日收盤價
            + "E110.EPK17, " //繳款日期
            + "(SELECT CP_Price From E_CLOSING_PRICE WHERE CAST(CP_DATE AS DECIMAL(8,0))=E110.EPK17 AND CP_STOCK_ID='2454') AS EOD17, " //繳款日期收盤價
            + "CASE EPK37 "//TAXABLE COMPENSATION
            + "WHEN 'A' THEN Round((CAST((SELECT CP_Price From E_CLOSING_PRICE WHERE CAST(CP_DATE AS DECIMAL(8,0))=E110.EPK17 AND CP_STOCK_ID='2454') AS Decimal(8,4))*E110.EPK10-E110.EPK14),4) "
            + "WHEN 'B' THEN Round((CAST((SELECT CP_Price From E_CLOSING_PRICE WHERE CAST(CP_DATE AS DECIMAL(8,0))=CAST(E110.TDATE AS DECIMAL(8,0)) AND CP_STOCK_ID='2454') AS Decimal(8,4))*E110.EPK10-E110.EPK14),4) "
            + "WHEN 'C' THEN Round((CAST((SELECT CP_Price From E_CLOSING_PRICE WHERE CAST(CP_DATE AS DECIMAL(8,0))=CAST(E110.TDATE AS DECIMAL(8,0)) AND CP_STOCK_ID='2454') AS Decimal(8,4))*E110.EPK10-E110.EPK14),4) "
            + "ELSE 0 END AS TAXABLE "
            //+ "0 AS TAXABLE "
            + "FROM EPS110 E110 "
            + "LEFT JOIN EPS040 E040 ON E040.EPD01=E110.EPK01 AND E040.EPD02=E110.EPK04 ";

        //------------WHERE 條件式 -----------
        sql += " WHERE ((CASE EPK37 WHEN 'A' THEN EPK17 WHEN 'B' THEN EPK33 WHEN 'C' THEN EPK33 END) BETWEEN "
            + strF_DATE + " AND " + strE_DATE + " ) AND E110.EPK01='2454'";

        //------------ 排 序 -----------
        sql += strSort.Trim().Equals(string.Empty) ? "  ORDER BY EPD19,EPD03,EPK19 " : strSort;
        sql += strSortType;
        sql += " ";

        return sql;
    }
    #endregion

    #region - Export Excel by NPOI -

    public void exportExcelbyNPOI(string fileName, string sheetName, string UserName,ref GridView gvTable)
    {
        //declare all need variable
        HSSFWorkbook myWorkBook = new HSSFWorkbook();
        HSSFSheet mySheet;
        HSSFRow myRow = new HSSFRow();
        //HSSFCell myCell;
        int iCloumLimit = 0, iRowLimit = 0, iTitlerows = 7;
        NPOI.HSSF.Util.Region myRegion = new NPOI.HSSF.Util.Region();
        MemoryStream memStream = new MemoryStream();

        //set Style

        //Note Font
        HSSFFont noteFont = myWorkBook.CreateFont();
        noteFont.FontName = "新細明體";
        noteFont.Color = NPOI.HSSF.Util.HSSFColor.BLUE.index;
        noteFont.FontHeightInPoints = 16;
        noteFont.FontHeight = 16;

        //Normal Font
        HSSFFont normalFont = myWorkBook.CreateFont();
        normalFont.FontName = "新細明體";
        normalFont.Color = NPOI.HSSF.Util.HSSFColor.AUTOMATIC.index;
        normalFont.FontHeight = 12;
        normalFont.FontHeightInPoints = 12;

        //set the rows and cloum limit of the gridview
        iCloumLimit = gvTable.Columns.Count;
        iRowLimit = gvTable.Rows.Count;

        mySheet = myWorkBook.CreateSheet(sheetName);
        //set the title
        myRow = mySheet.CreateRow(1);
        myRow.CreateCell(0).SetCellValue("員工行使情形明細報表 The detail of Employee Exercise");
        myRow.GetCell(0).CellStyle.SetFont(noteFont);

        myRow.CreateCell(11).SetCellValue("列印日期" + DateTime.Now.ToString("yyyy/MM/dd"));


        myRow = mySheet.CreateRow(2);
        myRow.CreateCell(0).SetCellValue("聯發科技股份有限公司");
        myRow.GetCell(0).CellStyle.SetFont(noteFont);
        myRow.CreateCell(11).SetCellValue("列印人員：" + UserName);

        myRow = mySheet.CreateRow(3);
        myRow.CreateCell(0).SetCellValue("員工認股權憑證");
        myRow.GetCell(0).CellStyle.SetFont(noteFont);

        //Load Gridview CloumHeader
        for (int iCloum = 0; iCloum < iCloumLimit; iCloum++)
        {
            mySheet.CreateRow(iTitlerows - 2).CreateCell(iCloum).SetCellValue(strHeaders[iCloum]);//設定title
            mySheet.CreateRow(iTitlerows - 1).CreateCell(iCloum).SetCellValue(gvTable.Columns[iCloum].HeaderText);//設定title
            mySheet.SetColumnWidth(iCloum, 17 * 256);//設定欄寬
        }

        //load Gridview Data
        for (int iRow = iTitlerows; iRow < (iRowLimit + iTitlerows); iRow++)
        {
            for (int iCell = 0; iCell < iCloumLimit; iCell++)
            {
                mySheet.CreateRow(iRow).CreateCell(iCell).SetCellValue(gvTable.Rows[iRow - iTitlerows].Cells[iCell].Text.Replace("&nbsp;", ""));
                mySheet.GetRow(iRow).GetCell(iCell).CellStyle.SetFont(normalFont);
            }

        }
        myWorkBook.Write(memStream);
        Response.AddHeader("Content-Disposition", String.Format("attachment; filename=" + fileName));
        Response.BinaryWrite(memStream.ToArray());


        //release the resource
        myWorkBook = null;
        memStream.Close();
        memStream.Dispose();

    }
    #endregion

    #region - 彈出警示 視窗 -
    /// <summary>
    /// javascript alert windows
    /// </summary>
    /// <param name="message"></param>
    protected void showAlert(string message)
    {
        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), DateTime.Now.ToString("HHmmss") + "errorAlrt", "alert('" + message + "')", true);
    }
    #endregion

    #region - DateTranFormat -
    /// <summary>
    /// - DateTranFormat -
    /// </summary>
    /// <param name="temp"></param>
    /// <returns></returns>
    public string dateTransFormat(string temp, int itype)
    {
        DateTime dTime = DateTime.ParseExact(temp, "yyyy/MM/dd", null, System.Globalization.DateTimeStyles.AllowWhiteSpaces);
        switch (itype)
        {
            case 1:
                return dTime.ToString("yyyyMMdd");
                //break;
            case 2:
                return dTime.ToString("yyyy/MM/dd");
                //break;
            default:
                return temp;
                //break;
        }

    }
    #endregion 

    #endregion

    #region --- Behavior ---

    protected void btn_query_Click(object sender, EventArgs e)
    {
        if (tb_BeginDate.Text.Trim().Equals(string.Empty)|| tb_EndDate.Text.Trim().Equals(string.Empty))
        {
            showAlert("Date can not be empty");
            return;
        }
        //----- 讀取
        BindData("","");
    }
    protected void gv_list_RowCreated(object sender, GridViewRowEventArgs e)
    {

    }
    protected void gv_list_Sorting(object sender, GridViewSortEventArgs e)
    {
        string sorts = e.SortExpression;
        hideSortType.Value = hideSortType.Value.Trim().Equals(string.Empty) ? " ASC " : hideSortType.Value.Trim().Equals("ASC") ? " DESC " : " ASC ";
        BindData("ORDER BY EPD1903," + sorts, hideSortType.Value);
    }
    protected void gv_list_DataBound(object sender, EventArgs e)
    {
        foreach (GridViewRow gvr in gv_list.Rows)
        {
            if (gvr.RowType == DataControlRowType.DataRow)
            {
                gvr.Cells[9].Text = gvr.Cells[9].Text.Trim().Equals("&nbsp;") ? "N/A" : gvr.Cells[9].Text;
                gvr.Cells[10].Text = gvr.Cells[10].Text.Trim().Equals("&nbsp;") ? "N/A" : gvr.Cells[10].Text;
                gvr.Cells[11].Text = gvr.Cells[11].Text.Trim().Equals("0") ? "N/A" : gvr.Cells[11].Text;
                gvr.Cells[12].Text = gvr.Cells[12].Text.Trim().Equals("&nbsp;") ? "N/A" : gvr.Cells[12].Text;
                if (gvr.Cells[13].Text.Trim().Equals("&nbsp;"))
                {
                    decimal dCP = 0;
                    decimal dStocks = 0;
                    decimal dStockPay = 0;
                    decimal dTaxable = 0;
                    dStocks = gvr.Cells[6].Text.Equals("&nbsp;") ? 0 : Convert.ToDecimal(gvr.Cells[6].Text.Trim().Replace(",", ""));
                    dStockPay = gvr.Cells[8].Text.Equals("&nbsp;") ? 0 : Convert.ToDecimal(gvr.Cells[8].Text.Trim().Replace(",", ""));
                    if (gvr.Cells[5].Text.Trim().Equals("EH"))
                    {
                        dCP = seekPrice(gvr.Cells[11].Text.Trim());
                        gvr.Cells[12].Text = dCP.ToString();
                        dTaxable = dStocks * dCP - dStockPay;
                    }
                    else
                    {
                        dCP = seekPrice(gvr.Cells[9].Text.Trim());
                        gvr.Cells[10].Text = dCP.ToString();
                        dTaxable = dStocks * dCP - dStockPay;
                    }
                    gvr.Cells[13].Text = dTaxable.ToString("###,###,###0.####");
                }
            }
        }
    }
    protected void btn_print_Click(object sender, EventArgs e)
    {
        if(null!=Session["soption"])
        {
            if (gv_list.Rows.Count < 1)
            {
                showAlert("無資料可匯出");
                return;
            }
            souser su = (souser)Session["soption"];
            exportExcelbyNPOI(DateTime.Now.ToLongDateString() + "_report.xls", "員工行使情形明細報表", su.UNAME, ref gv_list);
            su = null;
        }
    }
    
    #endregion  
}