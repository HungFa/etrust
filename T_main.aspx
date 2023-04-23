<%@ Page Title="" Language="C#" MasterPageFile="~/eTrust/etrust.master" AutoEventWireup="true" CodeFile="T_main.aspx.cs" Inherits="eTrust_T_main" %>

<%@ Register src="../modules/wucBoard.ascx" tagname="wucBoard" tagprefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:wucBoard ID="wucBoard1" runat="server" APID="SU001" SID="A" />
</asp:Content>

