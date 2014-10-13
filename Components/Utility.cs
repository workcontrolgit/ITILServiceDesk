//
// www.adefhelpdesk.com
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
using System.Web.UI;
using System.Collections;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Portals;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DotNetNuke.Entities.Host;

namespace ITIL.Modules.ServiceDesk
{
    public static class Utility
    {
        #region FixURLLink
        public static string FixURLLink(string strURL, string strHTTPAlias)
        {
            string strFixedURL = strURL;

            // If http is not present add it
            if ((!strFixedURL.Contains("http://")) & (!strFixedURL.Contains("https://")))
            {
                strFixedURL = String.Format("http://{0}", strHTTPAlias) + strFixedURL;
            }

            return strFixedURL;
        }
        #endregion


        // ITIL Customization - adding png/zip
        //#region ValidFileExtensions
        //public static List<string> ValidFileExtensions = new List<string> { "gif", "jpg", "jpeg", "doc", "docx", "xls", "xlsx", "pdf", "png", "zip", "ppt", "pptx", "vsd", "vsdx", "cfm", "rar", "mpp", "mppx", "txt" };
        //#endregion

        // ITIL Customization - validate extension
        //#region IsValidExtension
        //public static bool IsValidExtension(string fileExtension, List<string> validFileExtensions)
        //{
        //    // default flag to true 
        //    bool flag = true;
        //    // find match using List.Find  method
        //    string result = validFileExtensions.Find(delegate(string Ext) { return Ext == fileExtension; });

        //    if (result == null) //extension not in allowable list
        //    {
        //        flag = false; //file upload extension validation fail
        //    }

        //    return flag;
        //}
        //#endregion

        public static bool IsAllowedExtension(string fileName, string extension)
        {
            //string extension = Path.GetExtension(fileName);

            //regex matches a dot followed by 1 or more chars followed by a semi-colon
            //regex is meant to block files like "foo.asp;.png" which can take advantage
            //of a vulnerability in IIS6 which treasts such files as .asp, not .png
            return !string.IsNullOrEmpty(extension)
                   && Host.AllowedExtensionWhitelist.IsAllowedExtension(extension)
                   && !Regex.IsMatch(fileName, @"\..+;");
        }

        #region IsValidEmail

        public static bool IsValidEmail(this string email)
        {
            var r = new Regex(@"^[\w\.\-]+@[a-zA-Z0-9\-]+(\.[a-zA-Z0-9\-]{1,})*(\.[a-zA-Z]{2,3}){1,2}$");

            return !string.IsNullOrEmpty(email) && r.IsMatch(email);
        }
        #endregion

            // ITIL Customization - token replacement
        #region ReplaceTicketToken
        public static string ReplaceTicketToken(string strBody, string strPasswordLinkUrl, ServiceDesk_Task objServiceDesk_Tasks)
        {
            DotNetNuke.Entities.Portals.PortalSettings objPortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings();
            //TaskID
            if (strBody.Contains("[TaskID]"))
            {
                strBody = strBody.Replace("[TaskID]", objServiceDesk_Tasks.TaskID.ToString());
            }
            //PasswordLinkUrl
            if (strBody.Contains("[PasswordLinkUrl]"))
            {
                strBody = strBody.Replace("[PasswordLinkUrl]", strPasswordLinkUrl);
            }
            //Description
            if (strBody.Contains("[Description]"))
            {
                strBody = strBody.Replace("[Description]", objServiceDesk_Tasks.Description);

            }
            //Details
            if (strBody.Contains("[Details]"))
            {
                strBody = strBody.Replace("[Details]", GetDetailsOfTicket(objServiceDesk_Tasks.TaskID));
            }

            //Comments
            if (strBody.Contains("[Comments]"))
            {
                strBody = strBody.Replace("[Comments]", GetCommentsOfTicket(objServiceDesk_Tasks.TaskID));
            }
            //Requestor
            if (strBody.Contains("[Requestor]"))
            {
                strBody = strBody.Replace("[Requestor]", objServiceDesk_Tasks.RequesterName);
            }
            //Priority Name
            if (strBody.Contains("[PriorityName]"))
            {
                strBody = strBody.Replace("[PriorityName]", objServiceDesk_Tasks.Priority);
            }
            //Email
            if (strBody.Contains("[Email]"))
            {
                strBody = strBody.Replace("[Email]", GetEmailOfRequestor(objServiceDesk_Tasks.RequesterUserID, objServiceDesk_Tasks.RequesterEmail));
            }

            //CreatedDate
            if (strBody.Contains("[CreatedDate]"))
            {
                strBody = strBody.Replace("[CreatedDate]", objServiceDesk_Tasks.CreatedDate.ToShortDateString());
            }
            //DueDate
            if (strBody.Contains("[DueDate]"))
            {
                if (objServiceDesk_Tasks.DueDate.HasValue)
                {
                    strBody = strBody.Replace("[DueDate]", objServiceDesk_Tasks.DueDate.Value.ToShortDateString());
                }
                else
                {
                    strBody = strBody.Replace("[DueDate]", string.Empty);
                }

            }
            //Phone
            if (strBody.Contains("[Phone]"))
            {
                strBody = strBody.Replace("[Phone]", objServiceDesk_Tasks.RequesterPhone);
            }
            //Assigned
            if (strBody.Contains("[Assigned]"))
            {
                strBody = strBody.Replace("[Assigned]", GetGetAssignedRoleName(objServiceDesk_Tasks.AssignedRoleID));
            }
            //StatusName
            if (strBody.Contains("[StatusName]"))
            {
                strBody = strBody.Replace("[StatusName]", objServiceDesk_Tasks.Status);
            }

            //StartDate
            if (strBody.Contains("[StartDate]"))
            {
                if (objServiceDesk_Tasks.EstimatedStart.HasValue)
                {
                    strBody = strBody.Replace("[StartDate]", objServiceDesk_Tasks.EstimatedStart.Value.ToShortDateString());
                }
                else  //blank out token
                {
                    strBody = strBody.Replace("[StartDate]", string.Empty);
                }

            }
            //EstimatedHours
            if (strBody.Contains("[EstimatedHours]"))
            {
                if (objServiceDesk_Tasks.EstimatedHours.HasValue)
                {
                    strBody = strBody.Replace("[EstimatedHours]", objServiceDesk_Tasks.EstimatedHours.Value.ToString());
                }
                else  //blank out token
                {
                    strBody = strBody.Replace("[EstimatedHours]", string.Empty);
                }

            }
            //CompleteDate
            if (strBody.Contains("[CompleteDate]"))
            {
                if (objServiceDesk_Tasks.EstimatedCompletion.HasValue)
                {
                    strBody = strBody.Replace("[CompleteDate]", objServiceDesk_Tasks.EstimatedCompletion.Value.ToShortDateString());
                }
                else  //blank out token
                {
                    strBody = strBody.Replace("[CompleteDate]", string.Empty);
                }
            }



            return strBody;
        }

        #endregion

        // ITIL Customization - get detail
        #region GetDetailsOfTicket

        static string GetDetailsOfTicket(int TaskId)
        {
            string strDescription = "";

            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();
            ServiceDesk_TaskDetail objServiceDesk_TaskDetail = (from ServiceDesk_TaskDetails in objServiceDeskDALDataContext.ServiceDesk_TaskDetails
                                                                  where ServiceDesk_TaskDetails.TaskID == TaskId
                                                                  where (ServiceDesk_TaskDetails.DetailType == "Comment" || ServiceDesk_TaskDetails.DetailType == "Comment-Visible")
                                                                  orderby ServiceDesk_TaskDetails.DetailID
                                                                  select ServiceDesk_TaskDetails).FirstOrDefault();


            if (objServiceDesk_TaskDetail != null)
            {
                strDescription = objServiceDesk_TaskDetail.Description;
            }

            return strDescription;
        }
        #endregion

        #region GetCommentsOfTicket

        static string GetCommentsOfTicket(int TaskId)
        {
            string strComments = "";

            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();
            ServiceDesk_TaskDetail objServiceDesk_TaskDetail = (from ServiceDesk_TaskDetails in objServiceDeskDALDataContext.ServiceDesk_TaskDetails
                                                                  where ServiceDesk_TaskDetails.TaskID == TaskId
                                                                  where (ServiceDesk_TaskDetails.DetailType == "Comment" || ServiceDesk_TaskDetails.DetailType == "Comment-Visible")
                                                                  orderby ServiceDesk_TaskDetails.DetailID descending
                                                                  select ServiceDesk_TaskDetails).FirstOrDefault();


            if (objServiceDesk_TaskDetail != null)
            {
                strComments = objServiceDesk_TaskDetail.Description;
            }

            return strComments;
        }
        #endregion

        #region GetEmailOfRequestor
        static string GetEmailOfRequestor(int RequesterUserID, string RequesterEmail)
        {
            string strEmail = "";

            if (RequesterUserID == -1)
            {
                try
                {
                    strEmail = RequesterEmail;
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
                    DotNetNuke.Entities.Portals.PortalSettings objPortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings();
                    strEmail = DotNetNuke.Entities.Users.UserController.GetUserById(objPortalSettings.PortalId, RequesterUserID).Email;
                }
                catch (Exception)
                {
                    // User no longer exists
                    strEmail = "";
                }
            }


            return strEmail;
        }
        #endregion

        #region GetAssignedRoleName
        static string GetGetAssignedRoleName(int AssignedRoleID)
        {
            string strAssignedRoleName = "";

            if (AssignedRoleID > -1)
            {
                try
                {
                    DotNetNuke.Security.Roles.RoleController objRoleController = new DotNetNuke.Security.Roles.RoleController();

                    DotNetNuke.Entities.Portals.PortalSettings objPortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings();
                    strAssignedRoleName = String.Format("{0}", objRoleController.GetRole(AssignedRoleID, objPortalSettings.PortalId).RoleName);
                }
                catch (Exception)
                {
                    // User no longer exists
                    strAssignedRoleName = "";
                }
            }


            return strAssignedRoleName;
        }
        #endregion



    }
}
