<%@ Page Title="" Language="C#" MasterPageFile="~/eTrust/etrust.master" AutoEventWireup="true" CodeFile="T_EPS105A.aspx.cs" Inherits="eTrust_T_EPS105A" %>

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
                    <strong>Grant Information</strong>
                    <asp:Label ID="lbUserInfo" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:MultiView ID="MultiView1" runat="server">
   <asp:View ID="View1" runat="server">
   <asp:GridView ID="gv_ExPrice" runat="server" Height="149px" Width="1000px" 
                    AutoGenerateColumns="False">
     <Columns>
     <asp:BoundField DataField="DT" HeaderText="Date" 
           ItemStyle-CssClass="tdTxt4n" >
         <ItemStyle CssClass="tdTxt4n" />
         </asp:BoundField>
         <asp:BoundField DataField="EP" DataFormatString="{0:N2}" HeaderText="Exercise Price" 
           ItemStyle-CssClass="tdTxt4n" >
         <ItemStyle CssClass="tdTxt4n" />
         </asp:BoundField>    
     </Columns>
   </asp:GridView>
       <div style="text-align:center;" > <asp:Button ID="btV1Back" runat="server" 
               Text="Back" onclick="btV1Back_Click" /></div>
   </asp:View>
   <asp:View ID="View3" runat="server">
   <asp:GridView ID="gv_Vest" runat="server" Height="149px" Width="1000px" 
                    AutoGenerateColumns="False">
     <Columns>
     <asp:BoundField DataField="EPJ05" HeaderText="Vesting Date" DataFormatString="{0:####/##/##}"
           ItemStyle-CssClass="tdTxt4n" >
         <ItemStyle CssClass="tdTxt4n" />
         </asp:BoundField>
         <asp:BoundField DataField="EPJ06" DataFormatString="{0:F2}%" HeaderText="Percent Exercisable" 
           ItemStyle-CssClass="tdTxt4n" >
         <ItemStyle CssClass="tdTxt4n" />
         </asp:BoundField>
         <asp:BoundField DataField="EPJ24" HeaderText="Vested Shares" 
           ItemStyle-CssClass="tdTxt4n" >
         <ItemStyle CssClass="tdTxt4n" />
         </asp:BoundField>
     </Columns>
   </asp:GridView>
       <div style="text-align:center;" > <asp:Button ID="btV2Back" runat="server" 
               Text="Back" onclick="btV2Back_Click"  /></div>          
   </asp:View>
   <asp:View ID="View4" runat="server"> 
                    <td class="font_12y">
                        Company List (公司清單)&nbsp;
                        <asp:DropDownList ID="ddlCompany" runat="server" AutoPostBack="True" >
                        </asp:DropDownList>
                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    </td>
                    &nbsp;<td class="font_12y">
                        IDNO (身分證號)&nbsp;
                    </td>
                    <td>
                    <asp:TextBox ID="Idno" runat="server" Width="144px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="Idno"
                        Display="Dynamic" ErrorMessage="*"></asp:RequiredFieldValidator>&nbsp;
                  </span></td>
       <br />
       <div style="text-align:center;" >
           <asp:Button ID="btQry1" runat="server"
               Text="Query" OnClick="btQry_Click"  /></div>
   </asp:View>
   <asp:View ID="View2" runat="server"> 
     <asp:GridView ID="gv_Main" runat="server" Height="149px" Width="1000px" 
                      AutoGenerateColumns="False" ondatabound="gv_Main_DataBound" 
           ShowFooter="True" EnableModelValidation="True">
     <Columns>
     <asp:BoundField DataField="EPJ03" HeaderText="Grant Date" DataFormatString="{0:####/##/##}" 
           ItemStyle-CssClass="tdTxt4n" >
         <ItemStyle CssClass="tdTxt4n" />
         </asp:BoundField>
          <asp:TemplateField HeaderText="Exercise Price">
                        <ItemTemplate>
                            <div align="center">
                            <asp:Label ID="lbPrice" runat="server" CssClass="tdTxt4n" Text='<%#string.Format("{0:###,###,###0.00}",Eval("EPJ12"))%>'></asp:Label>
                            <br />
                            <asp:Button ID="Button1" runat="server" Text="More Information"
                                onclick="btnExPrice_Click" CommandArgument='<%#Eval("EPJ01")+ "," +Eval("EPJ02")+ "," + Eval("EPJ03")%>' />
                                </div>
                        </ItemTemplate>
                    </asp:TemplateField>
     <asp:BoundField DataField="aa" DataFormatString="{0:###,###,###0}" HeaderText="Granted" 
           ItemStyle-CssClass="tdTxt4n" >
         <ItemStyle CssClass="tdTxt4n" />
         </asp:BoundField>
     <asp:BoundField DataField="bb" DataFormatString="{0:###,###,###0}" HeaderText="Unvested" 
           ItemStyle-CssClass="tdTxt4n" >
         <ItemStyle CssClass="tdTxt4n" />
         </asp:BoundField>
         <asp:TemplateField HeaderText="Vested"> 
                       <ItemTemplate>
                        <div align="center">
                           <asp:Label ID="lbVest" runat="server" CssClass="tdTxt4n" Text='<%#string.Format("{0:###,###,###0}",Eval("EPJ08"))%>'></asp:Label>
                           <br />          
                           <asp:Button ID="Button2" runat="server" Text="More Information" 
                            onclick="btnVest_Click" CommandArgument='<%#Eval("EPJ01") + "," + Eval("EPJ02") + "," + Eval("EPJ03")+ "," + Eval("EPJ04")%>' />
                            </div>
                        </ItemTemplate>
                   </asp:TemplateField>
     <asp:BoundField DataField="EPJ09" DataFormatString="{0:###,###,###0}" HeaderText="Exercised" 
           ItemStyle-CssClass="tdTxt4n" >
         <ItemStyle CssClass="tdTxt4n" />
         </asp:BoundField>
     <asp:BoundField DataField="Eshares" DataFormatString="{0:###,###,###0}" HeaderText="Exercisable(A)" 
           ItemStyle-CssClass="tdTxt4n" >
         <ItemStyle CssClass="tdTxt4n" />
         </asp:BoundField>
     <asp:BoundField DataField="Cprice" DataFormatString="{0:N2}" HeaderText="Last Closing Price(B)" 
           ItemStyle-CssClass="tdTxt4n" >
         <ItemStyle CssClass="tdTxt4n" />
         </asp:BoundField>
     <asp:BoundField DataField="mValue" DataFormatString="{0:###,###,###0}" HeaderText="Estimated Market Value (A)*(B)" 
           ItemStyle-CssClass="tdTxt4n" >
         <ItemStyle CssClass="tdTxt4n" />
         </asp:BoundField>
     <asp:BoundField DataField="EPJ28" HeaderText="Expiration Date" DataFormatString="{0:####/##/##}"
           ItemStyle-CssClass="tdTxt4n" >
         <ItemStyle CssClass="tdTxt4n" />
         </asp:BoundField>
         <asp:TemplateField HeaderText="Plan">
             <ItemTemplate>
                 <asp:ImageButton ID="ibtPlanDownload" runat="server" 
                     ImageUrl="~/images/link.png" ToolTip="Click here to download"
                     CommandArgument='<%#Eval("PlanFile")%>' onclick="ibtPlanDownload_Click" />
             </ItemTemplate>
         </asp:TemplateField>
         <asp:TemplateField HeaderText="Guide">
             <ItemTemplate>
                 <asp:ImageButton ID="ibtGuideDownload" runat="server" 
                     ImageUrl="~/images/link.png" ToolTip="Click here to download" 
                     CommandArgument='<%#Eval("GuideFile")%>' onclick="ibtGuideDownload_Click" />
             </ItemTemplate>
         </asp:TemplateField>
     </Columns>
         <FooterStyle CssClass="tdTxt2" />
         <HeaderStyle CssClass="tdTxt3r" />
     </asp:GridView>
     <!--
     <tbody>
     <tr>
        <td class="tdTxt2">
                    Total 
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:TextBox ID="txtSUM0" runat="server" Width="50px"></asp:TextBox>
                    <asp:TextBox ID="txtSUM1" runat="server" Width="55px"></asp:TextBox>
                    <asp:TextBox ID="txtSUM8" runat="server" Width="135px"></asp:TextBox>
                    <asp:TextBox ID="txtSUM9" runat="server" Width="60px"></asp:TextBox>
                    <asp:TextBox ID="txtSUM4" runat="server" Width="84px"></asp:TextBox>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:TextBox ID="txtSUM5" runat="server" Width="136px" style="margin-left: 0px"></asp:TextBox> 
               &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        </td>     
     </tr>
     </tbody>
     -->
   </asp:View>        
</asp:MultiView> 
                </td>
           </tr>
           <tr>
           <td>
           <div style="text-align: center">
               <uc1:wucBoard ID="wucBoard1" runat="server" APID="SU003" SID="A" />
           </div>
           </td>
           </tr>
    </table>
  </ContentTemplate>   
        
  </asp:UpdatePanel>
  </asp:Content>


