//
// ADefwebserver.com
// Copyright (c) 2009
// by Michael Washington
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//
//

using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using DotNetNuke.Common;
using DotNetNuke.Security.Roles;
using DotNetNuke.Entities.Users;
using System.Collections;
using System.Drawing;
using Microsoft.VisualBasic;
using System.Text;
using System.IO;
using DotNetNuke.Services.Localization;

namespace ITIL.Modules.ServiceDesk
{
    public partial class EditTask : ITILServiceDeskModuleBase
    {
        //ITIL Customization - create viewstate variable for saving original status
        public string Status
        {
            get
            {
                if (ViewState["Status"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return ViewState["Status"].ToString();
                }
            }
            set { ViewState["Status"] = value; }
        }

        //ITIL Customization - create viewstate variable for saving requestor's email
        public string RequesterEmail
        {
            get
            {
                if (ViewState["RequesterEmail"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return ViewState["RequesterEmail"].ToString();
                }
            }
            set { ViewState["RequesterEmail"] = value; }
        }

        //ITIL Customization - create viewstate variable for saving requestor's name
        public string RequesterName
        {
            get
            {
                if (ViewState["RequesterName"] == null)
                {
                    return string.Empty;
                }
                else
                {
                    return ViewState["RequesterName"].ToString();
                }
            }
            set { ViewState["RequesterName"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            cmdtxtDueDateCalendar.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtDueDate);
            cmdtxtStartCalendar.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtStart);
            cmdtxtCompleteCalendar.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtComplete);
            //memu tool tip localization
            lnkNewTicket.ToolTip = Localization.GetString("lnkNewTicketToolTip", LocalResourceFile);
            lnkExistingTickets.ToolTip = Localization.GetString("lnkExistingTicketsToolTip", LocalResourceFile);
            lnkAdministratorSettings.ToolTip = Localization.GetString("lnkAdministratorSettingsToolTip", LocalResourceFile);

            btnComments.ToolTip = Localization.GetString("btnCommentsToolTip", LocalResourceFile);
            btnLogs.ToolTip = Localization.GetString("btnLogsToolTip", LocalResourceFile);
            btnWorkItems.ToolTip = Localization.GetString("btnWorkItemsToolTip", LocalResourceFile);

            if (!Page.IsPostBack)
            {
                
                if (Request.QueryString["TaskID"] != null)
                {
                    CommentsControl.ViewOnly = true;
                    if (CheckSecurity())
                    {
                        ShowAdministratorLink();
                        ShowExistingTicketsLink();
                        LoadRolesDropDown();
                        DisplayCategoryTree();
                        DisplayTicketData();
                        CommentsControl.TaskID = Convert.ToInt32(lblTask.Text);
                        CommentsControl.ModuleID = ModuleId;
                        WorkControl.TaskID = Convert.ToInt32(lblTask.Text);
                        WorkControl.ModuleID = ModuleId;
                        LogsControl.TaskID = Convert.ToInt32(lblTask.Text);

                        // If at this point CommentsControl is in View Only mode
                        // Set main form in View Only mode
                        if (CommentsControl.ViewOnly == true)
                        {
                            SetViewOnlyMode();
                        }
                        else
                        {
                            btnComments.Font.Bold = true;
                            btnComments.Font.Underline = true;
                        }
                        // Insert Log
                        Log.InsertLog(Convert.ToInt32(lblTask.Text), UserId, String.Format("{0} viewed ticket.", (UserId == -1) ? "Requester" : UserInfo.DisplayName));
                    }
                    else
                    {
                        pnlEditTask.Visible = false;
                        Response.Redirect(DotNetNuke.Common.Globals.NavigateURL());
                    }
                }
                else
                {
                    pnlEditTask.Visible = false;
                    Response.Redirect(DotNetNuke.Common.Globals.NavigateURL());
                }
            }
        }

        #region SetViewOnlyMode
        private void SetViewOnlyMode()
        {
            btnSave.Visible = false;
            btnComments.Visible = false;
            btnWorkItems.Visible = false;
            btnLogs.Visible = false;
            ddlAssigned.Enabled = false;
            ddlStatus.Enabled = false;
            ddlPriority.Enabled = false;
        }
        #endregion

        #region LoadRolesDropDown
        private void LoadRolesDropDown()
        {
            RoleController objRoleController = new RoleController();
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            List<ITILServiceDesk_Role> colITILServiceDesk_Roles = (from ITILServiceDesk_Roles in objServiceDeskDALDataContext.ITILServiceDesk_Roles
                                                             where ITILServiceDesk_Roles.PortalID == PortalId
                                                             select ITILServiceDesk_Roles).ToList();

            // Create a ListItemCollection to hold the Roles 
            ListItemCollection colListItemCollection = new ListItemCollection();

            // Add the Roles to the List
            foreach (ITILServiceDesk_Role objITILServiceDesk_Role in colITILServiceDesk_Roles)
            {
                try
                {
                    RoleInfo objRoleInfo = objRoleController.GetRole(Convert.ToInt32(objITILServiceDesk_Role.RoleID), PortalId);

                    ListItem RoleListItem = new ListItem();
                    RoleListItem.Text = objRoleInfo.RoleName;
                    RoleListItem.Value = objITILServiceDesk_Role.RoleID.ToString();
                    ddlAssigned.Items.Add(RoleListItem);
                }
                catch
                {
                    // Role no longer exists in Portal
                    ListItem RoleListItem = new ListItem();
                    RoleListItem.Text = Localization.GetString("DeletedRole.Text", LocalResourceFile);
                    RoleListItem.Value = objITILServiceDesk_Role.RoleID.ToString();
                    ddlAssigned.Items.Add(RoleListItem);
                }
            }

            // Add UnAssigned
            ListItem UnassignedRoleListItem = new ListItem();
            UnassignedRoleListItem.Text = Localization.GetString("Unassigned.Text", LocalResourceFile);
            UnassignedRoleListItem.Value = "-1";
            ddlAssigned.Items.Add(UnassignedRoleListItem);

        }
        #endregion

        #region CheckSecurity
        private bool CheckSecurity()
        {
            bool boolPassedSecurity = false;
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            ITILServiceDesk_Task objITILServiceDesk_Tasks = (from ITILServiceDesk_Tasks in objServiceDeskDALDataContext.ITILServiceDesk_Tasks
                                                       where ITILServiceDesk_Tasks.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                                                       select ITILServiceDesk_Tasks).FirstOrDefault();
            if (objITILServiceDesk_Tasks == null)
            {
                pnlEditTask.Visible = false;
                Response.Redirect(DotNetNuke.Common.Globals.NavigateURL());
            }

            // User not logged in
            if (UserId == -1)
            {
                // Must have the valid password
                if (Request.QueryString["TP"] != null)
                {
                    // Check the password for this Ticket
                    if (objITILServiceDesk_Tasks.TicketPassword == Convert.ToString(Request.QueryString["TP"]))
                    {
                        boolPassedSecurity = true;
                    }
                    else
                    {
                        boolPassedSecurity = false;
                    }
                }
            }

            // User is logged in
            if (UserId > -1)
            {
                // Is user an Admin?
                string strAdminRoleID = GetAdminRole();
                if (UserInfo.IsInRole(strAdminRoleID) || UserInfo.IsInRole("Administrators") || UserInfo.IsSuperUser)
                {
                    boolPassedSecurity = true;
                    CommentsControl.ViewOnly = false;
                }

                // Is user the Requestor?
                if (UserId == objITILServiceDesk_Tasks.RequesterUserID)
                {
                    boolPassedSecurity = true;
                }

                //Is user in the Assigned Role?
                RoleController objRoleController = new RoleController();
                RoleInfo objRoleInfo = objRoleController.GetRole(objITILServiceDesk_Tasks.AssignedRoleID, PortalId);
                if (objRoleInfo != null)
                {
                    if (UserInfo.IsInRole(objRoleInfo.RoleName))
                    {
                        boolPassedSecurity = true;
                        CommentsControl.ViewOnly = false;
                    }
                }

                // Does user have a valid temporary password?
                if (Request.QueryString["TP"] != null)
                {
                    // Check the password for this Ticket
                    if (objITILServiceDesk_Tasks.TicketPassword == Convert.ToString(Request.QueryString["TP"]))
                    {
                        boolPassedSecurity = true;
                    }
                    else
                    {
                        boolPassedSecurity = false;
                    }
                }
            }

            return boolPassedSecurity;
        }
        #endregion

        #region lnkAdministratorSettings_Click
        protected void lnkAdministratorSettings_Click(object sender, EventArgs e)
        {
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "AdminSettings", "mid=" + ModuleId.ToString()));
        }
        #endregion

        #region lnkNewTicket_Click
        protected void lnkNewTicket_Click(object sender, EventArgs e)
        {
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(null, "Ticket=new"));
        }
        #endregion

        #region lnkExistingTickets_Click
        protected void lnkExistingTickets_Click(object sender, EventArgs e)
        {
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
        }
        #endregion

        #region ShowExistingTicketsLink
        private void ShowExistingTicketsLink()
        {
            // Show Existing Tickets link if user is logged in
            if (UserId > -1)
            {
                lnkExistingTickets.Visible = true;
                //imgExitingTickets.Visible = true;
            }
            else
            {
                lnkExistingTickets.Visible = false;
                //imgExitingTickets.Visible = false;
            }
        }
        #endregion

        #region ShowAdministratorLink
        private void ShowAdministratorLink()
        {
            // Get Admin Role
            string strAdminRoleID = GetAdminRole();
            // Show Admin link if user is an Administrator
            if (UserInfo.IsInRole(strAdminRoleID) || UserInfo.IsInRole("Administrators") || UserInfo.IsSuperUser)
            {
                lnkAdministratorSettings.Visible = true;
                //imgAdministrator.Visible = true;
            }
            else
            {
                lnkAdministratorSettings.Visible = false;
                //imgAdministrator.Visible = false;
            }
        }
        #endregion

        #region GetAdminRole
        private string GetAdminRole()
        {
            List<ITILServiceDesk_Setting> objITILServiceDesk_Settings = GetSettings();
            ITILServiceDesk_Setting objITILServiceDesk_Setting = objITILServiceDesk_Settings.Where(x => x.SettingName == "AdminRole").FirstOrDefault();

            string strAdminRoleID = "Administrators";
            if (objITILServiceDesk_Setting != null)
            {
                strAdminRoleID = objITILServiceDesk_Setting.SettingValue;
            }

            return strAdminRoleID;
        }
        #endregion

        #region GetSettings
        private List<ITILServiceDesk_Setting> GetSettings()
        {
            // Get Settings
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            List<ITILServiceDesk_Setting> colITILServiceDesk_Setting = (from ITILServiceDesk_Settings in objServiceDeskDALDataContext.ITILServiceDesk_Settings
                                                                  where ITILServiceDesk_Settings.PortalID == PortalId
                                                                  select ITILServiceDesk_Settings).ToList();

            if (colITILServiceDesk_Setting.Count == 0)
            {
                // Create Default vaules
                ITILServiceDesk_Setting objITILServiceDesk_Setting1 = new ITILServiceDesk_Setting();

                objITILServiceDesk_Setting1.PortalID = PortalId;
                objITILServiceDesk_Setting1.SettingName = "AdminRole";
                objITILServiceDesk_Setting1.SettingValue = "Administrators";

                objServiceDeskDALDataContext.ITILServiceDesk_Settings.InsertOnSubmit(objITILServiceDesk_Setting1);
                objServiceDeskDALDataContext.SubmitChanges();

                ITILServiceDesk_Setting objITILServiceDesk_Setting2 = new ITILServiceDesk_Setting();

                objITILServiceDesk_Setting2.PortalID = PortalId;
                objITILServiceDesk_Setting2.SettingName = "UploadefFilesPath";
                objITILServiceDesk_Setting2.SettingValue = Server.MapPath("~/DesktopModules/ServiceDesk/Upload");

                objServiceDeskDALDataContext.ITILServiceDesk_Settings.InsertOnSubmit(objITILServiceDesk_Setting2);
                objServiceDeskDALDataContext.SubmitChanges();

                colITILServiceDesk_Setting = (from ITILServiceDesk_Settings in objServiceDeskDALDataContext.ITILServiceDesk_Settings
                                           where ITILServiceDesk_Settings.PortalID == PortalId
                                           select ITILServiceDesk_Settings).ToList();
            }

            return colITILServiceDesk_Setting;
        }
        #endregion

        // Tags

        #region DisplayCategoryTree
        private void DisplayCategoryTree()
        {
            bool boolUserAssignedToTask = false;
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            ITILServiceDesk_Task objITILServiceDesk_Tasks = (from ITILServiceDesk_Tasks in objServiceDeskDALDataContext.ITILServiceDesk_Tasks
                                                       where ITILServiceDesk_Tasks.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                                                       select ITILServiceDesk_Tasks).FirstOrDefault();

            //Is user in the Assigned Role?
            RoleController objRoleController = new RoleController();
            RoleInfo objRoleInfo = objRoleController.GetRole(objITILServiceDesk_Tasks.AssignedRoleID, PortalId);

            if (objRoleInfo != null)
            {
                if (UserInfo.IsInRole(objRoleInfo.RoleName))
                {
                    boolUserAssignedToTask = true;
                }
            }

            if (boolUserAssignedToTask || UserInfo.IsInRole(GetAdminRole()) || UserInfo.IsInRole("Administrators") || UserInfo.IsSuperUser)
            {
                // Show all Tags
                TagsTreeExistingTasks.Visible = true;
                TagsTreeExistingTasks.TagID = Convert.ToInt32(Request.QueryString["TaskID"]);
                TagsTreeExistingTasks.DisplayType = "Administrator";
                TagsTreeExistingTasks.Expand = false;
            }
            else
            {
                // Show only Visible Tags
                TagsTreeExistingTasks.Visible = true;
                TagsTreeExistingTasks.TagID = Convert.ToInt32(Request.QueryString["TaskID"]);
                TagsTreeExistingTasks.DisplayType = "Requestor";
                TagsTreeExistingTasks.Expand = false;
            }

            // Select Existing values
            if (objITILServiceDesk_Tasks.ITILServiceDesk_TaskCategories.Select(x => x.CategoryID).ToArray<int>().Count() > 0)
            {
                int[] ArrStrCategories = objITILServiceDesk_Tasks.ITILServiceDesk_TaskCategories.Select(x => x.CategoryID).ToArray<int>();
                int?[] ArrIntCatagories = Array.ConvertAll<int, int?>(ArrStrCategories, new Converter<int, int?>(ConvertToNullableInt));

                TagsTreeExistingTasks.SelectedCategories = ArrIntCatagories;
            }

            // Set visibility of Tags
            bool RequestorCatagories = (TagsTreeExistingTasks.DisplayType == "Administrator") ? false : true;

            int CountOfCatagories = (from ServiceDeskCategories in CategoriesTable.GetCategoriesTable(PortalId, RequestorCatagories)
                                     where ServiceDeskCategories.PortalID == PortalId
                                     where ServiceDeskCategories.Level == 1
                                     select ServiceDeskCategories).Count();

            imgTags.Visible = (CountOfCatagories > 0);
            lbltxtTags.Visible = (CountOfCatagories > 0);
        }
        #endregion

        #region ConvertToNullableInt
        private int? ConvertToNullableInt(int strParameter)
        {
            return Convert.ToInt32(strParameter);
        }
        #endregion

        // Display Ticket Data

        #region DisplayTicketData
        private void DisplayTicketData()
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            ITILServiceDesk_Task objITILServiceDesk_Tasks = (from ITILServiceDesk_Tasks in objServiceDeskDALDataContext.ITILServiceDesk_Tasks
                                                       where ITILServiceDesk_Tasks.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                                                       select ITILServiceDesk_Tasks).FirstOrDefault();

            // Name is editable only if user is Anonymous
            if (objITILServiceDesk_Tasks.RequesterUserID == -1)
            {
                txtEmail.Visible = true;
                txtName.Visible = true;
                //lblEmail.Visible = false;
                //lblName.Visible = false;
                txtEmail.Text = objITILServiceDesk_Tasks.RequesterEmail;
                txtName.Text = objITILServiceDesk_Tasks.RequesterName;

                //ITIL Customization - assigning (txtEmail.Text to RequesterEmail) and (txtName.Text to RequesterName) in case user is anonymous
                RequesterEmail = txtEmail.Text;
                RequesterName = txtName.Text;
            }
            else
            {
                //txtEmail.Visible = false;
                //txtName.Visible = false;
                //lblEmail.Visible = true;
                //lblName.Visible = true;

                UserInfo objRequester = UserController.GetUser(PortalId, objITILServiceDesk_Tasks.RequesterUserID, false);

                if (objRequester != null)
                {
                    //lblEmail.Text = UserController.GetUser(PortalId, objITILServiceDesk_Tasks.RequesterUserID, false).Email;
                    //lblName.Text = UserController.GetUser(PortalId, objITILServiceDesk_Tasks.RequesterUserID, false).DisplayName;

                    txtEmail.Text = UserController.GetUser(PortalId, objITILServiceDesk_Tasks.RequesterUserID, false).Email;
                    txtName.Text = UserController.GetUser(PortalId, objITILServiceDesk_Tasks.RequesterUserID, false).DisplayName;

                    //ITIL Customization - assigning (lblEmail.Text to RequesterEmail) and (lblName.Text to RequesterName) in case user is anonymous
                    RequesterEmail = txtEmail.Text;
                    RequesterName = txtName.Text;
                }
                else
                {
                    txtName.Text = "[User Deleted]";
                }
            }

            lblTask.Text = objITILServiceDesk_Tasks.TaskID.ToString();
            lblCreatedData.Text = String.Format(objITILServiceDesk_Tasks.CreatedDate.ToShortDateString(), objITILServiceDesk_Tasks.CreatedDate.ToShortTimeString());
            ddlStatus.SelectedValue = objITILServiceDesk_Tasks.Status;

            //ITIL Customization - assign objITILServiceDesk_Tasks.Status to Status for the purpose of preventing multiple email notifications
            Status = objITILServiceDesk_Tasks.Status;
 
            ddlPriority.SelectedValue = objITILServiceDesk_Tasks.Priority;
            txtDescription.Text = objITILServiceDesk_Tasks.Description;
            txtPhone.Text = objITILServiceDesk_Tasks.RequesterPhone;
            txtDueDate.Text = (objITILServiceDesk_Tasks.DueDate.HasValue) ? objITILServiceDesk_Tasks.DueDate.Value.ToShortDateString() : "";
            txtStart.Text = (objITILServiceDesk_Tasks.EstimatedStart.HasValue) ? objITILServiceDesk_Tasks.EstimatedStart.Value.ToShortDateString() : "";
            txtComplete.Text = (objITILServiceDesk_Tasks.EstimatedCompletion.HasValue) ? objITILServiceDesk_Tasks.EstimatedCompletion.Value.ToShortDateString() : "";
            txtEstimate.Text = (objITILServiceDesk_Tasks.EstimatedHours.HasValue) ? objITILServiceDesk_Tasks.EstimatedHours.Value.ToString() : "";

            ListItem TmpRoleListItem = ddlAssigned.Items.FindByValue(objITILServiceDesk_Tasks.AssignedRoleID.ToString());
            if (TmpRoleListItem == null)
            {
                // Value was not found so add it
                RoleController objRoleController = new RoleController();
                RoleInfo objRoleInfo = objRoleController.GetRole(objITILServiceDesk_Tasks.AssignedRoleID, PortalId);

                if (objRoleInfo != null)
                {
                    ListItem RoleListItem = new ListItem();
                    RoleListItem.Text = objRoleInfo.RoleName;
                    RoleListItem.Value = objITILServiceDesk_Tasks.AssignedRoleID.ToString();
                    ddlAssigned.Items.Add(RoleListItem);

                    ddlAssigned.SelectedValue = objITILServiceDesk_Tasks.AssignedRoleID.ToString();
                }
                else
                {
                    // Role no longer exists in Portal
                    ddlAssigned.SelectedValue = "-1";
                }
            }
            else
            {
                // The Value already exists so set it
                ddlAssigned.SelectedValue = objITILServiceDesk_Tasks.AssignedRoleID.ToString();
            }
        }
        #endregion

        // Save Form Data

        #region btnSave_Click
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateTicketForm())
                {
                    int intTaskID = SaveTicketForm();
                    SaveTags(intTaskID);
                    ShowUpdated();
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }
        #endregion

        #region ValidateTicketForm
        private bool ValidateTicketForm()
        {
            List<string> ColErrors = new List<string>();

            // Only validate Name and email if Ticket is not for a DNN user
            // lblName will be hidden if it is not a DNN user
                if (txtName.Text.Trim().Length < 1)
                {
                    ColErrors.Add(Localization.GetString("NameIsRequired.Text", LocalResourceFile));
                }

                if (txtEmail.Text.Trim().Length < 1)
                {
                    ColErrors.Add(Localization.GetString("EmailIsRequired.Text", LocalResourceFile));
                }

            // Validate the date only if a date was entered
            if (txtDueDate.Text.Trim().Length > 1)
            {
                try
                {
                    DateTime tmpDate = Convert.ToDateTime(txtDueDate.Text.Trim());
                }
                catch
                {
                    ColErrors.Add(Localization.GetString("MustUseAValidDate.Text", LocalResourceFile));
                }
            }

            if (txtStart.Text.Trim().Length > 1)
            {
                try
                {
                    DateTime tmpDate = Convert.ToDateTime(txtStart.Text.Trim());
                }
                catch
                {
                    ColErrors.Add(Localization.GetString("MustUseAValidDate.Text", LocalResourceFile));
                }
            }

            if (txtComplete.Text.Trim().Length > 1)
            {
                try
                {
                    DateTime tmpDate = Convert.ToDateTime(txtComplete.Text.Trim());
                }
                catch
                {
                    ColErrors.Add(Localization.GetString("MustUseAValidDate.Text", LocalResourceFile));
                }
            }

            if (txtEstimate.Text.Trim().Length > 0)
            {
                try
                {
                    int tmpInt = Convert.ToInt32(txtEstimate.Text.Trim());
                }
                catch
                {
                    ColErrors.Add(Localization.GetString("MustUseAValidEstimateHours.Text", LocalResourceFile));
                }
            }

            // Display Validation Errors
            if (ColErrors.Count > 0)
            {
                foreach (string objError in ColErrors)
                {
                    lblError.Text = lblError.Text + String.Format("* {0}<br />", objError);
                }
            }

            return (ColErrors.Count == 0);
        }
        #endregion

        #region SaveTicketForm
        private int SaveTicketForm()
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            ITILServiceDesk_Task objITILServiceDesk_Task = (from ITILServiceDesk_Tasks in objServiceDeskDALDataContext.ITILServiceDesk_Tasks
                                                      where ITILServiceDesk_Tasks.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                                                      select ITILServiceDesk_Tasks).FirstOrDefault();

            // Save original Assigned Group
            int intOriginalAssignedGroup = objITILServiceDesk_Task.AssignedRoleID;

            // Save Task
            objITILServiceDesk_Task.Status = ddlStatus.SelectedValue;
            objITILServiceDesk_Task.Description = txtDescription.Text;
            objITILServiceDesk_Task.PortalID = PortalId;
            objITILServiceDesk_Task.Priority = ddlPriority.SelectedValue;
            objITILServiceDesk_Task.RequesterPhone = txtPhone.Text;
            objITILServiceDesk_Task.AssignedRoleID = Convert.ToInt32(ddlAssigned.SelectedValue);

            // Only validate Name and email if Ticket is not for a DNN user
            // lblName will be hidden if it is not a DNN user
                // not a DNN user
                objITILServiceDesk_Task.RequesterEmail = txtEmail.Text;
                objITILServiceDesk_Task.RequesterName = txtName.Text;
                //objITILServiceDesk_Task.RequesterUserID = -1;

            // DueDate
            if (txtDueDate.Text.Trim().Length > 1)
            {
                objITILServiceDesk_Task.DueDate = Convert.ToDateTime(txtDueDate.Text.Trim());
            }
            else
            {
                objITILServiceDesk_Task.DueDate = null;
            }

            // EstimatedStart
            if (txtStart.Text.Trim().Length > 1)
            {
                objITILServiceDesk_Task.EstimatedStart = Convert.ToDateTime(txtStart.Text.Trim());
            }
            else
            {
                objITILServiceDesk_Task.EstimatedStart = null;
            }

            // EstimatedCompletion
            if (txtComplete.Text.Trim().Length > 1)
            {
                objITILServiceDesk_Task.EstimatedCompletion = Convert.ToDateTime(txtComplete.Text.Trim());
            }
            else
            {
                objITILServiceDesk_Task.EstimatedCompletion = null;
            }

            // EstimatedHours
            if (txtEstimate.Text.Trim().Length > 0)
            {
                objITILServiceDesk_Task.EstimatedHours = Convert.ToInt32(txtEstimate.Text.Trim());
            }
            else
            {
                objITILServiceDesk_Task.EstimatedHours = null;
            }

            objServiceDeskDALDataContext.SubmitChanges();

            //ITIL Customization - notify requester when ticket is resolved
            if (ddlStatus.SelectedValue == "Resolved")
            {
                if (Status != "Resolved")
                {
                    NotifyRequesterTicketResolved(objITILServiceDesk_Task.TaskID.ToString());
                    Status = ddlStatus.SelectedValue;
                }
            }


            // Notify Assigned Group
            if (Convert.ToInt32(ddlAssigned.SelectedValue) > -1)
            {
                // Only notify if Assigned group has changed
                if (intOriginalAssignedGroup != Convert.ToInt32(ddlAssigned.SelectedValue))
                {
                    //NotifyAssignedGroupOfAssignment(objITILServiceDesk_Task.TaskID.ToString());
                    NotifyGroupAssignTicket(objITILServiceDesk_Task.TaskID.ToString());
                }
            }

            // Insert Log
            Log.InsertLog(objITILServiceDesk_Task.TaskID, UserId, String.Format(Localization.GetString("UpdatedTicket.Text", LocalResourceFile), UserInfo.DisplayName));

            return objITILServiceDesk_Task.TaskID;
        }
        #endregion

        #region SaveTags
        private void SaveTags(int intTaskID)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            var ExistingTaskCategories = from ITILServiceDesk_TaskCategories in objServiceDeskDALDataContext.ITILServiceDesk_TaskCategories
                                         where ITILServiceDesk_TaskCategories.TaskID == intTaskID
                                         select ITILServiceDesk_TaskCategories;

            // Delete all existing TaskCategories
            if (ExistingTaskCategories != null)
            {
                objServiceDeskDALDataContext.ITILServiceDesk_TaskCategories.DeleteAllOnSubmit(ExistingTaskCategories);
                objServiceDeskDALDataContext.SubmitChanges();
            }

            // Add TaskCategories
            TreeView objTreeView = (TreeView)TagsTreeExistingTasks.FindControl("tvCategories");
            if (objTreeView.CheckedNodes.Count > 0)
            {
                // Iterate through the CheckedNodes collection 
                foreach (TreeNode node in objTreeView.CheckedNodes)
                {
                    ITILServiceDesk_TaskCategory objITILServiceDesk_TaskCategory = new ITILServiceDesk_TaskCategory();

                    objITILServiceDesk_TaskCategory.TaskID = intTaskID;
                    objITILServiceDesk_TaskCategory.CategoryID = Convert.ToInt32(node.Value);

                    objServiceDeskDALDataContext.ITILServiceDesk_TaskCategories.InsertOnSubmit(objITILServiceDesk_TaskCategory);
                    objServiceDeskDALDataContext.SubmitChanges();
                }
            }
        }
        #endregion

        #region ShowUpdated
        private void ShowUpdated()
        {
            lblError.Text = Localization.GetString("Updated.Text", LocalResourceFile);

            // Provide a way for the user to see that a record has been updated
            // multiple times by changing the color each time
            lblError.ForeColor = (lblError.ForeColor == Color.Red) ? Color.Blue : Color.Red;

        }
        #endregion

        // Details

        #region DisableAllButtons
        private void DisableAllButtons()
        {
            btnComments.Font.Underline = false;
            btnComments.Font.Bold = false;
            pnlComments.Visible = false;

            btnWorkItems.Font.Underline = false;
            btnWorkItems.Font.Bold = false;
            pnlWorkItems.Visible = false;

            btnLogs.Font.Underline = false;
            btnLogs.Font.Bold = false;
            pnlLogs.Visible = false;


        }
        #endregion

        // Comments

        
        #region btnComments_Click
        protected void btnComments_Click(object sender, EventArgs e)
        {
            // If we are already on the Comments screen then switch Comments to Default mode
            if (pnlComments.Visible == true)
            {
                CommentsControl.SetView("Default");
            }

            DisableAllButtons();
            btnComments.Font.Bold = true;
            btnComments.Font.Underline = true;

            pnlComments.Visible = true;
        }
        #endregion

        // Work Items

        #region btnWorkItems_Click
        protected void btnWorkItems_Click(object sender, EventArgs e)
        {
            if (pnlWorkItems.Visible == true)
            {
                WorkControl.SetView("Default");
            }

            DisableAllButtons();
            btnWorkItems.Font.Bold = true;
            btnWorkItems.Font.Underline = true;
            pnlWorkItems.Visible = true;
        }
        #endregion

        // Emails


        //ITIL Customization - send email requester when ticket is resolved
        #region NotifyRequesterTicketResolved
        private void NotifyRequesterTicketResolved(string TaskID)
        {

            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            ITILServiceDesk_Task objITILServiceDesk_Tasks = (from ITILServiceDesk_Tasks in objServiceDeskDALDataContext.ITILServiceDesk_Tasks
                                                       where ITILServiceDesk_Tasks.TaskID == Convert.ToInt32(TaskID)
                                                       select ITILServiceDesk_Tasks).FirstOrDefault();

            string strDomainServerUrl = DotNetNuke.Common.Globals.AddHTTP(Request.Url.Host);  // ITIL Customization - get DomainServerUrl for use in Utility.FixURLLink
            string strPasswordLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}&TP={1}", TaskID, objITILServiceDesk_Tasks.TicketPassword)), strDomainServerUrl);

            string strSubject = String.Format(Localization.GetString("TicketIsResolved.Text", LocalResourceFile), TaskID);
            string strEmail = objITILServiceDesk_Tasks.RequesterEmail;

            // If userId is not -1 then get the Email
            if (objITILServiceDesk_Tasks.RequesterUserID > -1)
            {
                strEmail = UserController.GetUserById(PortalId, objITILServiceDesk_Tasks.RequesterUserID).Email;
            }

            string strBody = Localization.GetString("HTMLTicketEmailRequester.Text", LocalResourceFile);
            strBody = Utility.ReplaceTicketToken(strBody, strPasswordLinkUrl, objITILServiceDesk_Tasks);

            DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, strEmail, "", strSubject, strBody, "", "HTML", "", "", "", "");

        }
        #endregion


        #region NotifyAssignedGroupOfAssignment
        private void NotifyAssignedGroupOfAssignment(string TaskID)
        {
            // ITIL Customization - email notifies the Admins of a new ticket and also includes the ticket details
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();
            ITILServiceDesk_Task objITILServiceDesk_Tasks = (from ITILServiceDesk_Tasks in objServiceDeskDALDataContext.ITILServiceDesk_Tasks
                                                       where ITILServiceDesk_Tasks.TaskID == Convert.ToInt32(TaskID)
                                                       select ITILServiceDesk_Tasks).FirstOrDefault();

            string strDomainServerUrl = DotNetNuke.Common.Globals.AddHTTP(Request.Url.Host);  // ITIL Customization - get DomainServerUrl for use in Utility.FixURLLink
            string strPasswordLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}&TP={1}", TaskID, objITILServiceDesk_Tasks.TicketPassword)), strDomainServerUrl); // ITIL Customization 

            RoleController objRoleController = new RoleController();
            //string strAssignedRole = String.Format("{0}", objRoleController.GetRoleById(Convert.ToInt32(ddlAssigned.SelectedValue), PortalId).RoleName);
            string strAssignedRole = ddlAssigned.SelectedItem.Text;
            string strLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}", TaskID)), PortalSettings.PortalAlias.HTTPAlias);

            string strSubject = "[" + Localization.GetString(String.Format("ddlStatusAdmin{0}.Text", ddlStatus.SelectedValue), LocalResourceFile) + "] " + String.Format(Localization.GetString("HelpDeskTicketAtHasBeenAssigned.Text", LocalResourceFile), TaskID, PortalSettings.PortalAlias.HTTPAlias, strAssignedRole);
            string strBody = String.Format(Localization.GetString("HTMLTicketEmailAssignee.Text", LocalResourceFile), TaskID, txtDescription.Text);
            strBody = strBody + Environment.NewLine;
            strBody = strBody + String.Format(Localization.GetString("YouMaySeeStatusHere.Text", LocalResourceFile), strLinkUrl);

            // Get all users in the AssignedRole Role
            //ArrayList colAssignedRoleUsers = objRoleController.GetUsersByRoleName(PortalId, strAssignedRole);

            IList<UserInfo> colAssignedRoleUsers = RoleController.Instance.GetUsersByRole(PortalId, strAssignedRole);

            foreach (UserInfo objUserInfo in colAssignedRoleUsers)
            {
                DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, objUserInfo.Email, "", strSubject, strBody, "", "HTML", "", "", "", "");
            }

            Log.InsertLog(Convert.ToInt32(TaskID), UserId, String.Format(Localization.GetString("AssignedTicketTo.Text", LocalResourceFile), UserInfo.DisplayName, strAssignedRole));
        }

        private void NotifyGroupAssignTicket(string TaskID)
        {
            // ITIL Customization - email notifies the Admins of a new ticket and also includes the ticket details
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();
            ITILServiceDesk_Task objITILServiceDesk_Tasks = (from ITILServiceDesk_Tasks in objServiceDeskDALDataContext.ITILServiceDesk_Tasks
                                                       where ITILServiceDesk_Tasks.TaskID == Convert.ToInt32(TaskID)
                                                       select ITILServiceDesk_Tasks).FirstOrDefault();

            string strDomainServerUrl = DotNetNuke.Common.Globals.AddHTTP(Request.Url.Host);  // ITIL Customization - get DomainServerUrl for use in Utility.FixURLLink
            string strPasswordLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}&TP={1}", TaskID, objITILServiceDesk_Tasks.TicketPassword)), strDomainServerUrl);
            
            RoleController objRoleController = new RoleController();
            //string strAssignedRole = String.Format("{0}", objRoleController.GetRoleById(Convert.ToInt32(ddlAssigned.SelectedValue), PortalId).RoleName);
            string strAssignedRole = ddlAssigned.SelectedItem.Text;
            string strLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}", TaskID)), PortalSettings.PortalAlias.HTTPAlias);

            string strSubject = String.Format(Localization.GetString("HelpDeskTicketAtHasBeenAssigned.Text", LocalResourceFile), TaskID, strAssignedRole);
            string strBody = Localization.GetString("HTMLTicketEmailAssignee.Text", LocalResourceFile);
            strBody = Utility.ReplaceTicketToken(strBody, strPasswordLinkUrl, objITILServiceDesk_Tasks);
            strBody = strBody + Environment.NewLine;
            strBody = strBody + String.Format(Localization.GetString("YouMaySeeStatusHere.Text", LocalResourceFile), strLinkUrl);

            // Get all users in the AssignedRole Role
           // ArrayList colAssignedRoleUsers = objRoleController.GetUsersByRoleName(PortalId, strAssignedRole);
            IList<UserInfo> colAssignedRoleUsers = RoleController.Instance.GetUsersByRole(PortalId, strAssignedRole);

            foreach (UserInfo objUserInfo in colAssignedRoleUsers)
            {
                DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, objUserInfo.Email, "", strSubject, strBody, "", "HTML", "", "", "", "");
            }

            Log.InsertLog(Convert.ToInt32(TaskID), UserId, String.Format(Localization.GetString("AssignedTicketTo.Text", LocalResourceFile), UserInfo.DisplayName, strAssignedRole));
        }

        #endregion

        // Logs

        #region btnLogs_Click
        protected void btnLogs_Click(object sender, EventArgs e)
        {
            DisableAllButtons();
            //btnLogs.BorderStyle = BorderStyle.Inset;
            //btnLogs.BackColor = Color.LightGray;
            //btnLogs.Font.Bold = true;
            //btnLogs.ForeColor = Color.White;
            pnlLogs.Visible = true;
            btnLogs.Font.Bold = true;
            btnLogs.Font.Underline = true;

            LogsControl.RefreshLogs();
        }
        #endregion

    }
}