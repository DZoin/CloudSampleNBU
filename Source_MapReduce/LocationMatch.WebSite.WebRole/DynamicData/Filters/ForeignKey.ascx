<%@ Control Language="C#" CodeBehind="ForeignKey.ascx.cs" Inherits="LocationMatch.WebSite.WebRole.ForeignKeyFilter" %>

<asp:DropDownList runat="server" ID="DropDownList1" AutoPostBack="True" CssClass="DDFilter"
    OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged">
    <asp:ListItem Text="All" Value="" />
</asp:DropDownList>

