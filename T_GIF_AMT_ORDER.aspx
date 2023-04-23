<%@ Page Title="" Language="C#" MasterPageFile="~/eTrust/etrust.master" AutoEventWireup="true" CodeFile="T_GIF_AMT_ORDER.aspx.cs" Inherits="eTrust_T_GIF_AMT_ORDER" %>

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
                    <strong>Outward Remittance</strong>
                    <asp:Label ID="lbUserInfo" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>  
     <asp:GridView ID="gv_InputConfirm" runat="server" Height="149px" Width="1000px" 
                    AutoGenerateColumns="False" ondatabound="gv_InputConfirm_DataBound" EnableModelValidation="True" >
                    <RowStyle CssClass="tdTxt4" />
     <Columns>
     <asp:TemplateField HeaderText="Cancel Order">
          <ItemTemplate>
          <asp:Button ID="btCancel" runat="server" Width="100px" Text="Cancel" onclick="btCancel_Click" Visible="true"
           CommandArgument='<%#Eval("idno") + "," + Eval("broker_id") + "," + Eval("account") + "," + Eval("TDATE")%>' />
          </ItemTemplate>
          </asp:TemplateField>
     <asp:TemplateField HeaderText="Beneficiary Account Number">
          <ItemTemplate>
              <asp:Label ID="lbBANKNO" runat="server" Width="100px"  Text='<%#Eval("obank") + "-" + Eval("obank_account")%>'></asp:Label>
          </ItemTemplate>
     </asp:TemplateField>
     <asp:BoundField DataField="COUNTRY" HeaderText="Country" 
               ItemStyle-CssClass="tdTxt4n" >
         <ItemStyle CssClass="tdTxt4n" />
          </asp:BoundField>
     <asp:BoundField DataField="OUTCUENCYID" HeaderText="Remittance of Currency"
               ItemStyle-CssClass="tdTxt4n" >
         <ItemStyle CssClass="tdTxt4n" />
          </asp:BoundField>
     <asp:BoundField DataField="AMT" HeaderText="Wire Amount"
               ItemStyle-CssClass="tdTxt4n" >
         <ItemStyle CssClass="tdTxt4n" />
          </asp:BoundField>
     <asp:BoundField DataField="Fee" HeaderText="FEE"
               ItemStyle-CssClass="tdTxt4n" >
         <ItemStyle CssClass="tdTxt4n" />
          </asp:BoundField>
     <asp:BoundField DataField="SUBMIT_TIME" HeaderText="Date of Application"
               ItemStyle-CssClass="tdTxt4n" DataFormatString="{0:s}" >
         <ItemStyle CssClass="tdTxt4n" />
          </asp:BoundField>
     <asp:BoundField DataField="STATUS" HeaderText="Status"
               ItemStyle-CssClass="tdTxt4n" >
         <ItemStyle CssClass="tdTxt4n" />
          </asp:BoundField>
     </Columns>
       <HeaderStyle CssClass="tdTxt2" />
       </asp:GridView>
                        <uc1:wucBoard ID="wucBoard1" runat="server" APID="SU007" SID="A" />
                    <br />
                        <asp:HiddenField ID="hidrealamt" runat="server" />
                        <asp:HiddenField ID="hidfee" runat="server" />
                        <asp:HiddenField ID="hidoutamt" runat="server" />
                        <asp:HiddenField ID="hidcurrency" runat="server" />
                        <asp:HiddenField ID="hidcountry" runat="server" />
                        <asp:HiddenField ID="hidbank" runat="server" />
                        <asp:HiddenField ID="hidacnumber" runat="server"  />
                        <asp:HiddenField ID="hidinst" runat="server" />

       </td>
      </tr>
     </table>
    </ContentTemplate>   
    </asp:UpdatePanel>
</asp:Content>

