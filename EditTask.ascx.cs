

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
    public partial class EditTask : DotNetNuke.Entities.Modules.PortalModuleBase
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

            List<ServiceDesk_Role> colServiceDesk_Roles = (from ServiceDesk_Roles in objServiceDeskDALDataContext.ServiceDesk_Roles
                                                             where ServiceDesk_Roles.PortalID == PortalId
                                                             select ServiceDesk_Roles).ToList();

            // Create a ListItemCollection to hold the Roles 
            ListItemCollection colListItemCollection = new ListItemCollection();

            // Add the Roles to the List
            foreach (ServiceDesk_Role objServiceDesk_Role in colServiceDesk_Roles)
            {
                try
                {
                    RoleInfo objRoleInfo = objRoleController.GetRole(Convert.ToInt32(objServiceDesk_Role.RoleID), PortalId);

                    ListItem RoleListItem = new ListItem();
                    RoleListItem.Text = objRoleInfo.RoleName;
                    RoleListItem.Value = objServiceDesk_Role.RoleID.ToString();
                    ddlAssigned.Items.Add(RoleListItem);
                }
                catch
                {
                    // Role no longer exists in Portal
                    ListItem RoleListItem = new ListItem();
                    RoleListItem.Text = Localization.GetString("DeletedRole.Text", LocalResourceFile);
                    RoleListItem.Value = objServiceDesk_Role.RoleID.ToString();
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

            ServiceDesk_Task objServiceDesk_Tasks = (from ServiceDesk_Tasks in objServiceDeskDALDataContext.ServiceDesk_Tasks
                                                       where ServiceDesk_Tasks.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                                                       select ServiceDesk_Tasks).FirstOrDefault();
            if (objServiceDesk_Tasks == null)
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
                    if (objServiceDesk_Tasks.TicketPassword == Convert.ToString(Request.QueryString["TP"]))
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
                if (UserId == objServiceDesk_Tasks.RequesterUserID)
                {
                    boolPassedSecurity = true;
                }

                //Is user in the Assigned Role?
                RoleController objRoleController = new RoleController();
                RoleInfo objRoleInfo = objRoleController.GetRole(objServiceDesk_Tasks.AssignedRoleID, PortalId);
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
                    if (objServiceDesk_Tasks.TicketPassword == Convert.ToString(Request.QueryString["TP"]))
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
            List<ServiceDesk_Setting> objServiceDesk_Settings = GetSettings();
            ServiceDesk_Setting objServiceDesk_Setting = objServiceDesk_Settings.Where(x => x.SettingName == "AdminRole").FirstOrDefault();

            string strAdminRoleID = "Administrators";
            if (objServiceDesk_Setting != null)
            {
                strAdminRoleID = objServiceDesk_Setting.SettingValue;
            }

            return strAdminRoleID;
        }
        #endregion

        #region GetSettings
        private List<ServiceDesk_Setting> GetSettings()
        {
            // Get Settings
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            List<ServiceDesk_Setting> colServiceDesk_Setting = (from ServiceDesk_Settings in objServiceDeskDALDataContext.ServiceDesk_Settings
                                                                  where ServiceDesk_Settings.PortalID == PortalId
                                                                  select ServiceDesk_Settings).ToList();

            if (colServiceDesk_Setting.Count == 0)
            {
                // Create Default vaules
                ServiceDesk_Setting objServiceDesk_Setting1 = new ServiceDesk_Setting();

                objServiceDesk_Setting1.PortalID = PortalId;
                objServiceDesk_Setting1.SettingName = "AdminRole";
                objServiceDesk_Setting1.SettingValue = "Administrators";

                objServiceDeskDALDataContext.ServiceDesk_Settings.InsertOnSubmit(objServiceDesk_Setting1);
                objServiceDeskDALDataContext.SubmitChanges();

                ServiceDesk_Setting objServiceDesk_Setting2 = new ServiceDesk_Setting();

                objServiceDesk_Setting2.PortalID = PortalId;
                objServiceDesk_Setting2.SettingName = "UploadefFilesPath";
                objServiceDesk_Setting2.SettingValue = Server.MapPath("~/DesktopModules/ServiceDesk/Upload");

                objServiceDeskDALDataContext.ServiceDesk_Settings.InsertOnSubmit(objServiceDesk_Setting2);
                objServiceDeskDALDataContext.SubmitChanges();

                colServiceDesk_Setting = (from ServiceDesk_Settings in objServiceDeskDALDataContext.ServiceDesk_Settings
                                           where ServiceDesk_Settings.PortalID == PortalId
                                           select ServiceDesk_Settings).ToList();
            }

            return colServiceDesk_Setting;
        }
        #endregion

        // Tags

        #region DisplayCategoryTree
        private void DisplayCategoryTree()
        {
            bool boolUserAssignedToTask = false;
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            ServiceDesk_Task objServiceDesk_Tasks = (from ServiceDesk_Tasks in objServiceDeskDALDataContext.ServiceDesk_Tasks
                                                       where ServiceDesk_Tasks.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                                                       select ServiceDesk_Tasks).FirstOrDefault();

            //Is user in the Assigned Role?
            RoleController objRoleController = new RoleController();
            RoleInfo objRoleInfo = objRoleController.GetRole(objServiceDesk_Tasks.AssignedRoleID, PortalId);

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
            if (objServiceDesk_Tasks.ServiceDesk_TaskCategories.Select(x => x.CategoryID).ToArray<int>().Count() > 0)
            {
                int[] ArrStrCategories = objServiceDesk_Tasks.ServiceDesk_TaskCategories.Select(x => x.CategoryID).ToArray<int>();
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

            ServiceDesk_Task objServiceDesk_Tasks = (from ServiceDesk_Tasks in objServiceDeskDALDataContext.ServiceDesk_Tasks
                                                       where ServiceDesk_Tasks.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                                                       select ServiceDesk_Tasks).FirstOrDefault();

            // Name is editable only if user is Anonymous
            if (objServiceDesk_Tasks.RequesterUserID == -1)
            {
                txtEmail.Visible = true;
                txtName.Visible = true;
                lblEmail.Visible = false;
                lblName.Visible = false;
                txtEmail.Text = objServiceDesk_Tasks.RequesterEmail;
                txtName.Text = objServiceDesk_Tasks.RequesterName;

                //ITIL Customization - assigning (txtEmail.Text to RequesterEmail) and (txtName.Text to RequesterName) in case user is anonymous
                RequesterEmail = txtEmail.Text;
                RequesterName = txtName.Text;
            }
            else
            {
                txtEmail.Visible = false;
                txtName.Visible = false;
                lblEmail.Visible = true;
                lblName.Visible = true;

                UserInfo objRequester = UserController.GetUser(PortalId, objServiceDesk_Tasks.RequesterUserID, false);

                if (objRequester != null)
                {
                    lblEmail.Text = UserController.GetUser(PortalId, objServiceDesk_Tasks.RequesterUserID, false).Email;
                    lblName.Text = UserController.GetUser(PortalId, objServiceDesk_Tasks.RequesterUserID, false).DisplayName;

                    //ITIL Customization - assigning (lblEmail.Text to RequesterEmail) and (lblName.Text to RequesterName) in case user is anonymous
                    RequesterEmail = lblEmail.Text;
                    RequesterName = lblName.Text;
                }
                else
                {
                    lblName.Text = "[User Deleted]";
                }
            }

            lblTask.Text = objServiceDesk_Tasks.TaskID.ToString();
            lblCreatedData.Text = String.Format(objServiceDesk_Tasks.CreatedDate.ToShortDateString(), objServiceDesk_Tasks.CreatedDate.ToShortTimeString());
            ddlStatus.SelectedValue = objServiceDesk_Tasks.Status;

            //ITIL Customization - assign objServiceDesk_Tasks.Status to Status for the purpose of preventing multiple email notifications
            Status = objServiceDesk_Tasks.Status;
 
            ddlPriority.SelectedValue = objServiceDesk_Tasks.Priority;
            txtDescription.Text = objServiceDesk_Tasks.Description;
            txtPhone.Text = objServiceDesk_Tasks.RequesterPhone;
            txtDueDate.Text = (objServiceDesk_Tasks.DueDate.HasValue) ? objServiceDesk_Tasks.DueDate.Value.ToShortDateString() : "";
            txtStart.Text = (objServiceDesk_Tasks.EstimatedStart.HasValue) ? objServiceDesk_Tasks.EstimatedStart.Value.ToShortDateString() : "";
            txtComplete.Text = (objServiceDesk_Tasks.EstimatedCompletion.HasValue) ? objServiceDesk_Tasks.EstimatedCompletion.Value.ToShortDateString() : "";
            txtEstimate.Text = (objServiceDesk_Tasks.EstimatedHours.HasValue) ? objServiceDesk_Tasks.EstimatedHours.Value.ToString() : "";

            ListItem TmpRoleListItem = ddlAssigned.Items.FindByValue(objServiceDesk_Tasks.AssignedRoleID.ToString());
            if (TmpRoleListItem == null)
            {
                // Value was not found so add it
                RoleController objRoleController = new RoleController();
                RoleInfo objRoleInfo = objRoleController.GetRole(objServiceDesk_Tasks.AssignedRoleID, PortalId);

                if (objRoleInfo != null)
                {
                    ListItem RoleListItem = new ListItem();
                    RoleListItem.Text = objRoleInfo.RoleName;
                    RoleListItem.Value = objServiceDesk_Tasks.AssignedRoleID.ToString();
                    ddlAssigned.Items.Add(RoleListItem);

                    ddlAssigned.SelectedValue = objServiceDesk_Tasks.AssignedRoleID.ToString();
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
                ddlAssigned.SelectedValue = objServiceDesk_Tasks.AssignedRoleID.ToString();
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
            if (lblName.Visible == false)
            {
                if (txtName.Text.Trim().Length < 1)
                {
                    ColErrors.Add(Localization.GetString("NameIsRequired.Text", LocalResourceFile));
                }

                if (txtEmail.Text.Trim().Length < 1)
                {
                    ColErrors.Add(Localization.GetString("EmailIsRequired.Text", LocalResourceFile));
                }
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

            ServiceDesk_Task objServiceDesk_Task = (from ServiceDesk_Tasks in objServiceDeskDALDataContext.ServiceDesk_Tasks
                                                      where ServiceDesk_Tasks.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                                                      select ServiceDesk_Tasks).FirstOrDefault();

            // Save original Assigned Group
            int intOriginalAssignedGroup = objServiceDesk_Task.AssignedRoleID;

            // Save Task
            objServiceDesk_Task.Status = ddlStatus.SelectedValue;
            objServiceDesk_Task.Description = txtDescription.Text;
            objServiceDesk_Task.PortalID = PortalId;
            objServiceDesk_Task.Priority = ddlPriority.SelectedValue;
            objServiceDesk_Task.RequesterPhone = txtPhone.Text;
            objServiceDesk_Task.AssignedRoleID = Convert.ToInt32(ddlAssigned.SelectedValue);

            // Only validate Name and email if Ticket is not for a DNN user
            // lblName will be hidden if it is not a DNN user
            if (lblName.Visible == false)
            {
                // not a DNN user
                objServiceDesk_Task.RequesterEmail = txtEmail.Text;
                objServiceDesk_Task.RequesterName = txtName.Text;
                objServiceDesk_Task.RequesterUserID = -1;
            }

            // DueDate
            if (txtDueDate.Text.Trim().Length > 1)
            {
                objServiceDesk_Task.DueDate = Convert.ToDateTime(txtDueDate.Text.Trim());
            }
            else
            {
                objServiceDesk_Task.DueDate = null;
            }

            // EstimatedStart
            if (txtStart.Text.Trim().Length > 1)
            {
                objServiceDesk_Task.EstimatedStart = Convert.ToDateTime(txtStart.Text.Trim());
            }
            else
            {
                objServiceDesk_Task.EstimatedStart = null;
            }

            // EstimatedCompletion
            if (txtComplete.Text.Trim().Length > 1)
            {
                objServiceDesk_Task.EstimatedCompletion = Convert.ToDateTime(txtComplete.Text.Trim());
            }
            else
            {
                objServiceDesk_Task.EstimatedCompletion = null;
            }

            // EstimatedHours
            if (txtEstimate.Text.Trim().Length > 0)
            {
                objServiceDesk_Task.EstimatedHours = Convert.ToInt32(txtEstimate.Text.Trim());
            }
            else
            {
                objServiceDesk_Task.EstimatedHours = null;
            }

            objServiceDeskDALDataContext.SubmitChanges();

            //ITIL Customization - notify requester when ticket is resolved
            if (ddlStatus.SelectedValue == "Resolved")
            {
                if (Status != "Resolved")
                {
                    NotifyRequesterTicketResolved(objServiceDesk_Task.TaskID.ToString());
                    Status = ddlStatus.SelectedValue;
                }
            }


            // Notify Assigned Group
            if (Convert.ToInt32(ddlAssigned.SelectedValue) > -1)
            {
                // Only notify if Assigned group has changed
                if (intOriginalAssignedGroup != Convert.ToInt32(ddlAssigned.SelectedValue))
                {
                    //NotifyAssignedGroupOfAssignment(objServiceDesk_Task.TaskID.ToString());
                    NotifyGroupAssignTicket(objServiceDesk_Task.TaskID.ToString());
                }
            }

            // Insert Log
            Log.InsertLog(objServiceDesk_Task.TaskID, UserId, String.Format(Localization.GetString("UpdatedTicket.Text", LocalResourceFile), UserInfo.DisplayName));

            return objServiceDesk_Task.TaskID;
        }
        #endregion

        #region SaveTags
        private void SaveTags(int intTaskID)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            var ExistingTaskCategories = from ServiceDesk_TaskCategories in objServiceDeskDALDataContext.ServiceDesk_TaskCategories
                                         where ServiceDesk_TaskCategories.TaskID == intTaskID
                                         select ServiceDesk_TaskCategories;

            // Delete all existing TaskCategories
            if (ExistingTaskCategories != null)
            {
                objServiceDeskDALDataContext.ServiceDesk_TaskCategories.DeleteAllOnSubmit(ExistingTaskCategories);
                objServiceDeskDALDataContext.SubmitChanges();
            }

            // Add TaskCategories
            TreeView objTreeView = (TreeView)TagsTreeExistingTasks.FindControl("tvCategories");
            if (objTreeView.CheckedNodes.Count > 0)
            {
                // Iterate through the CheckedNodes collection 
                foreach (TreeNode node in objTreeView.CheckedNodes)
                {
                    ServiceDesk_TaskCategory objServiceDesk_TaskCategory = new ServiceDesk_TaskCategory();

                    objServiceDesk_TaskCategory.TaskID = intTaskID;
                    objServiceDesk_TaskCategory.CategoryID = Convert.ToInt32(node.Value);

                    objServiceDeskDALDataContext.ServiceDesk_TaskCategories.InsertOnSubmit(objServiceDesk_TaskCategory);
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
            //btnComments.BorderStyle = BorderStyle.Outset;
            //btnComments.BackColor = Color.WhiteSmoke;
            //btnComments.Font.Bold = false;
            //btnComments.ForeColor = Color.White;
            pnlComments.Visible = false;

            //btnWorkItems.BorderStyle = BorderStyle.Outset;
            //btnWorkItems.BackColor = Color.WhiteSmoke;
            //btnWorkItems.Font.Bold = false;
            //btnWorkItems.ForeColor = Color.White;
            pnlWorkItems.Visible = false;

            //btnLogs.BorderStyle = BorderStyle.Outset;
            //btnLogs.BackColor = Color.WhiteSmoke;
            //btnLogs.Font.Bold = false;
            //btnLogs.ForeColor = Color.White;
            pnlLogs.Visible = false;

            btnComments.CssClass = "btn btn-default";
            btnWorkItems.CssClass = "btn btn-default";
            btnLogs.CssClass = "btn btn-default";

            //btnComments.ForeColor = System.Drawing.Color.DodgerBlue;
            //btnWorkItems.ForeColor = System.Drawing.Color.DodgerBlue;
            //btnLogs.ForeColor = System.Drawing.Color.DodgerBlue;
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
            //btnComments.BorderStyle = BorderStyle.Inset;
            //btnComments.BackColor = Color.LightGray;
            //btnComments.Font.Bold = true;
            //btnComments.ForeColor = Color.White;
            pnlComments.Visible = true;
            btnComments.CssClass = "btn btn-info";
        }
        #endregion

        // Work Items

        #region btnWorkItems_Click
        protected void btnWorkItems_Click(object sender, EventArgs e)
        {
            DisableAllButtons();
            //btnWorkItems.BorderStyle = BorderStyle.Inset;
            //btnWorkItems.BackColor = Color.LightGray;
            //btnWorkItems.Font.Bold = true;
            //btnWorkItems.ForeColor = Color.White;
            pnlWorkItems.Visible = true;
            btnWorkItems.CssClass = "btn btn-info";
        }
        #endregion

        // Emails


        //ITIL Customization - send email requester when ticket is resolved
        #region NotifyRequesterTicketResolved
        private void NotifyRequesterTicketResolved(string TaskID)
        {

            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            ServiceDesk_Task objServiceDesk_Tasks = (from ServiceDesk_Tasks in objServiceDeskDALDataContext.ServiceDesk_Tasks
                                                       where ServiceDesk_Tasks.TaskID == Convert.ToInt32(TaskID)
                                                       select ServiceDesk_Tasks).FirstOrDefault();

            string strDomainServerUrl = DotNetNuke.Common.Globals.AddHTTP(Request.Url.Host);  // ITIL Customization - get DomainServerUrl for use in Utility.FixURLLink
            string strPasswordLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}&TP={1}", TaskID, objServiceDesk_Tasks.TicketPassword)), strDomainServerUrl);

            string strSubject = String.Format(Localization.GetString("TicketIsResolved.Text", LocalResourceFile), TaskID);
            string strEmail = objServiceDesk_Tasks.RequesterEmail;

            // If userId is not -1 then get the Email
            if (objServiceDesk_Tasks.RequesterUserID > -1)
            {
                strEmail = UserController.GetUser(PortalId, objServiceDesk_Tasks.RequesterUserID, false).Email;
            }

            string strBody = Localization.GetString("HTMLTicketEmailRequester.Text", LocalResourceFile);
            strBody = Utility.ReplaceTicketToken(strBody, strPasswordLinkUrl, objServiceDesk_Tasks);

            DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, strEmail, "", strSubject, strBody, "", "HTML", "", "", "", "");

        }
        #endregion


        #region NotifyAssignedGroupOfAssignment
        private void NotifyAssignedGroupOfAssignment(string TaskID)
        {
            // ITIL Customization - email notifies the Admins of a new ticket and also includes the ticket details
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();
            ServiceDesk_Task objServiceDesk_Tasks = (from ServiceDesk_Tasks in objServiceDeskDALDataContext.ServiceDesk_Tasks
                                                       where ServiceDesk_Tasks.TaskID == Convert.ToInt32(TaskID)
                                                       select ServiceDesk_Tasks).FirstOrDefault();

            string strDomainServerUrl = DotNetNuke.Common.Globals.AddHTTP(Request.Url.Host);  // ITIL Customization - get DomainServerUrl for use in Utility.FixURLLink
            string strPasswordLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}&TP={1}", TaskID, objServiceDesk_Tasks.TicketPassword)), strDomainServerUrl); // ITIL Customization 

            RoleController objRoleController = new RoleController();
            string strAssignedRole = String.Format("{0}", objRoleController.GetRole(Convert.ToInt32(ddlAssigned.SelectedValue), PortalId).RoleName);
            string strLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}", TaskID)), PortalSettings.PortalAlias.HTTPAlias);

            string strSubject = "[" + Localization.GetString(String.Format("ddlStatusAdmin{0}.Text", ddlStatus.SelectedValue), LocalResourceFile) + "] " + String.Format(Localization.GetString("HelpDeskTicketAtHasBeenAssigned.Text", LocalResourceFile), TaskID, PortalSettings.PortalAlias.HTTPAlias, strAssignedRole);
            string strBody = String.Format(Localization.GetString("HTMLTicketEmailAssignee.Text", LocalResourceFile), TaskID, txtDescription.Text);
            strBody = strBody + Environment.NewLine;
            strBody = strBody + String.Format(Localization.GetString("YouMaySeeStatusHere.Text", LocalResourceFile), strLinkUrl);

            // Get all users in the AssignedRole Role
            ArrayList colAssignedRoleUsers = objRoleController.GetUsersByRoleName(PortalId, strAssignedRole);

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
            ServiceDesk_Task objServiceDesk_Tasks = (from ServiceDesk_Tasks in objServiceDeskDALDataContext.ServiceDesk_Tasks
                                                       where ServiceDesk_Tasks.TaskID == Convert.ToInt32(TaskID)
                                                       select ServiceDesk_Tasks).FirstOrDefault();

            string strDomainServerUrl = DotNetNuke.Common.Globals.AddHTTP(Request.Url.Host);  // ITIL Customization - get DomainServerUrl for use in Utility.FixURLLink
            string strPasswordLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}&TP={1}", TaskID, objServiceDesk_Tasks.TicketPassword)), strDomainServerUrl);
            
            RoleController objRoleController = new RoleController();
            string strAssignedRole = String.Format("{0}", objRoleController.GetRole(Convert.ToInt32(ddlAssigned.SelectedValue), PortalId).RoleName);
            string strLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}", TaskID)), PortalSettings.PortalAlias.HTTPAlias);

            string strSubject = String.Format(Localization.GetString("HelpDeskTicketAtHasBeenAssigned.Text", LocalResourceFile), TaskID, strAssignedRole);
            string strBody = Localization.GetString("HTMLTicketEmailAssignee.Text", LocalResourceFile);
            strBody = Utility.ReplaceTicketToken(strBody, strPasswordLinkUrl, objServiceDesk_Tasks);
            strBody = strBody + Environment.NewLine;
            strBody = strBody + String.Format(Localization.GetString("YouMaySeeStatusHere.Text", LocalResourceFile), strLinkUrl);

            // Get all users in the AssignedRole Role
            ArrayList colAssignedRoleUsers = objRoleController.GetUsersByRoleName(PortalId, strAssignedRole);

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
            btnLogs.CssClass = "btn btn-info";

            LogsControl.RefreshLogs();
        }
        #endregion

    }
}