<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TimeSheets.aspx.cs" Inherits="authtesting.TimeSheets" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <style type="text/css">
        table.statusfinder > tbody > tr:first-child {
            text-align: center;
        }

        table.statusfinder {
            background-color: #eee;
            border: 1px dashed #9e9e9e;
        }
        /*#lblEmailstatus {
            color: #00bd00;
        }
        #lblEmailstatus.reject {
            color: #f44336;
        }*/
            table.statusfinder > tbody > tr:first-child > td {
                padding: 15px 15px 0;
            }
       table.statusfinder > tbody > tr:last-child #divEmailid > section > div {
            border: 0 !important;
        }
       table.statusfinder > tbody > tr:last-child #divEmailid > section > div > div:first-child{
            background: #7d7d7d !important;
            color: #eee !important;
            text-shadow: 1px 1px 1px #555 !important;
        }
    </style>
</head>
<body runat="server">
    <form id="form1" runat="server">
        <div>
            <table class="statusfinder" border="0" cellpadding="0" cellspacing="0">
                <tr>
                     <td> <b>Email Status : </b>
                     <%--  <b> <asp:Label ID="lblEmailstatus" runat="server" Text="" /></b>--%>
                         <b><asp:Label ID="lblEmailstatus" runat="server" Text=""></asp:Label></b>
                   </td>
                </tr>
                <tr>
                    <td colspan="2">
                       <%-- <div id="divEmailid" runat="server">--%>
                        <div id="divEmailid" runat="server"> </div>
                    </td>   

                </tr>

            </table>

        </div>
    </form>
</body>
</html>
