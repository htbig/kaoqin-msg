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
                if (id > WebServer.WebApiApplication.users.Length)
                {
                    System.Diagnostics.Debug.WriteLine("has no machine number");
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent("{\"code\":1,\"msg\":\"has no such machine number\",\"output\":[]}", Encoding.UTF8, "application/json"),
                    };
                }
                bool bCvtBTime = false, bCvtETime = false;
                bCvtBTime = DateTime.TryParse(begin_time, out DateTime t1);
                bCvtETime = DateTime.TryParse(end_time, out DateTime t2);
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
                    data = WebServer.WebApiApplication.users[id - 1].BtnGetGeneralLogData_Click(t1, t2);
                }else
                {
                    for (int i = 0; i < WebServer.WebApiApplication.users.Length; i++)
                    {
                        string data_tmp = WebServer.WebApiApplication.users[i].BtnGetGeneralLogData_Click(t1, t2);
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
            WebServer.WebApiApplication.users[index - 1].BtnClearGLog_Click();
            System.Diagnostics.Debug.WriteLine("delete att logs successfull"+id);
            return Ok(0);
        }    
    }
}
