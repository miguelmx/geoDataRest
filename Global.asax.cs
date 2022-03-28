using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace geoDataRest
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            SqlServerTypes.Utilities.LoadNativeAssemblies(Server.MapPath("~/bin"));
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
