
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using Microsoft.VisualBasic;

namespace ITIL.Modules.ServiceDesk.Controls
{
    public partial class Work : DotNetNuke.Entities.Modules.PortalModuleBase
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
            try
            {
                cmdtxtStartCalendar1.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtStartDay);
                cmdtxtStartCalendar2.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtStopDay);
                cmdtxtStartCalendar3.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtStartDayEdit);
                cmdtxtStartCalendar4.NavigateUrl = DotNetNuke.Common.Utilities.Calendar.InvokePopupCal(txtStopDayEdit);

                pnlInsertComment.GroupingText = Localization.GetString("pnlInsertComment.Text", LocalResourceFile);

                if (!Page.IsPostBack)
                {
                    // Insert Default dates and times
                    txtStartDay.Text = DateTime.Now.ToShortDateString();
                    txtStopDay.Text = DateTime.Now.ToShortDateString();
                    txtStartTime.Text = DateTime.Now.AddHours(-1).ToShortTimeString();
                    txtStopTime.Text = DateTime.Now.ToShortTimeString();

                    SetView("Default");

                    if (ViewOnly)
                    {
                        SetViewOnlyMode();
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        #region SetView
        public void SetView(string ViewMode)
        {
            if (ViewMode == "Default")
            {
                pnlInsertComment.Visible = true;
                //pnlTableHeader.Visible = true;
                pnlExistingComments.Visible = true;
                pnlEditComment.Visible = false;
            }

            if (ViewMode == "Edit")
            {
                pnlInsertComment.Visible = false;
                //pnlTableHeader.Visible = false;
                pnlExistingComments.Visible = false;
                pnlEditComment.Visible = true;
            }
        }
        #endregion

        #region SetViewOnlyMode
        private void SetViewOnlyMode()
        {
            lnkDelete.Visible = false;
            Image5.Visible = false;
            lnkUpdate.Visible = false;
            Image4.Visible = false;
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
        }
        #endregion

        #region InsertComment
        private void InsertComment()
        {
            if (txtComment.Text.Trim().Length > 0)
            {
                try
                {
                    // Try to Make Start and Stop Time
                    DateTime StartTime = Convert.ToDateTime(String.Format("{0} {1}", txtStartDay.Text, txtStartTime.Text));
                    DateTime StopTime = Convert.ToDateTime(String.Format("{0} {1}", txtStopDay.Text, txtStopTime.Text));
                }
                catch
                {
                    lblError.Text = Localization.GetString("MustProvideValidStarAndStopTimes.Text", LocalResourceFile);
                    return;
                }

                ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

                string strComment = txtComment.Text.Trim();

                // Save Task Details
                ServiceDesk_TaskDetail objServiceDesk_TaskDetail = new ServiceDesk_TaskDetail();

                objServiceDesk_TaskDetail.TaskID = TaskID;
                objServiceDesk_TaskDetail.Description = txtComment.Text.Trim();
                objServiceDesk_TaskDetail.InsertDate = DateTime.Now;
                objServiceDesk_TaskDetail.UserID = UserId;
                objServiceDesk_TaskDetail.DetailType = "Work";
                objServiceDesk_TaskDetail.StartTime = Convert.ToDateTime(String.Format("{0} {1}", txtStartDay.Text, txtStartTime.Text));
                objServiceDesk_TaskDetail.StopTime = Convert.ToDateTime(String.Format("{0} {1}", txtStopDay.Text, txtStopTime.Text));

                objServiceDeskDALDataContext.ServiceDesk_TaskDetails.InsertOnSubmit(objServiceDesk_TaskDetail);
                objServiceDeskDALDataContext.SubmitChanges();
                txtComment.Text = "";

                // Insert Log
                Log.InsertLog(TaskID, UserId, String.Format("{0} inserted Work comment.", GetUserName()));

                gvComments.DataBind();
            }
            else
            {
                lblError.Text = Localization.GetString("MustProvideADescription.Text", LocalResourceFile);
            }
        }
        #endregion

        #region LDSComments_Selecting
        protected void LDSComments_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();
            var result = from ServiceDesk_TaskDetails in objServiceDeskDALDataContext.ServiceDesk_TaskDetails
                         where ServiceDesk_TaskDetails.TaskID == TaskID
                         where (ServiceDesk_TaskDetails.DetailType == "Work")
                         select ServiceDesk_TaskDetails;

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
                    string mylblComment = lblComment.Text;
                    lblComment.Text = String.Format("{0}...", mylblComment.Substring(0,100));
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
                            gvlblUser.Text = String.Format("{0}...", strDisplayName.Substring(0, 25));
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


                // Time
                Label lblTimeSpan = (Label)objGridViewRow.FindControl("lblTimeSpan");
                try
                {

                    Label lblStartTime = (Label)objGridViewRow.FindControl("lblStartTime");
                    Label lblStopTime = (Label)objGridViewRow.FindControl("lblStopTime");

                    DateTime StartDate = Convert.ToDateTime(lblStartTime.Text);
                    DateTime StopDate = Convert.ToDateTime(lblStopTime.Text);
                    TimeSpan TimeDifference = StopDate.Subtract(StartDate);

                    // if no Days
                    if (TimeDifference.Days == 0)
                    {
                        if (TimeDifference.Hours == 0)
                        {
                            lblTimeSpan.Text = String.Format(Localization.GetString("Minute.Text", LocalResourceFile), TimeDifference.Minutes.ToString(), ((TimeDifference.Minutes > 1) ? "s" : ""));
                        }
                        else
                        {
                            lblTimeSpan.Text = String.Format(Localization.GetString("HoursandMinute.Text", LocalResourceFile), TimeDifference.Hours.ToString(), TimeDifference.Minutes.ToString(), ((TimeDifference.Minutes > 1) ? "s" : ""));
                        }
                    }
                    else
                    {
                        lblTimeSpan.Text = String.Format(Localization.GetString("DaysHoursMinutes.Text", LocalResourceFile), TimeDifference.Days.ToString(), ((TimeDifference.Days > 1) ? "s" : ""), TimeDifference.Hours.ToString(), TimeDifference.Minutes.ToString(), ((TimeDifference.Minutes > 1) ? "s" : ""));
                    }
                }
                catch (Exception ex)
                {
                    lblTimeSpan.Text = ex.Message;
                    Exceptions.ProcessModuleLoadException(this, ex);
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
                txtStartDayEdit.Text = objServiceDesk_TaskDetail.StartTime.Value.ToShortDateString();
                txtStopDayEdit.Text = objServiceDesk_TaskDetail.StopTime.Value.ToShortDateString();
                txtStartTimeEdit.Text = objServiceDesk_TaskDetail.StartTime.Value.ToShortTimeString();
                txtStopTimeEdit.Text = objServiceDesk_TaskDetail.StopTime.Value.ToShortTimeString();
                lblInsertDate.Text = String.Format("{0} {1}", objServiceDesk_TaskDetail.InsertDate.ToLongDateString(), objServiceDesk_TaskDetail.InsertDate.ToLongTimeString());
            }
        }
        #endregion

        #region lnkDelete_Click
        protected void lnkDelete_Click(object sender, EventArgs e)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            var objServiceDesk_TaskDetail = (from ServiceDesk_TaskDetails in objServiceDeskDALDataContext.ServiceDesk_TaskDetails
                                              where ServiceDesk_TaskDetails.DetailID == Convert.ToInt32(lblDetailID.Text)
                                              select ServiceDesk_TaskDetails).FirstOrDefault();

            // Delete the Record
            objServiceDeskDALDataContext.ServiceDesk_TaskDetails.DeleteOnSubmit(objServiceDesk_TaskDetail);
            objServiceDeskDALDataContext.SubmitChanges();

            // Insert Log
            Log.InsertLog(TaskID, UserId, String.Format("{0} deleted Work comment: {1}", GetUserName(), txtDescription.Text));

            SetView("Default");
            gvComments.DataBind();
        }
        #endregion

        #region lnkUpdate_Click
        protected void lnkUpdate_Click(object sender, EventArgs e)
        {
            UpdateComment();
        }
        #endregion

        #region UpdateComment
        private void UpdateComment()
        {
            try
            {
                // Try to Make Start and Stop Time
                DateTime StartTime = Convert.ToDateTime(String.Format("{0} {1}", txtStartDayEdit.Text, txtStartTimeEdit.Text));
                DateTime StopTime = Convert.ToDateTime(String.Format("{0} {1}", txtStopDayEdit.Text, txtStopTimeEdit.Text));
            }
            catch
            {
                lblErrorEditComment.Text = Localization.GetString("MustProvideValidStarAndStopTimes.Text", LocalResourceFile);
                return;
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
                    objServiceDesk_TaskDetail.DetailType = "Work";
                    objServiceDesk_TaskDetail.StartTime = Convert.ToDateTime(String.Format("{0} {1}", txtStartDayEdit.Text, txtStartTimeEdit.Text));
                    objServiceDesk_TaskDetail.StopTime = Convert.ToDateTime(String.Format("{0} {1}", txtStopDayEdit.Text, txtStopTimeEdit.Text));

                    objServiceDeskDALDataContext.SubmitChanges();
                    txtDescription.Text = "";

                    // Insert Log
                    Log.InsertLog(TaskID, UserId, String.Format("{0} updated Work comment.", GetUserName()));

                    SetView("Default");
                    gvComments.DataBind();
                }
            }
            else
            {
                lblErrorEditComment.Text = Localization.GetString("MustProvideADescription.Text", LocalResourceFile);
            }
        }
        #endregion

        // Utility

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
    }
}