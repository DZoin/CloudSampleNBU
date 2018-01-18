<%@ Page Title="NBU Location Match" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LocationMatch.WebSite.WebRole._Default" MaintainScrollPositionOnPostback="true"%>

<asp:Content ID="headContent" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    
    <link rel="stylesheet" href="http://js.arcgis.com/3.11/esri/css/esri.css" />
    <script src="http://js.arcgis.com/3.11/"></script>
    <script src="/Scripts/jquery-1.8.2.min.js"></script>

    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server" />

    <asp:GridView ID="Menu1" runat="server" AutoGenerateColumns="false"
        CssClass="DDGridView" RowStyle-CssClass="td" HeaderStyle-CssClass="th" CellPadding="6">
        <Columns>
            <asp:TemplateField HeaderText="Entities" SortExpression="TableName">
                <ItemTemplate>
                    <asp:DynamicHyperLink ID="HyperLink1" runat="server"><%# Eval("DisplayName") %></asp:DynamicHyperLink>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <h2 class="DDSubHeader">Location Match Analysis</h2>
    
    <asp:Label ID="Label1" runat="server" Width="120px">Location Lists</asp:Label>
    <asp:DropDownList runat="server" ID="ddLocationLists" Width="254px" />
    <asp:Button runat="server" ID="btnRunLocationMatchAnalysis" Text="Run Location Match Analysis" Width="250px" OnClick="btnRunLocationMatchAnalysis_Click" />
    <asp:Label runat="server" ID="lblErrorMessage" ForeColor="Red"></asp:Label>
    <br />
    <asp:Label ID="Label2" runat="server" Width="120px">Tracks</asp:Label>
    <asp:DropDownList runat="server" ID="ddTracks" Width="254px" />
    <asp:Button runat="server" ID="btnLoadResults" Text="Load Results" Width="250px" OnClick="btnLoadResults_Click" />
    <br />
    <asp:Label ID="Label3" runat="server" Width="120px">Radius in meters</asp:Label>
    <asp:TextBox runat="server" ID="tbRadius" Width="250px"></asp:TextBox>
    <br />
    <br />
    <div style="width: 100%; height: 185px; overflow: scroll;">
        <asp:GridView 
            ID="gridResults" 
            runat="server" 
            Width="100%" 
            OnSelectedIndexChanged="gridResults_SelectedIndexChanged" 
            AllowPaging="True" 
            OnPageIndexChanging="gridResults_PageIndexChanging" 
            PageSize="5" 
            AlternatingRowStyle-Wrap="False" 
            RowStyle-Wrap="False">
            <Columns>
                <asp:CommandField ShowSelectButton="True" SelectText="Map" />
            </Columns>
            <SelectedRowStyle BackColor="#FF9933" />
        </asp:GridView>
    </div>
    <div 
        id="divMap" 
        style="width: 100%; height: 350px; border: 1px solid; border-color: black; display: none;">
    </div>

    <script src="Default.js"></script>
</asp:Content>
