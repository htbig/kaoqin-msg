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
            if (id == null)
            {
                id = "1";
            }
            int index = int.Parse(id);
            if (index >= WebServer.WebApiApplication.users.Length )
            {
                System.Diagnostics.Debug.WriteLine("has no machine number");
                return Ok(-1);
            }
            WebServer.WebApiApplication.users[index].btnDownloadUserInfo_Click();
            return Ok(0);
        }
        [HttpPost]
        public IHttpActionResult PostUserInfo(string id)
        {
            if (id == null)
            {
                id = "1";
            }
            int index = int.Parse(id);
            if (index >= WebServer.WebApiApplication.users.Length)
            {
                System.Diagnostics.Debug.WriteLine("has no machine number");
                return Ok(-1);
            }
            WebServer.WebApiApplication.users[index].btnUploadUserInfo_Click();
            return Ok(0);
        }
        [HttpPost]
        public IHttpActionResult PostBatchUserInfo(string id)
        {
            if (id == null)
            {
                id = "1";
            }
            int index = int.Parse(id);
            if (index >= WebServer.WebApiApplication.users.Length)
            {
                System.Diagnostics.Debug.WriteLine("has no machine number");
                return Ok(-1);
            }
            WebServer.WebApiApplication.users[index].btnBatchUpdate_Click();
            return Ok(0);
        }
        [HttpPost]
        public IHttpActionResult PostAttLogs(dynamic obj)
        {
            string begin_time = Convert.ToString(obj.begin_time);
            string end_time = Convert.ToString(obj.end_time);
            int id = Convert.ToInt32(obj.id);
            if (id >= WebServer.WebApiApplication.users.Length)
            {
                System.Diagnostics.Debug.WriteLine("has no machine number");
                return Ok(-1);
            }
            bool bCvtBTime = false, bCvtETime = false ;
            DateTime t1, t2;
            bCvtBTime = DateTime.TryParse(begin_time, out t1);
            bCvtETime = DateTime.TryParse(end_time, out t2);
            if ((bCvtBTime ^ bCvtETime) == true || (bCvtBTime == true && (t2 < t1)))
            {
                System.Diagnostics.Debug.WriteLine("bad parameter");
                return Ok(0);
            }
            WebServer.WebApiApplication.users[id].btnGetGeneralLogData_Click(t1,t2);
            return Ok(0);
        }
        [HttpPost]
        public IHttpActionResult DeleteAttLogs(string id)
        {
            int index = Convert.ToInt32(id);
            if (index >= WebServer.WebApiApplication.users.Length)
            {
                System.Diagnostics.Debug.WriteLine("has no machine number");
                return Ok(-1);
            }
            WebServer.WebApiApplication.users[index].btnClearGLog_Click();
            return Ok(0);
        }
    }
}