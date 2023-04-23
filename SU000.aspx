<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SU000.aspx.cs" Inherits="eTrust_SU000_1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="../css/sectw_style.css" type="text/css">
    <link rel="stylesheet" href="../css/sotw_style.css" type="text/css">
    <style type="text/css">
        .style1
        {
            width: 1024;
        }
        .tableTxt
        {
            width: 1024px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table class="tableTxt">
            <tr>
                <td class="font_16B_b">
                    <img alt="" src="../images/title_head01.gif" style="width: 11px; height: 16px" /><asp:Label 
                        ID="lbAPId" runat="server" Text="Error Response."></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="style1">
                    <asp:ScriptManager ID="ScriptManager1" runat="server">
                    </asp:ScriptManager>
                </td>
            </tr>
            <tr>
                <td class="font_12gr2">
                    <img alt="" src="../images/dot_red_1.gif" style="width: 13px; height: 14px" /><asp:Label 
                        ID="lbtime" runat="server" Text="now"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: center">
                   <asp:Literal ID="litErrors" runat="server"></asp:Literal>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
