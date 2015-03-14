<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditTask.ascx.cs" Inherits="ITIL.Modules.ServiceDesk.EditTask" %>
<%@ Register Src="Controls/Tags.ascx" TagName="Tags" TagPrefix="uc1" %>
<%@ Register Src="Controls/Comments.ascx" TagName="Comments" TagPrefix="ITIL" %>
<%@ Register Src="Controls/Logs.ascx" TagName="Logs" TagPrefix="ITIL" %>
<%@ Register Src="Controls/Work.ascx" TagName="Work" TagPrefix="ITIL" %>

<div class="table-responsive">
    <asp:Panel ID="pnlEditTask" runat="server" HorizontalAlign="Left">

        <p>
            <ul class="nav nav-pills">
                <li>
                    <asp:LinkButton ID="lnkNewTicket" resourcekey="lnkNewTicket" runat="server" OnClick="lnkNewTicket_Click">New Ticket</asp:LinkButton></li>
                <li>
                    <asp:LinkButton ID="lnkExistingTickets" resourcekey="lnkExistingTickets" runat="server" OnClick="lnkExistingTickets_Click">Existing Tickets</asp:LinkButton></li>
                <li>
                    <asp:LinkButton ID="lnkAdministratorSettings" resourcekey="lnkAdministratorSettings" runat="server" OnClick="lnkAdministratorSettings_Click">Administrator Settings</asp:LinkButton></li>
            </ul>
        </p>


        <div class="panel panel-default">
            <div class="panel-heading">
                <h4><asp:Label ID="lblServiceDeskTicket" runat="server" Text="Ticket - Edit" resourcekey="lblServiceDeskTicket" /></h4>
            </div>
            <div class="panel-body">
                <asp:Label ID="lblRequiredfield" runat="server" Text="(*) Required Field" CssClass="text-danger" resourcekey="lblRequiredfield" />




            </div>

            <div class="row">
                <div class="col-md-9">

                    <div class="form-horizontal">
                        <div class="form-group">
                            <asp:Label ID="lblTicket" runat="server" resourcekey="lblTicket" Text="Ticket:" CssClass="control-label col-xs-2" AssociatedControlID="lblTask" />
                            <div class="col-xs-4">
                                <p class="form-control-static">
                                    <asp:Label ID="lblTask" runat="server"></asp:Label>
                                </p>
                            </div>
                            <asp:Label ID="lblCreated" runat="server" CssClass="control-label col-xs-2" resourcekey="lblCreated" AssociatedControlID="lblCreatedData"></asp:Label>
                            <div class="col-xs-4">
                                <p class="form-control-static">
                                    <asp:Label ID="lblCreatedData" runat="server"></asp:Label>
                                </p>
                            </div>
                        </div>


                        <div class="form-group">
                            <asp:Label ID="lblStatus" runat="server" resourcekey="lblStatus" Text="Status:" AssociatedControlID="ddlStatus" CssClass="control-label col-xs-2" />
                            <div class="col-xs-4">
                                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control">
                                    <asp:ListItem resourcekey="ddlStatusAdminNew" Value="New" Text="New" />
                                    <asp:ListItem resourcekey="ddlStatusAdminActive" Value="Active" Text="Active" />
                                    <asp:ListItem resourcekey="ddlStatusAdminOnHold" Value="On Hold" Text="On Hold" />
                                    <asp:ListItem resourcekey="ddlStatusAdminResolved" Value="Resolved" Text="Resolved" />
                                    <asp:ListItem resourcekey="ddlStatusAdminCancelled" Value="Cancelled" Text="Cancelled" />
                                </asp:DropDownList>
                            </div>

                            <asp:Label ID="lblAssigned" runat="server" resourcekey="lblAssigned" Text="Assigned:" CssClass="control-label col-xs-2" AssociatedControlID="ddlAssigned" />
                            <div class="col-xs-4">
                                <asp:DropDownList ID="ddlAssigned" runat="server" CssClass="form-control"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="lbltxtPriority" runat="server" resourcekey="lbltxtPriority" Text="Priority:" CssClass="control-label col-xs-2" AssociatedControlID="ddlPriority" />
                            <div class="col-xs-4">
                                <asp:DropDownList ID="ddlPriority" runat="server" CssClass="form-control">
                                    <asp:ListItem resourcekey="ddlPriorityNormal" Value="Normal" Text="Normal" />
                                    <asp:ListItem resourcekey="ddlPriorityHigh" Value="High" Text="High" />
                                    <asp:ListItem resourcekey="ddlPriorityLow" Value="Low" Text="Low" />
                                </asp:DropDownList>
                            </div>
                            <asp:Label ID="lbltxtDueDate" runat="server" resourcekey="lbltxtDueDate" Text="Date Due:" CssClass="control-label col-xs-2" AssociatedControlID="txtDueDate" />
                            <div class="col-xs-4">
                                <div class="input-group">
                                    <asp:TextBox ID="txtDueDate" runat="server" Columns="8" CssClass="form-control"></asp:TextBox>
                                    <span class="input-group-addon">
                                        <asp:HyperLink ID="cmdtxtDueDateCalendar" runat="server" ToolTip="calendar"><span class="glyphicon glyphicon-calendar"></span></asp:HyperLink></span>
                                </div>
                                <asp:CompareValidator ID="DueDatValidator" runat="server" Display="Dynamic" Type="Date" Operator="DataTypeCheck" CssClass="text-danger" ControlToValidate="txtDueDate" ErrorMessage="Please enter a valid date." ValidationGroup="ticket">
                                </asp:CompareValidator>

                            </div>
                        </div>


                        <div class="form-group">
                            <asp:Label ID="lbltxtName" runat="server" resourcekey="lbltxtName" Text="Name:" CssClass="control-label col-xs-2" AssociatedControlID="txtName" />
                            <div class="col-xs-4">
                                <div class="input-group">
                                    <asp:TextBox ID="txtName" runat="server" MaxLength="350" CssClass="form-control"></asp:TextBox>
                                    <span class="input-group-addon"><span class="text-danger">*</span></span>
                                </div>
                                <asp:RequiredFieldValidator ID="nameRequired" runat="server" Display="Dynamic"
                                    ControlToValidate="txtName" ErrorMessage="Please enter name" CssClass="text-danger" ValidationGroup="ticket"></asp:RequiredFieldValidator>

                                <%--<p class="form-control-static"><asp:Label ID="lblName" runat="server" Visible="False"></asp:Label></p>--%>
                            </div>
                            <asp:Label ID="lbltxtEmail" runat="server" resourcekey="lbltxtEmail" Text="Email:" CssClass="control-label col-xs-2" AssociatedControlID="txtEmail" />
                            <div class="col-xs-4">
                                <div class="input-group">
                                    <asp:TextBox ID="txtEmail" runat="server" MaxLength="350" CssClass="form-control"></asp:TextBox>
                                    <span class="input-group-addon"><span class="text-danger">*</span></span>
                                </div>
                                <asp:RegularExpressionValidator ID="emailValidator" runat="server" Display="Dynamic" ErrorMessage="Please enter valid email address" ValidationExpression="^[\w\.\-]+@[a-zA-Z0-9\-]+(\.[a-zA-Z0-9\-]{1,})*(\.[a-zA-Z]{2,3}){1,2}$"
                                    ControlToValidate="txtEmail" CssClass="text-danger" ValidationGroup="ticket">
                                </asp:RegularExpressionValidator>

                                <asp:RequiredFieldValidator ID="emailRequired" runat="server" Display="Dynamic"
                                    ControlToValidate="txtEmail" ErrorMessage="Please enter an email" CssClass="text-danger" ValidationGroup="ticket"></asp:RequiredFieldValidator>

                                <%--<p class="form-control-static"><asp:Label ID="lblEmail" runat="server" Visible="False"></asp:Label></p>--%>
                            </div>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="lbltxtPhone" runat="server" resourcekey="lbltxtPhone" Text="Phone:" CssClass="control-label col-xs-2" AssociatedControlID="txtPhone" />
                            <div class="col-xs-4">
                                <asp:TextBox ID="txtPhone" runat="server" Columns="20" MaxLength="50" TextMode="Phone" CssClass="form-control"></asp:TextBox>
                            </div>
                            <asp:Label ID="lbltxtEstimateHours" runat="server" resourcekey="lbltxtEstimateHours" Text="Estimate Hours:" CssClass="control-label col-xs-2" AssociatedControlID="txtEstimate" />
                            <div class="col-xs-4">
                                <asp:TextBox ID="txtEstimate" runat="server" CssClass="form-control"></asp:TextBox>
                                <asp:CompareValidator ID="EstimateValidator" runat="server" Display="Dynamic" CssClass="text-danger" ControlToValidate="txtEstimate" Operator="DataTypeCheck" Type="Double" ErrorMessage="Value must be a number" ValidationGroup="ticket" />
                            </div>
                        </div>

                        <div class="form-group">
                            <asp:Label ID="lbltxtDescription" runat="server" resourcekey="lbltxtDescription" Text="Description:" CssClass="control-label col-xs-2" AssociatedControlID="txtDescription" />
                            <div class="col-xs-10">
                                <div class="input-group">
                                    <asp:TextBox ID="txtDescription" runat="server" MaxLength="150" CssClass="form-control"></asp:TextBox>
                                    <span class="input-group-addon"><span class="text-danger">*</span></span>
                                </div>
                                <asp:RequiredFieldValidator ID="descriptionRequired" runat="server" Display="Dynamic"
                                    ControlToValidate="txtDescription" ErrorMessage="Please enter description" CssClass="text-danger" ValidationGroup="ticket"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label ID="lbltxtStart" runat="server" resourcekey="lbltxtStart" Text="Start:" CssClass="control-label col-xs-2" AssociatedControlID="txtStart" />
                            <div class="col-xs-4">
                                <div class="input-group">
                                    <asp:TextBox ID="txtStart" runat="server" Columns="8" TextMode="DateTime" CssClass="form-control"></asp:TextBox>
                                    <span class="input-group-addon">
                                        <asp:HyperLink ID="cmdtxtStartCalendar" runat="server" ToolTip="calendar"><span class="glyphicon glyphicon-calendar"></span></asp:HyperLink></span>
                                </div>
                                <asp:CompareValidator ID="StartValidator" runat="server" Display="Dynamic" Type="Date" Operator="DataTypeCheck" CssClass="text-danger" ControlToValidate="txtStart" ErrorMessage="Please enter a valid date." ValidationGroup="ticket">
                                </asp:CompareValidator>

                            </div>
                            <asp:Label ID="lbltxtComplete" runat="server" resourcekey="lbltxtComplete" Text="Complete:" CssClass="control-label col-xs-2" AssociatedControlID="txtComplete" />
                            <div class="col-xs-4">
                                <div class="input-group">
                                    <asp:TextBox ID="txtComplete" runat="server" Columns="8" TextMode="DateTime" CssClass="form-control"></asp:TextBox>
                                    <span class="input-group-addon">
                                        <asp:HyperLink ID="cmdtxtCompleteCalendar" runat="server" ToolTip="calendar"><span class="glyphicon glyphicon-calendar"></span></asp:HyperLink></span>
                                </div>
                                <asp:CompareValidator ID="CompleteValidator" runat="server" Display="Dynamic" Type="Date" Operator="DataTypeCheck" CssClass="text-danger" ControlToValidate="txtComplete" ErrorMessage="Please enter a valid date." ValidationGroup="ticket">
                                </asp:CompareValidator>
                            </div>
                        </div>

                        <div class="form-group">
                            <div class="col-xs-offset-2 col-xs-10">
                                <asp:Button ID="btnSave" resourcekey="btnSave" runat="server" OnClick="btnSave_Click" Text="Save" CssClass="btn btn-primary" ValidationGroup="ticket" />
                            </div>
                        </div>

                        <div class="form-group">
                            <div class="col-xs-offset-2 col-xs-10">
                                <asp:Label ID="lblError" runat="server" CssClass="label label-success" />
                            </div>
                        </div>



                    </div>

                </div>
                <div class="col-md-3">
                    <p>
                        <asp:Image ID="imgTags" runat="server" ImageUrl="~/DesktopModules/ITILServiceDesk/images/tag_blue.png" /><b>
                            <asp:Label ID="lbltxtTags" runat="server" resourcekey="lbltxtTags" Text="Tags:" /></b>
                    </p>
                    <uc1:Tags ID="TagsTreeExistingTasks" runat="server" Visible="True" />
                </div>
            </div>
    </asp:Panel>


    <asp:Panel ID="pnlDetails" runat="server">

        <p>
            <ul class="nav nav-pills">
                <li>
                    <asp:LinkButton ID="btnComments" runat="server" OnClick="btnComments_Click" Text="Comments"
                        resourcekey="btnComments" /></li>
                <li>
                    <asp:LinkButton ID="btnWorkItems" runat="server"
                        OnClick="btnWorkItems_Click" Text="Work" resourcekey="btnWorkItems" /></li>
                <li>
                    <asp:LinkButton ID="btnLogs" runat="server"
                        OnClick="btnLogs_Click" Text="Logs" resourcekey="btnLogs" /></li>
            </ul>
        </p>

        <asp:Panel ID="pnlComments" runat="server">
            <ITIL:Comments ID="CommentsControl" runat="server" />
        </asp:Panel>
    </asp:Panel>

    <asp:Panel ID="pnlWorkItems" runat="server" Visible="False">
        <ITIL:Work ID="WorkControl" runat="server" />
    </asp:Panel>

    <asp:Panel ID="pnlLogs" runat="server" ScrollBars="Auto" Visible="False" Wrap="False">
        <ITIL:Logs ID="LogsControl" runat="server" />
    </asp:Panel>



    <%--<asp:Label ID="lblDetailsError" runat="server" EnableViewState="False" ForeColor="Red" />--%>
</div>
