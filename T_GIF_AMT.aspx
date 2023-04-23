<%@ Page Title="" Language="C#" MasterPageFile="~/eTrust/etrust.master" AutoEventWireup="true"
    CodeFile="T_GIF_AMT.aspx.cs" Inherits="eTrust_T_GIF_AMT" %>

<%@ Register Src="../modules/wucBoard.ascx" TagName="wucBoard" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .style2
        {
            color: #FF0000;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <img alt="" src="../images/al_loading.gif" style="width: 43px; height: 43px" />
            <span class="style2">Now Processing</span>
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table style="width: 100%;">
                <tr>
                    <td class="tdTxt3r">
                        <strong>To Outward Remittance</strong>
                        <asp:Label ID="lbUserInfo" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:GridView ID="gv_Outward" runat="server" AutoGenerateColumns="False" EnableModelValidation="True"
                            Height="149px" OnDataBound="gv_Outward_DataBound" Width="1000px">
                            <Columns>
                                <asp:BoundField DataField="real_amt" DataFormatString="{0:###,###,###0}" HeaderText="Account Balance"
                                    ItemStyle-CssClass="tdTxt4n">
                                    <ItemStyle CssClass="tdTxt4n" />
                                </asp:BoundField>
                                <asp:BoundField DataField="fee" DataFormatString="{0:###,###,###0}" HeaderText="Fee"
                                    ItemStyle-CssClass="tdTxt4n">
                                    <ItemStyle CssClass="tdTxt4n" />
                                </asp:BoundField>
                                <asp:TemplateField HeaderText="Wire Amount">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtOUTAMT" runat="server" CssClass="tdTxt4n" MaxLength="14" Text='<%#Eval("out_amts") %>' Width="100px"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Remittance of Currency">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="ddlCURRENCY" runat="server" AutoPostBack="true" Width="100px">
                                            <asp:ListItem Value="USD">USD U.S.Dollar</asp:ListItem>
                                            <asp:ListItem Value="AUD">AUD Australian Dollar</asp:ListItem>
                                            <asp:ListItem Value="CAD">CAD Canadian Dollar</asp:ListItem>
                                            <asp:ListItem Value="CHF">CHF Swiss Franc</asp:ListItem>
                                            <asp:ListItem Value="GBP">GBP Pound Sterling</asp:ListItem>
                                            <asp:ListItem Value="HKD">HKD Hong Kong Dollar</asp:ListItem>
                                            <asp:ListItem Value="JPY">JPY Yen</asp:ListItem>
                                            <asp:ListItem Value="NZD">NZD New Zealand Dollar</asp:ListItem>
                                            <asp:ListItem Value="SEK">SEK Swedish Krona</asp:ListItem>
                                            <asp:ListItem Value="SGD">SGD Singapore Dollar</asp:ListItem>
                                            <asp:ListItem Value="ZAR">ZAR Rand</asp:ListItem>
                                            <asp:ListItem Value="EUR">EUR EURO</asp:ListItem>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Country">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtCOUNTRY" runat="server" Text='<%#Eval("country") %>'
                                            CssClass="tdTxt4n" MaxLength="50" Width="100px"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Beneficiary Bank">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtBANK" runat="server" Text='<%#Eval("bank_name") %>'
                                            CssClass="tdTxt4n" MaxLength="50" Width="100px"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Beneficiary Account Number">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtACNUMBER" runat="server" Text='<%#Eval("account_no") %>'
                                            CssClass="tdTxt4n" MaxLength="14" Width="100px"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Swift Code / ABA number / Message or Instructions ">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtINST" runat="server" Text='<%#Eval("description") %>'
                                            CssClass="tdTxt4n" MaxLength="50" Width="100px"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Submit">
                                    <ItemTemplate>
                                        <asp:Button ID="btConfirmSubmit" runat="server" CommandArgument='<%#Eval("idno") + "," + Eval("broker_id") + "," + Eval("account")+","+ (iGvRowCount++).ToString() %>'
                                            OnClick="btConfirmSubmit_Click" Text="Confirm to submit" Visible="false" Width="100px" />
                                        <br />
                                        <asp:HiddenField ID="hideBroker_ID" runat="server" Value='<%#Eval("broker_id") %>' />
                                        <asp:HiddenField ID="hideAccount" runat="server" Value='<%#Eval("account") %>' />
                                        <asp:HiddenField ID="hideAmt_Limit" runat="server" Value='<%#Eval("real_amt") %>' />
                                        <asp:HiddenField ID="hideStock_No" runat="server" Value='<%#Eval("stock_no") %>' />
                                        <asp:HiddenField ID="hideFee" runat="server" Value='<%#Eval("fee") %>' />
                                        <asp:HiddenField ID="hideAccount_Name" runat="server" Value='<%#Eval("EMPNAME") %>' />
                                        <asp:HiddenField ID="hideAcct_Name" runat="server" Value='<%#Eval("ACCOUNT_NAME") %>' />
                                        <asp:HiddenField ID="hideOutAmts" runat="server" Value='<%#Eval("out_amts") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                        <br />
                        <uc1:wucBoard ID="wucBoard1" runat="server" APID="SU006" SID="A" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
