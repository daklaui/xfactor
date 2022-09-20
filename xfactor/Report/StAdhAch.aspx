<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StAdhAch.aspx.cs" Inherits="xfactor.Report.StAdhAch" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt">
            <LocalReport ReportPath="Report\Situation Adh&#233;rent Acheteur.rdlc">
                <DataSources>
                    <rsweb:ReportDataSource Name="Situation_Adh_Ach" DataSourceId="SqlDataSource1"></rsweb:ReportDataSource>
                </DataSources>
            </LocalReport>
        </rsweb:ReportViewer>
        <asp:SqlDataSource runat="server" ID="SqlDataSource1" ConnectionString='<%$ ConnectionStrings:Xfactor_prodConnectionString %>' SelectCommand="usp_SituationAdhAch2" SelectCommandType="StoredProcedure">
            <SelectParameters>
                <asp:Parameter Name="param_RefCtr" Type="Int32"></asp:Parameter>
                <asp:Parameter Name="param_RefAch" Type="Int32"></asp:Parameter>
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
    </form>
</body>
</html>
