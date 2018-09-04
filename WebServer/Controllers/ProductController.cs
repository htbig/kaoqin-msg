using WebServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UserInfo;
using WebServer;

namespace WebServer.Controllers
{
    public class ProductController : ApiController
    {
        User[] products = new User[]
        {
            new User { sName = "ht"},
        };

        public IEnumerable<User> GetAllProducts()
        {
            return products;
        }

        public IHttpActionResult GetProduct(int id)
        {
            var product = products.FirstOrDefault((p) => p.idwFingerIndex == id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        public IHttpActionResult GetUserInfo(string id)
        {          
            WebServer.WebApiApplication.user.btnDownloadUserInfo_Click();
            return Ok(0);
        }

        public IHttpActionResult PostUserInfo(string id)
        {
            WebServer.WebApiApplication.user.btnUploadUserInfo_Click();
            return Ok(0);
        }
        public IHttpActionResult PostBatchUserInfo(string id)
        {
            WebServer.WebApiApplication.user.btnBatchUpdate_Click();
            return Ok(0);
        }
        [HttpPost]
        public IHttpActionResult PostAttLogs(dynamic obj)
        {
            string begin_time = Convert.ToString(obj.begin_time);
            string end_time = Convert.ToString(obj.end_time);
            bool bCvtBTime = false, bCvtETime = false ;
            DateTime t1, t2;
            bCvtBTime = DateTime.TryParse(begin_time, out t1);
            bCvtETime = DateTime.TryParse(end_time, out t2);
            if ((bCvtBTime ^ bCvtETime) == true || (bCvtBTime == true && (t2 < t1)))
            {
                System.Diagnostics.Debug.WriteLine("bad request");
                return Ok(0);
            }
            WebServer.WebApiApplication.user.btnGetGeneralLogData_Click(t1,t2);
            return Ok(0);
        }
    }
}