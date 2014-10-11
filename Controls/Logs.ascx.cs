using System;
using System.Linq;
using System.Web.UI.WebControls;
using ITIL.Modules.ServiceDesk;

namespace ITIL.Modules.ServiceDesk.Controls
{
    public partial class Logs : DotNetNuke.Entities.Modules.PortalModuleBase
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
            var result = from ServiceDesk_Logs in objServiceDeskDALDataContext.ServiceDesk_Logs
                         where ServiceDesk_Logs.TaskID == TaskID
                         select ServiceDesk_Logs;

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