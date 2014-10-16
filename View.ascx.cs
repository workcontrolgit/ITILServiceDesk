//
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
// Silk icon set 1.3 by
// Mark James
// http://www.famfamfam.com/lab/icons/silk/
// Creative Commons Attribution 2.5 License.
// [ http://creativecommons.org/licenses/by/2.5/ ]

using System;
using System.Linq;
using System.Collections.Generic;
using DotNetNuke.Common;
using DotNetNuke.Entities.Users;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;
using System.Collections;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Security.Roles;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using Microsoft.VisualBasic;
using DotNetNuke.Services.Localization;
using DotNetNuke.Entities.Host;

namespace ITIL.Modules.ServiceDesk
{
    #region ExistingTasks
    [Serializable]
    public class ExistingTasks
    {
        public int TaskID { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string Assigned { get; set; }
        public string Description { get; set; }
        public string Requester { get; set; }
        public string RequesterName { get; set; }
        public string Search { get; set; }
        public int?[] Categories { get; set; }
    }
    #endregion

    #region AssignedRoles
    public class AssignedRoles
    {
        public string AssignedRoleID { get; set; }
        public string Key { get; set; }
    }
    #endregion

    #region ListPage
    public class ListPage
    {
        public int PageNumber { get; set; }
    }
    #endregion

    public partial class View : ITILServiceDeskModuleBase
    {
        #region SortExpression
        public string SortExpression
        {
            get
            {
                if (ViewState["SortExpression"] == null)
                {
                    ViewState["SortExpression"] = string.Empty;
                }

                return Convert.ToString(ViewState["SortExpression"]);
            }
            set
            {
                ViewState["SortExpression"] = value;
            }
        }
        #endregion

        #region SortDirection
        public string SortDirection
        {
            get
            {
                if (ViewState["SortDirection"] == null)
                {
                    ViewState["SortDirection"] = string.Empty;
                }

                return Convert.ToString(ViewState["SortDirection"]);
            }
            set
            {
                ViewState["SortDirection"] = value;
            }
        }
        #endregion

        #region SearchCriteria
        public ITILServiceDesk_LastSearch SearchCriteria
        {
            get
            {
                return GetLastSearchCriteria();
            }
        }
        #endregion

        #region CurrentPage
        public string CurrentPage
        {
            get
            {
                if (ViewState["CurrentPage"] == null)
                {
                    ViewState["CurrentPage"] = "1";
                }

                return Convert.ToString(ViewState["CurrentPage"]);
            }
            set
            {
                ViewState["CurrentPage"] = value;
            }
        }
        #endregion

        #region LocalizeStatusBinding
        public string LocalizeStatusBinding(string Value)
        {
            // From: http://ServiceDesk.codeplex.com/workitem/26043
            return Localization.GetString(string.Format("ddlStatusAdmin{0}", Value.Replace(" ", "")), LocalResourceFile);
        }
        #endregion

        #region LocalizePriorityBinding
        public string LocalizePriorityBinding(string Value)
        {
            // From: http://ServiceDesk.codeplex.com/workitem/26043
            return Localization.GetString(string.Format("ddlPriority{0}", Value.Replace(" ", "")), LocalResourceFile);
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // ITIL customization 

                cmdStartCalendar.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtDueDate);
                if (!Page.IsPostBack)
                {
                    ShowAdministratorLinkAndFileUpload();
                    ShowExistingTicketsLink();
                    txtUserID.Text = UserId.ToString();
                    DisplayCategoryTree();

                    if (Request.QueryString["Ticket"] != null)
                    {
                        if (Convert.ToString(Request.QueryString["Ticket"]) == "new")
                        {
                            SetView("New Ticket");
                            ShowAdministratorLinkAndFileUpload();

                        }
                    }
                    // ITIL customization 
                    //SearchList(int.Parse(Session["portalId"].ToString()));
                    // the end
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        #region GetLastSearchCriteria
        private ITILServiceDesk_LastSearch GetLastSearchCriteria()
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            ITILServiceDesk_LastSearch objITILServiceDesk_LastSearch = (from ITILServiceDesk_LastSearches in objServiceDeskDALDataContext.ITILServiceDesk_LastSearches
                                                                  where ITILServiceDesk_LastSearches.PortalID == PortalId
                                                                  where ITILServiceDesk_LastSearches.UserID == UserId
                                                                  select ITILServiceDesk_LastSearches).FirstOrDefault();

            if (objITILServiceDesk_LastSearch == null)
            {
                ITILServiceDesk_LastSearch InsertITILServiceDesk_LastSearch = new ITILServiceDesk_LastSearch();
                InsertITILServiceDesk_LastSearch.UserID = UserId;
                InsertITILServiceDesk_LastSearch.PortalID = PortalId;
                objServiceDeskDALDataContext.ITILServiceDesk_LastSearches.InsertOnSubmit(InsertITILServiceDesk_LastSearch);

                // Only save is user is logged in
                if (UserId > -1)
                {
                    objServiceDeskDALDataContext.SubmitChanges();
                }

                return InsertITILServiceDesk_LastSearch;
            }
            else
            {
                return objITILServiceDesk_LastSearch;
            }
        }
        #endregion

        #region SaveLastSearchCriteria
        private void SaveLastSearchCriteria(ITILServiceDesk_LastSearch UpdateITILServiceDesk_LastSearch)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            ITILServiceDesk_LastSearch objITILServiceDesk_LastSearch = (from ITILServiceDesk_LastSearches in objServiceDeskDALDataContext.ITILServiceDesk_LastSearches
                                                                  where ITILServiceDesk_LastSearches.PortalID == PortalId
                                                                  where ITILServiceDesk_LastSearches.UserID == UserId
                                                                  select ITILServiceDesk_LastSearches).FirstOrDefault();

            if (objITILServiceDesk_LastSearch == null)
            {
                objITILServiceDesk_LastSearch = new ITILServiceDesk_LastSearch();
                objITILServiceDesk_LastSearch.UserID = UserId;
                objITILServiceDesk_LastSearch.PortalID = PortalId;
                objServiceDeskDALDataContext.ITILServiceDesk_LastSearches.InsertOnSubmit(objITILServiceDesk_LastSearch);
                objServiceDeskDALDataContext.SubmitChanges();
            }

            objITILServiceDesk_LastSearch.AssignedRoleID = UpdateITILServiceDesk_LastSearch.AssignedRoleID;
            objITILServiceDesk_LastSearch.Categories = UpdateITILServiceDesk_LastSearch.Categories;
            objITILServiceDesk_LastSearch.CreatedDate = UpdateITILServiceDesk_LastSearch.CreatedDate;
            objITILServiceDesk_LastSearch.SearchText = UpdateITILServiceDesk_LastSearch.SearchText;
            objITILServiceDesk_LastSearch.DueDate = UpdateITILServiceDesk_LastSearch.DueDate;
            objITILServiceDesk_LastSearch.Priority = UpdateITILServiceDesk_LastSearch.Priority;
            objITILServiceDesk_LastSearch.Status = UpdateITILServiceDesk_LastSearch.Status;
            objITILServiceDesk_LastSearch.CurrentPage = UpdateITILServiceDesk_LastSearch.CurrentPage;
            objITILServiceDesk_LastSearch.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);

            objServiceDeskDALDataContext.SubmitChanges();
        }
        #endregion

        #region ShowExistingTicketsLink
        private void ShowExistingTicketsLink()
        {
            // Show Existing Tickets link if user is logged in
            if (UserId > -1)
            {

                lnkExistingTickets.Visible = true;
                lvTasks.Visible = true;
                //imgExitingTickets.Visible = true;
                SetView("Existing Tickets");
            }
            else
            {
                lnkExistingTickets.Visible = false;
                lvTasks.Visible = false;
                //imgExitingTickets.Visible = false;
                SetView("New Ticket");
            }
        }
        #endregion

        #region ShowAdministratorLinkAndFileUpload
        private void ShowAdministratorLinkAndFileUpload()
        {
            // Get Admin Role
            string strAdminRoleID = GetAdminRole();
            string strUploadPermission = GetUploadPermission();
            // Show Admin link if user is an Administrator
            if (UserInfo.IsInRole(strAdminRoleID) || UserInfo.IsInRole("Administrators") || UserInfo.IsSuperUser)
            {
                lnkAdministratorSettings.Visible = true;
                //imgAdministrator.Visible = true;
                TicketFileUpload.Visible = true;
                lblAttachFile.Visible = true;

                // Show the Administrator user selector and Ticket Status selectors
                pnlAdminUserSelection.Visible = true;
                pnlAdminUserSelection.GroupingText = Localization.GetString("AdminUserSelectionGrouping.Text", LocalResourceFile);
                pnlAdminTicketStatus.Visible = true;

                // Load the Roles dropdown
                LoadRolesDropDown();

                // Display default Admin view
                DisplayAdminView();
            }
            else
            {
                // ** Non Administrators **
                lnkAdministratorSettings.Visible = false;
                //imgAdministrator.Visible = false;

                // Do not show the Administrator user selector
                pnlAdminUserSelection.Visible = false;

                // Only supress Upload if permission is not set to All
                #region if (strUploadPermission != "All")
                if (strUploadPermission != "All")
                {
                    // Is user Logged in?
                    if (UserId > -1)
                    {
                        #region if (strUploadPermission != "Administrator/Registered Users")
                        // Only check this if security is set to "Administrator/Registered Users"
                        if (strUploadPermission != "Administrator/Registered Users")
                        {
                            // If User is not an Administrator so they cannot see upload
                            lblAttachFile.Visible = false;
                            TicketFileUpload.Visible = false;
                        }
                        else
                        {
                            TicketFileUpload.Visible = true;
                            lblAttachFile.Visible = true;
                        }
                        #endregion
                    }
                    else
                    {
                        // If User is not logged in they cannot see upload
                        lblAttachFile.Visible = false;
                        TicketFileUpload.Visible = false;
                    }
                }
                else
                {
                    TicketFileUpload.Visible = true;
                    lblAttachFile.Visible = true;
                }
                #endregion
            }
        }
        #endregion

        #region LoadRolesDropDown
        private void LoadRolesDropDown()
        {
            ddlAssignedAdmin.Items.Clear();

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
                    ddlAssignedAdmin.Items.Add(RoleListItem);
                }
                catch
                {
                    // Role no longer exists in Portal
                    ListItem RoleListItem = new ListItem();
                    RoleListItem.Text = Localization.GetString("DeletedRole.Text", LocalResourceFile);
                    RoleListItem.Value = objITILServiceDesk_Role.RoleID.ToString();
                    ddlAssignedAdmin.Items.Add(RoleListItem);
                }
            }

            // Add UnAssigned
            ListItem UnassignedRoleListItem = new ListItem();
            UnassignedRoleListItem.Text = Localization.GetString("Unassigned.Text", LocalResourceFile);
            UnassignedRoleListItem.Value = "-1";
            ddlAssignedAdmin.Items.Add(UnassignedRoleListItem);
        }
        #endregion

        #region DisplayCategoryTree
        private void DisplayCategoryTree()
        {
            if (UserInfo.IsInRole(GetAdminRole()) || UserInfo.IsInRole("Administrators") || UserInfo.IsSuperUser)
            {
                TagsTree.Visible = true;
                TagsTree.TagID = -1;
                TagsTree.DisplayType = "Administrator";
                TagsTree.Expand = false;

                TagsTreeExistingTasks.Visible = true;
                TagsTreeExistingTasks.TagID = -1;
                TagsTreeExistingTasks.DisplayType = "Administrator";
                TagsTreeExistingTasks.Expand = false;
            }
            else
            {
                TagsTree.Visible = true;
                TagsTree.TagID = -1;
                TagsTree.DisplayType = "Requestor";
                TagsTree.Expand = false;

                TagsTreeExistingTasks.Visible = true;
                TagsTreeExistingTasks.TagID = -1;
                TagsTreeExistingTasks.DisplayType = "Requestor";
                TagsTreeExistingTasks.Expand = false;
            }

            // Only Logged in users can have saved Categories in the Tag tree
            if ((UserId > -1) & (SearchCriteria.Categories != null))
            {
                if (SearchCriteria.Categories.Trim() != "")
                {
                    char[] delimiterChars = { ',' };
                    string[] ArrStrCategories = SearchCriteria.Categories.Split(delimiterChars);
                    // Convert the Categories selected from the Tags tree to an array of integers
                    int?[] ArrIntCatagories = Array.ConvertAll<string, int?>(ArrStrCategories, new Converter<string, int?>(ConvertStringToNullableInt));

                    TagsTreeExistingTasks.SelectedCategories = ArrIntCatagories;
                }
            }

            // Set visibility of Tags
            bool RequestorCatagories = (TagsTreeExistingTasks.DisplayType == "Administrator") ? false : true;
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            int CountOfCatagories = (from ServiceDeskCategories in CategoriesTable.GetCategoriesTable(PortalId, RequestorCatagories)
                                     where ServiceDeskCategories.PortalID == PortalId
                                     where ServiceDeskCategories.Level == 1
                                     select ServiceDeskCategories).Count();

            imgTags.Visible = (CountOfCatagories > 0);
            img2Tags.Visible = (CountOfCatagories > 0);
            lblCheckTags.Visible = (CountOfCatagories > 0);
            lblSearchTags.Visible = (CountOfCatagories > 0);
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

        #region GetUploadPermission
        private string GetUploadPermission()
        {
            List<ITILServiceDesk_Setting> objITILServiceDesk_Settings = GetSettings();
            ITILServiceDesk_Setting objITILServiceDesk_Setting = objITILServiceDesk_Settings.Where(x => x.SettingName == "UploadPermission").FirstOrDefault();

            string strUploadPermission = "All";
            if (objITILServiceDesk_Setting != null)
            {
                strUploadPermission = objITILServiceDesk_Setting.SettingValue;
            }

            return strUploadPermission;
        }
        #endregion

        #region lnkAdministratorSettings_Click
        protected void lnkAdministratorSettings_Click(object sender, EventArgs e)
        {
            //SetButtonDefault();
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "AdminSettings", "mid=" + ModuleId.ToString()));
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

            // Upload Permission
            ITILServiceDesk_Setting UploadPermissionITILServiceDesk_Setting = (from ITILServiceDesk_Settings in objServiceDeskDALDataContext.ITILServiceDesk_Settings
                                                                         where ITILServiceDesk_Settings.PortalID == PortalId
                                                                         where ITILServiceDesk_Settings.SettingName == "UploadPermission"
                                                                         select ITILServiceDesk_Settings).FirstOrDefault();

            if (UploadPermissionITILServiceDesk_Setting != null)
            {
                // Add to collection
                colITILServiceDesk_Setting.Add(UploadPermissionITILServiceDesk_Setting);
            }
            else
            {
                // Add Default value
                ITILServiceDesk_Setting objITILServiceDesk_Setting = new ITILServiceDesk_Setting();
                objITILServiceDesk_Setting.SettingName = "UploadPermission";
                objITILServiceDesk_Setting.SettingValue = "All";
                objITILServiceDesk_Setting.PortalID = PortalId;
                objServiceDeskDALDataContext.ITILServiceDesk_Settings.InsertOnSubmit(objITILServiceDesk_Setting);
                objServiceDeskDALDataContext.SubmitChanges();

                // Add to collection
                colITILServiceDesk_Setting.Add(objITILServiceDesk_Setting);
            }

            return colITILServiceDesk_Setting;
        }
        #endregion

        #region lnkNewTicket_Click
        protected void lnkNewTicket_Click(object sender, EventArgs e)
        {
            // Clear the form
            txtName.Text = "";
            txtEmail.Text = "";
            txtPhone.Text = "";
            txtDescription.Text = "";
            txtDetails.Text = "";
            txtDueDate.Text = "";
            ddlPriority.SelectedValue = "Normal";
            txtUserID.Text = UserId.ToString();

            SetView("New Ticket");
            ShowAdministratorLinkAndFileUpload();
        }
        #endregion

        #region lnkExistingTickets_Click
        protected void lnkExistingTickets_Click(object sender, EventArgs e)
        {
            SetView("Existing Tickets");
        }
        #endregion

        #region lnkResetSearch_Click

        private void ResetSearch()
        {
            ITILServiceDesk_LastSearch ExistingITILServiceDesk_LastSearch = GetLastSearchCriteria();
            ExistingITILServiceDesk_LastSearch.AssignedRoleID = null;
            ExistingITILServiceDesk_LastSearch.Categories = null;
            ExistingITILServiceDesk_LastSearch.CreatedDate = null;
            ExistingITILServiceDesk_LastSearch.SearchText = null;
            ExistingITILServiceDesk_LastSearch.DueDate = null;
            ExistingITILServiceDesk_LastSearch.Priority = null;
            ExistingITILServiceDesk_LastSearch.Status = null;
            ExistingITILServiceDesk_LastSearch.CurrentPage = 1;
            ExistingITILServiceDesk_LastSearch.PageSize = 25;

            ddlPageSize.SelectedValue = "25";
            CurrentPage = "1";

            SaveLastSearchCriteria(ExistingITILServiceDesk_LastSearch);

            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
        }

        protected void lnkResetSearch_Click(object sender, EventArgs e)
        {
            //SetButtonDefault();
            ResetSearch();
        }
        #endregion

        #region lnlAnonymousContinue_Click
        protected void lnlAnonymousContinue_Click(object sender, EventArgs e)
        {
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(), true);
        }
        #endregion

        #region SetView
        //private void SetButtonDefault()
        //{
        //    //lnkNewTicket.CssClass = "btn btn-default";
        //    lnkExistingTickets.CssClass = "btn btn-default";
        //    lnkResetSearch.CssClass = "btn btn-default";
        //    lnkAdministratorSettings.CssClass = "btn btn-default";

        //    //lnkNewTicket.ForeColor = System.Drawing.Color.Chocolate;
        //    lnkExistingTickets.ForeColor = System.Drawing.Color.Chocolate;
        //    lnkResetSearch.ForeColor = System.Drawing.Color.Chocolate;
        //    lnkAdministratorSettings.ForeColor = System.Drawing.Color.Chocolate;
        //}

        private void SetView(string ViewName)
        {
            //SetButtonDefault();

            if (ViewName == "New Ticket")
            {
                pnlNewTicket.Visible = true;
                pnlExistingTickets.Visible = false;
                pnlConfirmAnonymousUserEntry.Visible = false;
                //imgMagnifier.Visible = false;
                lnkResetSearch.Visible = false;
                lnkNewTicket.CssClass = "btn btn-info";
                lnkExistingTickets.CssClass = "btn btn-default";
                //lnkNewTicket.ForeColor = System.Drawing.Color.White;
                //lnkNewTicket.Font.Bold = true;
                //lnkNewTicket.BackColor = System.Drawing.Color.LightGray;
                lnkExistingTickets.Font.Bold = false;
                //lnkExistingTickets.BackColor = System.Drawing.Color.Transparent;

                DisplayNewTicketForm();
            }

            if (ViewName == "Existing Tickets")
            {
                
                pnlNewTicket.Visible = false;
                pnlExistingTickets.Visible = true;
                pnlConfirmAnonymousUserEntry.Visible = false;
                //imgMagnifier.Visible = true;
                lnkResetSearch.Visible = true;
                lnkExistingTickets.CssClass = "btn btn-info";
                lnkNewTicket.CssClass = "btn btn-default";
                //lnkExistingTickets.ForeColor = System.Drawing.Color.White;

                //lnkNewTicket.Font.Bold = false;
                //lnkNewTicket.BackColor = System.Drawing.Color.Transparent;
                //lnkExistingTickets.Font.Bold = true;
                //lnkExistingTickets.BackColor = System.Drawing.Color.LightGray;

                DisplayExistingTickets(SearchCriteria);
            }
        }
        #endregion

        #region DisplayNewTicketForm
        private void DisplayNewTicketForm()
        {
            // Logged in User
            if (UserId > -1)
            {

                // Load values for user
                txtName.Text = UserInfo.DisplayName;
                txtEmail.Text = UserInfo.Email;
                txtPhone.Text = UserInfo.Profile.Telephone;
            }
        }
        #endregion

        #region DisplayAdminView
        private void DisplayAdminView()
        {
            btnClearUser.Visible = false;
            txtName.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtPhone.Text = string.Empty;

            // Admin forms is set for anonymous user be default
            txtUserID.Text = "-1";
        }
        #endregion

        // Submit New Ticket

        #region btnSubmit_Click
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            int intTaskID = 0;

            try
            {
                if (ValidateNewTicketForm())
                {
                    intTaskID = SaveNewTicketForm();
                    SaveTags(intTaskID);

                    // Display post save view

                    if (txtUserID.Text == "-1")
                    {
                        // User not logged in
                        // Say "Request Submitted"
                        pnlConfirmAnonymousUserEntry.Visible = true;
                        pnlExistingTickets.Visible = false;
                        pnlNewTicket.Visible = false;
                        lblConfirmAnonymousUser.Text = String.Format(Localization.GetString("YourTicketNumber.Text", LocalResourceFile), intTaskID.ToString());
                    }
                    else
                    {
                        // User logged in
                        SetView("Existing Tickets");
                    }

                    SendEmail(intTaskID.ToString());
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }
        #endregion

        #region ValidateNewTicketForm
        private bool ValidateNewTicketForm()
        {
            List<string> ColErrors = new List<string>();

            // Validate Name and email
                if (txtName.Text.Trim().Length < 1)
                {
                    ColErrors.Add(Localization.GetString("NameIsRequired.Text", LocalResourceFile));
                }

                if (txtEmail.Text.Trim().Length < 1)
                {
                    ColErrors.Add(Localization.GetString("EmailIsRequired.Text", LocalResourceFile));
                }

                if (!txtEmail.Text.Trim().IsValidEmail())
                {
                    ColErrors.Add(Localization.GetString("EmailFormatInvalid.Text", LocalResourceFile));
                }
            


            if (txtDescription.Text.Trim().Length < 1)
            {
                ColErrors.Add(Localization.GetString("DescriptionIsRequired.Text", LocalResourceFile));
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

            // Validate file upload
            if (TicketFileUpload.HasFile)
            {

                // ITIL Customization - use DNN host setting for file extension
                string fileName = TicketFileUpload.PostedFile.FileName;

                string extension = Path.GetExtension(fileName);

                if (!Utility.IsAllowedExtension(fileName, extension))
                {
                    //string ErrorMessage = String.Format(Localization.GetString("InvalidFileExtension.Text", LocalResourceFile), string.Join(",", Host.AllowedExtensionWhitelist), extension);
                    string ErrorMessage = string.Format(Localization.GetExceptionMessage("AddFileExtensionNotAllowed", "The extension '{0}' is not allowed. The file has not been added."), extension);
                    ColErrors.Add(ErrorMessage);
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

        #region SaveNewTicketForm
        private int SaveNewTicketForm()
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            // Save Task
            ITILServiceDesk_Task objITILServiceDesk_Task = new ITILServiceDesk_Task();

            objITILServiceDesk_Task.Status = "New";
            objITILServiceDesk_Task.CreatedDate = DateTime.Now;
            objITILServiceDesk_Task.Description = txtDescription.Text;
            objITILServiceDesk_Task.PortalID = PortalId;
            objITILServiceDesk_Task.Priority = ddlPriority.SelectedValue;
            objITILServiceDesk_Task.RequesterPhone = txtPhone.Text;
            objITILServiceDesk_Task.AssignedRoleID = -1;
            objITILServiceDesk_Task.TicketPassword = GetRandomPassword();

            if (Convert.ToInt32(txtUserID.Text) == -1)
            {
                // User not logged in
                objITILServiceDesk_Task.RequesterEmail = txtEmail.Text;
                objITILServiceDesk_Task.RequesterName = txtName.Text;
                objITILServiceDesk_Task.RequesterUserID = -1;
            }
            else
            {
                // User logged in
                objITILServiceDesk_Task.RequesterUserID = Convert.ToInt32(txtUserID.Text);
                objITILServiceDesk_Task.RequesterName = UserController.GetUser(PortalId, Convert.ToInt32(txtUserID.Text), false).DisplayName;
            }

            if (txtDueDate.Text.Trim().Length > 1)
            {
                objITILServiceDesk_Task.DueDate = Convert.ToDateTime(txtDueDate.Text.Trim());
            }

            // If Admin panel is visible this is an admin
            // Save the Status and Assignment
            if (pnlAdminTicketStatus.Visible == true)
            {
                objITILServiceDesk_Task.AssignedRoleID = Convert.ToInt32(ddlAssignedAdmin.SelectedValue);
                objITILServiceDesk_Task.Status = ddlStatusAdmin.SelectedValue;
            }

            objServiceDeskDALDataContext.ITILServiceDesk_Tasks.InsertOnSubmit(objITILServiceDesk_Task);
            objServiceDeskDALDataContext.SubmitChanges();

            // Save Task Details
            ITILServiceDesk_TaskDetail objITILServiceDesk_TaskDetail = new ITILServiceDesk_TaskDetail();

            if ((txtDetails.Text.Trim().Length > 0) || (TicketFileUpload.HasFile))
            {
                objITILServiceDesk_TaskDetail.TaskID = objITILServiceDesk_Task.TaskID;
                objITILServiceDesk_TaskDetail.Description = txtDetails.Text;
                objITILServiceDesk_TaskDetail.DetailType = "Comment-Visible";
                objITILServiceDesk_TaskDetail.InsertDate = DateTime.Now;

                if (Convert.ToInt32(txtUserID.Text) == -1)
                {
                    // User not logged in
                    objITILServiceDesk_TaskDetail.UserID = -1;
                }
                else
                {
                    // User logged in
                    objITILServiceDesk_TaskDetail.UserID = Convert.ToInt32(txtUserID.Text);
                }

                objServiceDeskDALDataContext.ITILServiceDesk_TaskDetails.InsertOnSubmit(objITILServiceDesk_TaskDetail);
                objServiceDeskDALDataContext.SubmitChanges();

                // Upload the File
                if (TicketFileUpload.HasFile)
                {
                    UploadFile(objITILServiceDesk_TaskDetail.DetailID);
                    // Insert Log
                    Log.InsertLog(objITILServiceDesk_Task.TaskID, UserId, String.Format("{0} uploaded file '{1}'.", GetUserName(), TicketFileUpload.FileName));
                }
            }

            // Insert Log
            Log.InsertLog(objITILServiceDesk_Task.TaskID, UserId, String.Format("{0} created ticket.", GetUserName()));

            return objITILServiceDesk_Task.TaskID;
        }
        #endregion

        #region SaveTags
        private void SaveTags(int intTaskID)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            TreeView objTreeView = (TreeView)TagsTree.FindControl("tvCategories");
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

        #region GetRandomPassword
        public string GetRandomPassword()
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            int intElements = random.Next(10, 26);

            for (int i = 0; i < intElements; i++)
            {
                int intRandomType = random.Next(0, 2);
                if (intRandomType == 1)
                {
                    char ch;
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                    builder.Append(ch);
                }
                else
                {
                    builder.Append(random.Next(0, 9));
                }
            }
            return builder.ToString();
        }
        #endregion

        #region GetUserName
        private string GetUserName()
        {
            string strUserName = "Anonymous";

            if (UserId > -1)
            {
                strUserName = strUserName = UserInfo.DisplayName;
            }

            return strUserName;
        }
        #endregion

        #region btnClearUser_Click
        protected void btnClearUser_Click(object sender, EventArgs e)
        {
            DisplayAdminView();
        }
        #endregion

        // File upload

        #region UploadFile
        private void UploadFile(int intDetailID)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            string strUploadefFilesPath = (from ITILServiceDesk_Settings in objServiceDeskDALDataContext.ITILServiceDesk_Settings
                                           where ITILServiceDesk_Settings.PortalID == PortalId
                                           where ITILServiceDesk_Settings.SettingName == "UploadefFilesPath"
                                           select ITILServiceDesk_Settings).FirstOrDefault().SettingValue;

            EnsureDirectory(new System.IO.DirectoryInfo(strUploadefFilesPath));
            string strfilename = Convert.ToString(intDetailID) + "_" + GetRandomPassword() + Path.GetExtension(TicketFileUpload.FileName).ToLower();
            strUploadefFilesPath = strUploadefFilesPath + @"\" + strfilename;
            TicketFileUpload.SaveAs(strUploadefFilesPath);

            ITILServiceDesk_Attachment objITILServiceDesk_Attachment = new ITILServiceDesk_Attachment();
            objITILServiceDesk_Attachment.DetailID = intDetailID;
            objITILServiceDesk_Attachment.FileName = strfilename;
            objITILServiceDesk_Attachment.OriginalFileName = TicketFileUpload.FileName;
            objITILServiceDesk_Attachment.AttachmentPath = strUploadefFilesPath;
            objITILServiceDesk_Attachment.UserID = UserId;

            objServiceDeskDALDataContext.ITILServiceDesk_Attachments.InsertOnSubmit(objITILServiceDesk_Attachment);
            objServiceDeskDALDataContext.SubmitChanges();
        }
        #endregion

        #region EnsureDirectory
        public static void EnsureDirectory(System.IO.DirectoryInfo oDirInfo)
        {
            if (oDirInfo.Parent != null)
                EnsureDirectory(oDirInfo.Parent);
            if (!oDirInfo.Exists)
            {
                oDirInfo.Create();
            }
        }
        #endregion

        // Admin User Search

        #region btnSearchUser_Click
        protected void btnSearchUser_Click(object sender, EventArgs e)
        {
            if (txtSearchForUser.Text.Trim().Length != 0)
            {
                ArrayList Users;
                int TotalRecords = 0;
                if (ddlSearchForUserType.SelectedValue == "Email")
                {
                    Users = UserController.GetUsersByEmail(PortalId, false, txtSearchForUser.Text + "%", 0, 50, ref TotalRecords);
                }
                else
                {
                    String propertyName = ddlSearchForUserType.SelectedItem.Value;
                    Users = UserController.GetUsersByProfileProperty(PortalId, false, propertyName, txtSearchForUser.Text + "%", 0, 50, ref TotalRecords);
                }
                if (Users.Count > 0)
                {
                    lblCurrentProcessorNotFound.Visible = false;
                    gvCurrentProcessor.Visible = true;
                    gvCurrentProcessor.DataSource = Users;
                    gvCurrentProcessor.DataBind();
                }
                else
                {
                    lblCurrentProcessorNotFound.Text = Localization.GetString("UserIsNotFound.Text", LocalResourceFile);
                    lblCurrentProcessorNotFound.Visible = true;
                    gvCurrentProcessor.Visible = false;
                }
            }
        }
        #endregion

        #region gvCurrentProcessor_SelectedIndexChanged
        protected void gvCurrentProcessor_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridView GridView = (GridView)sender;
            GridViewRow GridViewRowAdd = (GridViewRow)GridView.SelectedRow;
            LinkButton LinkButtonAdd = (LinkButton)GridViewRowAdd.FindControl("lnkDisplayName");

            UserInfo objUserInfo = new UserInfo();
            objUserInfo = UserController.GetUser(PortalId, Convert.ToInt16(LinkButtonAdd.CommandArgument), false);

            txtName.Text = objUserInfo.DisplayName;
            txtEmail.Text = objUserInfo.Email;
            txtPhone.Text = objUserInfo.Profile.Telephone;


            txtUserID.Text = LinkButtonAdd.CommandArgument;
            txtSearchForUser.Text = "";
            gvCurrentProcessor.Visible = false;
            btnClearUser.Visible = true;
        }
        #endregion

        #region gvCurrentProcessor PageIndexChanging
        protected void gvCurrentProcessor_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (txtSearchForUser.Text.Trim().Length != 0)
            {
                ArrayList Users;
                int TotalRecords = 0;
                if (ddlSearchForUserType.SelectedValue == "Email")
                {
                    Users = UserController.GetUsersByEmail(PortalId, false, txtSearchForUser.Text + "%", 0, 50, ref TotalRecords);
                }
                else
                {
                    String propertyName = ddlSearchForUserType.SelectedItem.Value;
                    Users = UserController.GetUsersByProfileProperty(PortalId, false, propertyName, txtSearchForUser.Text + "%", 0, 50, ref TotalRecords);
                }
                if (Users.Count > 0)
                {
                    lblCurrentProcessorNotFound.Visible = false;
                    gvCurrentProcessor.Visible = true;
                    gvCurrentProcessor.PageIndex = e.NewPageIndex;
                    gvCurrentProcessor.DataSource = Users;
                    gvCurrentProcessor.DataBind();
                }
                else
                {
                    lblCurrentProcessorNotFound.Text = Localization.GetString("UserIsNotFound.Text", LocalResourceFile);
                    lblCurrentProcessorNotFound.Visible = true;
                    gvCurrentProcessor.Visible = false;
                }
            }

            
            //gvCurrentProcessor.DataBind();
            
        }
        #endregion gvCurrentProcessor PageIndexChanging


        // Email

        #region SendEmail
        private void SendEmail(string TaskID)
        {
            try
            {
                ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

                ITILServiceDesk_Task objITILServiceDesk_Tasks = (from ITILServiceDesk_Tasks in objServiceDeskDALDataContext.ITILServiceDesk_Tasks
                                                           where ITILServiceDesk_Tasks.TaskID == Convert.ToInt32(TaskID)
                                                           select ITILServiceDesk_Tasks).FirstOrDefault();

                string strPasswordLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}&TP={1}", TaskID, objITILServiceDesk_Tasks.TicketPassword)), PortalSettings.PortalAlias.HTTPAlias);
                string strLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}", TaskID)), PortalSettings.PortalAlias.HTTPAlias);
                string strSubject = String.Format(Localization.GetString("NewHelpDeskTicketCreated.Text", LocalResourceFile), TaskID);
                string strBody = "";

                if (Convert.ToInt32(txtUserID.Text) != UserId || UserId == -1)
                {
                    //Anonymous or login user submit ticket for another user
                    NotifyRequesterSubmitTicket(TaskID.ToString(), objITILServiceDesk_Tasks); //ITIL Customization - removed strPasswordLinkURL

                    // Get Admin Role
                    string strAdminRoleID = GetAdminRole();
                    // User is an Administrator
                    if (UserInfo.IsInRole(strAdminRoleID) || UserInfo.IsInRole(PortalSettings.AdministratorRoleName) || UserInfo.IsSuperUser)
                    {
                        // If Ticket is assigned to any group other than unassigned notify them
                        if (Convert.ToInt32(ddlAssignedAdmin.SelectedValue) > -1)
                        {
                            NotifyGroupAssignTicket(TaskID, objITILServiceDesk_Tasks); //ITIL Customization - removed strPasswordLinkURL
                        }
                    }
                    else
                    {
                        // This is not an Admin so Notify the Admins
                        // ITIL Customization - email notifies the Admins of a new ticket and also includes the ticket details 
                        NotifyAdminSubmitTicket(TaskID, objITILServiceDesk_Tasks); //ITIL Customization - removed strPasswordLinkURL
                    }
                }
                else
                {
                    // A normal ticket has been created

                    // ITIL Customization - email notifies the Admins of a new ticket and also includes the ticket details
                    NotifyAdminSubmitTicket(TaskID, objITILServiceDesk_Tasks); //ITIL Customization - removed strPasswordLinkURL
                    // ITIL Customization - email notify login user who entered ticket.  The email contains password protected link
                    NotifyRequesterSubmitTicket(TaskID.ToString(), objITILServiceDesk_Tasks); //ITIL Customization - removed strPasswordLinkURL
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }

            

        }
        #endregion

        // ITIL Customization - email notifies (logged in) requester of the new submitted ticket.  The email contains password protected link

        #region NotifyRequesterSubmitTicket
        private void NotifyRequesterSubmitTicket(string TaskID, ITILServiceDesk_Task objITILServiceDesk_Tasks)  //ITIL Customization - removed strPasswordLinkURL
        {
            string strDomainServerUrl = DotNetNuke.Common.Globals.AddHTTP(Request.Url.Host);  // ITIL Customization - get DomainServerUrl for use in Utility.FixURLLink
            string strPasswordLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}&TP={1}", TaskID, objITILServiceDesk_Tasks.TicketPassword)), strDomainServerUrl);
            string strSubject = String.Format(Localization.GetString("NewHelpDeskTicketCreated.Text", LocalResourceFile), TaskID);
            string strBody = Localization.GetString("HTMLTicketEmailRequester.Text", LocalResourceFile);
            strBody = Utility.ReplaceTicketToken(strBody, strPasswordLinkUrl, objITILServiceDesk_Tasks);

            string strEmail = txtEmail.Text;

            // If userId is not -1 then get the Email
            if (objITILServiceDesk_Tasks.RequesterUserID > -1)
            {
                strEmail = UserController.GetUser(PortalId, objITILServiceDesk_Tasks.RequesterUserID, false).Email;
            }

            DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, strEmail, "", strSubject, strBody, "", "HTML", "", "", "", "");


        }
        #endregion

        // ITIL Customization - email notifies services desk admins of the new submitted ticket.  The email contains password protected link

        #region NotifyAdminSubmitTicket
        private void NotifyAdminSubmitTicket(string TaskID, ITILServiceDesk_Task objITILServiceDesk_Tasks)  //ITIL Customization - removed strPasswordLinkURL
        {
            // This is not an Admin so Notify the Admins

            // ITIL Customization - email notifies the Admins of a new ticket and also includes the ticket details 
            string strDomainServerUrl = DotNetNuke.Common.Globals.AddHTTP(Request.Url.Host);  // ITIL Customization - get DomainServerUrl for use in Utility.FixURLLink
            string strPasswordLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}&TP={1}", TaskID, objITILServiceDesk_Tasks.TicketPassword)), strDomainServerUrl);

            string strSubject = String.Format(Localization.GetString("NewHelpDeskTicketCreatedAt.Text", LocalResourceFile), TaskID, strDomainServerUrl);
            string strBody = Localization.GetString("HTMLTicketEmailAdmin.Text", LocalResourceFile);
            strBody = Utility.ReplaceTicketToken(strBody, strPasswordLinkUrl, objITILServiceDesk_Tasks);

            // Get all users in the Admin Role
            RoleController objRoleController = new RoleController();
            ArrayList colAdminUsers = objRoleController.GetUsersByRoleName(PortalId, GetAdminRole());

            foreach (UserInfo objUserInfo in colAdminUsers)
            {
                DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, objUserInfo.Email, "", strSubject, strBody, "", "HTML", "", "", "", "");
            }
        }
        #endregion

        // ITIL Customization - added email to notify (logged in) requester who submitted the ticket.  The email contains password protected link

        #region NotifyGroupAssignTicket
        private void NotifyGroupAssignTicket(string TaskID, ITILServiceDesk_Task objITILServiceDesk_Tasks)  //ITIL Customization - removed strPasswordLinkURL
        {
            try
            {
                // ITIL Customization - email notifies the Admins of the new ticket and also includes the ticket details
                RoleController objRoleController = new RoleController();
                string strAssignedRole = String.Format("{0}", objRoleController.GetRole(Convert.ToInt32(ddlAssignedAdmin.SelectedValue), PortalId).RoleName);

                string strDomainServerUrl = DotNetNuke.Common.Globals.AddHTTP(Request.Url.Host);  // ITIL Customization - get DomainServerUrl for use in Utility.FixURLLink
                string strPasswordLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}&TP={1}", TaskID, objITILServiceDesk_Tasks.TicketPassword)), strDomainServerUrl);

                string strSubject = String.Format(Localization.GetString("HelpDeskTicketAtHasBeenAssigned.Text", LocalResourceFile), TaskID, strAssignedRole);
                string strBody = Localization.GetString("HTMLTicketEmailAssignee.Text", LocalResourceFile);
                strBody = Utility.ReplaceTicketToken(strBody, strPasswordLinkUrl, objITILServiceDesk_Tasks);

                // Get all users in the AssignedRole Role
                ArrayList colAssignedRoleUsers = objRoleController.GetUsersByRoleName(PortalId, strAssignedRole);

                foreach (UserInfo objUserInfo in colAssignedRoleUsers)
                {
                    DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, objUserInfo.Email, "", strSubject, strBody, "", "HTML", "", "", "", "");
                }

                Log.InsertLog(Convert.ToInt32(TaskID), UserId, String.Format(Localization.GetString("AssignedTicketTo.Text", LocalResourceFile), UserInfo.DisplayName, strAssignedRole));

            }
            catch (Exception ex)
            { }

            
            }
        #endregion

        #region NotifyAssignedGroup
        private void NotifyAssignedGroup(string TaskID)
        {
            RoleController objRoleController = new RoleController();
            string strAssignedRole = String.Format("{0}", objRoleController.GetRole(Convert.ToInt32(ddlAssignedAdmin.SelectedValue), PortalId).RoleName);

            string strSubject = String.Format(Localization.GetString("HelpDeskTicketAtHasBeenAssigned.Text", LocalResourceFile), TaskID, PortalSettings.PortalAlias.HTTPAlias, strAssignedRole);
            string strBody = String.Format(Localization.GetString("ANewHelpDeskTicketHasBeenAssigned.Text", LocalResourceFile), TaskID, txtDescription.Text);
            strBody = strBody + Environment.NewLine;
            strBody = strBody + String.Format(Localization.GetString("YouMaySeeStatusHere.Text", LocalResourceFile), DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}", TaskID)));

            // Get all users in the AssignedRole Role
            ArrayList colAssignedRoleUsers = objRoleController.GetUsersByRoleName(PortalId, strAssignedRole);

            foreach (UserInfo objUserInfo in colAssignedRoleUsers)
            {
                DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, objUserInfo.Email, "", strSubject, strBody, "", "HTML", "", "", "", "");
            }

            Log.InsertLog(Convert.ToInt32(TaskID), UserId, String.Format(Localization.GetString("AssignedTicketTo.Text", LocalResourceFile), UserInfo.DisplayName, strAssignedRole));
        }
        #endregion

        // Existing Tickets

        #region DisplayExistingTickets
        private void DisplayExistingTickets(ITILServiceDesk_LastSearch objLastSearch)
        {
            string[] UsersRoles = UserInfo.Roles;
            List<int> UsersRoleIDs = new List<int>();
            string strSearchText = (objLastSearch.SearchText == null) ? "" : objLastSearch.SearchText;

            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            IQueryable<ExistingTasks> result = from ITILServiceDesk_Tasks in objServiceDeskDALDataContext.ITILServiceDesk_Tasks
                                               where ITILServiceDesk_Tasks.PortalID == PortalId
                                               orderby ITILServiceDesk_Tasks.CreatedDate descending
                                               select new ExistingTasks
                                               {
                                                   TaskID = ITILServiceDesk_Tasks.TaskID,
                                                   Status = ITILServiceDesk_Tasks.Status,
                                                   Priority = ITILServiceDesk_Tasks.Priority,
                                                   DueDate = ITILServiceDesk_Tasks.DueDate,
                                                   CreatedDate = ITILServiceDesk_Tasks.CreatedDate,
                                                   Assigned = ITILServiceDesk_Tasks.AssignedRoleID.ToString(),
                                                   Description = ITILServiceDesk_Tasks.Description,
                                                   Requester = ITILServiceDesk_Tasks.RequesterUserID.ToString(),
                                                   RequesterName = ITILServiceDesk_Tasks.RequesterName
                                               };

            #region Only show users the records they should see
            // Only show users the records they should see
            if (!(UserInfo.IsInRole(GetAdminRole()) || UserInfo.IsInRole("Administrators") || UserInfo.IsSuperUser))
            {
                RoleController objRoleController = new RoleController();
                foreach (RoleInfo objRoleInfo in objRoleController.GetUserRoles(PortalId, UserId))
                {
                    UsersRoleIDs.Add(objRoleInfo.RoleID);
                }

                result = from UsersRecords in result
                         where Convert.ToInt32(UsersRecords.Requester) == UserId ||
                         UsersRoleIDs.Contains(Convert.ToInt32(UsersRecords.Assigned))
                         select UsersRecords;
            }
            #endregion

            #region Filter Status
            // Filter Status
            if (objLastSearch.Status != null)
            {
                result = from Status in result
                         where Status.Status == objLastSearch.Status
                         select Status;
            }
            #endregion

            #region Filter Priority
            // Filter Priority
            if (objLastSearch.Priority != null)
            {
                result = from Priority in result
                         where Priority.Priority == objLastSearch.Priority
                         select Priority;
            }
            #endregion

            #region Filter Assigned
            // Filter Assigned
            if (objLastSearch.AssignedRoleID.HasValue)
            {
                if (!(objLastSearch.AssignedRoleID == -2))
                {
                    result = from Assigned in result
                             where Assigned.Assigned == objLastSearch.AssignedRoleID.ToString()
                             select Assigned;
                }
            }
            #endregion

            #region Filter DueDate
            // Filter DueDate
            if (objLastSearch.DueDate.HasValue)
            {
                result = from objDueDate in result
                         where objDueDate.DueDate > objLastSearch.DueDate
                         select objDueDate;
            }
            #endregion

            #region Filter CreatedDate
            // Filter CreatedDate
            if (objLastSearch.CreatedDate.HasValue)
            {
                result = from CreatedDate in result
                         where CreatedDate.CreatedDate > objLastSearch.CreatedDate
                         select CreatedDate;
            }
            #endregion

            #region Filter TextBox (Search)
            // Filter TextBox
            if (strSearchText.Trim().Length > 0)
            {
                result = (from Search in result
                          join details in objServiceDeskDALDataContext.ITILServiceDesk_TaskDetails
                          on Search.TaskID equals details.TaskID into joined
                          from leftjoin in joined.DefaultIfEmpty()
                          where Search.Description.Contains(strSearchText) ||
                          Search.RequesterName.Contains(strSearchText) ||
                          Search.TaskID.ToString().Contains(strSearchText) ||
                          leftjoin.Description.Contains(strSearchText)
                          select Search).Distinct();
            }
            #endregion

            // Convert the results to a list because the query to filter the tags 
            // must be made after the preceeding query results have been pulled from the database
            List<ExistingTasks> FinalResult = result.Distinct().ToList();

            #region Filter Tags
            // Filter Tags
            if (objLastSearch.Categories != null)
            {
                char[] delimiterChars = { ',' };
                string[] ArrStrCategories = objLastSearch.Categories.Split(delimiterChars);
                // Convert the Categories selected from the Tags tree to an array of integers
                int[] ArrIntCatagories = Array.ConvertAll<string, int>(ArrStrCategories, new Converter<string, int>(ConvertStringToInt));

                // Perform a query that does in intersect between all the Catagories selected and all the categories that each TaskID has
                // The number of values that match must equal the number of values that were selected in the Tags tree
                FinalResult = (from Categories in FinalResult.AsQueryable()
                               where ((from ITILServiceDesk_TaskCategories in objServiceDeskDALDataContext.ITILServiceDesk_TaskCategories
                                       where ITILServiceDesk_TaskCategories.TaskID == Categories.TaskID
                                       select ITILServiceDesk_TaskCategories.CategoryID).ToArray<int>()).Intersect(ArrIntCatagories).Count() == ArrIntCatagories.Length
                               select Categories).ToList();
            }
            #endregion

            #region Sort
            switch (SortExpression)
            {
                case "TaskID":
                case "TaskID ASC":
                    FinalResult = FinalResult.AsEnumerable().OrderBy(p => p.TaskID).ToList();
                    break;
                case "TaskID DESC":
                    FinalResult = FinalResult.AsEnumerable().OrderByDescending(p => p.TaskID).ToList();
                    break;
                case "Status":
                case "Status ASC":
                    FinalResult = FinalResult.AsEnumerable().OrderBy(p => p.Status).ToList();
                    break;
                case "Status DESC":
                    FinalResult = FinalResult.AsEnumerable().OrderByDescending(p => p.Status).ToList();
                    break;
                case "Priority":
                case "Priority ASC":
                    FinalResult = FinalResult.AsEnumerable().OrderBy(p => p.Priority).ToList();
                    break;
                case "Priority DESC":
                    FinalResult = FinalResult.AsEnumerable().OrderByDescending(p => p.Priority).ToList();
                    break;
                case "DueDate":
                case "DueDate ASC":
                    FinalResult = FinalResult.AsEnumerable().OrderBy(p => p.DueDate).ToList();
                    break;
                case "DueDate DESC":
                    FinalResult = FinalResult.AsEnumerable().OrderByDescending(p => p.DueDate).ToList();
                    break;
                case "CreatedDate":
                case "CreatedDate ASC":
                    FinalResult = FinalResult.AsEnumerable().OrderBy(p => p.CreatedDate).ToList();
                    break;
                case "CreatedDate DESC":
                    FinalResult = FinalResult.AsEnumerable().OrderByDescending(p => p.CreatedDate).ToList();
                    break;
                case "Assigned":
                case "Assigned ASC":
                    FinalResult = FinalResult.AsEnumerable().OrderBy(p => p.Assigned).ToList();
                    break;
                case "Assigned DESC":
                    FinalResult = FinalResult.AsEnumerable().OrderByDescending(p => p.Assigned).ToList();
                    break;
                case "Description":
                case "Description ASC":
                    FinalResult = FinalResult.AsEnumerable().OrderBy(p => p.Description).ToList();
                    break;
                case "Description DESC":
                    FinalResult = FinalResult.AsEnumerable().OrderByDescending(p => p.Description).ToList();
                    break;
                case "Requester":
                case "Requester ASC":
                    FinalResult = FinalResult.AsEnumerable().OrderBy(p => p.RequesterName).ToList();
                    break;
                case "Requester DESC":
                    FinalResult = FinalResult.AsEnumerable().OrderByDescending(p => p.RequesterName).ToList();
                    break;
            }
            #endregion

            #region Paging
            int intPageSize = (objLastSearch.PageSize != null) ? Convert.ToInt32(objLastSearch.PageSize) : Convert.ToInt32(ddlPageSize.SelectedValue);
            int intCurrentPage = (Convert.ToInt32(objLastSearch.CurrentPage) == 0) ? 1 : Convert.ToInt32(objLastSearch.CurrentPage);

            //Paging
            int intTotalPages = 1;
            int intRecords = FinalResult.Count();
            if ((intRecords > 0) & (intRecords > intPageSize))
            {
                intTotalPages = (intRecords / intPageSize);

                // If there are more records add 1 to page count
                if (intRecords % intPageSize > 0)
                {
                    intTotalPages += 1;
                }

                // If Current Page is -1 then it is intended to be set to last page
                if (intCurrentPage == -1)
                {
                    intCurrentPage = intTotalPages;
                    ITILServiceDesk_LastSearch objITILServiceDesk_LastSearch = GetLastSearchCriteria();
                    objITILServiceDesk_LastSearch.CurrentPage = intCurrentPage;
                    SaveLastSearchCriteria(objITILServiceDesk_LastSearch);
                }

                // Show and hide buttons
                lnkFirst.Visible = (intCurrentPage > 1);
                lnkPrevious.Visible = (intCurrentPage > 1);
                lnkNext.Visible = (intCurrentPage != intTotalPages);
                lnkLast.Visible = (intCurrentPage != intTotalPages);
            }
            #endregion

            // If the current page is greater than the number of pages
            // reset to page one and save 
            if (intCurrentPage > intTotalPages)
            {
                intCurrentPage = 1;
                ITILServiceDesk_LastSearch objITILServiceDesk_LastSearch = GetLastSearchCriteria();
                objITILServiceDesk_LastSearch.CurrentPage = intCurrentPage;
                SaveLastSearchCriteria(objITILServiceDesk_LastSearch);

                lnkPrevious.Visible = true;
            }

            // Display Records
            lvTasks.DataSource = FinalResult.Skip((intCurrentPage - 1) * intPageSize).Take(intPageSize);
            lvTasks.DataBind();

            // Display paging panel
            pnlPaging.Visible = (intTotalPages > 1);

            // Set CurrentPage
            CurrentPage = intCurrentPage.ToString();

            #region Page number list
            List<ListPage> pageList = new List<ListPage>();

            int nStartRange = intCurrentPage > 10 ? intCurrentPage - 10 : 1;
            if (intTotalPages - nStartRange < 19)
                nStartRange = intTotalPages > 19 ? intTotalPages - 19 : 1;

            for (int nPage = nStartRange; nPage < nStartRange + 20 && nPage <= intTotalPages; nPage++)
                pageList.Add(new ListPage { PageNumber = nPage });
            PagingDataList.DataSource = pageList;
            PagingDataList.DataBind();
            #endregion
        }
        #endregion

        #region GetCurrentPage
        private int GetCurrentPage()
        {
            return Convert.ToInt32(CurrentPage);
        }
        #endregion

        #region ConvertStringToInt
        private int ConvertStringToInt(string strParameter)
        {
            return Convert.ToInt32(strParameter);
        }
        private int? ConvertStringToNullableInt(string strParameter)
        {
            return Convert.ToInt32(strParameter);
        }
        #endregion

        #region lvTasks_ItemDataBound
        protected void lvTasks_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            ListView listView = (ListView)sender;
            ListViewDataItem objListViewDataItem = (ListViewDataItem)e.Item;

            // Get instance of fields
            HyperLink lnkTaskID = (HyperLink)e.Item.FindControl("lnkTaskID");
            Label StatusLabel = (Label)e.Item.FindControl("StatusLabel");
            Label PriorityLabel = (Label)e.Item.FindControl("PriorityLabel");
            Label DueDateLabel = (Label)e.Item.FindControl("DueDateLabel");
            Label CreatedDateLabel = (Label)e.Item.FindControl("CreatedDateLabel");
            Label AssignedLabel = (Label)e.Item.FindControl("AssignedRoleIDLabel");
            Label DescriptionLabel = (Label)e.Item.FindControl("DescriptionLabel");
            Label RequesterLabel = (Label)e.Item.FindControl("RequesterUserIDLabel");
            Label RequesterNameLabel = (Label)e.Item.FindControl("RequesterNameLabel");

            // Get the data
            ExistingTasks objExistingTasks = (ExistingTasks)objListViewDataItem.DataItem;

            // Format the TaskID hyperlink
            lnkTaskID.Text = string.Format("{0}", objExistingTasks.TaskID.ToString());
            lnkTaskID.NavigateUrl = DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}", objExistingTasks.TaskID.ToString()));

            // Format DueDate
            if (objExistingTasks.DueDate != null)
            {
                DueDateLabel.Text = objExistingTasks.DueDate.Value.ToShortDateString();
                if ((objExistingTasks.DueDate < DateTime.Now) & (StatusLabel.Text == "New" || StatusLabel.Text == "Active" || StatusLabel.Text == "On Hold"))
                {
                    DueDateLabel.BackColor = System.Drawing.Color.Yellow;
                }
            }

            // Format CreatedDate
            if (objExistingTasks.CreatedDate != null)
            {
                DateTime dtCreatedDate = Convert.ToDateTime(objExistingTasks.CreatedDate.Value.ToShortDateString());
                DateTime dtNow = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                if (dtCreatedDate == dtNow)
                {
                    CreatedDateLabel.Text = objExistingTasks.CreatedDate.Value.ToLongTimeString();
                }
                else
                {
                    CreatedDateLabel.Text = objExistingTasks.CreatedDate.Value.ToShortDateString();
                }
            }

            // Format Requestor
            if (RequesterLabel.Text != "-1")
            {
                try
                {
                    RequesterNameLabel.Text = UserController.GetUser(PortalId, Convert.ToInt32(RequesterLabel.Text), false).DisplayName;
                }
                catch
                {
                    RequesterNameLabel.Text = String.Format("[User Deleted]");
                }
            }
            if (RequesterNameLabel.Text.Length > 10)
            {
                string lblRequesterNameLabel = RequesterNameLabel.Text;
                RequesterNameLabel.Text = String.Format("{0} ...", lblRequesterNameLabel.Substring(0, 10));
            }

            // Format Description
            if (DescriptionLabel.Text.Length > 10)
            {
                string lblDescriptionLabel = DescriptionLabel.Text;
                DescriptionLabel.Text = String.Format("{0} ...", lblDescriptionLabel.Substring(0, 10));

            }

            // Format Assigned
            if (AssignedLabel.Text != "-1")
            {
                RoleController objRoleController = new RoleController();
                try
                {
                    AssignedLabel.Text = String.Format("{0}", objRoleController.GetRole(Convert.ToInt32(AssignedLabel.Text), PortalId).RoleName);
                }
                catch
                {
                    AssignedLabel.Text = Localization.GetString("DeletedRole.Text", LocalResourceFile);
                }
                AssignedLabel.ToolTip = AssignedLabel.Text;

                if (AssignedLabel.Text.Length > 10)
                {
                    string lblAssignedLabel = AssignedLabel.Text;
                    AssignedLabel.Text = String.Format("{0} ...", lblAssignedLabel.Substring(0, 10));
                }
            }
            else
            {
                AssignedLabel.Text = Localization.GetString("Unassigned.Text", LocalResourceFile);
            }

        }
        #endregion

        #region lvTasks_Sorting
        protected void lvTasks_Sorting(object sender, ListViewSortEventArgs e)
        {
            if (SortDirection == "ASC")
            {
                SortDirection = "DESC";
            }
            else
            {
                SortDirection = "ASC";
            }

            SortExpression = String.Format("{0} {1}", e.SortExpression, SortDirection);

            // Check the sort direction to set the image URL accordingly.
            string imgUrl;
            if (SortDirection == "ASC")
                imgUrl = "~/DesktopModules/ServiceDesk/images/dt-arrow-up.png";
            else
                imgUrl = "~/DesktopModules/ServiceDesk/images/dt-arrow-dn.png";

            // Check which field is being sorted
            // to set the visibility of the image controls.
            Image TaskIDImage = (Image)lvTasks.FindControl("TaskIDImage");
            Image StatusImage = (Image)lvTasks.FindControl("StatusImage");
            Image PriorityImage = (Image)lvTasks.FindControl("PriorityImage");
            Image DueDateImage = (Image)lvTasks.FindControl("DueDateImage");
            Image CreatedDateImage = (Image)lvTasks.FindControl("CreatedDateImage");
            Image AssignedImage = (Image)lvTasks.FindControl("AssignedImage");
            Image DescriptionImage = (Image)lvTasks.FindControl("DescriptionImage");
            Image RequesterImage = (Image)lvTasks.FindControl("RequesterImage");

            // Set each Image to the proper direction
            TaskIDImage.ImageUrl = imgUrl;
            StatusImage.ImageUrl = imgUrl;
            PriorityImage.ImageUrl = imgUrl;
            DueDateImage.ImageUrl = imgUrl;
            CreatedDateImage.ImageUrl = imgUrl;
            AssignedImage.ImageUrl = imgUrl;
            DescriptionImage.ImageUrl = imgUrl;
            RequesterImage.ImageUrl = imgUrl;

            // Set each Image to false
            TaskIDImage.Visible = false;
            StatusImage.Visible = false;
            PriorityImage.Visible = false;
            DueDateImage.Visible = false;
            CreatedDateImage.Visible = false;
            AssignedImage.Visible = false;
            DescriptionImage.Visible = false;
            RequesterImage.Visible = false;

            switch (e.SortExpression)
            {
                case "TaskID":
                    TaskIDImage.Visible = true;
                    break;
                case "Status":
                    StatusImage.Visible = true;
                    break;
                case "Priority":
                    PriorityImage.Visible = true;
                    break;
                case "DueDate":
                    DueDateImage.Visible = true;
                    break;
                case "CreatedDate":
                    CreatedDateImage.Visible = true;
                    break;
                case "Assigned":
                    AssignedImage.Visible = true;
                    break;
                case "Description":
                    DescriptionImage.Visible = true;
                    break;
                case "Requester":
                    RequesterImage.Visible = true;
                    break;
            }

            ITILServiceDesk_LastSearch objITILServiceDesk_LastSearch = GetValuesFromSearchForm();
            // Save Search Criteria
            SaveLastSearchCriteria(objITILServiceDesk_LastSearch);
            // Execute Search
            DisplayExistingTickets(SearchCriteria);
        }
        #endregion

        #region lvTasks_ItemCommand
        protected void lvTasks_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            #region Search
            // Search
            if (e.CommandName == "Search")
            {
                ITILServiceDesk_LastSearch objITILServiceDesk_LastSearch = GetValuesFromSearchForm();
                // Save Search Criteria
                SaveLastSearchCriteria(objITILServiceDesk_LastSearch);
                // Execute Search
                DisplayExistingTickets(SearchCriteria);
            }
            #endregion

            #region EmptyDataTemplateSearch
            //EmptyDataTemplateSearch        
            if (e.CommandName == "EmptyDataTemplateSearch")
            {
                TextBox txtSearch = (TextBox)e.Item.FindControl("txtSearch");
                DropDownList ddlStatus = (DropDownList)e.Item.FindControl("ddlStatus");
                DropDownList ddlPriority = (DropDownList)e.Item.FindControl("ddlPriority");
                DropDownList ddlAssigned = (DropDownList)e.Item.FindControl("ddlAssigned");
                TextBox txtDue = (TextBox)e.Item.FindControl("txtDue");
                TextBox txtCreated = (TextBox)e.Item.FindControl("txtCreated");

                // Use an ExistingTasks object to pass the values to the Search method
                ITILServiceDesk_LastSearch objITILServiceDesk_LastSearch = new ITILServiceDesk_LastSearch();
                objITILServiceDesk_LastSearch.SearchText = (txtSearch.Text.Trim().Length == 0) ? null : txtSearch.Text.Trim();
                objITILServiceDesk_LastSearch.Status = (ddlStatus.SelectedValue == "*All*") ? null : ddlStatus.SelectedValue;
                objITILServiceDesk_LastSearch.AssignedRoleID = (ddlAssigned.SelectedValue == "-2") ? null : (int?)Convert.ToInt32(ddlAssigned.SelectedValue);
                objITILServiceDesk_LastSearch.Priority = (ddlPriority.SelectedValue == "*All*") ? null : ddlPriority.SelectedValue;

                // Created Date
                if (txtCreated.Text.Trim().Length > 4)
                {
                    try
                    {
                        DateTime dtCreated = Convert.ToDateTime(txtCreated.Text.Trim());
                        objITILServiceDesk_LastSearch.CreatedDate = dtCreated.AddDays(-1);
                    }
                    catch
                    {
                        txtCreated.Text = "";
                    }
                }
                else
                {
                    txtCreated.Text = "";
                }

                // Due Date
                if (txtDue.Text.Trim().Length > 4)
                {
                    try
                    {
                        DateTime dtDue = Convert.ToDateTime(txtDue.Text.Trim());
                        objITILServiceDesk_LastSearch.DueDate = dtDue.AddDays(-1);
                    }
                    catch
                    {
                        txtDue.Text = "";
                    }
                }
                else
                {
                    txtDue.Text = "";
                }

                // Get Category Tags
                string strCategories = GetTagsTreeExistingTasks();
                if (strCategories.Length > 1)
                {
                    objITILServiceDesk_LastSearch.Categories = strCategories;
                }

                // Current Page
                objITILServiceDesk_LastSearch.CurrentPage = GetCurrentPage();

                // Page Size
                objITILServiceDesk_LastSearch.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);

                // Save Search Criteria
                SaveLastSearchCriteria(objITILServiceDesk_LastSearch);
                // Execute Search
                DisplayExistingTickets(SearchCriteria);
            }
            #endregion

        }
        #endregion

        // Set controls to the Last Search criteria

        #region lvTasks_DataBound
        protected void lvTasks_DataBound(object sender, EventArgs e)
        {
            if (SearchCriteria != null)
            {
                TextBox txtSearch = new TextBox();
                DropDownList ddlStatus = new DropDownList();
                DropDownList ddlPriority = new DropDownList();
                DropDownList ddlAssigned = new DropDownList();
                TextBox txtDue = new TextBox();
                TextBox txtCreated = new TextBox();

                // Get an instance to the Search controls
                if (lvTasks.Items.Count == 0)
                {
                    // Empty Data Template
                    ListViewItem Ctrl0 = (ListViewItem)lvTasks.FindControl("Ctrl0");
                    HtmlTable EmptyDataTemplateTable = (HtmlTable)Ctrl0.FindControl("EmptyDataTemplateTable");

                    txtSearch = (TextBox)EmptyDataTemplateTable.FindControl("txtSearch");
                    ddlStatus = (DropDownList)EmptyDataTemplateTable.FindControl("ddlStatus");
                    ddlPriority = (DropDownList)EmptyDataTemplateTable.FindControl("ddlPriority");
                    txtDue = (TextBox)EmptyDataTemplateTable.FindControl("txtDue");
                    txtCreated = (TextBox)EmptyDataTemplateTable.FindControl("txtCreated");
                    ddlAssigned = (DropDownList)EmptyDataTemplateTable.FindControl("ddlAssigned");
                }
                else
                {
                    // Normal results template
                    txtSearch = (TextBox)lvTasks.FindControl("txtSearch");
                    ddlStatus = (DropDownList)lvTasks.FindControl("ddlStatus");
                    ddlPriority = (DropDownList)lvTasks.FindControl("ddlPriority");
                    txtDue = (TextBox)lvTasks.FindControl("txtDue");
                    txtCreated = (TextBox)lvTasks.FindControl("txtCreated");
                    ddlAssigned = (DropDownList)lvTasks.FindControl("ddlAssigned");
                }

                ITILServiceDesk_LastSearch objITILServiceDesk_LastSearch = (ITILServiceDesk_LastSearch)SearchCriteria;

                if (objITILServiceDesk_LastSearch.SearchText != null)
                {
                    txtSearch.Text = objITILServiceDesk_LastSearch.SearchText;
                }

                if (objITILServiceDesk_LastSearch.Status != null)
                {
                    ddlStatus.SelectedValue = objITILServiceDesk_LastSearch.Status;
                }

                if (objITILServiceDesk_LastSearch.Priority != null)
                {
                    ddlPriority.SelectedValue = objITILServiceDesk_LastSearch.Priority;
                }

                if (objITILServiceDesk_LastSearch.DueDate != null)
                {
                    txtDue.Text = objITILServiceDesk_LastSearch.DueDate.Value.AddDays(1).ToShortDateString();
                }

                if (objITILServiceDesk_LastSearch.CreatedDate != null)
                {
                    txtCreated.Text = objITILServiceDesk_LastSearch.CreatedDate.Value.AddDays(1).ToShortDateString();
                }

                // Page Size
                if (objITILServiceDesk_LastSearch.PageSize != null)
                {
                    ddlPageSize.SelectedValue = objITILServiceDesk_LastSearch.PageSize.ToString();
                }

                // Load Dropdown
                ddlAssigned.Items.Clear();

                RoleController objRoleController = new RoleController();
                ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

                List<ITILServiceDesk_Role> colITILServiceDesk_Roles = (from ITILServiceDesk_Roles in objServiceDeskDALDataContext.ITILServiceDesk_Roles
                                                                 where ITILServiceDesk_Roles.PortalID == PortalId
                                                                 select ITILServiceDesk_Roles).ToList();

                // Create a ListItemCollection to hold the Roles 
                ListItemCollection colListItemCollection = new ListItemCollection();

                // Add All
                ListItem AllRoleListItem = new ListItem();
                AllRoleListItem.Text = Localization.GetString("ddlAssignedAll.Text", LocalResourceFile);
                AllRoleListItem.Value = "-2";
                if (objITILServiceDesk_LastSearch.AssignedRoleID != null)
                {
                    if (objITILServiceDesk_LastSearch.AssignedRoleID == -2)
                    {
                        AllRoleListItem.Selected = true;
                    }
                }
                ddlAssigned.Items.Add(AllRoleListItem);

                // Add the Roles to the List
                foreach (ITILServiceDesk_Role objITILServiceDesk_Role in colITILServiceDesk_Roles)
                {
                    try
                    {
                        RoleInfo objRoleInfo = objRoleController.GetRole(Convert.ToInt32(objITILServiceDesk_Role.RoleID), PortalId);

                        ListItem RoleListItem = new ListItem();
                        RoleListItem.Text = objRoleInfo.RoleName;
                        RoleListItem.Value = objITILServiceDesk_Role.RoleID.ToString();

                        if (objITILServiceDesk_LastSearch.AssignedRoleID != null)
                        {
                            if (objITILServiceDesk_Role.RoleID == objITILServiceDesk_LastSearch.AssignedRoleID)
                            {
                                RoleListItem.Selected = true;
                            }
                        }

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
                if (objITILServiceDesk_LastSearch.AssignedRoleID != null)
                {
                    if (objITILServiceDesk_LastSearch.AssignedRoleID == -1)
                    {
                        UnassignedRoleListItem.Selected = true;
                    }
                }
                ddlAssigned.Items.Add(UnassignedRoleListItem);
            }
        }
        #endregion

        #region GetValuesFromSearchForm
        private ITILServiceDesk_LastSearch GetValuesFromSearchForm()
        {
            TextBox txtSearch = (TextBox)lvTasks.FindControl("txtSearch");
            DropDownList ddlStatus = (DropDownList)lvTasks.FindControl("ddlStatus");
            DropDownList ddlPriority = (DropDownList)lvTasks.FindControl("ddlPriority");
            DropDownList ddlAssigned = (DropDownList)lvTasks.FindControl("ddlAssigned");
            TextBox txtDue = (TextBox)lvTasks.FindControl("txtDue");
            TextBox txtCreated = (TextBox)lvTasks.FindControl("txtCreated");

            // Use an ExistingTasks object to pass the values to the Search method
            ITILServiceDesk_LastSearch objITILServiceDesk_LastSearch = new ITILServiceDesk_LastSearch();
            objITILServiceDesk_LastSearch.SearchText = (txtSearch.Text.Trim().Length == 0) ? null : txtSearch.Text.Trim();
            objITILServiceDesk_LastSearch.Status = (ddlStatus.SelectedValue == "*All*") ? null : ddlStatus.SelectedValue;
            objITILServiceDesk_LastSearch.AssignedRoleID = (ddlAssigned.SelectedValue == "-2") ? null : (int?)Convert.ToInt32(ddlAssigned.SelectedValue);
            objITILServiceDesk_LastSearch.Priority = (ddlPriority.SelectedValue == "*All*") ? null : ddlPriority.SelectedValue;

            // Created Date
            if (txtCreated.Text.Trim().Length > 4)
            {
                try
                {
                    DateTime dtCreated = Convert.ToDateTime(txtCreated.Text.Trim());
                    objITILServiceDesk_LastSearch.CreatedDate = dtCreated.AddDays(-1);
                }
                catch
                {
                    txtCreated.Text = "";
                }
            }
            else
            {
                txtCreated.Text = "";
            }

            // Due Date
            if (txtDue.Text.Trim().Length > 4)
            {
                try
                {
                    DateTime dtDue = Convert.ToDateTime(txtDue.Text.Trim());
                    objITILServiceDesk_LastSearch.DueDate = dtDue.AddDays(-1);
                }
                catch
                {
                    txtDue.Text = "";
                }
            }
            else
            {
                txtDue.Text = "";
            }

            // Get Category Tags
            string strCategories = GetTagsTreeExistingTasks();
            if (strCategories.Length > 0)
            {
                objITILServiceDesk_LastSearch.Categories = strCategories;
            }

            // Current Page
            objITILServiceDesk_LastSearch.CurrentPage = GetCurrentPage();

            // Page Size
            objITILServiceDesk_LastSearch.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);

            return objITILServiceDesk_LastSearch;
        }
        #endregion

        #region GetTagsTreeExistingTasks
        private string GetTagsTreeExistingTasks()
        {
            List<string> colSelectedCategories = new List<string>();

            try
            {
                TreeView objTreeView = (TreeView)TagsTreeExistingTasks.FindControl("tvCategories");
                if (objTreeView.CheckedNodes.Count > 0)
                {
                    // Iterate through the CheckedNodes collection 
                    foreach (TreeNode node in objTreeView.CheckedNodes)
                    {
                        colSelectedCategories.Add(node.Value);
                    }
                }

                string[] arrSelectedCategories = colSelectedCategories.ToArray<string>();
                string strSelectedCategories = String.Join(",", arrSelectedCategories);

                return strSelectedCategories.Substring(0, 2000);
            }
            catch
            {
                return "";
            }
        }
        #endregion

        // Datasource for Assigned Role drop down

        #region LDSAssignedRoleID_Selecting
        protected void LDSAssignedRoleID_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            List<AssignedRoles> resultcolAssignedRoles = new List<AssignedRoles>();
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            List<AssignedRoles> colAssignedRoles = (from ITILServiceDesk_Tasks in objServiceDeskDALDataContext.ITILServiceDesk_Tasks
                                                    where ITILServiceDesk_Tasks.AssignedRoleID > -1
                                                    group ITILServiceDesk_Tasks by ITILServiceDesk_Tasks.AssignedRoleID into AssignedRole
                                                    select new AssignedRoles
                                                    {
                                                        AssignedRoleID = GetRolebyID(AssignedRole.Key),
                                                        Key = AssignedRole.Key.ToString()
                                                    }).ToList();

            AssignedRoles objAssignedRolesAll = new AssignedRoles();
            objAssignedRolesAll.AssignedRoleID = "*All*";
            objAssignedRolesAll.Key = "-2";
            resultcolAssignedRoles.Add(objAssignedRolesAll);

            AssignedRoles objAssignedRolesUnassigned = new AssignedRoles();
            objAssignedRolesUnassigned.AssignedRoleID = "Unassigned";
            objAssignedRolesUnassigned.Key = "-1";
            resultcolAssignedRoles.Add(objAssignedRolesUnassigned);
            resultcolAssignedRoles.AddRange(colAssignedRoles);

            e.Result = resultcolAssignedRoles;
        }
        #endregion

        #region GetRolebyID
        private string GetRolebyID(int RoleID)
        {
            string strRoleName = "Unassigned";
            if (RoleID > -1)
            {
                RoleController objRoleController = new RoleController();
                strRoleName = objRoleController.GetRole(RoleID, PortalId).RoleName;
            }

            return strRoleName;
        }
        #endregion

        // Paging

        #region lnkNext_Click
        protected void lnkNext_Click(object sender, EventArgs e)
        {
            ITILServiceDesk_LastSearch objITILServiceDesk_LastSearch = GetLastSearchCriteria();
            int intCurrentPage = Convert.ToInt32(objITILServiceDesk_LastSearch.CurrentPage ?? 1);
            intCurrentPage++;
            objITILServiceDesk_LastSearch.CurrentPage = intCurrentPage;
            SaveLastSearchCriteria(objITILServiceDesk_LastSearch);
            DisplayExistingTickets(SearchCriteria);
        }
        #endregion

        #region lnkPrevious_Click
        protected void lnkPrevious_Click(object sender, EventArgs e)
        {
            ITILServiceDesk_LastSearch objITILServiceDesk_LastSearch = GetLastSearchCriteria();
            int intCurrentPage = Convert.ToInt32(objITILServiceDesk_LastSearch.CurrentPage ?? 2);
            intCurrentPage--;
            objITILServiceDesk_LastSearch.CurrentPage = intCurrentPage;
            SaveLastSearchCriteria(objITILServiceDesk_LastSearch);
            DisplayExistingTickets(SearchCriteria);
        }
        #endregion

        #region lnkFirst_Click
        protected void lnkFirst_Click(object sender, EventArgs e)
        {
            ITILServiceDesk_LastSearch objITILServiceDesk_LastSearch = GetLastSearchCriteria();
            int intCurrentPage = Convert.ToInt32(objITILServiceDesk_LastSearch.CurrentPage ?? 2);
            intCurrentPage = 1;
            objITILServiceDesk_LastSearch.CurrentPage = intCurrentPage;
            SaveLastSearchCriteria(objITILServiceDesk_LastSearch);
            DisplayExistingTickets(SearchCriteria);
        }
        #endregion

        #region lnkLast_Click
        protected void lnkLast_Click(object sender, EventArgs e)
        {
            ITILServiceDesk_LastSearch objITILServiceDesk_LastSearch = GetLastSearchCriteria();
            int intCurrentPage = Convert.ToInt32(objITILServiceDesk_LastSearch.CurrentPage ?? 1);
            intCurrentPage = -1;
            objITILServiceDesk_LastSearch.CurrentPage = intCurrentPage;
            SaveLastSearchCriteria(objITILServiceDesk_LastSearch);
            DisplayExistingTickets(SearchCriteria);
        }
        #endregion

        #region ddlPageSize_SelectedIndexChanged
        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            ITILServiceDesk_LastSearch objITILServiceDesk_LastSearch = GetLastSearchCriteria();
            objITILServiceDesk_LastSearch.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            SaveLastSearchCriteria(objITILServiceDesk_LastSearch);
            DisplayExistingTickets(SearchCriteria);
        }
        #endregion

        #region lnkPage_Click
        protected void lnkPage_Click(object sender, EventArgs e)
        {
            LinkButton lnkButton = sender as LinkButton;
            CurrentPage = lnkButton.CommandArgument;

            ITILServiceDesk_LastSearch objITILServiceDesk_LastSearch = GetLastSearchCriteria();
            objITILServiceDesk_LastSearch.CurrentPage = Convert.ToInt32(lnkButton.CommandArgument);
            SaveLastSearchCriteria(objITILServiceDesk_LastSearch);
            DisplayExistingTickets(SearchCriteria);
        }
        #endregion

        #region PagingDataList_ItemDataBound
        protected void PagingDataList_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            LinkButton pageLink = e.Item.FindControl("lnkPage") as LinkButton;
            if (Convert.ToInt32(pageLink.CommandArgument) == GetCurrentPage())
                pageLink.Font.Underline = false;
            else
                pageLink.Font.Underline = true;
        }
        #endregion


        // ITIL customization new user search for new ticket section
        #region PagingDataList_ItemDataBound



        private void SearchList(int portalId)
        {
            Session["portalId"] = portalId;

            //RadCombSearh.AllowCustomText = false;
            //RadCombSearh.MarkFirstMatch = false;
            //RadCombSearh.AutoCompleteSeparator = ";";


        }



        #endregion


    }
}