

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

            ServiceDesk_Log objServiceDesk_Log = new ServiceDesk_Log();
            objServiceDesk_Log.DateCreated = DateTime.Now;
            objServiceDesk_Log.LogDescription = LogDescription.Substring(0, LogDescription.Length>499 ? 499 : LogDescription.Length - 1); // Strings.Left(LogDescription, 499);
            objServiceDesk_Log.TaskID = TaskID;
            objServiceDesk_Log.UserID = UserID;

            objServiceDeskDALDataContext.ServiceDesk_Logs.InsertOnSubmit(objServiceDesk_Log);
            objServiceDeskDALDataContext.SubmitChanges();
        } 
        #endregion
    }
}
