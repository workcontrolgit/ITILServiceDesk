<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="View.ascx.cs" Inherits="ITIL.Modules.ServiceDesk.View" %>
<%@ Register Src="Controls/Tags.ascx" TagName="Tags" TagPrefix="ITIL" %>

<ul class="nav nav-pills">
    <li>
        <asp:LinkButton ID="lnkNewTicket" runat="server" OnClick="lnkNewTicket_Click" Text="New Ticket" resourcekey="lnkNewTicket"></asp:LinkButton></li>
    <li>
        <asp:LinkButton ID="lnkExistingTickets" runat="server" OnClick="lnkExistingTickets_Click"
            Text="Existing Tickets" resourcekey="lnkExistingTickets" Visible="False" /></li>
    <li>
        <asp:LinkButton ID="lnkAdministratorSettings" runat="server" OnClick="lnkAdministratorSettings_Click" Text="Administrator Settings"
            resourcekey="lnkAdministratorSettings" Visible="False" /></li>
    <%--        <li class="dropdown">
            <a href="#" data-toggle="dropdown" class="dropdown-toggle">Messages <b class="caret"></b></a>
            <ul class="dropdown-menu">
                <li><a href="#">Inbox</a></li>
                <li><a href="#">Drafts</a></li>
                <li><a href="#">Sent Items</a></li>
                <li class="divider"></li>
                <li><a href="#">Trash</a></li>
            </ul>
        </li>--%>
</ul>


<asp:Panel ID="pnlNewTicket" runat="server">



    <asp:Panel ID="pnlAdminUserSelection" runat="server" Visible="False">
        <div class="form-inline">
            <div class="form-group">
                <asp:TextBox ID="txtSearchForUser" runat="server" CssClass="form-control" placeholder="Enter criteria here"></asp:TextBox>
            </div>
            <div class="form-group">
                <asp:DropDownList ID="ddlSearchForUserType" runat="server" CssClass="form-control">
                    <asp:ListItem Selected="True" Value="LastName" Text="Last Name" resourcekey="ddlSearchForUserTypeLastName" />
                    <asp:ListItem Value="FirstName" Text="First Name" resourcekey="ddlSearchForUserTypeFirstName" />
                    <asp:ListItem Text="Email" resourcekey="ddlSearchForUserTypeEmail" />
                </asp:DropDownList>
            </div>
            <div class="form-group">
                <asp:LinkButton ID="btnSearchUser" runat="server" OnClick="btnSearchUser_Click" ToolTip="Search User" Text="Search" resourcekey="btnSearchUser" CssClass="btn btn-primary" />
            </div>
        </div>

        <div class="form-group">
            <asp:Label ID="lblCurrentProcessorNotFound" runat="server" Text="No record found" CssClass="form-control-static" Visible="False"></asp:Label>
            <asp:GridView ID="gvCurrentProcessor" runat="server" AutoGenerateColumns="False"
                DataKeyNames="UserID" GridLines="None" OnSelectedIndexChanged="gvCurrentProcessor_SelectedIndexChanged" OnPageIndexChanging="gvCurrentProcessor_PageIndexChanging"
                ShowHeader="True" CssClass="table table-bordered" AllowPaging="True" PageSize="5">
                <Columns>
                    <asp:TemplateField HeaderText="Display Name">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkDisplayName" runat="server" CausesValidation="False" CommandArgument='<%# Bind("UserID") %>'
                                CommandName="Select" Text='<%# Bind("DisplayName") %>'></asp:LinkButton>
                            &nbsp;
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField HeaderText="Last" DataField="LastName"></asp:BoundField>
                    <asp:BoundField HeaderText="First" DataField="FirstName"></asp:BoundField>
                    <asp:BoundField HeaderText="Email" DataField="Email"></asp:BoundField>
                </Columns>
            </asp:GridView>
        </div>


    </asp:Panel>

    <div class="panel panel-default">
        <div class="panel-heading">
            <h4>
                <asp:Label ID="lblServiceDeskTicket" runat="server" Text="Service Desk Ticket" resourcekey="lblServiceDeskTicket" /></h4>
        </div>
        <div class="panel-body">
            <asp:Label ID="lblRequiredfield" runat="server" Text="(*) Required Field" CssClass="text-danger" resourcekey="lblRequiredfield" />
        </div>


        <div class="row">

            <div class="col-md-9">

                <div class="form-horizontal">

                    <asp:TextBox ID="txtUserID" runat="server" Columns="1" Visible="False" CssClass="form-control"></asp:TextBox>



                    <div class="form-group">
                        <div class="col-xs-offset-2 col-xs-10">
                            <asp:Label ID="lblError" runat="server" EnableViewState="False" CssClass="label label-warning"></asp:Label>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lbltxtName" runat="server" Text="Name" resourcekey="lbltxtName" CssClass="control-label col-xs-2" AssociatedControlID="txtName" />
                        <div class="col-xs-10">
                            <div class="input-group">
                                <asp:TextBox ID="txtName" runat="server" MaxLength="350" TabIndex="1" CssClass="form-control" placeholder="<enter last name, first name>"></asp:TextBox>
                                <span class="input-group-addon"><span class="text-danger">*</span></span>
                            </div>
                            <asp:RequiredFieldValidator ID="nameRequired" runat="server" Display="Dynamic"
                                ControlToValidate="txtName" ErrorMessage="Please enter name" CssClass="text-danger" ValidationGroup="ticket"></asp:RequiredFieldValidator>
                            <asp:Button ID="btnClearUser" runat="server" OnClick="btnClearUser_Click" Text="Clear User" CssClass="btn btn-warning" Visible="False" />
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lbltxtEmail" runat="server" Text="Email:" resourcekey="lbltxtEmail" CssClass="control-label col-xs-2" AssociatedControlID="txtEmail" />
                        <div class="col-xs-10">
                            <div class="input-group">
                                <asp:TextBox ID="txtEmail" runat="server" MaxLength="350" TabIndex="2" CssClass="form-control" placeholder="<enter email address>"></asp:TextBox>
                                <span class="input-group-addon"><span class="text-danger">*</span></span>
                            </div>
                            <asp:RegularExpressionValidator ID="emailValidator" runat="server" Display="Dynamic" ErrorMessage="Please enter valid email address" ValidationExpression="^[\w\.\-]+@[a-zA-Z0-9\-]+(\.[a-zA-Z0-9\-]{1,})*(\.[a-zA-Z]{2,3}){1,2}$"
                                ControlToValidate="txtEmail" CssClass="text-danger" ValidationGroup="ticket">
                            </asp:RegularExpressionValidator>

                            <asp:RequiredFieldValidator ID="emailRequired" runat="server" Display="Dynamic"
                                ControlToValidate="txtEmail" ErrorMessage="Please enter an email" CssClass="text-danger" ValidationGroup="ticket"></asp:RequiredFieldValidator>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lbltxtPhone" runat="server" Text="Phone:" resourcekey="lbltxtPhone" CssClass="control-label col-xs-2" AssociatedControlID="txtPhone" />
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtPhone" runat="server" MaxLength="50" TabIndex="3" CssClass="form-control" placeholder="<enter phone number including area code>"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lbltxtDescription" runat="server"
                            Text="Description:" resourcekey="lbltxtDescription" CssClass="control-label col-xs-2" AssociatedControlID="txtDescription" />
                        <div class="col-xs-10">
                            <div class="input-group">
                                <asp:TextBox ID="txtDescription" runat="server" MaxLength="150" TabIndex="4" CssClass="form-control" placeholder="<enter brief description>"></asp:TextBox>
                                <span class="input-group-addon"><span class="text-danger">*</span></span>
                            </div>
                            <asp:RequiredFieldValidator ID="descriptionRequired" runat="server" Display="Dynamic"
                                ControlToValidate="txtDescription" ErrorMessage="Please enter description" CssClass="text-danger" ValidationGroup="ticket"></asp:RequiredFieldValidator>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lbltxtDetail" runat="server" Text="Detail:" resourcekey="lbltxtDetail" CssClass="control-label col-xs-2" AssociatedControlID="txtDetails" />
                        <div class="col-xs-10">
                            <asp:TextBox ID="txtDetails" runat="server" Columns="40" MaxLength="500" Rows="5" Width="100%" CssClass="form-control"
                                TabIndex="5" TextMode="MultiLine" placeholder="<enter system message, error code, etc.; upload screenshot via Attach File option below if available>"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lblAttachFile" runat="server" Text="Attach File:"
                            resourcekey="lblAttachFile" Visible="False" CssClass="control-label col-xs-2" AssociatedControlID="TicketFileUpload" />
                        <div class="col-xs-10">
                            <span class="btn btn-default btn-file">
                                <asp:FileUpload ID="TicketFileUpload" runat="server" TabIndex="8"
                                    Visible="False" /></span>
                        </div>
                    </div>


                    <div class="form-group">
                        <asp:Label ID="lbltxtDueDate" runat="server" Text="Date Due:" resourcekey="lbltxtDueDate" CssClass="control-label col-xs-2" AssociatedControlID="txtDueDate" />
                        <div class="col-xs-4">
                            <div class="input-group">
                                <asp:TextBox ID="txtDueDate" runat="server" MaxLength="25" TabIndex="6" TextMode="DateTime" CssClass="form-control"></asp:TextBox>
                                <span class="input-group-addon">
                                    <asp:HyperLink ID="cmdStartCalendar" runat="server" ImageUrl="~/DesktopModules/ITILServiceDesk/images/calendar.png"></asp:HyperLink></span>
                            </div>
                            <asp:CompareValidator ID="DueDatValidator" runat="server" Display="Dynamic" Type="Date" Operator="DataTypeCheck" CssClass="text-danger" ControlToValidate="txtDueDate" ErrorMessage="Please enter a valid date." ValidationGroup="ticket">
                            </asp:CompareValidator>
                        </div>

                        <asp:Label ID="lbltxtPriority" runat="server" Text="Priority:" resourcekey="lbltxtPriority" CssClass="control-label col-xs-2" AssociatedControlID="ddlPriority" />
                        <div class="col-xs-4">
                            <asp:DropDownList ID="ddlPriority" runat="server" TabIndex="7" CssClass="form-control">
                                <asp:ListItem Selected="True" resourcekey="ddlPriorityNormal" Value="Normal" Text="Normal" />
                                <asp:ListItem resourcekey="ddlPriorityHigh" Value="High" Text="High" />
                                <asp:ListItem resourcekey="ddlPriorityLow" Value="Low" Text="Low" />
                            </asp:DropDownList>
                        </div>
                    </div>



                    <div class="form-group">
                        <div class="col-xs-offset-2 col-xs-10">

                            <asp:Button ID="btnSubmit" runat="server" OnClick="btnSubmit_Click"
                                TabIndex="9" Text="Submit Ticket" resourcekey="btnSubmit" CssClass="btn btn-primary" ValidationGroup="ticket" />
                        </div>
                    </div>



                </div>

            </div>
            <div class="col-md-3">

                <asp:Panel ID="pnlAdminTicketStatus" runat="server" Visible="False">
                    <div class="form-group">
                        <asp:Label ID="lblStatusAdmin" runat="server" Text="Status:" resourcekey="lblStatus" AssociatedControlID="ddlStatusAdmin" />
                        <asp:DropDownList ID="ddlStatusAdmin" runat="server" TabIndex="11" CssClass="form-control">
                            <asp:ListItem resourcekey="ddlStatusAdminNew" Value="New" Text="New" />
                            <asp:ListItem resourcekey="ddlStatusAdminActive" Value="Active" Text="Active" />
                            <asp:ListItem resourcekey="ddlStatusAdminOnHold" Value="On Hold" Text="On Hold" />
                            <asp:ListItem resourcekey="ddlStatusAdminResolved" Value="Resolved" Text="Resolved" />
                            <asp:ListItem resourcekey="ddlStatusAdminCancelled" Value="Cancelled" Text="Cancelled" />
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblAssignedAdmin" runat="server" Text="Assigned:" resourcekey="lblAssigned" AssociatedControlID="ddlAssignedAdmin" />
                        <asp:DropDownList ID="ddlAssignedAdmin" runat="server" TabIndex="10" CssClass="form-control"></asp:DropDownList>
                    </div>
                </asp:Panel>


                <div class="form-group">
                    &nbsp;<asp:Image ID="imgTags" runat="server" ImageUrl="~/DesktopModules/ITILServiceDesk/images/tag_blue.png" />
                    &nbsp;
                            <asp:Label ID="lblCheckTags" runat="server" Font-Bold="True" Text="Check all Tags that apply:"
                                resourcekey="lblCheckTags" AssociatedControlID="TagsTree" />
                    <ITIL:Tags ID="TagsTree" runat="server" EnableViewState="True" Visible="False" />


                </div>


            </div>
        </div>

    </div>


</asp:Panel>

<asp:Panel ID="pnlConfirmAnonymousUserEntry" runat="server" Visible="False">
    <div class="panel panel-info">
        <div class="panel-heading">
            <h4>
                <asp:Label ID="lblTicketHasBeenSubmitted" resourcekey="lblTicketHasBeenSubmitted"
                    runat="server" Text="Your Ticket has been submitted." /></h4>
        </div>
        <div class="panel-body">
            <asp:Label ID="lblConfirmAnonymousUser" runat="server" CssClass="text-info"></asp:Label>
        </div>
        <div class="panel-footer">
            <asp:LinkButton ID="lnlAnonymousContinue" runat="server" OnClick="lnlAnonymousContinue_Click"
                Text="Click here to continue" resourcekey="lnlAnonymousContinue" CssClass="btn btn-primary" />
        </div>
    </div>

</asp:Panel>

<asp:Panel ID="pnlExistingTickets" runat="server" Visible="False">

    <div class="containter">

            <ul class="nav nav-pills pull-right">
                <!-- Trigger Button HTML -->
                <li>
                    <asp:HyperLink href="#" runat="server" class="btn btn-link btn-sm" data-toggle="collapse" data-target="#toggleDemo"><span class="glyphicon glyphicon-search"></span> Filter</asp:HyperLink></li>
                <li>
                    <asp:LinkButton ID="lnkResetSearch" runat="server" OnClick="lnkResetSearch_Click"
                        Text="Reset Search" resourcekey="lnkResetSearch" Visible="False" class="btn btn-link btn-sm" /></li>

            </ul>

            <!-- Collapsible Element HTML -->
            <div id="toggleDemo" class="collapse">
                <hr />
                <div class="form-horizontal">
                    <div class="form-group">
                        <asp:Label ID="lblStatusSearch" runat="server" Text="Status:" resourcekey="lblStatus" CssClass="control-label col-xs-2" AssociatedControlID="ddlStatusSearch" />
                        <div class="col-xs-4">
                            <asp:DropDownList ID="ddlStatusSearch" runat="server" CssClass="form-control">
                                <asp:ListItem resourcekey="ddlStatusAdminAll" Value="*All*" Text="*All*" />
                                <asp:ListItem resourcekey="ddlStatusAdminNew" Value="New" Text="New" />
                                <asp:ListItem resourcekey="ddlStatusAdminActive" Value="Active" Text="Active" />
                                <asp:ListItem resourcekey="ddlStatusAdminOnHold" Value="On Hold" Text="On Hold" />
                                <asp:ListItem resourcekey="ddlStatusAdminResolved" Value="Resolved" Text="Resolved" />
                                <asp:ListItem resourcekey="ddlStatusAdminCancelled" Value="Cancelled" Text="Cancelled" />
                            </asp:DropDownList>
                        </div>
                        <asp:Label ID="lblPrioritySearch" runat="server" Text="Priority:" resourcekey="lbltxtPriority" CssClass="control-label col-xs-2" AssociatedControlID="ddlPriority" />
                        <div class="col-xs-4">
                            <asp:DropDownList ID="ddlPrioritySearch" runat="server" CssClass="form-control">
                                <asp:ListItem resourcekey="ddlPriorityAdminAll" Value="*All*" Text="*All*" />
                                <asp:ListItem resourcekey="ddlPriorityNormal" Value="Normal" Text="Normal" />
                                <asp:ListItem resourcekey="ddlPriorityHigh" Value="High" Text="High" />
                                <asp:ListItem resourcekey="ddlPriorityLow" Value="Low" Text="Low" />
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblDueSearch" runat="server" Text="Date Due:" resourcekey="lbltxtDueDate" CssClass="control-label col-xs-2" AssociatedControlID="txtDueSearch" />
                        <div class="col-xs-4">
                            <asp:TextBox ID="txtDueSearch" runat="server" MaxLength="25" TextMode="DateTime" CssClass="form-control"></asp:TextBox>
                            <asp:CompareValidator ID="DueValidator" runat="server" Display="Dynamic" Type="Date" Operator="DataTypeCheck" CssClass="text-danger" ControlToValidate="txtDueSearch" ErrorMessage="Please enter a valid date.">
                            </asp:CompareValidator>
                        </div>
                        <label for="txtCreatedSearch" class="control-label col-xs-2">Created</label>
                        <div class="col-xs-4">
                            <asp:TextBox ID="txtCreatedSearch" runat="server" CssClass="form-control" />
                            <asp:CompareValidator ID="CreatedValidator" runat="server" Display="Dynamic" Type="Date" Operator="DataTypeCheck" CssClass="text-danger" ControlToValidate="txtCreatedSearch" ErrorMessage="Please enter a valid date.">
                            </asp:CompareValidator>

                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lblAssignedSearch" runat="server" Text="Assigned:" resourcekey="lblAssigned" CssClass="control-label col-xs-2" AssociatedControlID="ddlAssignedSearch" />
                        <div class="col-xs-4">
                            <asp:DropDownList ID="ddlAssignedSearch" runat="server" DataTextField="AssignedRoleID" CssClass="form-control"
                                DataValueField="Key" />
                        </div>
                        <asp:Label ID="lblSearch" resourcekey="KeyWordSearch" runat="server" class="control-label col-xs-2" Text="Key word search" AssociatedControlID="txtSearch" />
                        <div class="col-xs-4">
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="search text here" />
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lblSearchTags" resourcekey="lblSearchTags" runat="server" class="control-label col-xs-2"
                            Text="Search Tags:" AssociatedControlID="TagsTreeExistingTasks" />
                        <div class="col-xs-4">
                            <ITIL:Tags ID="TagsTreeExistingTasks" runat="server" Visible="False" />
                        </div>
                        <div class="col-xs-2">
                            <asp:LinkButton ID="btnSearch" runat="server" ToolTip="Search" Text="Search" resourcekey="btnSearch" CommandName="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" />
                        </div>
                    </div>
                </div>

            </div>

    </div>
    <asp:ListView ID="lvTasks" runat="server" OnItemDataBound="lvTasks_ItemDataBound"
        OnSorting="lvTasks_Sorting"
        OnDataBound="lvTasks_DataBound" Visible="False">

        <LayoutTemplate>

            <div class="row">
                <div class="table-responsive">
                    <table id="itemPlaceholderContainer" runat="server" class="table table-bordered table-hover table-striped">
                        <thead>
                            <tr id="Tr4" runat="server">
                                <td runat="server" nowrap="nowrap">
                                    <asp:LinkButton ID="lnkTaskID" runat="server" CommandName="Sort" CommandArgument="TaskID"
                                        Text="TaskID" resourcekey="lnkTaskID" />
                                    <asp:ImageButton ID="TaskIDImage" CommandName="Sort" CommandArgument="TaskID" runat="server"
                                        ImageUrl="~/DesktopModules/ITILServiceDesk/images/dt-arrow-dn.png" Visible="false" />
                                </td>
                                <td runat="server" nowrap="nowrap" align="left">
                                    <asp:LinkButton ID="lnkStatus" runat="server" CommandName="Sort" CommandArgument="Status"
                                        Text="Status" resourcekey="lnkStatus" />
                                    <asp:ImageButton ID="StatusImage" CommandName="Sort" CommandArgument="Status" runat="server"
                                        ImageUrl="~/DesktopModules/ITILServiceDesk/images/dt-arrow-dn.png" Visible="false" />
                                </td>
                                <td runat="server" nowrap="nowrap" align="left">
                                    <asp:LinkButton ID="lnkPriority" runat="server" CommandName="Sort" CommandArgument="Priority"
                                        Text="Priority" resourcekey="lnkPriority" /><asp:ImageButton
                                            ID="PriorityImage" CommandName="Sort" CommandArgument="Priority" runat="server"
                                            ImageUrl="~/DesktopModules/ITILServiceDesk/images/dt-arrow-dn.png" Visible="false" />
                                </td>
                                <td runat="server" nowrap="nowrap" align="left">
                                    <asp:LinkButton ID="lnkDueDate" runat="server" CommandName="Sort" CommandArgument="DueDate"
                                        Text="Due" resourcekey="lnkDueDate" /><asp:ImageButton ID="DueDateImage"
                                            CommandName="Sort" CommandArgument="DueDate" runat="server" ImageUrl="~/DesktopModules/ITILServiceDesk/images/dt-arrow-dn.png"
                                            Visible="false" />
                                </td>
                                <td runat="server" nowrap="nowrap" align="left">
                                    <asp:LinkButton ID="lnkCreatedDate" runat="server" CommandName="Sort" CommandArgument="CreatedDate"
                                        Text="Created" resourcekey="lnkCreatedDate" />
                                    <asp:ImageButton ID="CreatedDateImage" CommandName="Sort" CommandArgument="CreatedDate"
                                        runat="server" ImageUrl="~/DesktopModules/ITILServiceDesk/images/dt-arrow-dn.png"
                                        Visible="false" />
                                </td>
                                <td runat="server" nowrap="nowrap" align="left">
                                    <asp:LinkButton ID="lnkAssigned" runat="server" CommandName="Sort" CommandArgument="Assigned"
                                        Text="Assigned" resourcekey="lnkAssigned" />
                                    <asp:ImageButton ID="AssignedImage" CommandName="Sort" CommandArgument="Assigned"
                                        runat="server" ImageUrl="~/DesktopModules/ITILServiceDesk/images/dt-arrow-dn.png"
                                        Visible="false" />
                                </td>
                                <td runat="server" nowrap="nowrap" align="left">
                                    <asp:LinkButton ID="lnkDescription" runat="server" CommandName="Sort" CommandArgument="Description"
                                        Text="Description" resourcekey="lnkDescription" />
                                    <asp:ImageButton ID="DescriptionImage" CommandName="Sort" CommandArgument="Description"
                                        runat="server" ImageUrl="~/DesktopModules/ITILServiceDesk/images/dt-arrow-dn.png"
                                        Visible="false" />
                                </td>
                                <td runat="server" nowrap="nowrap" align="left">
                                    <asp:LinkButton ID="lnkRequester" runat="server" CommandName="Sort" CommandArgument="Requester"
                                        Text="Requester" resourcekey="lnkRequester" />
                                    <asp:ImageButton ID="RequesterImage" CommandName="Sort" CommandArgument="Requester"
                                        runat="server" ImageUrl="~/DesktopModules/ITILServiceDesk/images/dt-arrow-dn.png"
                                        Visible="false" />
                                </td>
                            </tr>
                        </thead>
                        <tbody>
                            <tr id="itemPlaceholder" runat="server">
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </LayoutTemplate>

        <ItemTemplate>
            <tr>
                <td align="center">
                    <asp:HyperLink ID="lnkTaskID" runat="server" Text='<%# Eval("TaskID") %>' />
                </td>
                <td align="left">
                    <asp:Label ID="StatusLabel" runat="server" Text='<%# LocalizeStatusBinding(Eval("Status", "{0}")) %>' />
                </td>
                <td align="left">
                    <asp:Label ID="PriorityLabel" runat="server" Text='<%# LocalizePriorityBinding(Eval("Priority", "{0}")) %>' />
                </td>
                <td align="left">
                    <asp:Label ID="DueDateLabel" runat="server" Text='<%# Eval("DueDate") %>' />
                </td>
                <td align="left" nowrap="nowrap">
                    <asp:Label ID="CreatedDateLabel" runat="server" Text='<%# Eval("CreatedDate") %>' />
                </td>
                <td align="left">
                    <asp:Label ID="AssignedRoleIDLabel" runat="server" Text='<%# Eval("Assigned") %>' />
                </td>
                <td align="left" nowrap="nowrap">
                    <asp:Label ID="DescriptionLabel" runat="server" Text='<%# Eval("Description") %>'
                        ToolTip='<%# Eval("Description") %>' />
                </td>
                <td align="left" nowrap="nowrap">
                    <asp:Label ID="RequesterUserIDLabel" runat="server" Text='<%# Eval("Requester") %>'
                        Visible="false" />
                    <asp:Label ID="RequesterNameLabel" runat="server" Text='<%# Eval("RequesterName") %>'
                        ToolTip='<%# Eval("RequesterName") %>' />
                </td>
            </tr>
        </ItemTemplate>

        <EmptyDataTemplate>
            <div>
                <asp:Label ID="lblNoRecords" runat="server" resourcekey="lblNoRecords" Text="No Records Returned" CssClass="text-warning" />
            </div>
        </EmptyDataTemplate>

    </asp:ListView>

    <div class="form-inline">
        <div class="form-group">
            <label class="control-label" for="ddlPageSize">Page Size:</label>
            <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="True"
                OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" CssClass="form-control">
                <asp:ListItem>5</asp:ListItem>
                <asp:ListItem>10</asp:ListItem>
                <asp:ListItem Selected="True">25</asp:ListItem>
                <asp:ListItem>50</asp:ListItem>
                <asp:ListItem>75</asp:ListItem>
                <asp:ListItem>100</asp:ListItem>
                <asp:ListItem>300</asp:ListItem>
                <asp:ListItem>500</asp:ListItem>
                <asp:ListItem>1000</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="form-group">
            <asp:Panel ID="pnlPaging" runat="server">
                <asp:LinkButton ID="lnkFirst" runat="server" CssClass="pagination" OnClick="lnkFirst_Click"
                    Text="&lt;&lt;" Visible="False" />
                &nbsp;<asp:LinkButton ID="lnkPrevious" runat="server" CssClass="btn btn-link" OnClick="lnkPrevious_Click"
                    Text="&lt;" />
                &nbsp;
                                <asp:DataList ID="PagingDataList" runat="server" DataKeyField="PageNumber" RepeatColumns="20"
                                    RepeatDirection="Horizontal" ShowFooter="False" ShowHeader="False" OnItemDataBound="PagingDataList_ItemDataBound"
                                    RepeatLayout="Flow">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkPage" Text='<%# Eval("PageNumber") %>'
                                            runat="server" CommandArgument='<%# Eval("PageNumber") %>' OnClick="lnkPage_Click" CssClass="btn btn-link" />
                                    </ItemTemplate>
                                </asp:DataList>
                &nbsp;<asp:LinkButton ID="lnkNext" runat="server" CssClass="btn btn-link" OnClick="lnkNext_Click"
                    Text="&gt;" />
                &nbsp;<asp:LinkButton ID="lnkLast" runat="server" CssClass="btn btn-link" OnClick="lnkLast_Click"
                    Text="&gt;&gt;" Visible="False" />&nbsp;
            </asp:Panel>
        </div>
    </div>

</asp:Panel>

<asp:ObjectDataSource ID="masterDataSource" runat="server" SelectMethod="GetUsers"
    TypeName="ITIL.Modules.ServiceDesk.Search">
    <SelectParameters>
        <asp:SessionParameter Name="portalId" SessionField="portalId" DbType="Int32" />
    </SelectParameters>
</asp:ObjectDataSource>

