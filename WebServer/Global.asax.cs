using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using UserInfo;
namespace WebServer
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        public static UserInfoMain user = new UserInfoMain();
        protected void Application_Start()
        {
            user.btnConnect_Click();  
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
