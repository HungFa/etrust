<%@ Page Title="" Language="C#" MasterPageFile="~/eTrust/etrust.master" AutoEventWireup="true"
    CodeFile="T_GIF_INV.aspx.cs" Inherits="eTrust_T_GIF_INV" %>

<%@ Register Src="../modules/wucBoard.ascx" TagName="wucBoard" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <img alt="" src="../images/al_loading.gif" style="width: 43px; height: 43px" />
            <span class="style2">Now Processing</span> </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <table style="width: 100%;">
                        <tr>
                            <td class="tdTxt3r">
                                <strong>Balance of Shares</strong> &nbsp;
                                <asp:Label ID="lbUserInfo" runat="server"></asp:Label>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:GridView ID="gvMain" runat="server" AutoGenerateColumns="False" EnableModelValidation="True"
                                    Style="text-align: center; margin-top: 2px" OnDataBound="gvMain_DataBound">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Security Code">
                                            <ItemTemplate>
                                                <asp:Label ID="lbSecuCode" runat="server" Text='<%# Eval("stock_no")%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="E-Collection No.">
                                            <ItemTemplate>
                                                <asp:Label ID="lbEcollectionNo" runat="server" Text='<%# Eval("ecollectionNo")%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Balance of Shares">
                                            <ItemTemplate>
                                                <asp:Label ID="lbBlanceShares" runat="server" Text='<%# Eval("quantity1")%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Reference Price">
                                            <ItemTemplate>
                                                <asp:Label ID="lbRefPrice" runat="server" Text='<%#Eval("Cprice")%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Lots Sold">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtLotsShares" runat="server" Width="50px" Visible="false"></asp:TextBox>
                                                <asp:Label ID="lbLotsShares" runat="server" Text="000 Shares" Visible="false"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Odd Lots Sold">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtOddLotShares" runat="server" Width="50px" Text='<%# Eval("qty")%>'
                                                    Visible="false"></asp:TextBox>
                                                &nbsp;shares
                                                <asp:RadioButtonList ID="rbOddList" runat="server" RepeatDirection="Horizontal" Visible="false">
                                                    <asp:ListItem Value="Y">YES</asp:ListItem>
                                                    <asp:ListItem Value="N" Selected="True">NO</asp:ListItem>
                                                </asp:RadioButtonList>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Sell Price">
                                            <ItemTemplate>
                                                <table style="width: 100%;">
                                                    <tr>
                                                        <td style="text-align: left">
                                                            <asp:RadioButton ID="rbSellPriceY" runat="server" Text="Limit price" Checked="true"
                                                                GroupName="rbSellPrice" Style="text-align: right" />
<!--
                                                            <br />
                                                            <asp:RadioButton ID="rbSellPriceN" runat="server" Text="Market price" GroupName="rbSellPrice"
                                                                tyle="text-align: left" />
-->
                                                        </td>
                                                        <td valign="top">
                                                            <asp:TextBox ID="txtLimitPrice" runat="server" Width="30px"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Confirm to Submit">
                                            <ItemTemplate>
                                                <asp:Button ID="btSubmit" runat="server" Text="Submit" OnClick="btSubmit_Click" />
                                                <asp:HiddenField ID="hideStock_No" runat="server" Value='<%# Eval("stock_no")%>' />
                                                <asp:HiddenField ID="hideBroker_Id" runat="server" Value='<%# Eval("broker_id")%>' />
                                                <asp:HiddenField ID="hideAccount" runat="server" Value='<%# Eval("account")%>' />
                                                <asp:HiddenField ID="hideQuantity1" runat="server" Value='<%# Eval("quantity1")%>' />
                                                <asp:HiddenField ID="hideQuantity" runat="server" Value='<%# Eval("quantity")%>' />
                                                <asp:HiddenField ID="hideShare" runat="server" Value="No" />
                                                <asp:HiddenField ID="hideShs" runat="server" Value="No" />
                                                <asp:HiddenField ID="hideQty" runat="server" Value='<%# Eval("qty")%>' />
                                                <asp:HiddenField ID="hideEcollectionNo" runat="server" Value='<%# Eval("ecollectionNo")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <HeaderStyle CssClass="tdTxt2" />
                                    <RowStyle CssClass="tdTxt4n" />
                                </asp:GridView>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <uc1:wucBoard ID="wucBoard1" runat="server" APID="SU008" SID="A" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
</asp:Content>
