//
// ServiceDesk.com
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
using DotNetNuke.Services.Localization;

namespace ITIL.Modules.ServiceDesk
{
    public partial class AdminSettings : DotNetNuke.Entities.Modules.PortalModuleBase
    {
        List<int> colProcessedCategoryIDs;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Get Admin Role
                string strAdminRoleID = GetAdminRole();
                // Only show if user is an Administrator
                if (!(UserInfo.IsInRole(strAdminRoleID) || UserInfo.IsInRole("Administrators") || UserInfo.IsSuperUser))
                {
                    pnlAdminSettings.Visible = false;
                    Response.Redirect(Globals.NavigateURL());
                }

                SetView("AdministratorRole");
                DisplayAdminRoleDropDown();

                btnAddNew.Text = Localization.GetString("btnAddNew.Text", LocalResourceFile);
                btnUpdate.Text = Localization.GetString("btnUpdateAdminRole.Text", LocalResourceFile);
            }
        }

        #region SetView
        private void SetView(string ViewName)
        {
            if (ViewName == "AdministratorRole")
            {
                pnlAdministratorRole.Visible = true;
                pnlUploadefFilesPath.Visible = false;
                pnlTagsAdmin.Visible = false;
                pnlRoles.Visible = false;

                lnkAdminRole.Font.Bold = true;
                lnkAdminRole.BackColor = Color.LightGray;
                lnkUploadefFilesPath.Font.Bold = false;
                lnkUploadefFilesPath.BackColor = Color.Transparent;
                lnkTagsAdmin.Font.Bold = false;
                lnkTagsAdmin.BackColor = Color.Transparent;
                lnkRoles.Font.Bold = false;
                lnkRoles.BackColor = Color.Transparent;
            }

            if (ViewName == "UploadedFilesPath")
            {
                pnlAdministratorRole.Visible = false;
                pnlUploadefFilesPath.Visible = true;
                pnlTagsAdmin.Visible = false;
                pnlRoles.Visible = false;

                lnkAdminRole.Font.Bold = false;
                lnkAdminRole.BackColor = Color.Transparent;
                lnkUploadefFilesPath.Font.Bold = true;
                lnkUploadefFilesPath.BackColor = Color.LightGray;
                lnkTagsAdmin.Font.Bold = false;
                lnkTagsAdmin.BackColor = Color.Transparent;
                lnkRoles.Font.Bold = false;
                lnkRoles.BackColor = Color.Transparent;
            }

            if (ViewName == "Roles")
            {
                pnlAdministratorRole.Visible = false;
                pnlUploadefFilesPath.Visible = false;
                pnlTagsAdmin.Visible = false;
                pnlRoles.Visible = true;

                lnkAdminRole.Font.Bold = false;
                lnkAdminRole.BackColor = Color.Transparent;
                lnkUploadefFilesPath.Font.Bold = false;
                lnkUploadefFilesPath.BackColor = Color.Transparent;
                lnkTagsAdmin.Font.Bold = false;
                lnkTagsAdmin.BackColor = Color.Transparent;
                lnkRoles.Font.Bold = true;
                lnkRoles.BackColor = Color.LightGray;
            }

            if (ViewName == "TagsAdministration")
            {
                pnlAdministratorRole.Visible = false;
                pnlUploadefFilesPath.Visible = false;
                pnlTagsAdmin.Visible = true;
                pnlRoles.Visible = false;

                lnkAdminRole.Font.Bold = false;
                lnkAdminRole.BackColor = Color.Transparent;
                lnkUploadefFilesPath.Font.Bold = false;
                lnkUploadefFilesPath.BackColor = Color.Transparent;
                lnkTagsAdmin.Font.Bold = true;
                lnkTagsAdmin.BackColor = Color.LightGray;
                lnkRoles.Font.Bold = false;
                lnkRoles.BackColor = Color.Transparent;
            }
        }
        #endregion

        #region lnkBack_Click
        protected void lnkBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL());
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

        #region lnkAdminRole_Click
        protected void lnkAdminRole_Click(object sender, EventArgs e)
        {
            SetView("AdministratorRole");
            DisplayAdminRoleDropDown();
        }
        #endregion

        #region lnkUploadefFilesPath_Click
        protected void lnkUploadefFilesPath_Click(object sender, EventArgs e)
        {
            SetView("UploadedFilesPath");
            DisplayUploadedFilesPath();
        }
        #endregion

        #region MyRegion
        protected void lnkRoles_Click(object sender, EventArgs e)
        {
            SetView("Roles");
            DisplayRoles();
        }
        #endregion

        #region DisplayAdminRoleDropDown
        private void DisplayAdminRoleDropDown()
        {
            // Get all the Roles
            RoleController RoleController = new RoleController();
            ArrayList colArrayList = RoleController.GetRoles();

            // Create a ListItemCollection to hold the Roles 
            ListItemCollection colListItemCollection = new ListItemCollection();

            // Add the Roles to the List
            foreach (RoleInfo Role in colArrayList)
            {
                if (Role.PortalID == PortalId)
                {
                    ListItem RoleListItem = new ListItem();
                    RoleListItem.Text = Role.RoleName;
                    RoleListItem.Value = Role.RoleID.ToString();
                    colListItemCollection.Add(RoleListItem);
                }
            }

            // Add the Roles to the ListBox
            ddlAdminRole.DataSource = colListItemCollection;
            ddlAdminRole.DataBind();

            // Get Admin Role
            string strAdminRoleID = GetAdminRole();

            try
            {
                // Try to set the role
                ddlAdminRole.SelectedValue = strAdminRoleID;
            }
            catch
            {

            }
        }
        #endregion

        #region DisplayUploadedFilesPath
        private void DisplayUploadedFilesPath()
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            // Uploaded Files Path
            ServiceDesk_Setting objServiceDesk_Setting = (from ServiceDesk_Settings in objServiceDeskDALDataContext.ServiceDesk_Settings
                                                            where ServiceDesk_Settings.PortalID == PortalId
                                                            where ServiceDesk_Settings.SettingName == "UploadefFilesPath"
                                                            select ServiceDesk_Settings).FirstOrDefault();

            txtUploadedFilesPath.Text = objServiceDesk_Setting.SettingValue;

            // Upload Permissions
            ServiceDesk_Setting UploadPermissionServiceDesk_Setting = (from ServiceDesk_Settings in objServiceDeskDALDataContext.ServiceDesk_Settings
                                                                         where ServiceDesk_Settings.PortalID == PortalId
                                                                         where ServiceDesk_Settings.SettingName == "UploadPermission"
                                                                         select ServiceDesk_Settings).FirstOrDefault();

            ddlUploadPermission.SelectedValue = UploadPermissionServiceDesk_Setting.SettingValue;
        }
        #endregion

        #region lnkTagsAdmin_Click
        protected void lnkTagsAdmin_Click(object sender, EventArgs e)
        {
            SetView("TagsAdministration");
            DisplayCatagories();
            tvCategories.CollapseAll();
        }
        #endregion

        #region btnUpdateAdminRole_Click
        protected void btnUpdateAdminRole_Click(object sender, EventArgs e)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            ServiceDesk_Setting objServiceDesk_Setting = (from ServiceDesk_Settings in objServiceDeskDALDataContext.ServiceDesk_Settings
                                                            where ServiceDesk_Settings.PortalID == PortalId
                                                            where ServiceDesk_Settings.SettingName == "AdminRole"
                                                            select ServiceDesk_Settings).FirstOrDefault();


            objServiceDesk_Setting.SettingValue = ddlAdminRole.SelectedValue;
            objServiceDeskDALDataContext.SubmitChanges();

            lblAdminRole.Text = Localization.GetString("Updated.Text", LocalResourceFile);
        }
        #endregion

        #region btnUploadedFiles_Click
        protected void btnUploadedFiles_Click(object sender, EventArgs e)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            ServiceDesk_Setting UploadefFilesServiceDesk_Setting = (from ServiceDesk_Settings in objServiceDeskDALDataContext.ServiceDesk_Settings
                                                                      where ServiceDesk_Settings.PortalID == PortalId
                                                                      where ServiceDesk_Settings.SettingName == "UploadefFilesPath"
                                                                      select ServiceDesk_Settings).FirstOrDefault();

            UploadefFilesServiceDesk_Setting.SettingValue = txtUploadedFilesPath.Text.Trim();
            objServiceDeskDALDataContext.SubmitChanges();

            ServiceDesk_Setting UploadPermissionServiceDesk_Setting = (from ServiceDesk_Settings in objServiceDeskDALDataContext.ServiceDesk_Settings
                                                                         where ServiceDesk_Settings.PortalID == PortalId
                                                                         where ServiceDesk_Settings.SettingName == "UploadPermission"
                                                                         select ServiceDesk_Settings).FirstOrDefault();

            UploadPermissionServiceDesk_Setting.SettingValue = ddlUploadPermission.SelectedValue;
            objServiceDeskDALDataContext.SubmitChanges();

            lblUploadedFilesPath.Text = Localization.GetString("Updated.Text", LocalResourceFile);
        }
        #endregion

        // Tags

        #region DisplayCatagories
        private void DisplayCatagories()
        {
            CatagoriesTree colCatagories = new CatagoriesTree(PortalId, false);
            tvCategories.DataSource = colCatagories;

            TreeNodeBinding RootBinding = new TreeNodeBinding();
            RootBinding.DataMember = "ListItem";
            RootBinding.TextField = "Text";
            RootBinding.ValueField = "Value";

            tvCategories.DataBindings.Add(RootBinding);

            tvCategories.DataBind();
            tvCategories.CollapseAll();

            // If a node was selected previously select it again
            if (txtCategoryID.Text != "")
            {
                int intCategoryID = Convert.ToInt32(txtCategoryID.Text);
                TreeNode objTreeNode = (TreeNode)tvCategories.FindNode(GetNodePath(intCategoryID));
                objTreeNode.Select();
                objTreeNode.Expand();

                // Expand it's parent nodes
                // Get the value of each parent node
                string[] strParentNodes = objTreeNode.ValuePath.Split(Convert.ToChar("/"));
                // Loop through each parent node
                for (int i = 0; i < objTreeNode.Depth; i++)
                {
                    // Get the parent node
                    TreeNode objParentTreeNode = (TreeNode)tvCategories.FindNode(GetNodePath(Convert.ToInt32(strParentNodes[i])));
                    // Expand the parent node
                    objParentTreeNode.Expand();
                }
            }
            else
            {
                //If there is at least one existing category, select it
                if (tvCategories.Nodes.Count > 0)
                {
                    tvCategories.Nodes[0].Select();
                    txtCategoryID.Text = "0";
                    SelectTreeNode();
                }
                else
                {
                    // There is no data so set form to Add New
                    SetFormToAddNew();
                }
            }

            // If a node is selected, remove it from the BindDropDown drop-down
            int intCategoryNotToShow = -1;
            TreeNode objSelectedTreeNode = (TreeNode)tvCategories.SelectedNode;
            if (objSelectedTreeNode != null)
            {
                intCategoryNotToShow = Convert.ToInt32(tvCategories.SelectedNode.Value);
            }

            BindDropDown(intCategoryNotToShow);
        }
        #endregion

        #region BindDropDown
        private void BindDropDown(int intCategoryNotToShow)
        {
            // Bind drop-down
            CategoriesDropDown colCategoriesDropDown = new CategoriesDropDown(PortalId);
            ListItemCollection objListItemCollection = colCategoriesDropDown.Categories(intCategoryNotToShow);

            // Don't show the currently selected node
            foreach (ListItem objListItem in objListItemCollection)
            {
                if (objListItem.Value == intCategoryNotToShow.ToString())
                {
                    objListItemCollection.Remove(objListItem);
                    break;
                }
            }

            ddlParentCategory.DataSource = objListItemCollection;
            ddlParentCategory.DataTextField = "Text";
            ddlParentCategory.DataValueField = "Value";
            ddlParentCategory.DataBind();
        }
        #endregion

        #region GetNodePath
        private string GetNodePath(int intCategoryID)
        {
            string strNodePath = intCategoryID.ToString();

            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            var result = (from ServiceDesk_Categories in objServiceDeskDALDataContext.ServiceDesk_Categories
                          where ServiceDesk_Categories.CategoryID == intCategoryID
                          select ServiceDesk_Categories).FirstOrDefault();

            // Only build a node path if the current level is not the root
            if (result.Level > 1)
            {
                int intCurrentCategoryID = result.CategoryID;

                for (int i = 1; i < result.Level; i++)
                {
                    var CurrentCategory = (from ServiceDesk_Categories in objServiceDeskDALDataContext.ServiceDesk_Categories
                                           where ServiceDesk_Categories.CategoryID == intCurrentCategoryID
                                           select ServiceDesk_Categories).FirstOrDefault();

                    strNodePath = CurrentCategory.ParentCategoryID.ToString() + @"/" + strNodePath;

                    var ParentCategory = (from ServiceDesk_Categories in objServiceDeskDALDataContext.ServiceDesk_Categories
                                          where ServiceDesk_Categories.CategoryID == CurrentCategory.ParentCategoryID
                                          select ServiceDesk_Categories).FirstOrDefault();

                    intCurrentCategoryID = ParentCategory.CategoryID;
                }
            }

            return strNodePath;
        }
        #endregion

        #region tvCategories_SelectedNodeChanged
        protected void tvCategories_SelectedNodeChanged(object sender, EventArgs e)
        {
            SelectTreeNode();
            ResetForm();
        }
        #endregion

        #region SelectTreeNode
        private void SelectTreeNode()
        {
            if (tvCategories.SelectedNode != null)
            {
                if (tvCategories.SelectedNode.Value != "")
                {
                    var result = (from ServiceDesk_Categories in CategoriesTable.GetCategoriesTable(PortalId, false)
                                  where ServiceDesk_Categories.CategoryID == Convert.ToInt32(tvCategories.SelectedNode.Value)
                                  select ServiceDesk_Categories).FirstOrDefault();

                    txtCategory.Text = result.CategoryName;
                    txtCategoryID.Text = result.CategoryID.ToString();
                    chkRequesterVisible.Checked = result.RequestorVisible;
                    chkSelectable.Checked = result.Selectable;

                    // Remove Node from the Bind DropDown drop-down
                    BindDropDown(result.CategoryID);

                    // Set the Parent drop-down
                    ddlParentCategory.SelectedValue = (result.ParentCategoryID == null) ? "0" : result.ParentCategoryID.ToString();
                    txtParentCategoryID.Text = (result.ParentCategoryID == null) ? "" : result.ParentCategoryID.ToString();
                }
            }
        }
        #endregion

        #region btnUpdate_Click
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            if (btnUpdate.CommandName == "Update")
            {
                var result = (from ServiceDesk_Categories in objServiceDeskDALDataContext.ServiceDesk_Categories
                              where ServiceDesk_Categories.CategoryID == Convert.ToInt32(txtCategoryID.Text)
                              select ServiceDesk_Categories).FirstOrDefault();

                result.CategoryName = txtCategory.Text.Trim();

                result.ParentCategoryID = (GetParentCategoryID(ddlParentCategory.SelectedValue) == "0") ? (int?)null : Convert.ToInt32(ddlParentCategory.SelectedValue);
                txtParentCategoryID.Text = (ddlParentCategory.SelectedValue == "0") ? "" : ddlParentCategory.SelectedValue;

                result.Level = (ddlParentCategory.SelectedValue == "0") ? 1 : GetLevelOfParent(Convert.ToInt32(ddlParentCategory.SelectedValue)) + 1;
                result.RequestorVisible = chkRequesterVisible.Checked;
                result.Selectable = chkSelectable.Checked;

                objServiceDeskDALDataContext.SubmitChanges();

                // Update levels off all the Children
                colProcessedCategoryIDs = new List<int>();
                UpdateLevelOfChildren(result);
            }
            else
            {
                // This is a Save for a new Node                

                ServiceDesk_Category objServiceDesk_Category = new ServiceDesk_Category();
                objServiceDesk_Category.PortalID = PortalId;
                objServiceDesk_Category.CategoryName = txtCategory.Text.Trim();
                objServiceDesk_Category.ParentCategoryID = (GetParentCategoryID(ddlParentCategory.SelectedValue) == "0") ? (int?)null : Convert.ToInt32(ddlParentCategory.SelectedValue);
                objServiceDesk_Category.Level = (ddlParentCategory.SelectedValue == "0") ? 1 : GetLevelOfParent(Convert.ToInt32(ddlParentCategory.SelectedValue)) + 1;
                objServiceDesk_Category.RequestorVisible = chkRequesterVisible.Checked;
                objServiceDesk_Category.Selectable = chkSelectable.Checked;

                objServiceDeskDALDataContext.ServiceDesk_Categories.InsertOnSubmit(objServiceDesk_Category);
                objServiceDeskDALDataContext.SubmitChanges();

                // Set the Hidden CategoryID
                txtParentCategoryID.Text = (objServiceDesk_Category.ParentCategoryID == null) ? "" : ddlParentCategory.SelectedValue;
                txtCategoryID.Text = objServiceDesk_Category.CategoryID.ToString();
                ResetForm();
            }

            RefreshCache();
            DisplayCatagories();

            // Set the Parent drop-down
            if (txtParentCategoryID.Text != "")
            {
                ddlParentCategory.SelectedValue = txtParentCategoryID.Text;
            }
        }
        #endregion

        #region UpdateLevelOfChildren
        private void UpdateLevelOfChildren(ServiceDesk_Category result)
        {
            int? intStartingLevel = result.Level;

            if (colProcessedCategoryIDs == null)
            {
                colProcessedCategoryIDs = new List<int>();
            }

            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            // Get the children of the current item
            // This method may be called from the top level or recuresively by one of the child items
            var CategoryChildren = from ServiceDesk_Categories in objServiceDeskDALDataContext.ServiceDesk_Categories
                                   where ServiceDesk_Categories.ParentCategoryID == result.CategoryID
                                   where !colProcessedCategoryIDs.Contains(result.CategoryID)
                                   select ServiceDesk_Categories;

            // Loop thru each item
            foreach (var objCategory in CategoryChildren)
            {
                colProcessedCategoryIDs.Add(objCategory.CategoryID);

                objCategory.Level = ((intStartingLevel) ?? 0) + 1;
                objServiceDeskDALDataContext.SubmitChanges();

                //Recursively call the UpdateLevelOfChildren method adding all children
                UpdateLevelOfChildren(objCategory);
            }
        }
        #endregion

        #region GetLevelOfParent
        private int? GetLevelOfParent(int? ParentCategoryID)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            var result = (from ServiceDesk_Categories in objServiceDeskDALDataContext.ServiceDesk_Categories
                          where ServiceDesk_Categories.CategoryID == ParentCategoryID
                          select ServiceDesk_Categories).FirstOrDefault();

            return (result == null) ? 0 : result.Level;
        }
        #endregion

        #region GetParentCategoryID
        private string GetParentCategoryID(string strParentCategoryID)
        {
            // This is to ensure that the ParentCategoryID does exist and has not been deleted since the last time the form was loaded
            int ParentCategoryID = 0;
            if (strParentCategoryID != "0")
            {
                ParentCategoryID = Convert.ToInt32(strParentCategoryID);
            }

            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            var result = (from ServiceDesk_Categories in objServiceDeskDALDataContext.ServiceDesk_Categories
                          where ServiceDesk_Categories.CategoryID == ParentCategoryID
                          select ServiceDesk_Categories).FirstOrDefault();

            string strResultParentCategoryID = "0";
            if (result != null)
            {
                strResultParentCategoryID = result.CategoryID.ToString();
            }

            return strResultParentCategoryID;
        }
        #endregion

        #region btnAddNew_Click
        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            if (btnAddNew.CommandName == "AddNew")
            {
                SetFormToAddNew();
            }
            else
            {
                // This is a Cancel
                ResetForm();
                DisplayCatagories();
                SelectTreeNode();
            }
        }
        #endregion

        #region SetFormToAddNew
        private void SetFormToAddNew()
        {
            txtCategory.Text = "";
            chkRequesterVisible.Checked = true;
            chkSelectable.Checked = true;
            btnAddNew.CommandName = "Cancel";
            btnUpdate.CommandName = "Save";
            btnAddNew.Text = Localization.GetString("Cancel.Text", LocalResourceFile);
            btnUpdate.Text = Localization.GetString("Save.Text", LocalResourceFile);
            btnDelete.Visible = false;
            BindDropDown(-1);

            if (tvCategories.SelectedNode == null)
            {
                ddlParentCategory.SelectedValue = "0";
            }
            else
            {
                try
                {
                    ddlParentCategory.SelectedValue = tvCategories.SelectedNode.Value;
                }
                catch (Exception ex)
                {
                    lblTagError.Text = ex.Message;
                }
            }
        }
        #endregion

        #region ResetForm
        private void ResetForm()
        {
            btnUpdate.CommandName = "Update";
            btnAddNew.CommandName = "AddNew";
            btnAddNew.Text = Localization.GetString("btnAddNew.Text", LocalResourceFile);
            btnUpdate.Text = Localization.GetString("btnUpdateAdminRole.Text", LocalResourceFile);
            btnDelete.Visible = true;
        }
        #endregion

        #region btnDelete_Click
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            // Get the node
            var result = (from ServiceDesk_Categories in objServiceDeskDALDataContext.ServiceDesk_Categories
                          where ServiceDesk_Categories.CategoryID == Convert.ToInt32(txtCategoryID.Text)
                          select ServiceDesk_Categories).FirstOrDefault();

            // Make a Temp object to use to update the child nodes
            ServiceDesk_Category TmpServiceDesk_Category = new ServiceDesk_Category();
            TmpServiceDesk_Category.CategoryID = result.CategoryID;
            if (result.ParentCategoryID == null)
            {
                TmpServiceDesk_Category.Level = 0;
            }
            else
            {
                TmpServiceDesk_Category.Level = GetLevelOfParent(result.ParentCategoryID);
            }

            // Get all TaskCategories that use the Node
            var colTaskCategories = from ServiceDesk_TaskCategories in objServiceDeskDALDataContext.ServiceDesk_TaskCategories
                                    where ServiceDesk_TaskCategories.CategoryID == Convert.ToInt32(txtCategoryID.Text)
                                    select ServiceDesk_TaskCategories;

            // Delete them
            objServiceDeskDALDataContext.ServiceDesk_TaskCategories.DeleteAllOnSubmit(colTaskCategories);
            objServiceDeskDALDataContext.SubmitChanges();

            // Delete the node
            objServiceDeskDALDataContext.ServiceDesk_Categories.DeleteOnSubmit(result);
            objServiceDeskDALDataContext.SubmitChanges();

            // Update levels of all the Children            
            UpdateLevelOfChildren(TmpServiceDesk_Category);

            // Update all the children nodes to give them a new parent
            var CategoryChildren = from ServiceDesk_Categories in objServiceDeskDALDataContext.ServiceDesk_Categories
                                   where ServiceDesk_Categories.ParentCategoryID == result.CategoryID
                                   select ServiceDesk_Categories;

            // Loop thru each item
            foreach (var objCategory in CategoryChildren)
            {
                objCategory.ParentCategoryID = result.ParentCategoryID;
                objServiceDeskDALDataContext.SubmitChanges();
            }

            // Delete the Catagory from any Ticket that uses it
            var DeleteHelpDesk_TaskCategories = from ServiceDesk_TaskCategories in objServiceDeskDALDataContext.ServiceDesk_TaskCategories
                                                where ServiceDesk_TaskCategories.CategoryID == TmpServiceDesk_Category.CategoryID
                                                select ServiceDesk_TaskCategories;

            objServiceDeskDALDataContext.ServiceDesk_TaskCategories.DeleteAllOnSubmit(DeleteHelpDesk_TaskCategories);
            objServiceDeskDALDataContext.SubmitChanges();

            RefreshCache();

            // Set the CategoryID
            txtCategoryID.Text = (result.ParentCategoryID == null) ? "" : result.ParentCategoryID.ToString();

            DisplayCatagories();
            SelectTreeNode();
        }
        #endregion

        #region RefreshCache
        private void RefreshCache()
        {
            // Get Table out of Cache
            object objCategoriesTable = HttpContext.Current.Cache.Get(String.Format("CategoriesTable_{0}", PortalId.ToString()));

            // Is the table in the cache?
            if (objCategoriesTable != null)
            {
                // Remove table from cache
                HttpContext.Current.Cache.Remove(String.Format("CategoriesTable_{0}", PortalId.ToString()));
            }

            // Get Table out of Cache
            object objRequestorCategoriesTable_ = HttpContext.Current.Cache.Get(String.Format("RequestorCategoriesTable_{0}", PortalId.ToString()));

            // Is the table in the cache?
            if (objRequestorCategoriesTable_ != null)
            {
                // Remove table from cache
                HttpContext.Current.Cache.Remove(String.Format("RequestorCategoriesTable_{0}", PortalId.ToString()));
            }
        }
        #endregion

        #region tvCategories_TreeNodeDataBound
        protected void tvCategories_TreeNodeDataBound(object sender, TreeNodeEventArgs e)
        {
            ListItem objListItem = (ListItem)e.Node.DataItem;
            e.Node.ShowCheckBox = Convert.ToBoolean(objListItem.Attributes["Selectable"]);
            if (Convert.ToBoolean(objListItem.Attributes["Selectable"]))
            {
                e.Node.ImageUrl = Convert.ToBoolean(objListItem.Attributes["RequestorVisible"]) ? "images/world.png" : "images/world_delete.png";
                e.Node.ToolTip = Convert.ToBoolean(objListItem.Attributes["RequestorVisible"]) ? "Requestor Visible" : "Requestor Not Visible";
            }
            else
            {
                e.Node.ImageUrl = "images/table.png";
                e.Node.ToolTip = Convert.ToBoolean(objListItem.Attributes["RequestorVisible"]) ? "Requestor Visible" : "Requestor Not Visible";
            }
        }
        #endregion

        // Roles

        #region ldsRoles_Selecting
        protected void ldsRoles_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.WhereParameters["PortalID"] = PortalId;
        }
        #endregion

        #region lvRoles_ItemDataBound
        protected void lvRoles_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            ListViewDataItem objListViewDataItem = (ListViewDataItem)e.Item;
            Label RoleIDLabel = (Label)e.Item.FindControl("RoleIDLabel");

            try
            {
                RoleController objRoleController = new RoleController();
                RoleIDLabel.Text = String.Format("{0}", objRoleController.GetRole(Convert.ToInt32(RoleIDLabel.Text), PortalId).RoleName);
            }
            catch (Exception)
            {
                RoleIDLabel.Text = Localization.GetString("DeletedRole.Text", LocalResourceFile);
            }
        }
        #endregion

        #region btnInsertRole_Click
        protected void btnInsertRole_Click(object sender, EventArgs e)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            // See if Role already exists
            ServiceDesk_Role colServiceDesk_Roles = (from ServiceDesk_Roles in objServiceDeskDALDataContext.ServiceDesk_Roles
                                                       where ServiceDesk_Roles.PortalID == PortalId
                                                       where ServiceDesk_Roles.RoleID == Convert.ToInt32(ddlRole.SelectedValue)
                                                       select ServiceDesk_Roles).FirstOrDefault();
            if (colServiceDesk_Roles != null)
            {
                RoleController objRoleController = new RoleController();
                lblRoleError.Text = String.Format(Localization.GetString("RoleAlreadyAdded.Text", LocalResourceFile), objRoleController.GetRole(Convert.ToInt32(ddlRole.SelectedValue), PortalId).RoleName);
            }
            else
            {
                ServiceDesk_Role objServiceDesk_Role = new ServiceDesk_Role();
                objServiceDesk_Role.PortalID = PortalId;
                objServiceDesk_Role.RoleID = Convert.ToInt32(ddlRole.SelectedValue);

                objServiceDeskDALDataContext.ServiceDesk_Roles.InsertOnSubmit(objServiceDesk_Role);
                objServiceDeskDALDataContext.SubmitChanges();

                lvRoles.DataBind();
            }
        }
        #endregion

        #region DisplayRoles
        private void DisplayRoles()
        {
            // Get all the Roles
            RoleController RoleController = new RoleController();
            ArrayList colArrayList = RoleController.GetRoles();

            // Create a ListItemCollection to hold the Roles 
            ListItemCollection colListItemCollection = new ListItemCollection();

            // Add the Roles to the List
            foreach (RoleInfo Role in colArrayList)
            {
                if (Role.PortalID == PortalId)
                {
                    ListItem RoleListItem = new ListItem();
                    RoleListItem.Text = Role.RoleName;
                    RoleListItem.Value = Role.RoleID.ToString();
                    colListItemCollection.Add(RoleListItem);
                }
            }

            // Add the Roles to the ListBox
            ddlRole.DataSource = colListItemCollection;
            ddlRole.DataBind();
        }
        #endregion
    }
}