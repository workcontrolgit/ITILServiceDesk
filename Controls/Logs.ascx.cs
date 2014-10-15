using System;
using System.Linq;
using System.Web.UI.WebControls;
using ITIL.Modules.ServiceDesk;

namespace ITIL.Modules.ServiceDesk.Controls
{
    public partial class Logs : ITILServiceDeskModuleBase
    {
        #region Properties
        public int TaskID
        {
            get { return Convert.ToInt32(ViewState["TaskID"]); }
            set { ViewState["TaskID"] = value; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        #region LDSLogs_Selecting
        protected void LDSLogs_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            ServiceDeskDALDataContext objServiceDeskDALDataContext = new ServiceDeskDALDataContext();
            var result = from ITILServiceDesk_Logs in objServiceDeskDALDataContext.ITILServiceDesk_Logs
                         where ITILServiceDesk_Logs.TaskID == TaskID
                         select ITILServiceDesk_Logs;

            e.Result = result;
        }
        #endregion

        #region RefreshLogs
        public void RefreshLogs()
        {
            gvLogs.DataBind();
        }
        #endregion
    }

}