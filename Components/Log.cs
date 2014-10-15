

using System;
using System.Linq;
using System.Web.UI;
using System.Collections;
using System.Web.UI.WebControls;
using Microsoft.VisualBasic;

namespace ITIL.Modules.ServiceDesk
{
    public class Log
    {
        public Log()
        {
        }

        #region InsertLog
        public static void InsertLog(int TaskID, int UserID, string LogDescription)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();

            ITILServiceDesk_Log objITILServiceDesk_Log = new ITILServiceDesk_Log();
            objITILServiceDesk_Log.DateCreated = DateTime.Now;
            objITILServiceDesk_Log.LogDescription = LogDescription.Substring(0, LogDescription.Length>499 ? 499 : LogDescription.Length - 1); // Strings.Left(LogDescription, 499);
            objITILServiceDesk_Log.TaskID = TaskID;
            objITILServiceDesk_Log.UserID = UserID;

            objServiceDeskDALDataContext.ITILServiceDesk_Logs.InsertOnSubmit(objITILServiceDesk_Log);
            objServiceDeskDALDataContext.SubmitChanges();
        } 
        #endregion
    }
}
