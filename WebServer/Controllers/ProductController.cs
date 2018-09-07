using WebServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
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
            if (index > WebServer.WebApiApplication.users.Length || index < 1)
            {
                System.Diagnostics.Debug.WriteLine("has no machine number");
                return Ok(-1);
            }
            WebServer.WebApiApplication.users[index-1].btnDownloadUserInfo_Click();
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
            if (index > WebServer.WebApiApplication.users.Length || index < 1)
            {
                System.Diagnostics.Debug.WriteLine("has no machine number");
                return Ok(-1);
            }
            WebServer.WebApiApplication.users[index-1].btnUploadUserInfo_Click();
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
            if (index > WebServer.WebApiApplication.users.Length || index < 1)
            {
                System.Diagnostics.Debug.WriteLine("has no machine number");
                return Ok(-1);
            }
            WebServer.WebApiApplication.users[index-1].btnBatchUpdate_Click();
            return Ok(0);
        }
        [HttpPost]
        public HttpResponseMessage PostAttLogs(dynamic obj)
        {
            string begin_time = Convert.ToString(obj.begin_time);
            string end_time = Convert.ToString(obj.end_time);
            int id = Convert.ToInt32(obj.id);
            if (id > WebServer.WebApiApplication.users.Length || id < 1)
            {
                System.Diagnostics.Debug.WriteLine("has no machine number");
                return new HttpResponseMessage()
                {
                    Content = new StringContent("[]", Encoding.UTF8, "application/json"),
                };
            }
            bool bCvtBTime = false, bCvtETime = false ;
            DateTime t1, t2;
            bCvtBTime = DateTime.TryParse(begin_time, out t1);
            bCvtETime = DateTime.TryParse(end_time, out t2);
            if ((bCvtBTime ^ bCvtETime) == true || (bCvtBTime == true && (t2 < t1)))
            {
                System.Diagnostics.Debug.WriteLine("bad parameter");
                return new HttpResponseMessage()
                {
                    Content = new StringContent("[]", Encoding.UTF8, "application/json"),
                };
            }
            string data = WebServer.WebApiApplication.users[id-1].btnGetGeneralLogData_Click(t1,t2);
            return new HttpResponseMessage()
            {
                Content = new StringContent(data, Encoding.UTF8, "application/json"),
            };
        }
        [HttpPost]
        public IHttpActionResult DeleteAttLogs(string id)
        {
            if (id == null)
            {
                id = "1";
            }
            int index = Convert.ToInt32(id);
            if (index > WebServer.WebApiApplication.users.Length || index < 1)
            {
                System.Diagnostics.Debug.WriteLine("has no machine number");
                return Ok(-1);
            }
            WebServer.WebApiApplication.users[index-1].btnClearGLog_Click();
            return Ok(0);
        }
    }
}