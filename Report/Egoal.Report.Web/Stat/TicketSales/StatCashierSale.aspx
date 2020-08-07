<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeBehind="StatCashierSale.aspx.cs" Inherits="Egoal.Report.Web.Stat.TicketSales.StatCashierSale" %>

<%@ Register Assembly="GrapeCity.ActiveReports.Web.v11, Version=11.2.10750.0, Culture=neutral, PublicKeyToken=cc4967777c49a3ff" Namespace="GrapeCity.ActiveReports.Web" TagPrefix="ActiveReportsWeb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <link rel="stylesheet" href="../../Content/index.css?v=<%=CssVersion %>" />
</head>
<body>
    <form id="form1" runat="server">
        <ActiveReportsWeb:WebViewer ID="WebViewer" runat="server" MaxReportRunTime="00:01:00"></ActiveReportsWeb:WebViewer>
    </form>
    <style>
        /*table{
            margin: 0 auto;
            position: relative;
        }
        table.component{
            position: relative;
        }*/
        .style0-2, .style0-12, .style1-2, .style1-12, .style2-2, .style2-12, .style3-2, .style3-12, .style4-2, .style4-12, .style5-2, .style5-12,
        .style6-2, .style6-12, .style7-2, .style7-12, .style8-2, .style8-12, .style9-2, .style9-12, .style10-2, .style10-12, .style11-2, .style11-12,
        .style12-2, .style12-12, .style13-2, .style13-12, .style14-2, .style14-12, .style15-2, .style15-12, .style16-2, .style16-12, .style17-2, .style17-12,
        .style18-2, .style18-12, .style19-2, .style19-12, .style20-2, .style20-12, .style21-2, .style21-12, .style22-2, .style22-12, .style23-2, .style23-12,
        .style0-11, .style1-11, .style2-11, .style3-11, .style4-11, .style5-11, .style6-11, .style7-11, .style8-11, .style9-11, .style10-11, .style11-11,
        .style12-11, .style13-11, .style14-11, .style15-11, .style16-11, .style17-11, .style18-11, .style19-11, .style20-11, .style21-11, .style22-11, .style23-11{
            width: 100% !important;
        }

        .style0-4, .style1-4, .style2-4, .style3-4, .style4-4, .style5-4, .style6-4, .style7-4, .style8-4, .style9-4, .style10-4, .style11-4, .style12-4,
        .style13-4, .style14-4, .style15-4, .style16-4, .style17-4, .style18-4, .style19-4, .style20-4, .style21-4, .style22-4, .style23-4 {
            right: 166px;
            left: unset !important;
        }

        .style0-1, .style1-1, .style2-1, .style3-1, .style4-1, .style5-1, .style6-1, .style7-1, .style8-1, .style9-1, .style10-1, .style11-1, .style12-1,
        .style13-1, .style14-1, .style15-1, .style16-1, .style17-1, .style18-1, .style19-1, .style20-1, .style21-1, .style22-1, .style23-1 {
            right: 30px;
            left: unset !important;
        }
    </style>
</body>
</html>
