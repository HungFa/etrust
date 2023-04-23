<%@ Page Title="" Language="C#" MasterPageFile="~/eTrust/etrust.master" AutoEventWireup="true"
    CodeFile="T_EPS105U.aspx.cs" Inherits="eTrust_T_EPS105U" %>

<%@ Register Src="../modules/wucBoard.ascx" TagName="wucBoard" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <asp:UpdateProgress ID="UpdateProgress1" runat="server" AssociatedUpdatePanelID="UpdatePanel1">
        <ProgressTemplate>
            <img alt="" src="../images/al_loading.gif" style="width: 43px; height: 43px" />
            <span class="style2">Now Processing</span></ProgressTemplate>
    </asp:UpdateProgress>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table style="width: 100%;">
                <tr>
                    <td class="tdTxt3r">
                        <strong>Exercise Options</strong> &nbsp;
                        <asp:Label ID="lbUserInfo" runat="server"></asp:Label>
                        &nbsp;
                        <asp:Label ID="lbExerciseMethod" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:MultiView ID="mainViews" runat="server">
                            <asp:View ID="viewDataList" runat="server">
                                <asp:GridView ID="gvExceriseList" runat="server" AutoGenerateColumns="False" Style="text-align: center"
                                    EnableModelValidation="True" OnDataBound="gvExceriseList_DataBound" Width="100%">
                                    <RowStyle CssClass="tdTxt4" />
                                    <Columns>
                                        <asp:BoundField HeaderText="Grant Date" DataField="EPJ03" DataFormatString="{0:####/##/##}" />
                                        <asp:BoundField HeaderText="Current Exercise Price" DataField="EPJ12" DataFormatString="{0:###,###0.00}" />
                                        <asp:BoundField HeaderText="Shares Exercisable" DataField="E200921" DataFormatString="{0:###,###0.##}" />
                                        <asp:TemplateField HeaderText="Method">
                                            <ItemTemplate>
                                                <asp:Button ID="btMethod01" runat="server" Text="Exercise-and-Hold" Width="220px"
                                                    CommandArgument='<%# Eval("EPJ235") %>' OnClick="btMethod01_Click" CommandName="A" Visible="false" />
                                                <br />
                                                <asp:Button ID="btMethod02" runat="server" Text="Exercise-and-Sell" Width="220px"
                                                    CommandArgument='<%# Eval("EPJ235") %>' OnClick="btMethod02_Click" CommandName="B" Visible="false" />
                                                <br />
                                                <asp:Button ID="btMethod03" runat="server" Text="Exercise-and Sell to Cover" Width="220px"
                                                    CommandArgument='<%# Eval("EPJ235") %>' OnClick="btMethod03_Click" CommandName="C" Visible="false" />
                                                <asp:HiddenField ID="HiddenEPD12" runat="server" Value='<%# Eval("EPD12") %>' />
                                                <asp:HiddenField ID="HiddenEPD13" runat="server" Value='<%# Eval("EPD13") %>' />
                                                <asp:HiddenField ID="HiddenEPD28" runat="server" Value='<%# Eval("EPD28") %>' />
                                                <asp:HiddenField ID="HiddenEPA35" runat="server" Value='<%# Eval("EPA35") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <HeaderStyle CssClass="tdTxt2" />
                                </asp:GridView>
                            </asp:View>
                            <asp:View ID="viewConfirm" runat="server">
                                <div align="center">
                                    <uc1:wucBoard ID="wucBoard1" runat="server" APID="SU004" SID="A" />
                                    <br />
                                    <asp:Label ID="Label1" runat="server" Text="Label" Visible="False"></asp:Label>
                                    <asp:Button ID="btConfirm" runat="server" Text="Continue" 
                                        OnClick="btConfirm_Click" />
                                </div>
                            </asp:View>
                            <asp:View ID="viewInput" runat="server">
                                <table style="width: 100%;">
                                    <tr>
                                        <td class="tdTxt2">
                                            Grant Date
                                        </td>
                                        <td class="tdTxt4n">
                                            <asp:Label ID="lbEPJ03" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tdTxt2">
                                            Exercise Price
                                        </td>
                                        <td class="tdTxt4n">
                                            <asp:Label ID="lbEPJ12" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tdTxt2">
                                            Shares Exercisable
                                        </td>
                                        <td class="tdTxt4n">
                                            <asp:Label ID="lbEPJ0809" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tdTxt2">
                                            Please input number of shares you want to exercise
                                        </td>
                                        <td class="tdTxt4n">
                                            <asp:TextBox ID="txtEPK10a" Width="50" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tdTxt3r" colspan="2">
                                            <div align="center">
                                                <asp:Button ID="btSubmit" runat="server" Text="Submit" 
                                                    onclick="btSubmit_Click" />
                                                &nbsp;<asp:Button ID="btClear" runat="server" Text="Clear" 
                                                    onclick="btClear_Click" />
                                                &nbsp;<asp:Button ID="btBack" runat="server" Text="Back" 
                                                    onclick="btBack_Click" />
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                            <asp:View ID="viewInputConfirm" runat="server">
                            <table style="width: 100%;">
                                    <tr>
                                        <td class="tdTxt2">
                                            Grant Date
                                        </td>
                                        <td class="tdTxt4n">
                                            <asp:Label ID="lbCirEPJ03" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tdTxt2">
                                            Exercise Price
                                        </td>
                                        <td class="tdTxt4n">
                                            <asp:Label ID="lbCirEPJ12" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tdTxt2">
                                            Exercised Shares
                                        </td>
                                        <td class="tdTxt4n">
                                            <asp:Label ID="lbCirEPK10" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tdTxt2">
                                            Current Shares Exercisable
                                        </td>
                                        <td class="tdTxt4n">
                                            <asp:Label ID="lbCirLimt_EPK10" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tdTxt2">
                                            Total Exercise Cost
                                        </td>
                                        <td class="tdTxt4n">
                                            <asp:Label ID="lbCirTotal" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tdTxt3r" colspan="2">
                                            
                                            <uc1:wucBoard ID="wucBoard2" runat="server" APID="SU004" SID="B" />
                                            
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="tdTxt3r" colspan="2">
                                            <div align="center">
                                                <asp:Button ID="btConfirmSubmit" runat="server" Text="Confirm to submit" 
                                                    onclick="btConfirmSubmit_Click" />
                                                &nbsp;<asp:Button ID="btConfirmCancel" runat="server" Text="Cancel" 
                                                    onclick="btConfirmCancel_Click" />
                                                &nbsp;</div>
                                        </td>
                                    </tr>
                                </table>
                            </asp:View>
                        </asp:MultiView>
                        <asp:HiddenField ID="hidEPA03" runat="server" />
                        <asp:HiddenField ID="hidEPJ02" runat="server" />
                        <asp:HiddenField ID="hidEPJ03" runat="server" />
                        <asp:HiddenField ID="hidEPJ05" runat="server" />
                        <asp:HiddenField ID="hidEPJ12" runat="server" />
                        <asp:HiddenField ID="hidEPJ13" runat="server" />
                        <asp:HiddenField ID="hidEPJ17" runat="server" />
                        <asp:HiddenField ID="hidEPJ22" runat="server" />
                        <asp:HiddenField ID="hidEPJ23" runat="server" />
                        <asp:HiddenField ID="hidEPJ37" runat="server" />
                        <asp:HiddenField ID="hidLimit" runat="server" />
                        <asp:HiddenField ID="hidEPG04" runat="server" />
                        <asp:HiddenField ID="hidEPL02" runat="server" />
                        <asp:HiddenField ID="hidEPL03" runat="server" />
                        <asp:HiddenField ID="hidEPK10" runat="server" />                        
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
