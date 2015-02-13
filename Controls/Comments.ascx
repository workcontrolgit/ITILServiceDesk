<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Comments.ascx.cs" Inherits="ITIL.Modules.ServiceDesk.Controls.Comments" %>
<asp:Panel ID="pnlInsertComment" runat="server" GroupingText="Add Comment" BorderWidth="0">

    <div class="form-horizontal">
        <div class="form-group">
            <div class="col-xs-9">
                <asp:TextBox ID="txtComment" runat="server" Width="100%" Rows="4" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="col-xs-3">
                <asp:CheckBox ID="chkCommentVisible" runat="server" Checked="True" Font-Size="Small"
                    Text=" Visible to Requestor" AutoPostBack="True" CssClass="checkbox"
                    OnCheckedChanged="chkCommentVisible_CheckedChanged" resourcekey="chkCommentVisible" />
            </div>
        </div>

        <div class="form-group">
            <div class="col-xs-12">
                <asp:Label ID="lblAttachFile1" runat="server" Text="File:" resourcekey="lblAttachFile" AssociatedControlID="TicketFileUpload" CssClass="control-label" />
                <asp:FileUpload ID="TicketFileUpload" runat="server" />
            </div>
        </div>
        <div class="form-group">
            <div class="col-xs-12">
                <asp:Button ID="btnInsertComment" runat="server" OnClick="btnInsertComment_Click" Text="Insert" resourcekey="btnInsertComment" CssClass="btn btn-primary" />
                <asp:Button ID="btnInsertCommentAndEmail" runat="server" OnClick="btnInsertCommentAndEmail_Click" Text="Insert and Email " resourcekey="btnInsertCommentAndEmail" CssClass="btn btn-warning" BackColor="LightGray" />
            </div>
        </div>
        <div class="form-group">
            <div class="col-xs-12">
                <asp:Label ID="lblError" runat="server" EnableViewState="False" CssClass="label label-warning"></asp:Label>
            </div>
        </div>
    </div>

</asp:Panel>


<asp:Panel ID="pnlExistingComments" runat="server" Height="250px" ScrollBars="Vertical">
    <asp:GridView ID="gvComments" runat="server" AutoGenerateColumns="False" DataKeyNames="DetailID"
        DataSourceID="LDSComments" ShowHeader="True" 
        OnRowDataBound="gvComments_RowDataBound" 
        onrowcommand="gvComments_RowCommand" BorderStyle="None"  CssClass="table table-bordered table-striped"
        CellPadding="2" CellSpacing="2" GridLines="None">
        <Columns>
            <asp:TemplateField ShowHeader="false">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False"  CommandArgument='<%# Bind("DetailID") %>' CommandName="Select" 
                        Text="Select" CssClass="btn btn-link"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField  HeaderText="Visible to Requester">
                <ItemTemplate>
                    <asp:Label ID="lblDetailType" runat="server" Text='<%# Bind("DetailType") %>' 
                        Visible="False" />
                    <asp:CheckBox ID="chkDetailType" runat="server" Checked="true" Enabled="False" 
                        ToolTip="Visible to Requestor" Width="34px" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Comment" SortExpression="Description" ItemStyle-Width="50%">
                <ItemTemplate>
                    <asp:Label ID="lblComment" Font-Size="Small" runat="server" Text='<%# Bind("Description") %>'
                        Width="180px"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="User" SortExpression="UserID" ItemStyle-Width="25%">
                <ItemTemplate>
                    <asp:Label ID="gvlblUser" Font-Size="Small" runat="server" Text='<%# Bind("UserID") %>'
                        Width="140px"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Date/Time" SortExpression="InsertDate" ItemStyle-Width="20%">
                <ItemTemplate>
                    <asp:Label ID="lblDate" runat="server" Font-Size="Small" Text='<%# Bind("InsertDate") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Panel>
<asp:Panel ID="pnlEditComment" runat="server" Visible="False">
        <div class="form-horizontal">
            <div class="form-group">
                <div class="col-xs-12">
                    <asp:Label ID="lblUserFooter" runat="server" Font-Bold="True"
                        resourcekey="lblUserFooter" Text="User:" />
                    &nbsp;<asp:Label ID="lblDisplayUser" runat="server"></asp:Label>
                    &nbsp;<asp:Label ID="lblInsertDateFooter" runat="server" Font-Bold="True"
                        resourcekey="lblInsertDateFooter" Text="Insert Date:" />
                    &nbsp;<asp:Label ID="lblInsertDate" runat="server"></asp:Label>
                    &nbsp;

                </div>
            </div>
            <div class="form-group">
                <div class="col-xs-9">
                    <asp:TextBox ID="txtDescription" runat="server" Rows="4" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                </div>
                <div class="col-xs-3">
                    <asp:CheckBox ID="chkCommentVisibleEdit" runat="server" Font-Size="Small"
                        Text="Visible to Requestor" AutoPostBack="True" resourcekey="chkCommentVisible"
                        OnCheckedChanged="chkCommentVisibleEdit_CheckedChanged" />
                </div>
            </div>
            <div class="form-group">
            <div class="col-xs-12">
                  <asp:Panel ID="pnlDisplayFile" runat="server" Visible="False">
                          <asp:Label ID="lblAttachment" resourcekey="lblAttachment" runat="server" CssClass="control-label" AssociatedControlID="lnkFileAttachment"
                              Text="Attachment:" />
                          &nbsp;<asp:ImageButton ID="imgDelete" runat="server"
                              ImageUrl="~/DesktopModules/ITILServiceDesk/images/cancel.png"
                              ToolTip="Delete Attachment"
                              OnClientClick='if (!confirm("Are you sure you want to delete?") ){return false;}'
                              OnClick="imgDelete_Click" />
                          &nbsp;<asp:LinkButton ID="lnkFileAttachment" runat="server"
                              OnClick="lnkFileAttachment_Click"
                              ToolTip="Click here to download this file"></asp:LinkButton>
                      &nbsp;
                    </asp:Panel>
                    <asp:Panel ID="pnlAttachFile" runat="server" Visible="False">
                            <asp:Label ID="lblAttachFile2" resourcekey="lblAttachFile2" runat="server" CssClass="control-label" AssociatedControlID="fuAttachment" 
                                Text="Attach File:" />
                        &nbsp;<asp:FileUpload ID="fuAttachment" runat="server" />
                    </asp:Panel>
            </div>
        </div>
        <div class="form-group">

            <div class="col-xs-12">
                <asp:LinkButton ID="lnkUpdate" runat="server" Text="Update"
                    OnClick="lnkUpdate_Click" resourcekey="lnkUpdate" CssClass="btn btn-primary" />
                    <asp:LinkButton
                        ID="lnkUpdateRequestor" runat="server"
                        OnClick="lnkUpdateRequestor_Click" Text="Update and Email Requestor" resourcekey="lnkUpdateAndEmail" CssClass="btn btn-warning" />
                    &nbsp;
                    <asp:LinkButton ID="lnkBack" runat="server"
                        OnClick="lnkBack_Click" Text="Back" resourcekey="lnkBack" CssClass="btn btn-default" />&nbsp;
                <asp:LinkButton ID="lnkDelete" runat="server"
                    OnClientClick='if (!confirm("Are you sure you want to delete?") ){return false;}'
                    Text="Delete" OnClick="lnkDelete_Click" resourcekey="lnkDelete" CssClass="btn btn-danger" />
                </div>
            </div>
            <div class="form-group">
                <div class="col-xs-12">
                    <asp:Label ID="lblErrorEditComment" runat="server" EnableViewState="False"  CssClass="label label-warning"></asp:Label>
                    <asp:Label ID="lblDetailID" runat="server" Visible="False"></asp:Label>
                </div>
            </div>
     </div>

</asp:Panel>
<asp:LinqDataSource ID="LDSComments" runat="server" ContextTypeName="ITIL.Modules.ServiceDesk.ServiceDeskDALDataContext"
    OrderBy="InsertDate desc" TableName="ITILServiceDesk_TaskDetails" OnSelecting="LDSComments_Selecting">
</asp:LinqDataSource>
