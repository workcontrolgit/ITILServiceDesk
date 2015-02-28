<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminSettings.ascx.cs" Inherits="ITIL.Modules.ServiceDesk.AdminSettings" %>
<asp:Panel ID="pnlAdminSettings" runat="server" align="left">
    <%--<asp:Image ID="Image3" runat="server" ImageUrl="~/DesktopModules/ITILServiceDesk/images/application_side_contract.png" />--%>
    <style type="text/css">
        .style1 {
            width: 19px;
        }
    </style>
    <asp:LinkButton ID="lnkBack" runat="server" resourcekey="lnkBack" CssClass="btn btn-primary"
        OnClick="lnkBack_Click" Text="Back" />
    <br />
    <br />


    <div class="row">

        <div class="col-md-3">
            <ul class="nav nav-pills nav-stacked">
                <li>
                    <asp:LinkButton ID="lnkAdminRole" runat="server" OnClick="lnkAdminRole_Click"
                        resourcekey="lnkAdminRole" Text="Administrator Role" /></li>
                <li>
                    <asp:LinkButton ID="lnkRoles" runat="server" OnClick="lnkRoles_Click"
                        resourcekey="lnkRoles" Text="Assignment Roles" /></li>
                <li>
                    <asp:LinkButton ID="lnkTagsAdmin" runat="server" OnClick="lnkTagsAdmin_Click"
                        resourcekey="lnkTagsAdmin" Text="Tags Administration" /></li>
                <li>
                    <asp:LinkButton ID="lnkUploadefFilesPath" runat="server" OnClick="lnkUploadefFilesPath_Click"
                        resourcekey="lnkUploadefFilesPath" Text="File Upload Settings" /></li>
            </ul>
        </div>
        <div class="col-md-9">
            <asp:Panel ID="pnlAdministratorRole" runat="server">
                <div class="form-horizontal">
                    <div class="form-group">
                        <asp:Label ID="lbltxtAdministratorRole" runat="server" resourcekey="lbltxtAdministratorRole" CssClass="control-label col-xs-3" AssociatedControlID="ddlAdminRole"
                            Text="Administrator Role:" />
                        <div class="col-xs-9">
                            <asp:DropDownList ID="ddlAdminRole" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-offset-3 col-xs-9">
                            <asp:Button ID="btnUpdateAdminRole" runat="server" OnClick="btnUpdateAdminRole_Click"
                                Text="Update" resourcekey="btnUpdateAdminRole" CssClass="btn btn-primary" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-offset-3 col-xs-9">
                            <asp:Label ID="lblAdminRole" runat="server" EnableViewState="False" CssClass="label label-warning"></asp:Label>
                        </div>
                    </div>
                </div>

            </asp:Panel>
            <asp:Panel ID="pnlUploadefFilesPath" runat="server">
                <div class="form-horizontal">
                    <div class="form-group">
                        <asp:Label ID="lbltxtFileUploadPath" runat="server" resourcekey="lbltxtFileUploadPath" Text="File Upload Path:" CssClass="control-label col-xs-2" AssociatedControlID="txtUploadedFilesPath" />
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtUploadedFilesPath" runat="server" Columns="50" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lbltxtFileUploadPermission" runat="server" resourcekey="lbltxtFileUploadPermission"
                            Text="File Upload Permission:" CssClass="control-label col-xs-2" AssociatedControlID="ddlUploadPermission" />
                        <div class="col-xs-10">
                            <asp:DropDownList ID="ddlUploadPermission" runat="server" CssClass="form-control">
                                <asp:ListItem Selected="True" Text="All" Value="All" resourcekey="ddlUploadPermissionAll" />
                                <asp:ListItem Text="Administrator" Value="Administrator" resourcekey="ddlUploadPermissionAdministrator" />
                                <asp:ListItem Text="Administrator/Registered Users" Value="Administrator/Registered Users" resourcekey="ddlUploadPermissionAdminRegUser" />
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="col-xs-offset-2 col-xs-10">
                            <asp:Button ID="btnUploadedFiles" runat="server" Text="Update" resourcekey="btnUpdateAdminRole" OnClick="btnUploadedFiles_Click" CssClass="btn btn-primary" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-offset-2 col-xs-10">
                            <asp:Label ID="lblUploadedFilesPath" runat="server" EnableViewState="False" CssClass="label label-warning"></asp:Label>
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="pnlRoles" runat="server">
                <div class="form-inline">
                    <div class="form-group">
                        <div class="col-xs-3">
                            <asp:Button ID="btnInsertRole" runat="server" resourcekey="btnInsertRole" OnClick="btnInsertRole_Click" CssClass="btn btn-primary"
                                Text="Insert" />
                        </div>
                        <div class="col-xs-9">
                            <asp:DropDownList ID="ddlRole" runat="server" DataTextField="Text" DataValueField="Value" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-xs-offset-3 col-xs-9">
                            <asp:Label ID="lblRoleError" runat="server" EnableViewState="False" CssClass="label label-warning"></asp:Label>
                        </div>
                    </div>
                </div>
                <asp:ListView ID="lvRoles" runat="server" DataKeyNames="ID" DataSourceID="ldsRoles"
                    OnItemDataBound="lvRoles_ItemDataBound">
                    <LayoutTemplate>
                        <asp:Label ID="lbltxtAssignmentRoles" runat="server" resourcekey="lbltxtAssignmentRoles" CssClass="lead"
                            Text="Assignment Roles:" />
                        <div class="container">
                            <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                        </div>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <p>
                        <div class="row">
                            <div class="col-xs-2">
                                <asp:Button ID="DeleteButton" runat="server" CommandName="Delete" Text="Delete" CssClass="btn btn-danger" resourcekey="DeleteButton" OnClientClick='if (!confirm("Are you sure you want to delete?") ){return false;}' />
                            </div>
                            <div class="col-xs-10">
                                <asp:Label ID="RoleIDLabel" runat="server" Text='<%# Eval("RoleID") %>' />
                            </div>
                        </div>
                            </p>

                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <div>
                            No record found.
                        </div>
                    </EmptyDataTemplate>
                </asp:ListView>



            </asp:Panel>
            <asp:Panel ID="pnlTagsAdmin" runat="server">
                <div class="row">
                    <div class="col-xs-5">
                        <p>
                        <asp:Button ID="btnAddNew" runat="server" OnClick="btnAddNew_Click" CssClass="btn btn-success"
                                                            Text="Add New" CommandName="AddNew" />
                            </p>
                        <asp:TreeView ID="tvCategories" runat="server" ExpandDepth="0" OnSelectedNodeChanged="tvCategories_SelectedNodeChanged"
                             OnTreeNodeDataBound="tvCategories_TreeNodeDataBound">
                            <SelectedNodeStyle BackColor="#CCCCCC" Font-Bold="False" Font-Underline="False" />
                            <DataBindings>
                                <asp:TreeNodeBinding DataMember="ITIL.Modules.ServiceDesk.Catagories" Depth="0"
                                    TextField="Value" ValueField="Value" />
                            </DataBindings>
                        </asp:TreeView>
                        <asp:Label ID="lblTagError" runat="server" EnableViewState="False"  CssClass="label label-warning"></asp:Label>
                    </div>
                    <div class="col-xs-7">
                        <div class="form-horizontal">
                            <asp:TextBox ID="txtCategoryID" runat="server" Columns="1" Visible="False"></asp:TextBox>
                            <asp:TextBox ID="txtParentCategoryID" runat="server" Columns="1" Visible="False"></asp:TextBox>
                            <div class="form-group">
                                <asp:Label ID="lblCategory" runat="server" CssClass="control-label col-xs-3" AssociatedControlID="txtCategory" resourcekey="lblCategory" Text="Tag:" />

                                <div class="col-xs-9">
                                    <asp:TextBox ID="txtCategory" runat="server" CssClass="form-control"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label ID="lblParentCategory" runat="server" CssClass="control-label col-xs-3" AssociatedControlID="ddlParentCategory" resourcekey="lblParentCategory" Text="Parent Tag:" />
                                <div class="col-xs-9">
                                    <asp:DropDownList ID="ddlParentCategory" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-xs-offset-3 col-xs-9">
                                    <asp:CheckBox ID="chkRequesterVisible" runat="server" Checked="True" resourcekey="chkRequesterVisible" Text="Requester Visible"
                                        ToolTip="This option will be visible to users making a ticket request" />
                                    &nbsp;
                                                        <asp:CheckBox ID="chkSelectable" runat="server" Checked="True" Text="Selectable" resourcekey="chkSelectable"
                                                            ToolTip="Is a user able to select this option or is it just used for grouping?" />
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="col-xs-offset-3 col-xs-9">
                                    <asp:Button ID="btnUpdate" runat="server" OnClick="btnUpdate_Click" CssClass="btn btn-primary"
                                        Text="Update" CommandName="Update" />
                                    &nbsp;
                                                        <asp:Button ID="btnDelete" runat="server" OnClick="btnDelete_Click" CssClass="btn btn-danger"
                                                            OnClientClick="if (!confirm(&quot;Are you sure you want to delete?&quot;) ){return false;}"
                                                            Text="Delete" resourcekey="btnDelete" />
                                </div>
                            </div>
                        </div>

                    </div>

                </div>


            </asp:Panel>
        </div>
    </div>


</asp:Panel>
<asp:LinqDataSource ID="ldsRoles" runat="server" ContextTypeName="ITIL.Modules.ServiceDesk.ServiceDeskDALDataContext"
    EnableDelete="True" EnableInsert="True" EnableUpdate="True" OnSelecting="ldsRoles_Selecting"
    TableName="ITILServiceDesk_Roles" Where="PortalID == @PortalID">
    <WhereParameters>
        <asp:Parameter Name="PortalID" Type="Int32" />
    </WhereParameters>
</asp:LinqDataSource>
