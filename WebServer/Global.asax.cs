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
        public static string[] ips = { "172.20.96.55",//for test
            "172.21.80.67", //新租办公区
            "172.21.76.68", //前台
            "172.21.76.69", //研发大办公室门口
            "172.21.76.71", //洗手间门口
            "172.21.76.72", //货梯门口
            "172.21.80.66", //采购            
            "172.21.96.64", //生产
            "172.21.96.65" //生产
         
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
