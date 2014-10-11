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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using Microsoft.VisualBasic;

namespace ITIL.Modules.ServiceDesk.Controls
{
    public partial class Comments : DotNetNuke.Entities.Modules.PortalModuleBase
    {
        #region Properties
        public int TaskID
        {
            get { return Convert.ToInt32(ViewState["TaskID"]); }
            set { ViewState["TaskID"] = value; }
        }

        public int ModuleID
        {
            get { return Convert.ToInt32(ViewState["ModuleID"]); }
            set { ViewState["ModuleID"] = value; }
        }

        public bool ViewOnly
        {
            get { return Convert.ToBoolean(ViewState["ViewOnly"]); }
            set { ViewState["ViewOnly"] = value; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            pnlInsertComment.GroupingText = Localization.GetString("pnlInsertComment.Text", LocalResourceFile);

            if (!Page.IsPostBack)
            {
                SetView("Default");

                if (ViewOnly)
                {
                    SetViewOnlyMode();
                }

                ShowFileUpload();
            }
        }

        #region ShowFileUpload
        private void ShowFileUpload()
        {
            string strAdminRoleID = GetAdminRole();
            if (!(UserInfo.IsInRole(strAdminRoleID) || UserInfo.IsInRole("Administrators") || UserInfo.IsSuperUser))
            {
                string strUploadPermission = GetUploadPermission();

                // Only supress Upload if permission is not set to All              
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
                            lblAttachFile1.Visible = false;
                            TicketFileUpload.Visible = false;
                            lblAttachFile2.Visible = false;
                            fuAttachment.Visible = false;
                        }
                        #endregion
                    }
                    else
                    {
                        // If User is not logged in they cannot see upload
                        lblAttachFile1.Visible = false;
                        TicketFileUpload.Visible = false;
                        lblAttachFile2.Visible = false;
                        fuAttachment.Visible = false;
                    }
                }
            }
        }
        #endregion

        #region SetView
        public void SetView(string ViewMode)
        {
            if (ViewMode == "Default")
            {
                pnlInsertComment.Visible = true;
                pnlExistingComments.Visible = true;
                pnlEditComment.Visible = false;
            }

            if (ViewMode == "Edit")
            {
                pnlInsertComment.Visible = false;
                pnlExistingComments.Visible = false;
                pnlEditComment.Visible = true;
            }
        }
        #endregion

        #region SetViewOnlyMode
        private void SetViewOnlyMode()
        {
            chkCommentVisible.Visible = false;
            chkCommentVisibleEdit.Visible = false;
            lnkDelete.Visible = false;
            Image5.Visible = false;
            lnkUpdate.Visible = false;
            Image4.Visible = false;
            pnlDisplayFile.Visible = false;
            pnlAttachFile.Visible = false;
            imgDelete.Visible = false;
            lnkUpdateRequestor.Visible = false;
            ImgEmailUser.Visible = false;
            btnInsertCommentAndEmail.Visible = false;
        }
        #endregion

        // Insert Comment

        #region btnInsertComment_Click
        protected void btnInsertComment_Click(object sender, EventArgs e)
        {
            InsertComment();
        }
        #endregion

        #region btnInsertCommentAndEmail_Click
        protected void btnInsertCommentAndEmail_Click(object sender, EventArgs e)
        {
            string strComment = txtComment.Text;
            InsertComment();
            NotifyRequestorOfComment(strComment);
        }
        #endregion

        #region InsertComment
        private void InsertComment()
        {
            // Validate file upload
            if (TicketFileUpload.HasFile)
            {

                // ITIL Customization - use DNN host setting for file extension
                string fileName = TicketFileUpload.PostedFile.FileName;

                string extension = Path.GetExtension(fileName);

                if (!Utility.IsAllowedExtension(fileName, extension))
                {
                    string ErrorMessage = string.Format(Localization.GetExceptionMessage("AddFileExtensionNotAllowed", "The extension '{0}' is not allowed. The file has not been added."), extension);
                    lblError.Text = ErrorMessage;
                    return;
                }

            }

            if (txtComment.Text.Trim().Length > 0)
            {
                ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

                string strComment = txtComment.Text.Trim();

                // Save Task Details
                ServiceDesk_TaskDetail objServiceDesk_TaskDetail = new ServiceDesk_TaskDetail();

                objServiceDesk_TaskDetail.TaskID = TaskID;
                objServiceDesk_TaskDetail.Description = txtComment.Text.Trim();
                objServiceDesk_TaskDetail.InsertDate = DateTime.Now;
                objServiceDesk_TaskDetail.UserID = UserId;

                if (chkCommentVisible.Checked)
                {
                    objServiceDesk_TaskDetail.DetailType = "Comment-Visible";
                }
                else
                {
                    objServiceDesk_TaskDetail.DetailType = "Comment";
                }

                objServiceDeskDALDataContext.ServiceDesk_TaskDetails.InsertOnSubmit(objServiceDesk_TaskDetail);
                objServiceDeskDALDataContext.SubmitChanges();
                txtComment.Text = "";

                // Insert Log
                Log.InsertLog(TaskID, UserId, String.Format("{0} inserted comment.", GetUserName()));

                // Upload the File
                if (TicketFileUpload.HasFile)
                {
                    UploadFile(objServiceDesk_TaskDetail.DetailID);
                    // Insert Log
                    Log.InsertLog(TaskID, UserId, String.Format("{0} uploaded file '{1}'.", GetUserName(), TicketFileUpload.FileName));
                }

                if (UserIsRequestor())
                {
                    NotifyAssignedGroupOfComment(strComment);
                }

                gvComments.DataBind();
            }
        }
        #endregion

        #region LDSComments_Selecting
        protected void LDSComments_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();
            var result = from ServiceDesk_TaskDetails in objServiceDeskDALDataContext.ServiceDesk_TaskDetails
                         where ServiceDesk_TaskDetails.TaskID == TaskID
                         where (ServiceDesk_TaskDetails.DetailType == "Comment" || ServiceDesk_TaskDetails.DetailType == "Comment-Visible")
                         select ServiceDesk_TaskDetails;

            // If View only mode
            if (ViewOnly)
            {
                result = from TaskDetails in result
                         where TaskDetails.DetailType == "Comment-Visible"
                         select TaskDetails;
            }

            e.Result = result;
        }
        #endregion

        #region gvComments_RowDataBound
        protected void gvComments_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridViewRow objGridViewRow = (GridViewRow)e.Row;

                // Comment
                Label lblComment = (Label)objGridViewRow.FindControl("lblComment");
                if (lblComment.Text.Trim().Length > 100)
                {
                    string newlblComment = lblComment.Text;
                    lblComment.Text = String.Format("{0}...", newlblComment.Substring(0,100));
                }

                // User
                Label gvlblUser = (Label)objGridViewRow.FindControl("gvlblUser");
                if (gvlblUser.Text != "-1")
                {
                    UserInfo objUser = UserController.GetUser(PortalId, Convert.ToInt32(gvlblUser.Text), false);

                    if (objUser != null)
                    {
                        string strDisplayName = objUser.DisplayName;

                        if (strDisplayName.Length > 25)
                        {
                            gvlblUser.Text = String.Format("{0}...", strDisplayName.Substring(0,25));
                        }
                        else
                        {
                            gvlblUser.Text = strDisplayName;
                        }
                    }
                    else
                    {
                        gvlblUser.Text = "[User Deleted]";
                    }
                }
                else
                {
                    gvlblUser.Text = Localization.GetString("Requestor.Text", LocalResourceFile);
                }

                // Comment Visible checkbox
                CheckBox chkDetailType = (CheckBox)objGridViewRow.FindControl("chkDetailType");
                Label lblDetailType = (Label)objGridViewRow.FindControl("lblDetailType");
                // lblDetailType
                chkDetailType.Checked = (lblDetailType.Text == "Comment") ? false : true;
            }
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
            string strUserName = Localization.GetString("Anonymous.Text", LocalResourceFile);

            if (UserId > -1)
            {
                strUserName = UserInfo.DisplayName;
            }

            return strUserName;
        }

        private string GetUserName(int intUserID)
        {
            string strUserName = Localization.GetString("Anonymous.Text", LocalResourceFile);

            if (intUserID > -1)
            {
                UserInfo objUser = UserController.GetUser(PortalId, intUserID, false);

                if (objUser != null)
                {
                    strUserName = objUser.DisplayName;
                }
                else
                {
                    strUserName = Localization.GetString("Anonymous.Text", LocalResourceFile);
                }
            }

            return strUserName;
        }
        #endregion

        // GridView

        #region gvComments_RowCommand
        protected void gvComments_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                SetView("Edit");
                lblDetailID.Text = Convert.ToString(e.CommandArgument);
                DisplayComment();
            }
        }
        #endregion

        // Comment Edit

        #region lnkBack_Click
        protected void lnkBack_Click(object sender, EventArgs e)
        {
            SetView("Default");
        }
        #endregion

        #region DisplayComment
        private void DisplayComment()
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            var objServiceDesk_TaskDetail = (from ServiceDesk_TaskDetails in objServiceDeskDALDataContext.ServiceDesk_TaskDetails
                                              where ServiceDesk_TaskDetails.DetailID == Convert.ToInt32(lblDetailID.Text)
                                              select ServiceDesk_TaskDetails).FirstOrDefault();

            if (objServiceDesk_TaskDetail != null)
            {
                txtDescription.Text = objServiceDesk_TaskDetail.Description;
                lblDisplayUser.Text = GetUserName(objServiceDesk_TaskDetail.UserID);
                lblInsertDate.Text = String.Format("{0} {1}", objServiceDesk_TaskDetail.InsertDate.ToLongDateString(), objServiceDesk_TaskDetail.InsertDate.ToLongTimeString());
                chkCommentVisibleEdit.Checked = (objServiceDesk_TaskDetail.DetailType == "Comment") ? false : true;

                if (!ViewOnly)
                {
                    ImgEmailUser.Visible = (objServiceDesk_TaskDetail.DetailType == "Comment") ? false : true;
                    lnkUpdateRequestor.Visible = (objServiceDesk_TaskDetail.DetailType == "Comment") ? false : true;
                }

                // Only set the Display of the Email to Requestor link if it is already showing
                if (lnkUpdateRequestor.Visible)
                {
                    // Only Display Email to Requestor link if chkCommentVisibleEdit is checked
                    lnkUpdateRequestor.Visible = chkCommentVisibleEdit.Checked;
                    ImgEmailUser.Visible = chkCommentVisibleEdit.Checked;
                }

                if (objServiceDesk_TaskDetail.ServiceDesk_Attachments.Count > 0)
                {
                    // There is a atachment
                    pnlAttachFile.Visible = false;
                    pnlDisplayFile.Visible = true;

                    lnkFileAttachment.Text = objServiceDesk_TaskDetail.ServiceDesk_Attachments.FirstOrDefault().OriginalFileName;
                    lnkFileAttachment.CommandArgument = objServiceDesk_TaskDetail.ServiceDesk_Attachments.FirstOrDefault().AttachmentID.ToString();
                }
                else
                {
                    // Only do this if not in View Only Mode
                    if (!ViewOnly)
                    {
                        // There is not a file attached
                        pnlAttachFile.Visible = true;
                        pnlDisplayFile.Visible = false;
                    }
                    else
                    {
                        pnlDisplayFile.Visible = false;
                    }
                }
            }
        }
        #endregion

        #region lnkFileAttachment_Click
        protected void lnkFileAttachment_Click(object sender, EventArgs e)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            var objServiceDesk_Attachment = (from ServiceDesk_Attachments in objServiceDeskDALDataContext.ServiceDesk_Attachments
                                              where ServiceDesk_Attachments.AttachmentID == Convert.ToInt32(lnkFileAttachment.CommandArgument)
                                              select ServiceDesk_Attachments).FirstOrDefault();

            if (objServiceDesk_Attachment != null)
            {
                string strPath = objServiceDesk_Attachment.AttachmentPath;
                string strOriginalFileName = objServiceDesk_Attachment.OriginalFileName;

                try
                {
                    Response.ClearHeaders();
                    Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", strOriginalFileName));

                    Response.ClearContent();
                    Response.ContentEncoding = System.Text.Encoding.UTF8;
                    Response.ContentType = GetContentType(strPath);

                    FileStream sourceFile = new FileStream(strPath, FileMode.Open);
                    long FileSize;
                    FileSize = sourceFile.Length;
                    byte[] getContent = new byte[(int)FileSize];
                    sourceFile.Read(getContent, 0, (int)sourceFile.Length);
                    sourceFile.Close();

                    Response.BinaryWrite(getContent);
                    Response.End(); 
                    Response.Flush();
                    Response.Close();

                }
                catch
                {
                }
            }
        }
        #endregion

        #region GetContentType
        public string GetContentType(string strextension)
        {
            string contentType;
            switch (strextension.ToLower())
            {
                case ".gif":
                    contentType = "image/gif";
                    break;
                case ".jpg":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                case ".doc":
                    contentType = "application/ms-word";
                    break;
                case ".docx":
                    contentType = "application/vnd.ms-word.document";
                    break;
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                case ".xls":
                    contentType = "application/vnd.ms-excel";
                    break;
                case ".ppt":
                    contentType = "application/vnd.ms-powerpoint";
                    break;
                case ".zip":
                    contentType = "application/zip";
                    break;
                case ".txt":
                    contentType = "text/plain";
                    break;
                default:
                    contentType = "application/octet-stream";
                    break;
            }
            return contentType;
        }
        #endregion

        #region lnkDelete_Click
        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            var objServiceDesk_TaskDetail = (from ServiceDesk_TaskDetails in objServiceDeskDALDataContext.ServiceDesk_TaskDetails
                                              where ServiceDesk_TaskDetails.DetailID == Convert.ToInt32(lblDetailID.Text)
                                              select ServiceDesk_TaskDetails).FirstOrDefault();

            // Delete any Attachments
            if (objServiceDesk_TaskDetail.ServiceDesk_Attachments.Count > 0)
            {
                ServiceDesk_Attachment objServiceDesk_Attachment = objServiceDesk_TaskDetail.ServiceDesk_Attachments.FirstOrDefault();
                string strOriginalFileName = objServiceDesk_Attachment.OriginalFileName;
                string strFile = objServiceDesk_Attachment.AttachmentPath;

                try
                {
                    // Delete file
                    if (strFile != "")
                    {
                        File.Delete(strFile);
                    }
                }
                catch (Exception exc)
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                }

                objServiceDeskDALDataContext.ServiceDesk_Attachments.DeleteOnSubmit(objServiceDesk_Attachment);
                objServiceDeskDALDataContext.SubmitChanges();

                // Insert Log
                Log.InsertLog(TaskID, UserId, String.Format("{0} deleted file '{1}'.", GetUserName(), strOriginalFileName));
            }

            // Delete the Record
            objServiceDeskDALDataContext.ServiceDesk_TaskDetails.DeleteOnSubmit(objServiceDesk_TaskDetail);
            objServiceDeskDALDataContext.SubmitChanges();

            // Insert Log
            Log.InsertLog(TaskID, UserId, String.Format("{0} deleted comment: {1}", GetUserName(), txtDescription.Text));

            SetView("Default");
            gvComments.DataBind();
        }
        #endregion

        #region imgDelete_Click
        protected void imgDelete_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            var objServiceDesk_TaskDetail = (from ServiceDesk_TaskDetails in objServiceDeskDALDataContext.ServiceDesk_TaskDetails
                                              where ServiceDesk_TaskDetails.DetailID == Convert.ToInt32(lblDetailID.Text)
                                              select ServiceDesk_TaskDetails).FirstOrDefault();

            // Delete Attachment
            if (objServiceDesk_TaskDetail.ServiceDesk_Attachments.Count > 0)
            {
                ServiceDesk_Attachment objServiceDesk_Attachment = objServiceDesk_TaskDetail.ServiceDesk_Attachments.FirstOrDefault();
                string strOriginalFileName = objServiceDesk_Attachment.OriginalFileName;
                string strFile = objServiceDesk_Attachment.AttachmentPath;

                try
                {
                    // Delete file
                    if (strFile != "")
                    {
                        File.Delete(strFile);
                    }
                }
                catch (Exception exc)
                {
                    Exceptions.ProcessModuleLoadException(this, exc);
                }

                objServiceDeskDALDataContext.ServiceDesk_Attachments.DeleteOnSubmit(objServiceDesk_Attachment);
                objServiceDeskDALDataContext.SubmitChanges();

                // Insert Log
                Log.InsertLog(TaskID, UserId, String.Format("{0} deleted file '{1}'.", GetUserName(), strOriginalFileName));

                pnlAttachFile.Visible = true;
                pnlDisplayFile.Visible = false;
            }
        }
        #endregion

        #region lnkUpdate_Click
        protected void lnkUpdate_Click(object sender, EventArgs e)
        {
            UpdateComment();
        }
        #endregion

        #region lnkUpdateRequestor_Click
        protected void lnkUpdateRequestor_Click(object sender, EventArgs e)
        {
            string strComment = txtDescription.Text;
            UpdateComment();
            NotifyRequestorOfComment(strComment);
        }
        #endregion

        #region UpdateComment
        private void UpdateComment()
        {
            // Validate file upload
            if (fuAttachment.HasFile)
            {
                if (
                    string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".gif", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".jpg", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".jpeg", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".doc", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".docx", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".xls", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".xlsx", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".pdf", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".txt", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".sql", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".rtf", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".zip", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".rar", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".cfm", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".html", true) != 0
                    & string.Compare(Path.GetExtension(fuAttachment.FileName).ToLower(), ".sdf", true) != 0
                    )
                {
                    lblErrorEditComment.Text = "Only .gif, .jpg, .jpeg, .doc, .docx, .xls, .xlsx, .pdf, .txt, .sql, .zip, .rar, .cfm, .html, .rtf files may be used.";
                    return;
                }
            }

            if (txtDescription.Text.Trim().Length > 0)
            {
                ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

                string strComment = txtDescription.Text.Trim();

                // Save Task Details
                var objServiceDesk_TaskDetail = (from ServiceDesk_TaskDetails in objServiceDeskDALDataContext.ServiceDesk_TaskDetails
                                                  where ServiceDesk_TaskDetails.DetailID == Convert.ToInt32(lblDetailID.Text)
                                                  select ServiceDesk_TaskDetails).FirstOrDefault();

                if (objServiceDesk_TaskDetail != null)
                {

                    objServiceDesk_TaskDetail.TaskID = TaskID;
                    objServiceDesk_TaskDetail.Description = txtDescription.Text.Trim();
                    objServiceDesk_TaskDetail.UserID = UserId;

                    if (chkCommentVisibleEdit.Checked)
                    {
                        objServiceDesk_TaskDetail.DetailType = "Comment-Visible";
                    }
                    else
                    {
                        objServiceDesk_TaskDetail.DetailType = "Comment";
                    }

                    objServiceDeskDALDataContext.SubmitChanges();
                    txtDescription.Text = "";

                    // Insert Log
                    Log.InsertLog(TaskID, UserId, String.Format("{0} updated comment.", GetUserName()));

                    // Upload the File
                    if (fuAttachment.HasFile)
                    {
                        UploadFileCommentEdit(objServiceDesk_TaskDetail.DetailID);
                    }

                    SetView("Default");
                    gvComments.DataBind();
                }
            }
        }
        #endregion

        #region UploadFileCommentEdit
        private void UploadFileCommentEdit(int intDetailID)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            string strUploadefFilesPath = (from ServiceDesk_Settings in objServiceDeskDALDataContext.ServiceDesk_Settings
                                           where ServiceDesk_Settings.PortalID == PortalId
                                           where ServiceDesk_Settings.SettingName == "UploadefFilesPath"
                                           select ServiceDesk_Settings).FirstOrDefault().SettingValue;

            EnsureDirectory(new System.IO.DirectoryInfo(strUploadefFilesPath));
            string strfilename = Convert.ToString(intDetailID) + "_" + GetRandomPassword() + Path.GetExtension(fuAttachment.FileName).ToLower();
            strUploadefFilesPath = strUploadefFilesPath + @"\" + strfilename;
            fuAttachment.SaveAs(strUploadefFilesPath);

            ServiceDesk_Attachment objServiceDesk_Attachment = new ServiceDesk_Attachment();
            objServiceDesk_Attachment.DetailID = intDetailID;
            objServiceDesk_Attachment.FileName = strfilename;
            objServiceDesk_Attachment.OriginalFileName = fuAttachment.FileName;
            objServiceDesk_Attachment.AttachmentPath = strUploadefFilesPath;
            objServiceDesk_Attachment.UserID = UserId;

            objServiceDeskDALDataContext.ServiceDesk_Attachments.InsertOnSubmit(objServiceDesk_Attachment);
            objServiceDeskDALDataContext.SubmitChanges();

            // Insert Log
            Log.InsertLog(TaskID, UserId, String.Format("{0} uploaded file '{1}'.", GetUserName(), fuAttachment.FileName));
        }
        #endregion

        // Emails

        #region NotifyAssignedGroupOfComment
        private void NotifyAssignedGroupOfComment(string strComment)
        {
            RoleController objRoleController = new RoleController();
            string strDescription = GetDescriptionOfTicket();

            // Send to Administrator Role
            string strAssignedRole = "Administrators";
            int intRole = GetAssignedRole();
            if (intRole > -1)
            {
                strAssignedRole = String.Format("{0}", objRoleController.GetRole(intRole, PortalId).RoleName);
            }
            else
            {
                strAssignedRole = GetAdminRole();
            }

            string strLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleID.ToString(), String.Format(@"&TaskID={0}", TaskID)), PortalSettings.PortalAlias.HTTPAlias);

            string strSubject = String.Format(Localization.GetString("HelpDeskTicketAtHasBeenupdated.Text", LocalResourceFile), Request.QueryString["TaskID"], PortalSettings.PortalAlias.HTTPAlias);
            string strBody = String.Format(Localization.GetString("HelpDeskTicketHasBeenupdated.Text", LocalResourceFile), Request.QueryString["TaskID"], strDescription);
            strBody = strBody + Environment.NewLine + Environment.NewLine;
            strBody = strBody + Localization.GetString("Comments.Text", LocalResourceFile) + Environment.NewLine;
            strBody = strBody + strComment;
            strBody = strBody + Environment.NewLine + Environment.NewLine;
            strBody = strBody + String.Format(Localization.GetString("YouMaySeeFullStatusHere.Text", LocalResourceFile), strLinkUrl);

            // Get all users in the AssignedRole Role
            ArrayList colAssignedRoleUsers = objRoleController.GetUsersByRoleName(PortalId, strAssignedRole);

            foreach (UserInfo objUserInfo in colAssignedRoleUsers)
            {
                DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, objUserInfo.Email, "", strSubject, strBody, "", "HTML", "", "", "", "");
            }

            Log.InsertLog(Convert.ToInt32(Request.QueryString["TaskID"]), UserId, String.Format(Localization.GetString("SentCommentTo.Text", LocalResourceFile), UserInfo.DisplayName, strAssignedRole));
        }
        #endregion

        #region NotifyRequestorOfComment
        private void NotifyRequestorOfComment(string strComment)
        {
            string strEmail = GetEmailOfRequestor();

            if (strEmail != "")
            {
                ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

                var result = (from ServiceDesk_TaskDetails in objServiceDeskDALDataContext.ServiceDesk_Tasks
                              where ServiceDesk_TaskDetails.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                              select ServiceDesk_TaskDetails).FirstOrDefault();

                if (result != null)
                {
                    string strLinkUrl = "";
                    if (result.RequesterUserID > -1)
                    {
                        // This is a registred User / Provide link to ticket
                        strLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleID.ToString(), String.Format(@"&TaskID={0}", TaskID)), PortalSettings.PortalAlias.HTTPAlias);
                    }
                    else
                    {
                        // This is NOT a registred User / Provide link to ticket with a password
                        strLinkUrl = Utility.FixURLLink(DotNetNuke.Common.Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "EditTask", "mid=" + ModuleID.ToString(), String.Format(@"&TaskID={0}&TP={1}", TaskID, result.TicketPassword)), PortalSettings.PortalAlias.HTTPAlias);
                    }

                    string strDescription = result.Description;
                    string strSubject = String.Format(Localization.GetString("HelpDeskTicketAtHasBeenupdated.Text", LocalResourceFile), Request.QueryString["TaskID"], PortalSettings.PortalAlias.HTTPAlias);
                    string strBody = String.Format(Localization.GetString("HelpDeskTicketHasBeenupdated.Text", LocalResourceFile), Request.QueryString["TaskID"], strDescription);
                    strBody = strBody + Environment.NewLine + Environment.NewLine;
                    strBody = strBody + Localization.GetString("Comments.Text", LocalResourceFile) + Environment.NewLine;
                    strBody = strBody + strComment;
                    strBody = strBody + Environment.NewLine + Environment.NewLine;
                    strBody = strBody + String.Format(Localization.GetString("YouMaySeeFullStatusHere.Text", LocalResourceFile), strLinkUrl);

                    DotNetNuke.Services.Mail.Mail.SendMail(PortalSettings.Email, strEmail, "", strSubject, strBody, "", "HTML", "", "", "", "");

                    Log.InsertLog(Convert.ToInt32(Request.QueryString["TaskID"]), UserId, String.Format(Localization.GetString("RequestorWasEmailed.Text", LocalResourceFile), strEmail, strComment));

                }
            }
        }
        #endregion

        #region GetAssignedRole
        private int GetAssignedRole()
        {
            int intRole = -1;

            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();
            var result = from ServiceDesk_TaskDetails in objServiceDeskDALDataContext.ServiceDesk_Tasks
                         where ServiceDesk_TaskDetails.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                         select ServiceDesk_TaskDetails;

            if (result != null)
            {
                intRole = result.FirstOrDefault().AssignedRoleID;
            }

            return intRole;
        }
        #endregion

        #region UserIsRequestor
        private bool UserIsRequestor()
        {
            bool isRequestor = false;

            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();
            var result = from ServiceDesk_TaskDetails in objServiceDeskDALDataContext.ServiceDesk_Tasks
                         where ServiceDesk_TaskDetails.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                         select ServiceDesk_TaskDetails;

            if (result != null)
            {
                if (UserId == result.FirstOrDefault().RequesterUserID)
                {
                    isRequestor = true;
                }
            }

            return isRequestor;
        }
        #endregion

        // Visible to Requestor CheckBox

        #region chkCommentVisibleEdit_CheckedChanged
        protected void chkCommentVisibleEdit_CheckedChanged(object sender, EventArgs e)
        {
            // Only Display Email to Requestor link if chkCommentVisibleEdit is checked
            lnkUpdateRequestor.Visible = chkCommentVisibleEdit.Checked;
            ImgEmailUser.Visible = chkCommentVisibleEdit.Checked;
        }
        #endregion

        #region chkCommentVisible_CheckedChanged
        protected void chkCommentVisible_CheckedChanged(object sender, EventArgs e)
        {
            // Only Display Email link if chkCommentVisibleEdit is checked
            btnInsertCommentAndEmail.Visible = chkCommentVisible.Checked;
        }
        #endregion

        // Utility

        #region GetEmailOfRequestor
        private string GetEmailOfRequestor()
        {
            string strEmail = "";
            int intTaskId = Convert.ToInt32(Request.QueryString["TaskID"]);

            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();
            var result = (from ServiceDesk_TaskDetails in objServiceDeskDALDataContext.ServiceDesk_Tasks
                          where ServiceDesk_TaskDetails.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                          select ServiceDesk_TaskDetails).FirstOrDefault();

            if (result != null)
            {
                if (result.RequesterUserID == -1)
                {
                    try
                    {
                        strEmail = result.RequesterEmail;
                    }
                    catch (Exception)
                    {
                        // User no longer exists
                        strEmail = "";
                    }
                }
                else
                {
                    try
                    {
                        strEmail = UserController.GetUser(PortalId, result.RequesterUserID, false).Email;
                    }
                    catch (Exception)
                    {
                        // User no longer exists
                        strEmail = "";
                    }
                }
            }

            return strEmail;
        }
        #endregion

        #region GetDescriptionOfTicket
        private string GetDescriptionOfTicket()
        {
            string strDescription = "";
            int intTaskId = Convert.ToInt32(Request.QueryString["TaskID"]);

            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();
            var result = (from ServiceDesk_TaskDetails in objServiceDeskDALDataContext.ServiceDesk_Tasks
                          where ServiceDesk_TaskDetails.TaskID == Convert.ToInt32(Request.QueryString["TaskID"])
                          select ServiceDesk_TaskDetails).FirstOrDefault();

            if (result != null)
            {
                strDescription = result.Description;
            }

            return strDescription;
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

        #region GetAdminRole
        private string GetAdminRole()
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            List<ServiceDesk_Setting> colServiceDesk_Setting = (from ServiceDesk_Settings in objServiceDeskDALDataContext.ServiceDesk_Settings
                                                                  where ServiceDesk_Settings.PortalID == PortalId
                                                                  select ServiceDesk_Settings).ToList();

            ServiceDesk_Setting objServiceDesk_Setting = colServiceDesk_Setting.Where(x => x.SettingName == "AdminRole").FirstOrDefault();

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
    }
}