<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminSettings.ascx.cs" Inherits="ITIL.Modules.ServiceDesk.AdminSettings" %>
<asp:Panel ID="pnlAdminSettings" runat="server" align="left">
    <%--<asp:Image ID="Image3" runat="server" ImageUrl="~/DesktopModules/ServiceDesk/images/application_side_contract.png" />--%>
    <style type="text/css">
        .style1
        {
            width: 19px;
        }
    </style>
    <asp:LinkButton ID="lnkBack" runat="server" resourcekey="lnkBack" Font-Underline="True" CssClass="btn btn-primary"
        OnClick="lnkBack_Click" Text="Back" />
    <br />
    <br />
    <table cellpadding="2">
        <tr>
            <td valign="top" align="left" nowrap="nowrap">
                <p>
                    <asp:Image ID="Image1" runat="server" ImageUrl="~/DesktopModules/ServiceDesk/images/user_suit.png" />
                    <asp:LinkButton ID="lnkAdminRole" runat="server" Font-Underline="True" OnClick="lnkAdminRole_Click"
                        resourcekey="lnkAdminRole" Text="Administrator Role" />
                </p>
                <p>
                    <asp:Image ID="Image4" runat="server" ImageUrl="~/DesktopModules/ServiceDesk/images/folder.png"
                        Height="16px" />
                    <asp:LinkButton ID="lnkUploadefFilesPath" runat="server" Font-Underline="True" OnClick="lnkUploadefFilesPath_Click"
                        resourcekey="lnkUploadefFilesPath" Text="File Upload Settings" />
                </p>
                <p>
                    <asp:Image ID="Image5" runat="server" ImageUrl="~/DesktopModules/ServiceDesk/images/group.png"
                        Height="16px" />
                    <asp:LinkButton ID="lnkRoles" runat="server" Font-Underline="True" OnClick="lnkRoles_Click"
                        resourcekey="lnkRoles" Text="Assignment Roles" />
                </p>
                <p>
                    <asp:Image ID="Image2" runat="server" ImageUrl="~/DesktopModules/ServiceDesk/images/tag_blue.png" />
                    <asp:LinkButton ID="lnkTagsAdmin" runat="server" Font-Underline="True" OnClick="lnkTagsAdmin_Click"
                        resourcekey="lnkTagsAdmin" Text="Tags Administration" />
                </p>
            </td>
            <td class="style1" valign="top">
                &nbsp;
            </td>
            <td align="left" valign="top">
                <asp:Panel ID="pnlAdministratorRole" runat="server" BorderColor="#CCCCCC" BorderStyle="Solid">
                    <table cellpadding="0">
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lbltxtAdministratorRole" runat="server" resourcekey="lbltxtAdministratorRole"
                                    Text="Administrator Role:" />
                                &nbsp;
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlAdminRole" runat="server" CssClass="form-control">
                                </asp:DropDownList>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Button ID="btnUpdateAdminRole" runat="server" OnClick="btnUpdateAdminRole_Click"
                                    Text="Update" resourcekey="btnUpdateAdminRole" CssClass="btn btn-primary" />
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblAdminRole" runat="server" EnableViewState="False" Font-Italic="True"
                                    ForeColor="#CC3300"></asp:Label>
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="pnlUploadefFilesPath" runat="server" BorderColor="#CCCCCC" BorderStyle="Solid">
                    <table cellpadding="0">
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td nowrap="nowrap" align="right">
                                <asp:Label ID="lbltxtFileUploadPath" runat="server" resourcekey="lbltxtFileUploadPath" Text="File Upload Path:" />
                                &nbsp;
                            </td>
                            <td>
                                <asp:TextBox ID="txtUploadedFilesPath" runat="server" Columns="50" CssClass="form-control"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td align="right" nowrap="nowrap">
                                <asp:Label ID="lbltxtFileUploadPermission" runat="server" resourcekey="lbltxtFileUploadPermission"
                                    Text="File Upload Permission:" />
                                &nbsp;
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlUploadPermission" runat="server" CssClass="form-control">
                                    <asp:ListItem Selected="True" Text="All" Value="All" resourcekey="ddlUploadPermissionAll" />
                                    <asp:ListItem Text="Administrator" Value="Administrator" resourcekey="ddlUploadPermissionAdministrator" />
                                    <asp:ListItem Text="Administrator/Registered Users" Value="Administrator/Registered Users" resourcekey="ddlUploadPermissionAdminRegUser"/>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Button ID="btnUploadedFiles" runat="server" Text="Update" resourcekey="btnUpdateAdminRole" OnClick="btnUploadedFiles_Click" CssClass="btn btn-primary" />
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="lblUploadedFilesPath" runat="server" EnableViewState="False" Font-Italic="True"
                                    ForeColor="#CC3300"></asp:Label>
                            </td>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="pnlRoles" runat="server" BorderColor="#CCCCCC" BorderStyle="Solid">
                    <table cellpadding="0">
                        <tr>
                            <td colspan="2">
                                &nbsp;<asp:Label ID="lbltxtAssignmentRoles" runat="server" resourcekey="lbltxtAssignmentRoles"
                                    Text="Assignment Roles:" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                            <td>
                                <asp:ListView ID="lvRoles" runat="server" DataKeyNames="ID" DataSourceID="ldsRoles"
                                    OnItemDataBound="lvRoles_ItemDataBound">
                                    <ItemTemplate>
                                        <tr style="">
                                            <td>
                                                <asp:Button ID="DeleteButton" runat="server" CommandName="Delete" Text="Delete" CssClass="btn btn-danger" resourcekey="DeleteButton" OnClientClick='if (!confirm("Are you sure you want to delete?") ){return false;}' />
                                            </td>
                                            <td>
                                                <asp:Label ID="RoleIDLabel" runat="server" Text='<%# Eval("RoleID") %>' />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                        <table id="Table1" runat="server" style="">
                                            <tr>
                                                <td>
                                                    No data was returned.
                                                </td>
                                            </tr>
                                        </table>
                                    </EmptyDataTemplate>
                                    <LayoutTemplate>
                                        <table id="Table2" runat="server">
                                            <tr id="Tr1" runat="server">
                                                <td id="Td1" runat="server">
                                                    <table id="itemPlaceholderContainer" runat="server" border="0" style="">
                                                        <tr id="Tr2" runat="server" style="">
                                                            <th id="Th1" runat="server">
                                                            </th>
                                                            <th id="Th2" runat="server">
                                                                <asp:Label ID="RoleLabel" resourcekey="RoleLabel" runat="server" Text="Role" />
                                                            </th>
                                                        </tr>
                                                        <tr id="itemPlaceholder" runat="server">
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                            <tr id="Tr3" runat="server">
                                                <td id="Td2" runat="server" style="">
                                                </td>
                                            </tr>
                                        </table>
                                    </LayoutTemplate>
                                </asp:ListView>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                &nbsp;<asp:Button ID="btnInsertRole" runat="server" resourcekey="btnInsertRole" OnClick="btnInsertRole_Click" CssClass="btn btn-primary"
                                    Text="Insert" />
                                &nbsp;<asp:DropDownList ID="ddlRole" runat="server" DataTextField="Text" DataValueField="Value" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td>
                                <asp:Label ID="lblRoleError" runat="server" EnableViewState="False" Font-Italic="True"
                                    ForeColor="Red"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel ID="pnlTagsAdmin" runat="server" BorderColor="#CCCCCC" BorderStyle="Solid">
                    <table cellpadding="8" cellspacing="8">
                        <tr>
                            <td valign="top">
                                <asp:Label ID="lblTagError" runat="server" EnableViewState="False" Font-Italic="True"
                                    ForeColor="Red"></asp:Label>
                                <asp:TreeView ID="tvCategories" runat="server" ExpandDepth="0" OnSelectedNodeChanged="tvCategories_SelectedNodeChanged"
                                    BorderColor="#CCCCCC" BorderStyle="Solid" OnTreeNodeDataBound="tvCategories_TreeNodeDataBound">
                                    <SelectedNodeStyle BackColor="#CCCCCC" Font-Bold="False" Font-Underline="False" />
                                    <DataBindings>
                                        <asp:TreeNodeBinding DataMember="ITIL.Modules.ServiceDesk.Catagories" Depth="0"
                                            TextField="Value" ValueField="Value" />
                                    </DataBindings>
                                </asp:TreeView>
                            </td>
                            <td valign="top">
                                <table bgcolor="#CCCCCC" cellpadding="2">
                                    <tr>
                                        <td>
                                            <table>
                                                <tr>
                                                    <td align="right">
                                                        <asp:Label ID="lblCategory" runat="server" resourcekey="lblCategory" Text="Tag:" />
                                                    </td>
                                                    <td>
                                                        <asp:TextBox ID="txtCategory" runat="server" CssClass="form-control"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right">
                                                        <asp:Label ID="lblParentCategory" runat="server" resourcekey="lblParentCategory" Text="Parent Tag:" />
                                                    </td>
                                                    <td>
                                                        <asp:DropDownList ID="ddlParentCategory" runat="server" CssClass="form-control">
                                                        </asp:DropDownList>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" colspan="2">
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="right" colspan="2" nowrap="nowrap">
                                                        <asp:CheckBox ID="chkRequesterVisible" runat="server" Checked="True" resourcekey="chkRequesterVisible" Text="Requester Visible" 
                                                            ToolTip="This option will be visible to users making a ticket request" />
                                                        &nbsp;
                                                        <asp:CheckBox ID="chkSelectable" runat="server" Checked="True" Text="Selectable" resourcekey="chkSelectable" 
                                                            ToolTip="Is a user able to select this option or is it just used for grouping?" />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtCategoryID" runat="server" Columns="1" Visible="False"></asp:TextBox>
                                                        <asp:TextBox ID="txtParentCategoryID" runat="server" Columns="1" Visible="False"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                        &nbsp;
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align="center" colspan="2">
                                                        <asp:Button ID="btnUpdate" runat="server"  OnClick="btnUpdate_Click" CssClass="btn btn-primary"
                                                            Text="Update" CommandName="Update" />
                                                        &nbsp;
                                                        <asp:Button ID="btnAddNew" runat="server"  OnClick="btnAddNew_Click"  CssClass="btn btn-success"
                                                            Text="Add New" CommandName="AddNew" />
                                                        &nbsp;
                                                        <asp:Button ID="btnDelete" runat="server" OnClick="btnDelete_Click"  CssClass="btn btn-danger"
                                                            OnClientClick="if (!confirm(&quot;Are you sure you want to delete?&quot;) ){return false;}"
                                                            Text="Delete" resourcekey="btnDelete" />
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:LinqDataSource ID="ldsRoles" runat="server" ContextTypeName="ITIL.Modules.ServiceDesk.ServiceDeskDALDataContext"
    EnableDelete="True" EnableInsert="True" EnableUpdate="True" OnSelecting="ldsRoles_Selecting"
    TableName="ITILServiceDesk_Roles" Where="PortalID == @PortalID">
    <WhereParameters>
        <asp:Parameter Name="PortalID" Type="Int32" />
    </WhereParameters>
</asp:LinqDataSource>
