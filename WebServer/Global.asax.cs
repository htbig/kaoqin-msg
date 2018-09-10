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
        public static UserInfoMain[] users = new UserInfoMain[1];
        public static string[] ips = { "172.21.30.16",//for test
            "172.21.76.10",
            "172.21.76.11",
            "172.21.76.12",
            //"172.21.76.13",//reserved
            "172.21.80.10",
            "172.21.80.11",
            "172.21.80.12",
            //"172.21.80.13",//reserved
            "172.21.96.10",
            "172.21.96.11",
            //"172.21.96.12",//reserved
            //"172.21.96.13"//reserved
        };

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
