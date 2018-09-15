using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using WebServer;
using System.Dynamic;

namespace OWIN_SignalR.Controller
{
    public class SignalRController : ApiController
    {
        private int downLoadUserInfoTask(object index)
        {
            int id = Convert.ToInt32(index);
            WebServer.WebApiApplication.users[id - 1].btnDownloadUserInfo_Click();
            System.Diagnostics.Debug.WriteLine("download user info successfull!");
            return 1;
        }
        async Task AsyncGetUserInfo(int index)
        {
            var task = Task<int>.Factory.StartNew(new Func<object, int>(downLoadUserInfoTask), index);
            await task;
        }
        [HttpGet]
        public void GetUserInfo(string id)
        {
            if (id == null)
            {
                id = "1";
            }
            int index = int.Parse(id);
            if (index > WebServer.WebApiApplication.users.Length || index < 1)
            {
                System.Diagnostics.Debug.WriteLine("has no machine number");
                return;
            }
            AsyncGetUserInfo(index);
        }
   
        private int batchUpLoadUserInfoTask(object index)
        {
            int id = Convert.ToInt32(index);
            WebServer.WebApiApplication.users[id - 1].btnBatchUpdate_Click();
            System.Diagnostics.Debug.WriteLine("batchUpLoad user info successfull!");
            return 1;
        }
        async Task AsyncBatchUserInfo(int index)
        {
            var task = Task<int>.Factory.StartNew(new Func<object, int>(batchUpLoadUserInfoTask), index);
            await task;
        }
        [HttpPut]
        public void PutBatchUserInfo(string id)
        {
            if (id == null)
            {
                id = "1";
            }
            int index = int.Parse(id);
            if (index > WebServer.WebApiApplication.users.Length || index < 1)
            {
                System.Diagnostics.Debug.WriteLine("has no machine number");
                return;
            }
            AsyncBatchUserInfo(index);
        }

        [HttpPost]
        public HttpResponseMessage PostAttLogs(dynamic obj)
        {
            try
            {
                string begin_time = Convert.ToString(obj.begin_time);
                string end_time = Convert.ToString(obj.end_time);
                int id = Convert.ToInt32(obj.id);
                if (id > WebServer.WebApiApplication.users.Length || id < 1)
                {
                    System.Diagnostics.Debug.WriteLine("has no machine number");
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent("{\"code\":1,\"msg\":\"has no such machine number\",\"output\":[]}", Encoding.UTF8, "application/json"),
                    };
                }
                bool bCvtBTime = false, bCvtETime = false;
                DateTime t1, t2;
                bCvtBTime = DateTime.TryParse(begin_time, out t1);
                bCvtETime = DateTime.TryParse(end_time, out t2);
                if ((bCvtBTime ^ bCvtETime) == true || (bCvtBTime == true && (t2 < t1)))
                {
                    System.Diagnostics.Debug.WriteLine("bad parameter");
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent("{\"code\":1,\"msg\":\"begin time must less than end time...\",\"output\":[]}", Encoding.UTF8, "application/json"),
                    };
                }
                string data = WebServer.WebApiApplication.users[id - 1].btnGetGeneralLogData_Click(t1, t2);
                return new HttpResponseMessage()
                {
                    Content = new StringContent("{\"code\":0,\"msg\":\"success\",\"output\":"+data+"}", Encoding.UTF8, "application/json"),
                };
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message );
                return new HttpResponseMessage()
                {
                    Content = new StringContent("{\"code\":1,\"msg\":\""+e.Message+"\",\"output\":[]}", Encoding.UTF8, "application/json"),
                };
            }
            
        }

        [HttpDelete]
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
            WebServer.WebApiApplication.users[index - 1].btnClearGLog_Click();
            System.Diagnostics.Debug.WriteLine("delete att logs successfull"+id);
            return Ok(0);
        }
    }
}
