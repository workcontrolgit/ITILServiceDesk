﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Work.ascx.cs" Inherits="ITIL.Modules.ServiceDesk.Controls.Work" %>


<asp:Panel ID="pnlInsertComment" runat="server" GroupingText="Insert New Work" 
     BorderStyle="None" CssClass="panel panel-default">
    <table>
        <tr>
            <td valign="top">
                <asp:TextBox ID="txtComment" runat="server" Width="100%" Rows="4" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
                &nbsp;
                <asp:Button ID="btnInsertComment" resourcekey="btnInsertComment" runat="server" Font-Bold="True" OnClick="btnInsertComment_Click"
                    Text="Insert" CssClass="btn btn-primary" />
                &nbsp;<br />
                <asp:Label ID="lblError" runat="server" EnableViewState="False" ForeColor="Red"></asp:Label>
            </td>
        </tr>
        <tr>
            <td valign="top">
                <table align="center">
                    <tr>
                        <td rowspan="2">
                            <asp:Label ID="lblStart1" runat="server" Font-Bold="True" Text="Start" resourcekey="lblStart" />
                        </td>
                        <td align="center">
                        <asp:Label ID="lblDate1" runat="server" Font-Bold="False" Text="Date" resourcekey="lblDate" />
                        </td>
                        <td align="center">
                        <asp:Label ID="lblTime1" runat="server" Font-Bold="False" Text="Time" resourcekey="lblTime" />
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td rowspan="2">
                        <asp:Label ID="lblStop1" runat="server" Font-Bold="True" Text="Stop" resourcekey="lblStop" />
                        </td>
                       <td align="center">
                        <asp:Label ID="lblDate2" runat="server" Font-Bold="False" Text="Date" 
                               resourcekey="lblDate" />
                        </td>
                        <td align="center">
                        <asp:Label ID="lblTime2" runat="server" Font-Bold="False" Text="Time" 
                                resourcekey="lblTime" />
                        </td>
                    </tr>
                    <tr>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtStartDay" runat="server" Columns="8"></asp:TextBox>
                            <b>
                                <asp:HyperLink ID="cmdtxtStartCalendar1" runat="server" ImageUrl="~/DesktopModules/ServiceDesk/images/calendar.png"></asp:HyperLink>
                            </b>
                        </td>
                        <td>
                            <asp:TextBox ID="txtStartTime" runat="server" Columns="8"></asp:TextBox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtStopDay" runat="server" Columns="8"></asp:TextBox>
                            <b>
                                <asp:HyperLink ID="cmdtxtStartCalendar2" runat="server" ImageUrl="~/DesktopModules/ServiceDesk/images/calendar.png"></asp:HyperLink>
                            </b>
                        </td>
                        <td>
                            <asp:TextBox ID="txtStopTime" runat="server" Columns="8"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>

        </tr>
    </table>
</asp:Panel>

<asp:Panel ID="pnlExistingComments" runat="server" Height="250px" ScrollBars="Vertical">
    <asp:GridView ID="gvComments" runat="server" AutoGenerateColumns="False" DataKeyNames="DetailID"
        DataSourceID="LDSComments" OnRowDataBound="gvComments_RowDataBound"
        OnRowCommand="gvComments_RowCommand" Width="100%" CellPadding="2"  CssClass="table table-bordered"
        CellSpacing="2" GridLines="None">
        <Columns>
            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" 
                        CommandArgument='<%# Bind("DetailID") %>' CommandName="Select" Text="Select"  CssClass="btn btn-link"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Comment" SortExpression="Description" ItemStyle-Width="50%">
                <ItemTemplate>
                    <asp:Label ID="lblComment" Font-Size="Small" runat="server" Text='<%# Bind("Description") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="User" SortExpression="UserID" ItemStyle-Width="25%">
                <ItemTemplate>
                    <asp:Label ID="gvlblUser" Font-Size="Small" runat="server" Text='<%# Bind("UserID") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Date/Time" SortExpression="InsertDate" ItemStyle-Width="20%">
                <ItemTemplate>
                    &nbsp;
                    <asp:Label ID="lblStartTime" runat="server" Text='<%# Bind("StartTime") %>' Visible="False" />
                    <asp:Label ID="lblStopTime" runat="server" Text='<%# Bind("StopTime") %>' Visible="False" />
                    <asp:Label ID="lblTimeSpan" runat="server" Font-Size="X-Small" Width="148px"></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Panel>
<asp:Panel ID="pnlEditComment" runat="server" Visible="False">
    <table>
        <tr>
            <td valign="top" nowrap="nowrap">
                <asp:Image ID="Image3" runat="server" ImageUrl="~/DesktopModules/ServiceDesk/images/application_side_contract.png" />
                <asp:LinkButton ID="lnkBack" resourcekey="lnkBack" runat="server" Font-Underline="True" OnClick="lnkBack_Click" Text="Back" />&nbsp;<asp:Image
                    ID="Image4" runat="server" ImageUrl="~/DesktopModules/ServiceDesk/images/page_add.png" />
                <asp:LinkButton ID="lnkUpdate" resourcekey="lnkUpdate" runat="server" Text="Update" Font-Underline="True"
                    OnClick="lnkUpdate_Click" />
                &nbsp;<asp:Image ID="Image5" runat="server" ImageUrl="~/DesktopModules/ServiceDesk/images/page_delete.png" />
                <asp:LinkButton ID="lnkDelete" resourcekey="lnkDelete" runat="server" OnClientClick='if (!confirm("Are you sure you want to delete?") ){return false;}'
                    Text="Delete" Font-Underline="True" OnClick="lnkDelete_Click" />
            </td>
            <td valign="top" align="right">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td valign="top" colspan="2" style="margin-left: 120px">
                <asp:TextBox ID="txtDescription" runat="server" Columns="60" Rows="10" TextMode="MultiLine"
                    Height="55px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="margin-left: 120px" valign="top">
                <table align="center">
                    <tr>
                        <td bgcolor="#CCCCCC" rowspan="2">
                            <asp:Label ID="lblStart2" runat="server" Font-Bold="True" 
                                resourcekey="lblStart" Text="Start" />
&nbsp;</td>
                        <td align="center">
                            <asp:Label ID="lblDate3" runat="server" Font-Bold="False" resourcekey="lblDate" 
                                Text="Date" />
&nbsp;</td>
                        <td align="center">
                            <asp:Label ID="lblTime3" runat="server" Font-Bold="False" resourcekey="lblTime" 
                                Text="Time" />
                            &nbsp;</td>
                        <td>
                            &nbsp;
                        </td>
                        <td bgcolor="#CCCCCC" rowspan="2">
                            <asp:Label ID="lblStop2" runat="server" Font-Bold="True" resourcekey="lblStop" 
                                Text="Stop" />
&nbsp;</td>
                        <td align="center">
                            <asp:Label ID="lblDate4" runat="server" Font-Bold="False" resourcekey="lblDate" 
                                Text="Date" />
                            &nbsp;</td>
                        <td align="center">
                            <asp:Label ID="lblTime4" runat="server" Font-Bold="False" resourcekey="lblTime" 
                                Text="Time" />
                            &nbsp;</td>
                    </tr>
                    <tr>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtStartDayEdit" runat="server" Columns="8"></asp:TextBox>
                            <b>
                                <asp:HyperLink ID="cmdtxtStartCalendar3" runat="server" ImageUrl="~/DesktopModules/ServiceDesk/images/calendar.png"></asp:HyperLink>
                            </b>
                        </td>
                        <td>
                            <asp:TextBox ID="txtStartTimeEdit" runat="server" Columns="8"></asp:TextBox>
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td nowrap="nowrap">
                            <asp:TextBox ID="txtStopDayEdit" runat="server" Columns="8"></asp:TextBox>
                            <b>
                                <asp:HyperLink ID="cmdtxtStartCalendar4" runat="server" ImageUrl="~/DesktopModules/ServiceDesk/images/calendar.png"></asp:HyperLink>
                            </b>
                        </td>
                        <td>
                            <asp:TextBox ID="txtStopTimeEdit" runat="server" Columns="8"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;<asp:Label ID="lblUserFooter" runat="server" Font-Bold="True" 
                    Text="User:" resourcekey="lblUserFooter" />
                &nbsp;<asp:Label ID="lblDisplayUser" runat="server"></asp:Label>
                &nbsp;<asp:Label ID="lblInsertDateFooter" runat="server" Font-Bold="True" 
                    Text="Insert Date:" resourcekey="lblInsertDateFooter" />&nbsp;<asp:Label ID="lblInsertDate" runat="server"></asp:Label>
                &nbsp;
                <br />
                <asp:Label ID="lblErrorEditComment" runat="server" EnableViewState="False" ForeColor="Red"></asp:Label>
                <asp:Label ID="lblDetailID" runat="server" Visible="False"></asp:Label>
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:LinqDataSource ID="LDSComments" runat="server" ContextTypeName="ITIL.Modules.ServiceDesk.ServiceDeskDALDataContext"
    OrderBy="InsertDate desc" TableName="ITILServiceDesk_TaskDetails" OnSelecting="LDSComments_Selecting">
</asp:LinqDataSource>

