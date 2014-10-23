<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditTask.ascx.cs" Inherits="ITIL.Modules.ServiceDesk.EditTask" %>
<%@ Register Src="Controls/Tags.ascx" TagName="Tags" TagPrefix="uc1" %>
<%@ Register Src="Controls/Comments.ascx" TagName="Comments" TagPrefix="uc2" %>
<%@ Register Src="Controls/Logs.ascx" TagName="Logs" TagPrefix="uc3" %>
<%@ Register Src="Controls/Work.ascx" TagName="Work" TagPrefix="uc4" %>

<div class="table-responsive">
    <asp:Panel ID="pnlEditTask" runat="server" HorizontalAlign="Left">




        <div class="form-inline">
                    <asp:LinkButton ID="lnkNewTicket" resourcekey="lnkNewTicket" runat="server" CssClass="btn btn-link" OnClick="lnkNewTicket_Click">New Ticket</asp:LinkButton>
                    <asp:LinkButton ID="lnkExistingTickets" resourcekey="lnkExistingTickets" runat="server" CssClass="btn btn-link" OnClick="lnkExistingTickets_Click">Existing Tickets</asp:LinkButton>
                    <asp:LinkButton ID="lnkAdministratorSettings" resourcekey="lnkAdministratorSettings" runat="server" CssClass="btn btn-link"
                        OnClick="lnkAdministratorSettings_Click">Administrator Settings</asp:LinkButton>
        </div>

        <div class="panel panel-default">
            <div class="panel-heading">
                <h4>Service Desk Ticket</h4>
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
                            <p class="form-control-static"><asp:Label ID="lblTask" runat="server"></asp:Label></p>
                        </div>
                        <asp:Label ID="lblCreated" runat="server" CssClass="control-label col-xs-2" resourcekey="lblCreated" AssociatedControlID="lblCreatedData"></asp:Label>
                        <div class="col-xs-4">
                              <p class="form-control-static"><asp:Label ID="lblCreatedData" runat="server"></asp:Label></p>
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
                                </asp:DropDownList>                        </div>

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
                                     <asp:HyperLink ID="cmdtxtDueDateCalendar" runat="server" ImageUrl="~/DesktopModules/ITILServiceDesk/images/calendar.png" ToolTip="calendar"></asp:HyperLink></span>
                            </div>
                        </div>
                    </div>


                   <div class="form-group">
                        <asp:Label ID="lbltxtName" runat="server" resourcekey="lbltxtName" Text="Name:" CssClass="control-label col-xs-2" AssociatedControlID="txtName" />
                        <div class="col-xs-4">
                                <asp:TextBox ID="txtName" runat="server" MaxLength="350" CssClass="form-control"></asp:TextBox>
                                <p class="form-control-static"><asp:Label ID="lblName" runat="server" Visible="False"></asp:Label></p>
                        </div>
                        <asp:Label ID="lbltxtEmail" runat="server" resourcekey="lbltxtEmail" Text="Email:" CssClass="control-label col-xs-2" AssociatedControlID="txtEmail" />
                        <div class="col-xs-4">
                       <asp:TextBox ID="txtEmail" runat="server" MaxLength="350" CssClass="form-control"></asp:TextBox>
                                <p class="form-control-static"><asp:Label ID="lblEmail" runat="server" Visible="False"></asp:Label></p>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lbltxtPhone" runat="server" resourcekey="lbltxtPhone" Text="Phone:" CssClass="control-label col-xs-2" AssociatedControlID="txtPhone" />
                        <div class="col-xs-4">
                        <asp:TextBox ID="txtPhone" runat="server" Columns="20" MaxLength="50" TextMode="Phone" CssClass="form-control"></asp:TextBox>
                        </div>
                        <asp:Label ID="lbltxtEstimateHours" runat="server" resourcekey="lbltxtEstimateHours" Text="Estimate Hours:" CssClass="control-label col-xs-2" AssociatedControlID="txtEstimate" />
                        <div class="col-xs-4">
                            <asp:TextBox ID="txtEstimate" runat="server" Width="80px" TextMode="Number" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group">
                        <asp:Label ID="lbltxtDescription" runat="server" resourcekey="lbltxtDescription" Text="Description:" CssClass="control-label col-xs-2" AssociatedControlID="txtDescription" />
                        <div class="col-xs-10">
                        <asp:TextBox ID="txtDescription" runat="server" MaxLength="50" Width="100%" TextMode="MultiLine" Wrap="true" Rows="3" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label ID="lbltxtStart" runat="server" resourcekey="lbltxtStart" Text="Start:" CssClass="control-label col-xs-2" AssociatedControlID="txtStart" />
                        <div class="col-xs-4">
                            <div class="input-group">
                        <asp:TextBox ID="txtStart" runat="server" Columns="8" TextMode="DateTime" CssClass="form-control"></asp:TextBox>
                                <span class="input-group-addon">
                            <asp:HyperLink ID="cmdtxtStartCalendar" runat="server" ImageUrl="~/DesktopModules/ITILServiceDesk/images/calendar.png" ToolTip="calendar"></asp:HyperLink></span>
                                </div>
                        </div>
                        <asp:Label ID="lbltxtComplete" runat="server" resourcekey="lbltxtComplete" Text="Complete:" CssClass="control-label col-xs-2" AssociatedControlID="txtComplete" />
                        <div class="col-xs-4">
                            <div class="input-group">
                            <asp:TextBox ID="txtComplete" runat="server" Columns="8" TextMode="DateTime" CssClass="form-control"></asp:TextBox>
                                <span class="input-group-addon">
                            <asp:HyperLink ID="cmdtxtCompleteCalendar" runat="server" ImageUrl="~/DesktopModules/ITILServiceDesk/images/calendar.png" ToolTip="calendar"></asp:HyperLink></span>
                                </div>
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="col-xs-offset-2 col-xs-10">
                             <asp:Button ID="btnSave" resourcekey="btnSave" runat="server" OnClick="btnSave_Click" Text="Save" CssClass="btn btn-primary"
                                    Width="63px" />
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

                    <div class="form-inline">
                                <asp:LinkButton ID="btnComments" runat="server" OnClick="btnComments_Click" Text="Comments"
                                    CssClass="btn btn-link" resourcekey="btnComments" />
                                <asp:LinkButton ID="btnWorkItems" runat="server" CssClass="btn btn-link" 
                                    OnClick="btnWorkItems_Click" Text="Work" resourcekey="btnWorkItems" />
                                <asp:LinkButton ID="btnLogs" runat="server" CssClass="btn btn-link" 
                                    OnClick="btnLogs_Click" Text="Logs" resourcekey="btnLogs" />
                    </div>

                    <asp:Panel ID="pnlComments" runat="server">
                        <uc2:Comments ID="CommentsControl" runat="server" />
                    </asp:Panel>
                </asp:Panel>

                <asp:Panel ID="pnlWorkItems" runat="server" Visible="False">
                    <uc4:Work ID="WorkControl" runat="server" />
                </asp:Panel>

                <asp:Panel ID="pnlLogs" runat="server" ScrollBars="Auto" Visible="False" Wrap="False">
                    <uc3:Logs ID="LogsControl" runat="server" />
                </asp:Panel>



    <%--<asp:Label ID="lblDetailsError" runat="server" EnableViewState="False" ForeColor="Red" />--%>
</div>
