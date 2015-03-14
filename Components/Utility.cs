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
using System.Text.RegularExpressions;
using DotNetNuke.Entities.Host;

namespace ITIL.Modules.ServiceDesk
{
    public static class Utility
    {
        public static string DisplayTypeAdministrator = "Administrator";
        public static string DisplayTypeRequestor = "Requestor";
        public static int ColumnDisplayWidth = 25;
        
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

            // Customization - token replacement
        #region ReplaceTicketToken
        public static string ReplaceTicketToken(string strBody, string strPasswordLinkUrl, ITILServiceDesk_Task objITILServiceDesk_Tasks)
        {
            DotNetNuke.Entities.Portals.PortalSettings objPortalSettings = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings();
            //TaskID
            if (strBody.Contains("[TaskID]"))
            {
                strBody = strBody.Replace("[TaskID]", objITILServiceDesk_Tasks.TaskID.ToString());
            }
            //PasswordLinkUrl
            if (strBody.Contains("[PasswordLinkUrl]"))
            {
                strBody = strBody.Replace("[PasswordLinkUrl]", strPasswordLinkUrl);
            }
            //Description
            if (strBody.Contains("[Description]"))
            {
                strBody = strBody.Replace("[Description]", objITILServiceDesk_Tasks.Description);

            }
            //Details
            if (strBody.Contains("[Details]"))
            {
                strBody = strBody.Replace("[Details]", GetDetailsOfTicket(objITILServiceDesk_Tasks.TaskID));
            }

            //Comments
            if (strBody.Contains("[Comments]"))
            {
                strBody = strBody.Replace("[Comments]", GetCommentsOfTicket(objITILServiceDesk_Tasks.TaskID));
            }
            //Requestor
            if (strBody.Contains("[Requestor]"))
            {
                strBody = strBody.Replace("[Requestor]", objITILServiceDesk_Tasks.RequesterName);
            }
            //Priority Name
            if (strBody.Contains("[PriorityName]"))
            {
                strBody = strBody.Replace("[PriorityName]", objITILServiceDesk_Tasks.Priority);
            }
            //Email
            if (strBody.Contains("[Email]"))
            {
                strBody = strBody.Replace("[Email]", GetEmailOfRequestor(objITILServiceDesk_Tasks.RequesterUserID, objITILServiceDesk_Tasks.RequesterEmail));
            }

            //CreatedDate
            if (strBody.Contains("[CreatedDate]"))
            {
                strBody = strBody.Replace("[CreatedDate]", objITILServiceDesk_Tasks.CreatedDate.ToShortDateString());
            }
            //DueDate
            if (strBody.Contains("[DueDate]"))
            {
                if (objITILServiceDesk_Tasks.DueDate.HasValue)
                {
                    strBody = strBody.Replace("[DueDate]", objITILServiceDesk_Tasks.DueDate.Value.ToShortDateString());
                }
                else
                {
                    strBody = strBody.Replace("[DueDate]", string.Empty);
                }

            }
            //Phone
            if (strBody.Contains("[Phone]"))
            {
                strBody = strBody.Replace("[Phone]", objITILServiceDesk_Tasks.RequesterPhone);
            }
            //Assigned
            if (strBody.Contains("[Assigned]"))
            {
                strBody = strBody.Replace("[Assigned]", GetGetAssignedRoleName(objITILServiceDesk_Tasks.AssignedRoleID));
            }
            //StatusName
            if (strBody.Contains("[StatusName]"))
            {
                strBody = strBody.Replace("[StatusName]", objITILServiceDesk_Tasks.Status);
            }

            //StartDate
            if (strBody.Contains("[StartDate]"))
            {
                if (objITILServiceDesk_Tasks.EstimatedStart.HasValue)
                {
                    strBody = strBody.Replace("[StartDate]", objITILServiceDesk_Tasks.EstimatedStart.Value.ToShortDateString());
                }
                else  //blank out token
                {
                    strBody = strBody.Replace("[StartDate]", string.Empty);
                }

            }
            //EstimatedHours
            if (strBody.Contains("[EstimatedHours]"))
            {
                if (objITILServiceDesk_Tasks.EstimatedHours.HasValue)
                {
                    strBody = strBody.Replace("[EstimatedHours]", objITILServiceDesk_Tasks.EstimatedHours.Value.ToString());
                }
                else  //blank out token
                {
                    strBody = strBody.Replace("[EstimatedHours]", string.Empty);
                }

            }
            //CompleteDate
            if (strBody.Contains("[CompleteDate]"))
            {
                if (objITILServiceDesk_Tasks.EstimatedCompletion.HasValue)
                {
                    strBody = strBody.Replace("[CompleteDate]", objITILServiceDesk_Tasks.EstimatedCompletion.Value.ToShortDateString());
                }
                else  //blank out token
                {
                    strBody = strBody.Replace("[CompleteDate]", string.Empty);
                }
            }



            return strBody;
        }

        #endregion

        // Customization - get detail
        #region GetDetailsOfTicket

        static string GetDetailsOfTicket(int TaskId)
        {
            string strDescription = "";

            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();
            ITILServiceDesk_TaskDetail objITILServiceDesk_TaskDetail = (from ITILServiceDesk_TaskDetails in objServiceDeskDALDataContext.ITILServiceDesk_TaskDetails
                                                                  where ITILServiceDesk_TaskDetails.TaskID == TaskId
                                                                  where (ITILServiceDesk_TaskDetails.DetailType == "Comment" || ITILServiceDesk_TaskDetails.DetailType == "Comment-Visible")
                                                                  orderby ITILServiceDesk_TaskDetails.DetailID
                                                                  select ITILServiceDesk_TaskDetails).FirstOrDefault();


            if (objITILServiceDesk_TaskDetail != null)
            {
                strDescription = objITILServiceDesk_TaskDetail.Description;
            }

            return strDescription;
        }
        #endregion

        #region GetCommentsOfTicket

        static string GetCommentsOfTicket(int TaskId)
        {
            string strComments = "";

            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();
            ITILServiceDesk_TaskDetail objITILServiceDesk_TaskDetail = (from ITILServiceDesk_TaskDetails in objServiceDeskDALDataContext.ITILServiceDesk_TaskDetails
                                                                  where ITILServiceDesk_TaskDetails.TaskID == TaskId
                                                                  where (ITILServiceDesk_TaskDetails.DetailType == "Comment" || ITILServiceDesk_TaskDetails.DetailType == "Comment-Visible")
                                                                  orderby ITILServiceDesk_TaskDetails.DetailID descending
                                                                  select ITILServiceDesk_TaskDetails).FirstOrDefault();


            if (objITILServiceDesk_TaskDetail != null)
            {
                strComments = objITILServiceDesk_TaskDetail.Description;
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
