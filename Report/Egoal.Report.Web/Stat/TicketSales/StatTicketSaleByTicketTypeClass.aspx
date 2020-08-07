<%@ Page Language="C#" Async="true" AutoEventWireup="true" CodeBehind="StatTicketSaleByTicketTypeClass.aspx.cs" Inherits="Egoal.Report.Web.Stat.TicketSales.StatTicketSaleByTicketTypeClass" %>

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
</body>
</html>
