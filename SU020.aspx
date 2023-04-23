<%@ Page Title="" Language="C#" MasterPageFile="~/eTrust/etrust.master" AutoEventWireup="true"
    CodeFile="SU020.aspx.cs" Inherits="eTrust_SU020" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <link href="../css/jquery.ui.all.css" rel="stylesheet" type="text/css" />
    <script type="text/javascript" src="../js/jquery-1.4.2.min.js"></script>
    <script type="text/javascript" src="../js/jquery-ui-1.8.1.custom.min.js"></script>
    <script type="text/javascript" language="javascript">
        function Showcalendar(obj1, obj2) {
            $("#" + obj1 + "").datepicker({ showOn: 'both', showOtherMonths: true,
                showWeeks: true, firstDay: 1, changeFirstDay: false, changeMonth: true,
                changeYear: true, buttonImageOnly: true, buttonImage: '../images/calendar.gif', dateFormat: 'yy/mm/dd'
            });
            $("#" + obj2 + "").datepicker({ showOn: 'both', showOtherMonths: true,
                showWeeks: true, firstDay: 1, changeFirstDay: false, changeMonth: true,
                changeYear: true, buttonImageOnly: true, buttonImage: '../images/calendar.gif',dateFormat: 'yy/mm/dd'
            });
        }
    </script>
    <table style="width: 100%;" border="0" cellpadding="0" cellspacing="0" width="100%">
        <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
            <ProgressTemplate>
                <img alt="" src="../images/al_loading.gif" style="width: 43px; height: 43px" />
                <span class="style2">Now Processing</span>
            </ProgressTemplate>
        </asp:UpdateProgress>
        &nbsp;&nbsp; </td> </tr>
        <tr style="color: #000000">
            <td>
                <!--Page Title End-->
                <!-- Search Panel Start -->
                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <table class="tableTxt" cellspacing="0" cellpadding="0" width="100%" align="center"
                            border="0">
                            <tbody>
                                <tr>
                                    <td style="width: 20%" class="tdTxt3">
                                        <asp:Label ID="lb_Date" runat="server"></asp:Label>
                                    </td>
                                    <td class="tdTxt3" style="width: 210px">
                                        <span style="font-size: 13px; color: #ffffff; background-color: #00a6dd"></span>
                                        <asp:TextBox ID="tb_BeginDate" TabIndex="2" runat="server" Width="65px" MaxLength="10"></asp:TextBox>
                                        <span id="spCalendar2" runat="server">
                                            <asp:Label ID="Label2" runat="server">~</asp:Label>
                                            <asp:TextBox ID="tb_EndDate" TabIndex="3" runat="server" Width="65px" MaxLength="10"></asp:TextBox>
                                        </span>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" width="50%" class="tdTxt3" colspan="2">
                                        <asp:Button ID="btn_query" TabIndex="6" OnClick="btn_query_Click" runat="server"
                                            Text="Query"></asp:Button>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <!-- Search Panel End -->
                <br />
            </td>
        </tr>
        <tr style="color: #000000">
            <td>
                <table align="center" bgcolor="white" border="0" cellpadding="0" cellspacing="0"
                        width="100%"  runat="server">
                        <tr>
                            <td align="right" class="tdTxt4n">
                                <div id="divExport" runat="server" style="text-align: right;">
                                    <asp:Button ID="btn_print" runat="server" Text="Export Excel" OnClick="btn_print_Click"
                                        Style="text-align: center" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td align="right" class="tdTxt4n">
                                <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                    <ContentTemplate>
                                        <!-- Report Title Start -->
                                        <div id="divHeader" runat="server" visible="false">
                                            <table id="Table1" align="center" bgcolor="white" border="0" cellpadding="0" cellspacing="0"
                                                width="100%" class="tableTxt" runat="server">
                                                <tr>
                                                    <td align="left" bgcolor="#00a6dd" class="tdTxt3" style="width: 460px; height: 22px"
                                                        rowspan="2">
                                                        <span style="color: #0066cc">Date：</span><asp:Label ID="Label3" runat="server" Width="60px"></asp:Label>&nbsp;&nbsp;
                                                        <asp:Label ID="Label4" runat="server" Text="~"></asp:Label>&nbsp;&nbsp;
                                                        <asp:Label ID="Label5" runat="server" Width="60px"></asp:Label>&nbsp;&nbsp;
                                                    </td>
                                                    <td align="right" bgcolor="#00a6dd" class="tdTxt3" rowspan="2">
                                                        <span style="color: #0066cc">Print Date：</span>
                                                        <asp:Label ID="lb_s_pdate" runat="server"></asp:Label>
                                                        &nbsp;
                                                        <br />
                                                        <br>
                                                        <span style="color: #0066cc">User：</span>
                                                        <asp:Label ID="lb_s_pemp_id" runat="server"></asp:Label>&nbsp;
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <!-- Report Title End -->
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" >
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                    <ContentTemplate>
                                        <asp:GridView ID="gv_list" runat="server" AutoGenerateColumns="False" AllowSorting="True"
                                            OnRowCreated="gv_list_RowCreated" OnSorting="gv_list_Sorting" Width="99%" OnDataBound="gv_list_DataBound">
                                            <Columns>
                                                <asp:BoundField DataField="EPD1903" HeaderText="子公司別" SortExpression="EPD1903" FooterText="合計" />
                                                <asp:BoundField DataField="EPK04" HeaderText="員工號碼" SortExpression="EPK04" />
                                                <asp:BoundField DataField="EPD07" HeaderText="姓名" SortExpression="EPD07" />
                                                <asp:BoundField DataField="EPK03" HeaderText="授與日期" SortExpression="EPK03" />
                                                <asp:BoundField DataField="EPK19" HeaderText="繳款書編號" SortExpression="EPK19" />
                                                <asp:BoundField DataField="EPK37C" HeaderText="認股型別" SortExpression="EPK37C" />
                                                <asp:BoundField DataField="EPK10" DataFormatString="{0:###,##0.####}" HeaderText="認股數"
                                                    SortExpression="EPK10" />
                                                <asp:BoundField DataField="EPK11" DataFormatString="{0:###,##0.####}" HeaderText="認購價"
                                                    SortExpression="EPK11" />
                                                <asp:BoundField DataField="EPK14" HeaderText="認股繳款金額" SortExpression="EPK14" DataFormatString="{0:###,##0.####}" />
                                                <asp:BoundField DataField="TDATE" HeaderText="行使出帳日" SortExpression="EPK33" />
                                                <asp:BoundField DataField="EOD33" HeaderText="行使出帳日收盤價" SortExpression="EPK30" DataFormatString="{0:###,##0.####}" />
                                                <asp:BoundField DataField="EPK17" HeaderText="繳款日期" SortExpression="EPK17" />
                                                <asp:BoundField DataField="EOD17" HeaderText="繳款日期收盤價" SortExpression="EPK31" DataFormatString="{0:###,##0.####}" />
                                                <asp:BoundField DataField="TAXABLE" HeaderText="Taxable compensation" SortExpression="TAXABLE"
                                                    DataFormatString="{0:###,##0.####}" />
                                            </Columns>
                                            <EmptyDataTemplate>
                                                <div class="tdTxt2" style="text-align:center;">
                                                    查無資料</div>
                                            </EmptyDataTemplate>
                                            <HeaderStyle Font-Size="10pt" CssClass="tdTxt2" />
                                            <RowStyle CssClass="tdTxt4" />
                                        </asp:GridView>
                                        <br />
                                        <asp:HiddenField ID="hideSortType" runat="server" />
                                        <asp:Label ID="lb_description" runat="server" Text="" Font-Size="Small"></asp:Label><br />
                                        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                                        <div id="divIsEmpty" class="tdTxt2" style="text-align:center;" runat="server" >
                                                    查無資料</div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </table>
            </td>
        </tr>
        <tr style="color: #000000">
            <!-- Report Body Start -->
            <td align="center">
            </td>
            <!-- Report Body End -->
        </tr>
    </table>
</asp:Content>
