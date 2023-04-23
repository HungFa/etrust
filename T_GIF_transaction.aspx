<%@ Page Title="" Language="C#" MasterPageFile="~/eTrust/etrust.master" AutoEventWireup="true" CodeFile="T_GIF_transaction.aspx.cs" Inherits="eTrust_T_GIF_transaction" %>

<%@ Register src="../modules/wucBoard.ascx" tagname="wucBoard" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
<style type="text/css">

        .style2
        {
            color: #FF0000;
        }

    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<asp:UpdateProgress ID="UpdateProgress1" runat="server" 
        AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <img alt="" src="../images/al_loading.gif" 
        style="width: 43px; height: 43px" /> <span class="style2">Now Processing</span>        
        </ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table style="width:100%;">
            <tr>
                <td class="tdTxt3r">
                    <strong>Stock Transaction</strong>
                    <asp:Label ID="lbIDNO" runat="server"></asp:Label>
                    <asp:Label ID="lbName" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
            <td>
            <asp:GridView ID="gv_Main" runat="server" Height="149px" Width="1000px" 
                      AutoGenerateColumns="False" ondatabound="gv_Main_DataBound" 
                          onselectedindexchanged="gv_Main_SelectedIndexChanged">           
                <Columns>
                    <asp:BoundField DataField="Tdate" HeaderText="Submission Date"
                      ItemStyle-CssClass="tdTxt4n" >
                    <ItemStyle CssClass="tdTxt4n" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Quantity" DataFormatString="{0:###,###,###0}" HeaderText="Quantity" 
                      ItemStyle-CssClass="tdTxt4n" >
                    <ItemStyle CssClass="tdTxt4n" />
                    </asp:BoundField>
                    <asp:BoundField DataField="Price" DataFormatString="{0:N2}" HeaderText="Price" 
                      ItemStyle-CssClass="tdTxt4n" >
                    <ItemStyle CssClass="tdTxt4n" />
                    </asp:BoundField>
                    <asp:BoundField DataField="tot_amt" DataFormatString="{0:###,###,###0}" HeaderText="Amount" 
                        SortExpression="tot_amt" 
                      ItemStyle-CssClass="tdTxt4n" >
                    <ItemStyle CssClass="tdTxt4n" />
                    </asp:BoundField>
                    <asp:BoundField DataField="fee" DataFormatString="{0:###,###,###0}" HeaderText="Fee" SortExpression="fee" 
                      ItemStyle-CssClass="tdTxt4n" >
                    <ItemStyle CssClass="tdTxt4n" />
                    </asp:BoundField>
                    <asp:BoundField DataField="tax" DataFormatString="{0:###,###,###0}" HeaderText="Tax" SortExpression="tax" 
                      ItemStyle-CssClass="tdTxt4n" >
                    <ItemStyle CssClass="tdTxt4n" />
                    </asp:BoundField>
                    <asp:BoundField DataField="net_pay" DataFormatString="{0:###,###,###0}" HeaderText="Net Amount" 
                        SortExpression="net_pay" 
                      ItemStyle-CssClass="tdTxt4n" >
                    <ItemStyle CssClass="tdTxt4n" />
                    </asp:BoundField>
                </Columns>
                <HeaderStyle CssClass="tdTxt2" />
                <RowStyle CssClass="tdTxt4n" />
            </asp:GridView>            
                <br />
                <uc1:wucBoard ID="wucBoard1" runat="server" APID="SU010" SID="A" />
            </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

