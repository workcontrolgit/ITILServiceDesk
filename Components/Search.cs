using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using ITIL.Modules.ServiceDesk.Data;
using DotNetNuke.Entities.Users;

namespace ITIL.Modules.ServiceDesk
{
    
    public class Search
    {
        public Search()
        {
        }

        public static DataSet GetUsers(int portalId)
        {
            return DataProvider.GetUsers(portalId);
        }
    }

}