using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebServer;
using WebServer.Controllers;
namespace WebServer
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Routes.MapHttpRoute(
                name: "UserInfo",
                routeTemplate: "userinfo/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );            
        }
    }
}
