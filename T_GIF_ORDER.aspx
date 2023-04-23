<%@ Page Title="" Language="C#" MasterPageFile="~/eTrust/etrust.master" AutoEventWireup="true" CodeFile="T_GIF_ORDER.aspx.cs" Inherits="eTrust_T_GIF_ORDER" %>
<%@ Register Src="../modules/wucBoard.ascx" TagName="wucBoard" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
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
                                <strong>Stock Sell Order</strong> &nbsp;
                                <asp:Label ID="lbUserInfo" runat="server"></asp:Label>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:GridView ID="gvMain" runat="server" AutoGenerateColumns="False" EnableModelValidation="True"
                                    Style="text-align: center; margin-top: 2px" OnDataBound="gvMain_DataBound">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Delete Sell Order">
                                            <ItemTemplate>
                                                <asp:Button ID="btDelete" runat="server" Text="Delete Sell Order" Visible="false"
                                                    onclick="btDelete_Click" />
                                                <br />
                                                <asp:HiddenField ID="hideInv_type" runat="server" Value='<%# Eval("Inv_type")%>' />
                                                <asp:HiddenField ID="hidePrice" runat="server" Value='<%# Eval("price")%>' />
                                                <asp:HiddenField ID="hideBrokerID" runat="server" Value='<%# Eval("broker_id")%>' />
                                                <asp:HiddenField ID="hideAccount" runat="server" Value='<%# Eval("account")%>' />
                                                <asp:HiddenField ID="hideSubmitTime" runat="server" Value='<%# Eval("submit_time")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="E-Collection No.">
                                            <ItemTemplate>
                                                <asp:Label ID="lbEcollectionNo" runat="server" Text='<%# Eval("ecollectionno")%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Security Code">
                                            <ItemTemplate>
                                                <asp:Label ID="lbSecurityCode" runat="server" Text='<%# Eval("stock_no")%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Price">
                                            <ItemTemplate>
                                                <asp:Label ID="lbPrice" runat="server" Text='<%# Eval("use_market_price")%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Share">
                                            <ItemTemplate>
                                                <asp:Label ID="lbShare" runat="server" Text='<%# Eval("quantity")%>' ></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Status of your sell order">
                                            <ItemTemplate>
                                                <asp:Label ID="lbStatus" runat="server" Text='<%# Eval("status")%>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Submit Time">
                                            <ItemTemplate>
                                                <asp:Label ID="lbSubmitTime" runat="server" Text='<%#Eval("submit_time")%>'></asp:Label>
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
                                <uc1:wucBoard ID="wucBoard1" runat="server" APID="SU009" SID="A" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
</asp:Content>

