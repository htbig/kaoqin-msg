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
        public static UserInfoMain[] users = new UserInfoMain[2];
        public static string[] ips = { "172.21.30.135", "172.21.30.195" };
        protected void Application_Start()
        {
            for (int i = 0; i < users.Length; i++)
            {
                users[i] = new UserInfoMain(i+1);
                users[i].btnConnect_Click(ips[i]);
            }
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
