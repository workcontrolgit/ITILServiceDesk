<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Tags.ascx.cs" Inherits="ITIL.Modules.ServiceDesk.Controls.Tags" %>

<asp:TreeView ID="tvCategories" runat="server" ExpandDepth="0" OnTreeNodeDataBound="tvCategories_TreeNodeDataBound">
    <SelectedNodeStyle BackColor="#CCCCCC" Font-Bold="False" Font-Underline="False" />
    <DataBindings>
        <asp:TreeNodeBinding DataMember="ITIL.Modules.ServiceDesk.Catagories" Depth="0"
            TextField="Value" ValueField="Value" />
    </DataBindings>
</asp:TreeView>

