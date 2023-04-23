<%@ Page Title="" Language="C#" AutoEventWireup="true" CodeFile="SUEXPOR.aspx.cs"
    Inherits="eTrust_SUEXPOR" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        .style1
        {
            width: 129px;
        }
        .style2
        {
            height: 18px;
        }
        .style3
        {
            height: 15px;
        }
        .style4
        {
            height: 17px;
        }
        .style5
        {
            height: 16px;
        }
    </style>
    <script type="text/javascript" language="javascript">
        function printPages() {
            self.focus();
            self.print();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Literal ID="Literal1" runat="server"></asp:Literal>
        <br />
        <table style="width: 600px;">
            <tr>
                <td style="text-align: right">
                    <a href="javascript:printPages();">Print</a>
                </td>
            </tr>
            <tr>
                <td class="style4">
                    <table style="width: 100%;">
                        <tr>
                            <td>
                                &nbsp;&nbsp;
                            </td>
                            <td style="text-align: center">
                                <asp:Label ID="lbHeader" runat="server" Style="font-size: large; font-weight: 700"></asp:Label>
                            </td>
                            <td>
                                &nbsp;&nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lbStockNo" runat="server"></asp:Label>
                            </td>
                            <td style="text-align: center">
                                Notice to Exercise Stock Option
                            </td>
                            <td style="text-align: right">
                                <asp:Label ID="lbSeriaNo" runat="server"></asp:Label>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="style5">
                                &nbsp;
                            </td>
                            <td class="style5">
                                &nbsp;
                            </td>
                            <td class="style5">
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Date:<asp:Label ID="lbPrintDate" runat="server"></asp:Label>
                            </td>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                The exercise details as following：
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="style4">
                    <table style="border: 1px solid #000000; width: 100%;" border="1" cellpadding="1"
                        cellspacing="0">
                        <tr>
                            <td colspan="4">
                                Name:<asp:Label ID="lbCompanyName" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                No. of Options
                            </td>
                            <td>
                                Share(s) Exercised
                            </td>
                            <td>
                                Exercise Price
                            </td>
                            <td>
                                Total Amount
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lbOptions" runat="server" Style="color: #FF3300"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lbShares" runat="server" Style="color: #FF3300"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lbExercisePrice" runat="server" Style="color: #FF3300"></asp:Label>
                            </td>
                            <td>
                                <asp:Label ID="lbTWD" runat="server" Style="color: #FF3300"></asp:Label>
                                <br />
                                <asp:Label ID="lbForginDollars" runat="server" Style="color: #FF3300"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                Certificate No:
                                <asp:Label ID="lbCertificateNo" runat="server"></asp:Label>
                                &nbsp;&nbsp; Date of Issue:
                                <asp:Label ID="lbDateofIssue" runat="server"></asp:Label>
                                <br />
                                Employee ID:<asp:Label ID="lbEmployeeID" runat="server"></asp:Label>
                            </td>
                            <td>
                                FX Rate (USD/TWD)<br />
                                <asp:Label ID="lbFXRate" runat="server"></asp:Label>
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                Remarks: You may be subject to local capital gains after you hold the stocks .
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="style3">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td class="style4">
                    &nbsp; Important Information:
                </td>
            </tr>
            <tr>
                <td class="style4">
                    &nbsp; 1. You must make sure that Bank SinoPac can receive your remittance
                    <asp:Label ID="lbInformations1" runat="server" Style="color: #FF0000"></asp:Label>
                    &nbsp;by&nbsp;3:30 pm&nbsp;
                    <asp:Label ID="lbInformations2" runat="server"></asp:Label>
                    &nbsp;(Taiwan Time)
                </td>
            </tr>
            <tr>
                <td class="style4">
                    &nbsp;&nbsp;
                </td>
            </tr>
            <tr>
                <td class="style4">
                    &nbsp; 2. To prevent impact of unstable market coditions on the total amount in
                    NTD Bank SinoPac 
                    will receive, one (1) NTD will be deducted from the foreign exchange
                    (FX) rate shown on the subscription form from the previous business day&#39;s FX rate.
                </td>
            </tr>
            <tr>
                <td class="style4">
                    &nbsp;&nbsp;
                </td>
            </tr>
            <tr>
                <td class="style4">
                    &nbsp;For your remittance, please instruct the remitting bank to send the payment
                    to:
                </td>
            </tr>
            <tr>
                <td class="style4">
                    <table style="border: 1px solid #000000; width: 100%;" border="1" cellpadding="1"
                        cellspacing="0">
                        <tr>
                            <td class="style1">
                                Remit to :
                                <br />
                                (Bank SinoPac&#39;s.<br />
                                Correspondent bank)
                            </td>
                            <td>
                                BANK 
                                OF NEW YORK<br />
                                N.Y.&nbsp;&nbsp;&nbsp; U.S.A<br />
                                SWIFT CODE: 
                                <span lang="EN-US" 
                                    style="font-size: 12.0pt; mso-bidi-font-size: 10.0pt; font-family: &quot;Times New Roman&quot;; mso-fareast-font-family: 新細明體; mso-font-kerning: 1.0pt; mso-ansi-language: EN-US; mso-fareast-language: ZH-TW; mso-bidi-language: AR-SA">
                                <span lang="EN-US" style="font-size:12.0pt;mso-bidi-font-size:
10.0pt;font-family:&quot;Times New Roman&quot;;mso-fareast-font-family:新細明體;mso-font-kerning:
1.0pt;mso-ansi-language:EN-US;mso-fareast-language:ZH-TW;mso-bidi-language:
AR-SA"><font face="新細明體" size="3"><span lang="EN-US" style="FONT-SIZE: 12pt">IRVTUS3N</span></font><br />
                                </span>
                            </td>
                        </tr> 
                        <tr>
                            <td class="style1">
                                Beneficiary&#39;s Bank :
                            </td>
                            <td>
                                BANK SINOPAC<br />
                                TAIPEI<br />
                                SWIFT CODE: <span lang="EN-US" 
                                    style="font-size: 12.0pt; mso-bidi-font-size: 10.0pt; font-family: &quot;Times New Roman&quot;; mso-fareast-font-family: 新細明體; mso-font-kerning: 1.0pt; mso-ansi-language: EN-US; mso-fareast-language: ZH-TW; mso-bidi-language: AR-SA">
                                <span lang="EN-US" style="font-size:12.0pt;mso-bidi-font-size:
10.0pt;font-family:&quot;Times New Roman&quot;;mso-fareast-font-family:新細明體;mso-font-kerning:
1.0pt;mso-ansi-language:EN-US;mso-fareast-language:ZH-TW;mso-bidi-language:
AR-SA"><font face="新細明體" size="3"><span lang="EN-US" style="FONT-SIZE: 12pt">SINOTWTP</span></font><br />
                                9F., No. 36, </span>Sec. 3, Nanjing E. Rd</span>., <span lang="EN-US" 
                                    style="font-size: 12.0pt; mso-bidi-font-size: 10.0pt; font-family: &quot;Times New Roman&quot;; mso-fareast-font-family: 新細明體; mso-font-kerning: 1.0pt; mso-ansi-language: EN-US; mso-fareast-language: ZH-TW; mso-bidi-language: AR-SA">
                                Zhongshan Dist.,&nbsp; Taipei City 104, Taiwan</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="style1">
                                In favour of :&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                             </td>
                            <td>
                                Beneficiary&#39;s A/C :
                                <asp:Label ID="lbBeneficiaryAC" runat="server"></asp:Label>
                                <br />
                                Beneficiary&#39;s Name :
                                <asp:Label ID="lbBeneficiaryName" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="style2" colspan="2">
                                Remarks : Please print the following E-Collection No. and ID No. on your remittance
                                form as the message for Beneficiary to identify the remitter.
                            </td>
                        </tr>
                        <tr>
                            <td class="style1">
                                (MT103/72)
                            </td>
                            <td>
                                E-Collection No :
                                <asp:Label ID="lbBankECollectionNo" runat="server"></asp:Label>
                                <br />
                                ID No :
                                <asp:Label ID="lbBankIdNo" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="style6" colspan="2">
                                <strong>The remittance must be for the full amount.</strong>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td class="style4">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td class="style4">
                    &nbsp;Note: You should be aware of the risks involved in second remittance if the
                    remitted amount 
                    in non-US dollars is insufficient to make such payment.&nbsp;
                </td>
            </tr>
            <tr>
                <td class="style4">
                    &nbsp;
                </td>
            </tr>
        </table>
        <br />
        <br />
        <br />
    </div>
    </form>
</body>
</html>
