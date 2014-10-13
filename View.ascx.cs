//
// http://ServiceDesk.com
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

    public partial class View : DotNetNuke.Entities.Modules.PortalModuleBase
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
        public ServiceDesk_LastSearch SearchCriteria
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
        private ServiceDesk_LastSearch GetLastSearchCriteria()
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            ServiceDesk_LastSearch objServiceDesk_LastSearch = (from ServiceDesk_LastSearches in objServiceDeskDALDataContext.ServiceDesk_LastSearches
                                                                  where ServiceDesk_LastSearches.PortalID == PortalId
                                                                  where ServiceDesk_LastSearches.UserID == UserId
                                                                  select ServiceDesk_LastSearches).FirstOrDefault();

            if (objServiceDesk_LastSearch == null)
            {
                ServiceDesk_LastSearch InsertServiceDesk_LastSearch = new ServiceDesk_LastSearch();
                InsertServiceDesk_LastSearch.UserID = UserId;
                InsertServiceDesk_LastSearch.PortalID = PortalId;
                objServiceDeskDALDataContext.ServiceDesk_LastSearches.InsertOnSubmit(InsertServiceDesk_LastSearch);

                // Only save is user is logged in
                if (UserId > -1)
                {
                    objServiceDeskDALDataContext.SubmitChanges();
                }

                return InsertServiceDesk_LastSearch;
            }
            else
            {
                return objServiceDesk_LastSearch;
            }
        }
        #endregion

        #region SaveLastSearchCriteria
        private void SaveLastSearchCriteria(ServiceDesk_LastSearch UpdateServiceDesk_LastSearch)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            ServiceDesk_LastSearch objServiceDesk_LastSearch = (from ServiceDesk_LastSearches in objServiceDeskDALDataContext.ServiceDesk_LastSearches
                                                                  where ServiceDesk_LastSearches.PortalID == PortalId
                                                                  where ServiceDesk_LastSearches.UserID == UserId
                                                                  select ServiceDesk_LastSearches).FirstOrDefault();

            if (objServiceDesk_LastSearch == null)
            {
                objServiceDesk_LastSearch = new ServiceDesk_LastSearch();
                objServiceDesk_LastSearch.UserID = UserId;
                objServiceDesk_LastSearch.PortalID = PortalId;
                objServiceDeskDALDataContext.ServiceDesk_LastSearches.InsertOnSubmit(objServiceDesk_LastSearch);
                objServiceDeskDALDataContext.SubmitChanges();
            }

            objServiceDesk_LastSearch.AssignedRoleID = UpdateServiceDesk_LastSearch.AssignedRoleID;
            objServiceDesk_LastSearch.Categories = UpdateServiceDesk_LastSearch.Categories;
            objServiceDesk_LastSearch.CreatedDate = UpdateServiceDesk_LastSearch.CreatedDate;
            objServiceDesk_LastSearch.SearchText = UpdateServiceDesk_LastSearch.SearchText;
            objServiceDesk_LastSearch.DueDate = UpdateServiceDesk_LastSearch.DueDate;
            objServiceDesk_LastSearch.Priority = UpdateServiceDesk_LastSearch.Priority;
            objServiceDesk_LastSearch.Status = UpdateServiceDesk_LastSearch.Status;
            objServiceDesk_LastSearch.CurrentPage = UpdateServiceDesk_LastSearch.CurrentPage;
            objServiceDesk_LastSearch.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);

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
                    ddlAssignedAdmin.Items.Add(RoleListItem);
                }
                catch
                {
                    // Role no longer exists in Portal
                    ListItem RoleListItem = new ListItem();
                    RoleListItem.Text = Localization.GetString("DeletedRole.Text", LocalResourceFile);
                    RoleListItem.Value = objServiceDesk_Role.RoleID.ToString();
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

        #region GetUploadPermission
        private string GetUploadPermission()
        {
            List<ServiceDesk_Setting> objServiceDesk_Settings = GetSettings();
            ServiceDesk_Setting objServiceDesk_Setting = objServiceDesk_Settings.Where(x => x.SettingName == "UploadPermission").FirstOrDefault();

            string strUploadPermission = "All";
            if (objServiceDesk_Setting != null)
            {
                strUploadPermission = objServiceDesk_Setting.SettingValue;
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
                objServiceDesk_Setting2.SettingValue = Server.MapPath("~/DesktopModules/ITILServiceDesk/Upload");

                objServiceDeskDALDataContext.ServiceDesk_Settings.InsertOnSubmit(objServiceDesk_Setting2);
                objServiceDeskDALDataContext.SubmitChanges();

                colServiceDesk_Setting = (from ServiceDesk_Settings in objServiceDeskDALDataContext.ServiceDesk_Settings
                                           where ServiceDesk_Settings.PortalID == PortalId
                                           select ServiceDesk_Settings).ToList();
            }

            // Upload Permission
            ServiceDesk_Setting UploadPermissionServiceDesk_Setting = (from ServiceDesk_Settings in objServiceDeskDALDataContext.ServiceDesk_Settings
                                                                         where ServiceDesk_Settings.PortalID == PortalId
                                                                         where ServiceDesk_Settings.SettingName == "UploadPermission"
                                                                         select ServiceDesk_Settings).FirstOrDefault();

            if (UploadPermissionServiceDesk_Setting != null)
            {
                // Add to collection
                colServiceDesk_Setting.Add(UploadPermissionServiceDesk_Setting);
            }
            else
            {
                // Add Default value
                ServiceDesk_Setting objServiceDesk_Setting = new ServiceDesk_Setting();
                objServiceDesk_Setting.SettingName = "UploadPermission";
                objServiceDesk_Setting.SettingValue = "All";
                objServiceDesk_Setting.PortalID = PortalId;
                objServiceDeskDALDataContext.ServiceDesk_Settings.InsertOnSubmit(objServiceDesk_Setting);
                objServiceDeskDALDataContext.SubmitChanges();

                // Add to collection
                colServiceDesk_Setting.Add(objServiceDesk_Setting);
            }

            return colServiceDesk_Setting;
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
            ServiceDesk_LastSearch ExistingServiceDesk_LastSearch = GetLastSearchCriteria();
            ExistingServiceDesk_LastSearch.AssignedRoleID = null;
            ExistingServiceDesk_LastSearch.Categories = null;
            ExistingServiceDesk_LastSearch.CreatedDate = null;
            ExistingServiceDesk_LastSearch.SearchText = null;
            ExistingServiceDesk_LastSearch.DueDate = null;
            ExistingServiceDesk_LastSearch.Priority = null;
            ExistingServiceDesk_LastSearch.Status = null;
            ExistingServiceDesk_LastSearch.CurrentPage = 1;
            ExistingServiceDesk_LastSearch.PageSize = 25;

            ddlPageSize.SelectedValue = "25";
            CurrentPage = "1";

            SaveLastSearchCriteria(ExistingServiceDesk_LastSearch);

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
            ServiceDesk_Task objServiceDesk_Task = new ServiceDesk_Task();

            objServiceDesk_Task.Status = "New";
            objServiceDesk_Task.CreatedDate = DateTime.Now;
            objServiceDesk_Task.Description = txtDescription.Text;
            objServiceDesk_Task.PortalID = PortalId;
            objServiceDesk_Task.Priority = ddlPriority.SelectedValue;
            objServiceDesk_Task.RequesterPhone = txtPhone.Text;
            objServiceDesk_Task.AssignedRoleID = -1;
            objServiceDesk_Task.TicketPassword = GetRandomPassword();

            if (Convert.ToInt32(txtUserID.Text) == -1)
            {
                // User not logged in
                objServiceDesk_Task.RequesterEmail = txtEmail.Text;
                objServiceDesk_Task.RequesterName = txtName.Text;
                objServiceDesk_Task.RequesterUserID = -1;
            }
            else
            {
                // User logged in
                objServiceDesk_Task.RequesterUserID = Convert.ToInt32(txtUserID.Text);
                objServiceDesk_Task.RequesterName = UserController.GetUser(PortalId, Convert.ToInt32(txtUserID.Text), false).DisplayName;
            }

            if (txtDueDate.Text.Trim().Length > 1)
            {
                objServiceDesk_Task.DueDate = Convert.ToDateTime(txtDueDate.Text.Trim());
            }

            // If Admin panel is visible this is an admin
            // Save the Status and Assignment
            if (pnlAdminTicketStatus.Visible == true)
            {
                objServiceDesk_Task.AssignedRoleID = Convert.ToInt32(ddlAssignedAdmin.SelectedValue);
                objServiceDesk_Task.Status = ddlStatusAdmin.SelectedValue;
            }

            objServiceDeskDALDataContext.ServiceDesk_Tasks.InsertOnSubmit(objServiceDesk_Task);
            objServiceDeskDALDataContext.SubmitChanges();

            // Save Task Details
            ServiceDesk_TaskDetail objServiceDesk_TaskDetail = new ServiceDesk_TaskDetail();

            if ((txtDetails.Text.Trim().Length > 0) || (TicketFileUpload.HasFile))
            {
                objServiceDesk_TaskDetail.TaskID = objServiceDesk_Task.TaskID;
                objServiceDesk_TaskDetail.Description = txtDetails.Text;
                objServiceDesk_TaskDetail.DetailType = "Comment-Visible";
                objServiceDesk_TaskDetail.InsertDate = DateTime.Now;

                if (Convert.ToInt32(txtUserID.Text) == -1)
                {
                    // User not logged in
                    objServiceDesk_TaskDetail.UserID = -1;
                }
                else
                {
                    // User logged in
                    objServiceDesk_TaskDetail.UserID = Convert.ToInt32(txtUserID.Text);
                }

                objServiceDeskDALDataContext.ServiceDesk_TaskDetails.InsertOnSubmit(objServiceDesk_TaskDetail);
                objServiceDeskDALDataContext.SubmitChanges();

                // Upload the File
                if (TicketFileUpload.HasFile)
                {
                    UploadFile(objServiceDesk_TaskDetail.DetailID);
                    // Insert Log
                    Log.InsertLog(objServiceDesk_Task.TaskID, UserId, String.Format("{0} uploaded file '{1}'.", GetUserName(), TicketFileUpload.FileName));
                }
            }

            // Insert Log
            Log.InsertLog(objServiceDesk_Task.TaskID, UserId, String.Format("{0} created ticket.", GetUserName()));

            return objServiceDesk_Task.TaskID;
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
                    ServiceDesk_TaskCategory objServiceDesk_TaskCategory = new ServiceDesk_TaskCategory();

                    objServiceDesk_TaskCategory.TaskID = intTaskID;
                    objServiceDesk_TaskCategory.CategoryID = Convert.ToInt32(node.Value);

                    objServiceDeskDALDataContext.ServiceDesk_TaskCategories.InsertOnSubmit(objServiceDesk_TaskCategory);
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

            string strUploadefFilesPath = (from ServiceDesk_Settings in objServiceDeskDALDataContext.ServiceDesk_Settings
                                           where ServiceDesk_Settings.PortalID == PortalId
                                           where ServiceDesk_Settings.SettingName == "UploadefFilesPath"
                                           select ServiceDesk_Settings).FirstOrDefault().SettingValue;

            EnsureDirectory(new System.IO.DirectoryInfo(strUploadefFilesPath));
            string strfilename = Convert.ToString(intDetailID) + "_" + GetRandomPassword() + Path.GetExtension(TicketFileUpload.FileName).ToLower();
            strUploadefFilesPath = strUploadefFilesPath + @"\" + strfilename;
            TicketFileUpload.SaveAs(strUploadefFilesPath);

            ServiceDesk_Attachment objServiceDesk_Attachment = new ServiceDesk_Attachment();
            objServiceDesk_Attachment.DetailID = intDetailID;
            objServiceDesk_Attachment.FileName = strfilename;
            objServiceDesk_Attachment.OriginalFileName = TicketFileUpload.FileName;
            objServiceDesk_Attachment.AttachmentPath = strUploadefFilesPath;
            objServiceDesk_Attachment.UserID = UserId;

            objServiceDeskDALDataContext.ServiceDesk_Attachments.InsertOnSubmit(objServiceDesk_Attachment);
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

                ServiceDesk_Task objServiceDesk_Tasks = (from ServiceDesk_Tasks in objServiceDeskDALDataContext.ServiceDesk_Tasks
                                                           where ServiceDesk_Tasks.TaskID == Convert.ToInt32(TaskID)
                                                           select ServiceDesk_Tasks).FirstOrDefault();

                string strPasswordLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}&TP={1}", TaskID, objServiceDesk_Tasks.TicketPassword)), PortalSettings.PortalAlias.HTTPAlias);
                string strLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}", TaskID)), PortalSettings.PortalAlias.HTTPAlias);
                string strSubject = String.Format(Localization.GetString("NewHelpDeskTicketCreated.Text", LocalResourceFile), TaskID);
                string strBody = "";

                if (Convert.ToInt32(txtUserID.Text) != UserId || UserId == -1)
                {
                    //Anonymous or login user submit ticket for another user
                    NotifyRequesterSubmitTicket(TaskID.ToString(), objServiceDesk_Tasks); //ITIL Customization - removed strPasswordLinkURL

                    // Get Admin Role
                    string strAdminRoleID = GetAdminRole();
                    // User is an Administrator
                    if (UserInfo.IsInRole(strAdminRoleID) || UserInfo.IsInRole(PortalSettings.AdministratorRoleName) || UserInfo.IsSuperUser)
                    {
                        // If Ticket is assigned to any group other than unassigned notify them
                        if (Convert.ToInt32(ddlAssignedAdmin.SelectedValue) > -1)
                        {
                            NotifyGroupAssignTicket(TaskID, objServiceDesk_Tasks); //ITIL Customization - removed strPasswordLinkURL
                        }
                    }
                    else
                    {
                        // This is not an Admin so Notify the Admins
                        // ITIL Customization - email notifies the Admins of a new ticket and also includes the ticket details 
                        NotifyAdminSubmitTicket(TaskID, objServiceDesk_Tasks); //ITIL Customization - removed strPasswordLinkURL
                    }
                }
                else
                {
                    // A normal ticket has been created

                    // ITIL Customization - email notifies the Admins of a new ticket and also includes the ticket details
                    NotifyAdminSubmitTicket(TaskID, objServiceDesk_Tasks); //ITIL Customization - removed strPasswordLinkURL
                    // ITIL Customization - email notify login user who entered ticket.  The email contains password protected link
                    NotifyRequesterSubmitTicket(TaskID.ToString(), objServiceDesk_Tasks); //ITIL Customization - removed strPasswordLinkURL
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
        private void NotifyRequesterSubmitTicket(string TaskID, ServiceDesk_Task objServiceDesk_Tasks)  //ITIL Customization - removed strPasswordLinkURL
        {
            string strDomainServerUrl = DotNetNuke.Common.Globals.AddHTTP(Request.Url.Host);  // ITIL Customization - get DomainServerUrl for use in Utility.FixURLLink
            string strPasswordLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}&TP={1}", TaskID, objServiceDesk_Tasks.TicketPassword)), strDomainServerUrl);
            string strSubject = String.Format(Localization.GetString("NewHelpDeskTicketCreated.Text", LocalResourceFile), TaskID);
            string strBody = Localization.GetString("HTMLTicketEmailRequester.Text", LocalResourceFile);
            strBody = Utility.ReplaceTicketToken(strBody, strPasswordLinkUrl, objServiceDesk_Tasks);

            string strEmail = txtEmail.Text;

            // If userId is not -1 then get the Email
            if (objServiceDesk_Tasks.RequesterUserID > -1)
            {
                strEmail = UserController.GetUser(PortalId, objServiceDesk_Tasks.RequesterUserID, false).Email;
            }

            DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, strEmail, "", strSubject, strBody, "", "HTML", "", "", "", "");


        }
        #endregion

        // ITIL Customization - email notifies services desk admins of the new submitted ticket.  The email contains password protected link

        #region NotifyAdminSubmitTicket
        private void NotifyAdminSubmitTicket(string TaskID, ServiceDesk_Task objServiceDesk_Tasks)  //ITIL Customization - removed strPasswordLinkURL
        {
            // This is not an Admin so Notify the Admins

            // ITIL Customization - email notifies the Admins of a new ticket and also includes the ticket details 
            string strDomainServerUrl = DotNetNuke.Common.Globals.AddHTTP(Request.Url.Host);  // ITIL Customization - get DomainServerUrl for use in Utility.FixURLLink
            string strPasswordLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}&TP={1}", TaskID, objServiceDesk_Tasks.TicketPassword)), strDomainServerUrl);

            string strSubject = String.Format(Localization.GetString("NewHelpDeskTicketCreatedAt.Text", LocalResourceFile), TaskID, strDomainServerUrl);
            string strBody = Localization.GetString("HTMLTicketEmailAdmin.Text", LocalResourceFile);
            strBody = Utility.ReplaceTicketToken(strBody, strPasswordLinkUrl, objServiceDesk_Tasks);

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
        private void NotifyGroupAssignTicket(string TaskID, ServiceDesk_Task objServiceDesk_Tasks)  //ITIL Customization - removed strPasswordLinkURL
        {
            try
            {
                // ITIL Customization - email notifies the Admins of the new ticket and also includes the ticket details
                RoleController objRoleController = new RoleController();
                string strAssignedRole = String.Format("{0}", objRoleController.GetRole(Convert.ToInt32(ddlAssignedAdmin.SelectedValue), PortalId).RoleName);

                string strDomainServerUrl = DotNetNuke.Common.Globals.AddHTTP(Request.Url.Host);  // ITIL Customization - get DomainServerUrl for use in Utility.FixURLLink
                string strPasswordLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleId.ToString(), String.Format(@"&TaskID={0}&TP={1}", TaskID, objServiceDesk_Tasks.TicketPassword)), strDomainServerUrl);

                string strSubject = String.Format(Localization.GetString("HelpDeskTicketAtHasBeenAssigned.Text", LocalResourceFile), TaskID, strAssignedRole);
                string strBody = Localization.GetString("HTMLTicketEmailAssignee.Text", LocalResourceFile);
                strBody = Utility.ReplaceTicketToken(strBody, strPasswordLinkUrl, objServiceDesk_Tasks);

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
        private void DisplayExistingTickets(ServiceDesk_LastSearch objLastSearch)
        {
            string[] UsersRoles = UserInfo.Roles;
            List<int> UsersRoleIDs = new List<int>();
            string strSearchText = (objLastSearch.SearchText == null) ? "" : objLastSearch.SearchText;

            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            IQueryable<ExistingTasks> result = from ServiceDesk_Tasks in objServiceDeskDALDataContext.ServiceDesk_Tasks
                                               where ServiceDesk_Tasks.PortalID == PortalId
                                               orderby ServiceDesk_Tasks.CreatedDate descending
                                               select new ExistingTasks
                                               {
                                                   TaskID = ServiceDesk_Tasks.TaskID,
                                                   Status = ServiceDesk_Tasks.Status,
                                                   Priority = ServiceDesk_Tasks.Priority,
                                                   DueDate = ServiceDesk_Tasks.DueDate,
                                                   CreatedDate = ServiceDesk_Tasks.CreatedDate,
                                                   Assigned = ServiceDesk_Tasks.AssignedRoleID.ToString(),
                                                   Description = ServiceDesk_Tasks.Description,
                                                   Requester = ServiceDesk_Tasks.RequesterUserID.ToString(),
                                                   RequesterName = ServiceDesk_Tasks.RequesterName
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
                          join details in objServiceDeskDALDataContext.ServiceDesk_TaskDetails
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
                               where ((from ServiceDesk_TaskCategories in objServiceDeskDALDataContext.ServiceDesk_TaskCategories
                                       where ServiceDesk_TaskCategories.TaskID == Categories.TaskID
                                       select ServiceDesk_TaskCategories.CategoryID).ToArray<int>()).Intersect(ArrIntCatagories).Count() == ArrIntCatagories.Length
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
                    ServiceDesk_LastSearch objServiceDesk_LastSearch = GetLastSearchCriteria();
                    objServiceDesk_LastSearch.CurrentPage = intCurrentPage;
                    SaveLastSearchCriteria(objServiceDesk_LastSearch);
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
                ServiceDesk_LastSearch objServiceDesk_LastSearch = GetLastSearchCriteria();
                objServiceDesk_LastSearch.CurrentPage = intCurrentPage;
                SaveLastSearchCriteria(objServiceDesk_LastSearch);

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
                imgUrl = "~/DesktopModules/ITILServiceDesk/images/dt-arrow-up.png";
            else
                imgUrl = "~/DesktopModules/ITILServiceDesk/images/dt-arrow-dn.png";

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

            ServiceDesk_LastSearch objServiceDesk_LastSearch = GetValuesFromSearchForm();
            // Save Search Criteria
            SaveLastSearchCriteria(objServiceDesk_LastSearch);
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
                ServiceDesk_LastSearch objServiceDesk_LastSearch = GetValuesFromSearchForm();
                // Save Search Criteria
                SaveLastSearchCriteria(objServiceDesk_LastSearch);
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
                ServiceDesk_LastSearch objServiceDesk_LastSearch = new ServiceDesk_LastSearch();
                objServiceDesk_LastSearch.SearchText = (txtSearch.Text.Trim().Length == 0) ? null : txtSearch.Text.Trim();
                objServiceDesk_LastSearch.Status = (ddlStatus.SelectedValue == "*All*") ? null : ddlStatus.SelectedValue;
                objServiceDesk_LastSearch.AssignedRoleID = (ddlAssigned.SelectedValue == "-2") ? null : (int?)Convert.ToInt32(ddlAssigned.SelectedValue);
                objServiceDesk_LastSearch.Priority = (ddlPriority.SelectedValue == "*All*") ? null : ddlPriority.SelectedValue;

                // Created Date
                if (txtCreated.Text.Trim().Length > 4)
                {
                    try
                    {
                        DateTime dtCreated = Convert.ToDateTime(txtCreated.Text.Trim());
                        objServiceDesk_LastSearch.CreatedDate = dtCreated.AddDays(-1);
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
                        objServiceDesk_LastSearch.DueDate = dtDue.AddDays(-1);
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
                    objServiceDesk_LastSearch.Categories = strCategories;
                }

                // Current Page
                objServiceDesk_LastSearch.CurrentPage = GetCurrentPage();

                // Page Size
                objServiceDesk_LastSearch.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);

                // Save Search Criteria
                SaveLastSearchCriteria(objServiceDesk_LastSearch);
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

                ServiceDesk_LastSearch objServiceDesk_LastSearch = (ServiceDesk_LastSearch)SearchCriteria;

                if (objServiceDesk_LastSearch.SearchText != null)
                {
                    txtSearch.Text = objServiceDesk_LastSearch.SearchText;
                }

                if (objServiceDesk_LastSearch.Status != null)
                {
                    ddlStatus.SelectedValue = objServiceDesk_LastSearch.Status;
                }

                if (objServiceDesk_LastSearch.Priority != null)
                {
                    ddlPriority.SelectedValue = objServiceDesk_LastSearch.Priority;
                }

                if (objServiceDesk_LastSearch.DueDate != null)
                {
                    txtDue.Text = objServiceDesk_LastSearch.DueDate.Value.AddDays(1).ToShortDateString();
                }

                if (objServiceDesk_LastSearch.CreatedDate != null)
                {
                    txtCreated.Text = objServiceDesk_LastSearch.CreatedDate.Value.AddDays(1).ToShortDateString();
                }

                // Page Size
                if (objServiceDesk_LastSearch.PageSize != null)
                {
                    ddlPageSize.SelectedValue = objServiceDesk_LastSearch.PageSize.ToString();
                }

                // Load Dropdown
                ddlAssigned.Items.Clear();

                RoleController objRoleController = new RoleController();
                ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

                List<ServiceDesk_Role> colServiceDesk_Roles = (from ServiceDesk_Roles in objServiceDeskDALDataContext.ServiceDesk_Roles
                                                                 where ServiceDesk_Roles.PortalID == PortalId
                                                                 select ServiceDesk_Roles).ToList();

                // Create a ListItemCollection to hold the Roles 
                ListItemCollection colListItemCollection = new ListItemCollection();

                // Add All
                ListItem AllRoleListItem = new ListItem();
                AllRoleListItem.Text = Localization.GetString("ddlAssignedAll.Text", LocalResourceFile);
                AllRoleListItem.Value = "-2";
                if (objServiceDesk_LastSearch.AssignedRoleID != null)
                {
                    if (objServiceDesk_LastSearch.AssignedRoleID == -2)
                    {
                        AllRoleListItem.Selected = true;
                    }
                }
                ddlAssigned.Items.Add(AllRoleListItem);

                // Add the Roles to the List
                foreach (ServiceDesk_Role objServiceDesk_Role in colServiceDesk_Roles)
                {
                    try
                    {
                        RoleInfo objRoleInfo = objRoleController.GetRole(Convert.ToInt32(objServiceDesk_Role.RoleID), PortalId);

                        ListItem RoleListItem = new ListItem();
                        RoleListItem.Text = objRoleInfo.RoleName;
                        RoleListItem.Value = objServiceDesk_Role.RoleID.ToString();

                        if (objServiceDesk_LastSearch.AssignedRoleID != null)
                        {
                            if (objServiceDesk_Role.RoleID == objServiceDesk_LastSearch.AssignedRoleID)
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
                        RoleListItem.Value = objServiceDesk_Role.RoleID.ToString();
                        ddlAssigned.Items.Add(RoleListItem);
                    }
                }

                // Add UnAssigned
                ListItem UnassignedRoleListItem = new ListItem();
                UnassignedRoleListItem.Text = Localization.GetString("Unassigned.Text", LocalResourceFile);
                UnassignedRoleListItem.Value = "-1";
                if (objServiceDesk_LastSearch.AssignedRoleID != null)
                {
                    if (objServiceDesk_LastSearch.AssignedRoleID == -1)
                    {
                        UnassignedRoleListItem.Selected = true;
                    }
                }
                ddlAssigned.Items.Add(UnassignedRoleListItem);
            }
        }
        #endregion

        #region GetValuesFromSearchForm
        private ServiceDesk_LastSearch GetValuesFromSearchForm()
        {
            TextBox txtSearch = (TextBox)lvTasks.FindControl("txtSearch");
            DropDownList ddlStatus = (DropDownList)lvTasks.FindControl("ddlStatus");
            DropDownList ddlPriority = (DropDownList)lvTasks.FindControl("ddlPriority");
            DropDownList ddlAssigned = (DropDownList)lvTasks.FindControl("ddlAssigned");
            TextBox txtDue = (TextBox)lvTasks.FindControl("txtDue");
            TextBox txtCreated = (TextBox)lvTasks.FindControl("txtCreated");

            // Use an ExistingTasks object to pass the values to the Search method
            ServiceDesk_LastSearch objServiceDesk_LastSearch = new ServiceDesk_LastSearch();
            objServiceDesk_LastSearch.SearchText = (txtSearch.Text.Trim().Length == 0) ? null : txtSearch.Text.Trim();
            objServiceDesk_LastSearch.Status = (ddlStatus.SelectedValue == "*All*") ? null : ddlStatus.SelectedValue;
            objServiceDesk_LastSearch.AssignedRoleID = (ddlAssigned.SelectedValue == "-2") ? null : (int?)Convert.ToInt32(ddlAssigned.SelectedValue);
            objServiceDesk_LastSearch.Priority = (ddlPriority.SelectedValue == "*All*") ? null : ddlPriority.SelectedValue;

            // Created Date
            if (txtCreated.Text.Trim().Length > 4)
            {
                try
                {
                    DateTime dtCreated = Convert.ToDateTime(txtCreated.Text.Trim());
                    objServiceDesk_LastSearch.CreatedDate = dtCreated.AddDays(-1);
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
                    objServiceDesk_LastSearch.DueDate = dtDue.AddDays(-1);
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
                objServiceDesk_LastSearch.Categories = strCategories;
            }

            // Current Page
            objServiceDesk_LastSearch.CurrentPage = GetCurrentPage();

            // Page Size
            objServiceDesk_LastSearch.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);

            return objServiceDesk_LastSearch;
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

            List<AssignedRoles> colAssignedRoles = (from ServiceDesk_Tasks in objServiceDeskDALDataContext.ServiceDesk_Tasks
                                                    where ServiceDesk_Tasks.AssignedRoleID > -1
                                                    group ServiceDesk_Tasks by ServiceDesk_Tasks.AssignedRoleID into AssignedRole
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
            ServiceDesk_LastSearch objServiceDesk_LastSearch = GetLastSearchCriteria();
            int intCurrentPage = Convert.ToInt32(objServiceDesk_LastSearch.CurrentPage ?? 1);
            intCurrentPage++;
            objServiceDesk_LastSearch.CurrentPage = intCurrentPage;
            SaveLastSearchCriteria(objServiceDesk_LastSearch);
            DisplayExistingTickets(SearchCriteria);
        }
        #endregion

        #region lnkPrevious_Click
        protected void lnkPrevious_Click(object sender, EventArgs e)
        {
            ServiceDesk_LastSearch objServiceDesk_LastSearch = GetLastSearchCriteria();
            int intCurrentPage = Convert.ToInt32(objServiceDesk_LastSearch.CurrentPage ?? 2);
            intCurrentPage--;
            objServiceDesk_LastSearch.CurrentPage = intCurrentPage;
            SaveLastSearchCriteria(objServiceDesk_LastSearch);
            DisplayExistingTickets(SearchCriteria);
        }
        #endregion

        #region lnkFirst_Click
        protected void lnkFirst_Click(object sender, EventArgs e)
        {
            ServiceDesk_LastSearch objServiceDesk_LastSearch = GetLastSearchCriteria();
            int intCurrentPage = Convert.ToInt32(objServiceDesk_LastSearch.CurrentPage ?? 2);
            intCurrentPage = 1;
            objServiceDesk_LastSearch.CurrentPage = intCurrentPage;
            SaveLastSearchCriteria(objServiceDesk_LastSearch);
            DisplayExistingTickets(SearchCriteria);
        }
        #endregion

        #region lnkLast_Click
        protected void lnkLast_Click(object sender, EventArgs e)
        {
            ServiceDesk_LastSearch objServiceDesk_LastSearch = GetLastSearchCriteria();
            int intCurrentPage = Convert.ToInt32(objServiceDesk_LastSearch.CurrentPage ?? 1);
            intCurrentPage = -1;
            objServiceDesk_LastSearch.CurrentPage = intCurrentPage;
            SaveLastSearchCriteria(objServiceDesk_LastSearch);
            DisplayExistingTickets(SearchCriteria);
        }
        #endregion

        #region ddlPageSize_SelectedIndexChanged
        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            ServiceDesk_LastSearch objServiceDesk_LastSearch = GetLastSearchCriteria();
            objServiceDesk_LastSearch.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            SaveLastSearchCriteria(objServiceDesk_LastSearch);
            DisplayExistingTickets(SearchCriteria);
        }
        #endregion

        #region lnkPage_Click
        protected void lnkPage_Click(object sender, EventArgs e)
        {
            LinkButton lnkButton = sender as LinkButton;
            CurrentPage = lnkButton.CommandArgument;

            ServiceDesk_LastSearch objServiceDesk_LastSearch = GetLastSearchCriteria();
            objServiceDesk_LastSearch.CurrentPage = Convert.ToInt32(lnkButton.CommandArgument);
            SaveLastSearchCriteria(objServiceDesk_LastSearch);
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