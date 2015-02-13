<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Logs.ascx.cs" Inherits="ITIL.Modules.ServiceDesk.Controls.Logs" %>
<asp:LinqDataSource ID="LDSLogs" runat="server" 
        ContextTypeName="ITIL.Modules.ServiceDesk.ServiceDeskDALDataContext" 
        OrderBy="DateCreated desc" TableName="ITILServiceDesk_Logs" 
    onselecting="LDSLogs_Selecting">
    </asp:LinqDataSource>
    <asp:GridView ID="gvLogs" runat="server" AllowPaging="True" 
        AutoGenerateColumns="False" DataKeyNames="LogID" DataSourceID="LDSLogs"  CssClass="table table-bordered table-striped"
        PageSize="6">
        <Columns>
            <asp:TemplateField HeaderText="Description" SortExpression="LogDescription">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("LogDescription") %>'></asp:TextBox>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblLogDescription" Font-Size="Small" runat="server" Text='<%# Bind("LogDescription") %>'
                        Width="100%"></asp:Label>
                    <%--<asp:TextBox ID="TextBox2" runat="server" Rows="2" 
                        Text='<%# Bind("LogDescription") %>' TextMode="MultiLine" Width="100%"></asp:TextBox>--%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="DateCreated" HeaderText="Date" 
                SortExpression="DateCreated" >
            <ItemStyle Wrap="False" />
            </asp:BoundField>
        </Columns>
    </asp:GridView>


