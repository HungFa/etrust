<%@ Page Title="" Language="C#" MasterPageFile="~/eTrust/etrust.master" AutoEventWireup="true" CodeFile="T_EPS040.aspx.cs" Inherits="eTrust_T_EPS040" %>

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
                <td class="tdTxt3r" colspan="2">
                    <strong>Personal Information&nbsp; </strong><asp:Label ID="lbUserInfo" runat="server"></asp:Label>
                </td>
            </tr>

            <tr>
                <td class="tdTxt2">
                    Name</td>
                <td class="tdTxt4n">
                    <asp:Label ID="lbName" 
                        runat="server" Text="N/A"></asp:Label>
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    &nbsp;</td>
            </tr>
            <!--
            <tr>
                <td class="tdTxt2">
                    Country</td>
                <td class="tdTxt4n">
                    <asp:Label ID="lbCountry" runat="server" Text="N/A"></asp:Label>
                </td>
            </tr>
            -->
            <tr>
                <td class="tdTxt2">
                    Email</td>
                <td class="tdTxt4n">
                    <asp:Label ID="lbemail" runat="server" Text="N/A"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="tdTxt2">
                    Personal Email</td>
                <td class="tdTxt4n">
                    <asp:TextBox ID="txtPemail" runat="server" Width="379px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="tdTxt2">
                    &nbsp;</td>
                <td class="tdTxt4n">
                    &nbsp;
                    <asp:Button ID="btAddPEmail" runat="server" Text="Add Personal Email" 
                        onclick="btAddPEmail_Click" />
&nbsp;<asp:Button ID="btChangePEmail" runat="server" Text="Change Personal Email" 
                        onclick="btChangePEmail_Click" />
&nbsp;<asp:Button ID="btDelPEmail" runat="server" Text="Delete Personal Email" 
                        onclick="btDelPEmail_Click" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div style="text-align: center"><uc1:wucBoard ID="wucBoard1" runat="server" 
                            APID="SU002" SID="A" /></div>
                </td>
            </tr>
        </table>
        </ContentTemplate>
    </asp:UpdatePanel>
        
    
 <br>
</asp:Content>

