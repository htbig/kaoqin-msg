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
        static int process = 0;
        static bool syncFlag = false;
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

        [HttpGet]
        public HttpResponseMessage GetSyncStatus()
        {
            return new HttpResponseMessage()
            {
                Content = new StringContent("{\"code\":0,\"msg\":\"success\",\"output\":{\"process\":"+ process +"}}", Encoding.UTF8, "application/json"),
            };
        }
        private int SyncUserInfoTask(object index)
        {
            int id = Convert.ToInt32(index);
            int total = WebServer.WebApiApplication.users.Length;
            int i = 0;
            try
            {
                WebServer.WebApiApplication.users[id - 1].btnDownloadUserInfo_Click();
                process = 20;
                System.Diagnostics.Debug.WriteLine("down load success");
                if (id == 1)
                {
                    for (i = 1; i < total; i++)
                    {
                        WebServer.WebApiApplication.users[i].btnBatchUpdate_Click();
                        process = +10;
                    }
                }
                else if (id == 9)
                {
                    for (i = 0; i < total - 1; i++)
                    {
                        WebServer.WebApiApplication.users[i].btnBatchUpdate_Click();
                        process = +10;
                    }
                }
                else
                {
                    for (i = 0; i < id - 1; i++)
                    {
                        WebServer.WebApiApplication.users[i].btnBatchUpdate_Click();
                        process = +10;
                    }
                    for (i = id; i < total; i++)
                    {
                        WebServer.WebApiApplication.users[i].btnBatchUpdate_Click();
                        process = +10;
                    }
                }
                System.Diagnostics.Debug.WriteLine("upload success");
                process = 100;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            syncFlag = false;
            return 1;
        }
        async Task AsyncSyncUserInfo(int index)
        {
            var task = Task<int>.Factory.StartNew(new Func<object, int>(SyncUserInfoTask), index);
            await task;
        }
        [HttpPut]
        public HttpResponseMessage Sync(dynamic obj)
        {
            if(syncFlag == true)
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent("{\"code\":1,\"msg\":\"" + "Synching" + "\",\"output\":[]}", Encoding.UTF8, "application/json"),
                };
            }
            process = 0;
            int id = 2; //default id is 2:前台
            try
            {
                id = Convert.ToInt32(obj.id);  //mathine id 1~9
                if (obj.id == null)
                {
                    id = 2;
                }
                if (id > WebServer.WebApiApplication.users.Length)
                {
                    System.Diagnostics.Debug.WriteLine("has no machine number");
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent("{\"code\":1,\"msg\":\"has no such machine number\",\"output\":[]}", Encoding.UTF8, "application/json"),
                    };
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return new HttpResponseMessage()
                {
                    Content = new StringContent("{\"code\":1,\"msg\":\"" + e.Message + "\",\"output\":[]}", Encoding.UTF8, "application/json"),
                };
            }
            syncFlag = true;
            AsyncSyncUserInfo(id);
            return new HttpResponseMessage()
            {
                Content = new StringContent("{\"code\":0,\"msg\":\"success\",\"output\":[]}", Encoding.UTF8, "application/json"),
            };
        }

        [HttpPost]
        public HttpResponseMessage PostAttLogs(dynamic obj)
        {
            try
            {
                string begin_time = Convert.ToString(obj.begin_time);
                string end_time = Convert.ToString(obj.end_time);
                int id = Convert.ToInt32(obj.id);
                if(obj.id == null)
                {
                    id = 0;
                }
                if (id > WebServer.WebApiApplication.users.Length/* || id < 1*/)
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
                string data = "";
                if (id != 0)
                {
                    data = WebServer.WebApiApplication.users[id - 1].btnGetGeneralLogData_Click(t1, t2);
                }else
                {
                    for (int i = 0; i < WebServer.WebApiApplication.users.Length; i++)
                    {
                        string data_tmp = WebServer.WebApiApplication.users[i].btnGetGeneralLogData_Click(t1, t2);
                        if (i == 0)
                            data += data_tmp;
                        else
                        {
                            data_tmp = data_tmp.TrimStart('[').TrimEnd(']');
                            if(data_tmp != "")
                            {
                                data = data.Insert((data.Length - 1), ",");
                            } 
                            data = data.Insert((data.Length - 1), data_tmp );
                        }
                    }
                }
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

        [HttpDelete]
        public HttpResponseMessage DeleteUser(dynamic obj)
        {
            try
            {
                int i = 0;
                string userid = Convert.ToString(obj.userid);
                if (obj.userid == null)
                {
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent("{\"code\":1,\"msg\":\"" + "no userid appoint" + "\",\"output\":[]}", Encoding.UTF8, "application/json"),
                    };
                }
                for(i = 0; i < WebServer.WebApiApplication.users.Length; i++)
                {
                    WebServer.WebApiApplication.users[i].btnDeleteEnrollData_Click(userid);
                }
                return new HttpResponseMessage()
                {
                    Content = new StringContent("{\"code\":0,\"msg\":\"success\",\"output\":[]}", Encoding.UTF8, "application/json"),
                };
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return new HttpResponseMessage()
                {
                    Content = new StringContent("{\"code\":1,\"msg\":\"" + e.Message + "\",\"output\":[]}", Encoding.UTF8, "application/json"),
                };
            }  
        }
        
    }
}
