using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace geoDataRest.Data
{
    public class Connection
    {
        // Change database access string HERE
        // Currently using an Azure SQL Instance

        public static string connectionDS = "Data Source=miserver.database.windows.net;Persist Security Info=True;User ID=geouser;Password=ncA6yOkhusY7Njobcnas;Initial Catalog=geoData;";
    }
}