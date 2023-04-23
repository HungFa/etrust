<%@ Page Title="" Language="C#" MasterPageFile="~/eTrust/etrust.master" AutoEventWireup="true"
    CodeFile="T_EPS110S.aspx.cs" Inherits="eTrust_T_EPS110S" %>

<%@ Register Src="../modules/wucBoard.ascx" TagName="wucBoard" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
        <div style="text-align:left;" >

    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <img alt="" src="../images/al_loading.gif" style="width: 43px; height: 43px" />
            <span class="style2">Now Processing</span></ProgressTemplate>
    </asp:UpdateProgress>
             </div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table style="width: 100%;">
                <tr>
                    <td class="tdTxt3r">
                        <strong>Exercise History / How to pay</strong> &nbsp;
                        <asp:Label ID="lbUserInfo" runat="server"></asp:Label>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:MultiView ID="mainViews" runat="server">
                            <asp:View ID="viewDataList" runat="server">
                                <asp:GridView ID="gvMain" runat="server" AutoGenerateColumns="False" EnableModelValidation="True"
                                    OnDataBound="gvMain_DataBound">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Cancel Order">
                                            <ItemTemplate>
                                                <asp:Button ID="btGVcancel" runat="server" Text="Cancel" Visible="false" OnClick="btGVcancel_Click" />
                                                <br />
                                                <asp:HiddenField ID="hideEPK02" runat="server" Value='<%# Eval("EPK02")%>' />
                                                <asp:HiddenField ID="hideEPK03" runat="server" Value='<%# Eval("EPK03")%>' />
                                                <asp:HiddenField ID="hideEPK05" runat="server" Value='<%# Eval("EPK05")%>' />
                                                <asp:HiddenField ID="hideEPK10" runat="server" Value='<%# Eval("EPK10")%>' />
                                                <asp:HiddenField ID="hideEPK15" runat="server" Value='<%# Eval("EPK15")%>' />
                                                <asp:HiddenField ID="hideEPK26" runat="server" Value='<%# Eval("EPK26")%>' />
                                                <asp:HiddenField ID="hideEPJ22" runat="server" Value='<%# Eval("EPJ22")%>' />
                                                <asp:HiddenField ID="hideEPK37" runat="server" Value='<%# Eval("EPK37")%>' />
                                                <asp:HiddenField ID="hideEPK38" runat="server" Value='<%# Eval("EPK38")%>' />
                                                <asp:HiddenField ID="hideStatus" runat="server" Value='<%# Eval("status")%>' />
                                                <asp:HiddenField ID="hideOnlinePay" runat="server" Value='<%# Eval("online_payment")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="How to Pay">
                                            <ItemTemplate>
                                                <asp:Button ID="btGVhowtopay" runat="server" Text="How to pay" Visible="false" OnClick="btGVhowtopay_Click" CommandArgument='<%# Eval("EPK02")+","+Eval("EPK03")+","+Eval("EPK05")+","+Eval("EPK10") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField HeaderText="E-Collection No." DataField="EPK19" />
                                        <asp:BoundField HeaderText="Grant Date" DataField="EPK03" DataFormatString="{0:####/##/##}" />
                                        <asp:BoundField HeaderText="Submission date" DataField="EPK08" DataFormatString="{0:####/##/##}" />
                                        <asp:BoundField HeaderText="Exercise Method" DataField="EPK37" />
                                        <asp:BoundField HeaderText="Exercise Shares (A)" DataField="EPK10" DataFormatString="{0:###,###,###0}" />
                                        <asp:BoundField HeaderText="Exercise Price (B)" DataField="EPK11" DataFormatString="{0:###,###,###0.00}" />
                                        <asp:BoundField HeaderText="Total Exercise Amount (A)×(B)" DataField="EPK12" DataFormatString="{0:###,###,###0}" />
                                        <asp:BoundField HeaderText="Due Date of Payment" DataField="EPK38" DataFormatString="{0:####/##/##}" />
                                        <asp:BoundField HeaderText="Amount Paid" DataField="EPK21" DataFormatString="{0:###,###,###0}" />
                                        <asp:BoundField HeaderText="Payment Date" DataField="EPK26" DataFormatString="{0:####/##/##}" />
                                        <asp:BoundField HeaderText="Shares Forfeited" DataField="GIVEUP" />
                                        <asp:BoundField HeaderText="Date of certificate transferred to custody account" DataField="EPK22"
                                            DataFormatString="{0:####/##/##}" />
                                        <asp:BoundField HeaderText="Stock's closing price on the date of certificate transferred (C)"
                                            DataField="EPK30" DataFormatString="{0:###,###,###0.00}" />
                                    </Columns>
                                    <HeaderStyle CssClass="tdTxt2" />
                                    <RowStyle CssClass="tdTxt4n" />
                                </asp:GridView>
                            </asp:View>
                            <asp:View ID="viewHowToPay" runat="server">
                                <table style="width: 100%;">
                                    <tr>
                                        <td class="tdTxt4n" align="center">
                                            <asp:Button ID="btSubscription" runat="server" Text="Subscription Form" 
                                                onclick="btSubscription_Click" />
                                        </td>
                                        <td class="tdTxt4n" align="center">
                                            Payment
                                        </td>
                                        <td class="tdTxt4n">
                                            <uc1:wucBoard ID="wucBoard1" runat="server" APID="SU005" SID="B" />
                                        </td>
                                    </tr>
                                </table>
                                <div align="center">
                                    <asp:Button ID="btPaymentBack" runat="server" Text="Back" 
                                        onclick="btPaymentBack_Click" />
                                </div>
                            </asp:View>
                            <asp:View ID="viewPrint" runat="server">
                                <table style="width: 100%;">
                                    <tr>
                                        <td class="tdTxt4n" align="center">
                                            <table style="width: 100%;">
                                                <tr>
                                                    <td class="tdTxt4n" align="center">
                                                        <asp:Label ID="lbCompanyIDPrint" runat="server"></asp:Label>
                                                    </td>
                                                    <td class="tdTxt4n" align="center">
                                                        <asp:Label ID="lbCompanyNamePrint" runat="server"></asp:Label><br />
                                                        Notice to Exercise Stock Option<br />
                                                        <asp:Label ID="lbTitlePrint" runat="server"></asp:Label><br />
                                                    </td>
                                                    <td class="tdTxt4n" align="center">
                                                        <asp:Label ID="lbSeriaNoPrint" runat="server"></asp:Label><br />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                        </asp:MultiView>
                    </td>
                </tr>
                <tr>
                    <td>
                        <uc1:wucBoard ID="wucBoard2" runat="server" APID="SU005" SID="A" />
                        <br />
                        <asp:HiddenField ID="hideEPD07B" runat="server" />
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
