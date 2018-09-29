/**********************************************************
 * Demo for Standalone SDK.Created by Darcy on Oct.15 2009*
***********************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Net;

using System.Threading;
using System.Timers;
using WebServer;
namespace UserInfo
{
    public partial class UserInfoMain
    {
        public UserInfoMain(int machieNumber)
        {
            iMachineNumber = machieNumber;
            //InitializeComponent();
        }
        public static string[] names = {"测试", "前台", "研发大办公室门口", "洗手间门口", "货梯门口", "采购", "新租办公区", "生产1", "生产2" };
        //Create Standalone SDK class dynamicly.
        public zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();
        private string logPath = "D:\\kaoqin\\WebServer\\";/*"C:\\ustar\\WebServer\\";*/

        /*************************************************************************************************
        * Before you refer to this demo,we strongly suggest you read the development manual deeply first.*
        * This part is for demonstrating the communication with your device.                             *
        * ************************************************************************************************/
        #region Communication
        private bool bIsConnected = false;//the boolean value identifies whether the device is connected
        private int iMachineNumber;//the serial number of the device.After connecting the device ,this value will be changed.
        private Thread trigger_t;
        private Thread check_online;

        private void Tfn_trigger()
        {
            while (true)
            {
                if (bIsConnected == true)
                {
                    lock (axCZKEM1)
                    {
                        if (axCZKEM1.ReadRTLog(iMachineNumber))
                        {
                            while (axCZKEM1.GetRTLog(iMachineNumber)) ;
                        }
                    }
                    Thread.Sleep(1000);
                }
                else
                {
                    Thread.Sleep(30000);
                }
            }
        }
        private void Tfn_check_online()
        {
            while (true)
            {
                lock (axCZKEM1)
                {
                    if ((false == axCZKEM1.EnableDevice(iMachineNumber, false)) || (bIsConnected == false))// disable the device,if return value is false,it means the machine has disconnectted,need reconnect 
                    {
                        axCZKEM1.Disconnect();
                        axCZKEM1.OnAttTransactionEx -= new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(AxCZKEM1_OnAttTransactionEx);
                        bIsConnected = false;
                        BtnConnect_Click(WebServer.WebApiApplication.ips[iMachineNumber - 1]);
                    }

                }
                Thread.Sleep(30000);
            }
        }
        private void Sync_time(object source, ElapsedEventArgs e)
        {
            if (DateTime.Now.Hour == 23 && DateTime.Now.Minute == 59)
            {
                int idwYear = 0, idwMonth = 0, idwDay = 0, idwHour = 0, idwMinute = 0, idwSecond = 0;
                axCZKEM1.GetDeviceTime(iMachineNumber, ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond);
                string machieTime = idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " +
                            idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString();
                DateTime.TryParse(machieTime, out DateTime machineDateTime);
                if ((DateTime.Now - machineDateTime).TotalSeconds > 5)
                {
                    axCZKEM1.SetDeviceTime(iMachineNumber);
                }
            }                
        }
        public void StartUpTickJob()
        {
            trigger_t = new Thread(Tfn_trigger);
            trigger_t.Start();
            check_online = new Thread(Tfn_check_online);
            check_online.Start();
            System.Timers.Timer timer_sync = new System.Timers.Timer
            {
                Enabled = true,
                Interval = 60000//执行间隔时间,单位为毫秒;此时时间间隔为1分钟  
            };
            timer_sync.Start();
            timer_sync.Elapsed += new System.Timers.ElapsedEventHandler(Sync_time);
        }
        //If your device supports the TCP/IP communications, you can refer to this.
        //when you are using the tcp/ip communication,you can distinguish different devices by their IP address.
        public void BtnConnect_Click(string ip_addr/*object sender, EventArgs e*/)
        {            
            int idwErrorCode = 0;

            axCZKEM1.PullMode = 1;            
            bIsConnected = axCZKEM1.Connect_Net(ip_addr, 4370);
            if (bIsConnected == true)
            {              
                //iMachineNumber = 1;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                if (axCZKEM1.RegEvent(iMachineNumber, 65535))
                {//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                    //this.axCZKEM1.OnFinger += new zkemkeeper._IZKEMEvents_OnFingerEventHandler(axCZKEM1_OnAttTransactionEx);
                    //axCZKEM1.OnVerify += new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify);
                    axCZKEM1.OnAttTransactionEx += new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(AxCZKEM1_OnAttTransactionEx);
                    //this.axCZKEM1.OnFingerFeature += new zkemkeeper._IZKEMEvents_OnFingerFeatureEventHandler(axCZKEM1_OnAttTransactionEx);
                    //this.axCZKEM1.OnEnrollFingerEx += new zkemkeeper._IZKEMEvents_OnEnrollFingerExEventHandler(axCZKEM1_OnAttTransactionEx);
                    //this.axCZKEM1.OnDeleteTemplate += new zkemkeeper._IZKEMEvents_OnDeleteTemplateEventHandler(axCZKEM1_OnAttTransactionEx);
                    //this.axCZKEM1.OnNewUser += new zkemkeeper._IZKEMEvents_OnNewUserEventHandler(axCZKEM1_OnAttTransactionEx);
                    //this.axCZKEM1.OnHIDNum += new zkemkeeper._IZKEMEvents_OnHIDNumEventHandler(axCZKEM1_OnAttTransactionEx);
                    //this.axCZKEM1.OnAlarm += new zkemkeeper._IZKEMEvents_OnAlarmEventHandler(axCZKEM1_OnAlarm);
                    //this.axCZKEM1.OnDoor += new zkemkeeper._IZKEMEvents_OnDoorEventHandler(axCZKEM1_OnAttTransactionEx);
                    //this.axCZKEM1.OnWriteCard += new zkemkeeper._IZKEMEvents_OnWriteCardEventHandler(axCZKEM1_OnAttTransactionEx);
                    //this.axCZKEM1.OnEmptyCard += new zkemkeeper._IZKEMEvents_OnEmptyCardEventHandler(axCZKEM1_OnAttTransactionEx);
                    //axCZKEM1.OnDisConnected += new zkemkeeper._IZKEMEvents_OnDisConnectedEventHandler(axCZKEM1_OnDisConnected);
                }
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
            }
        }

        #endregion

        /*************************************************************************************************
        * Before you refer to this demo,we strongly suggest you read the development manual deeply first.*
        * This part is for demonstrating operations with user(download/upload/delete/clear/modify).      *
        * ************************************************************************************************/
        #region UserInfo

        public string BtnGetGeneralLogData_Click(DateTime start_time, DateTime end_time) {
            if (bIsConnected == false)
            {
                System.Console.Write("Please connect the device first!", "Error");
                return "[]";
            }
            string sdwEnrollNumber = "";
            int idwWorkcode = 0;
            int idwErrorCode = 0;
            //string url = "http://10.4.32.248:8500/api/sw_version";
            //HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //request.Method = "POST";
            //request.ContentType = "application/json";
            //request.Headers.Add("Authorization", "Basic YXBpOlkycGpjMnhvY0N4b2MyMTVaMk56");
            string data = "[";
            //string S = "";
            //S = "工号,验证方式,考勤状态,考勤时间,工作号,机器号\r\n";
            //FileStream fs = new FileStream(logPath + "attLog-" + iMachineNumber.ToString() + ".csv", FileMode.OpenOrCreate);
            //StreamWriter sw = new StreamWriter(fs);
            //sw.Write(S);
 
            lock (axCZKEM1)
            {
                axCZKEM1.EnableDevice(iMachineNumber, false);// disable the device,if return value is false,it means the machine has disconnectted,need reconnect 
                if (axCZKEM1.ReadGeneralLogData(iMachineNumber))
                {// read all the attendance records to the memory
                 //get records from the memory
                    while (axCZKEM1.SSR_GetGeneralLogData(iMachineNumber, out sdwEnrollNumber, out int idwVerifyMode, out int idwInOutMode, out int idwYear, out int idwMonth, out int idwDay, out int idwHour, out int idwMinute, out int idwSecond, ref idwWorkcode))
                    {
                        //S = sdwEnrollNumber + "," + idwVerifyMode.ToString() + "," + idwInOutMode.ToString() + "," + idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " "
                        //+ idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString() + "," + idwWorkcode.ToString() + "," + iMachineNumber.ToString();
                        //sw.WriteLine(S);
                        if (start_time == DateTime.MinValue)
                        {
                            data += "{\"iMachineNumber\":" + iMachineNumber.ToString() + ",\"sMachineName\":\"" + names[iMachineNumber - 1] + "\",\"sEnrollNumber\":\"" + sdwEnrollNumber +
                                "\",\"Time\":\"" + idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " +
                                idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString() + "\",\"VerifyMode\":" +
                                idwVerifyMode.ToString() + ",\"AttState\":" + idwInOutMode.ToString() +
                                "},";
                        }
                        else
                        {
                            string logTime = idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " +
                                idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString();
                            DateTime.TryParse(logTime, out DateTime logDateTime);
                            if (logDateTime > start_time && logDateTime < end_time)
                            {
                                data += "{\"iMachineNumber\":" + iMachineNumber.ToString() + ",\"sMachineName\":\"" + names[iMachineNumber - 1] + "\",\"sEnrollNumber\":\"" + sdwEnrollNumber +
                                "\",\"Time\":\"" + idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " +
                                idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString() + "\",\"VerifyMode\":" +
                                idwVerifyMode.ToString() + ",\"AttState\":" + idwInOutMode.ToString() +
                                "},";
                            }
                        }
                    }
                    if (data.Length > 1)
                    {
                        data = data.Substring(0, (data.Length - 1));
                    }
                }
                else
                {
                    axCZKEM1.GetLastError(idwErrorCode);
                    if (idwErrorCode != 0)
                    {
                        System.Console.Write("Reading data from terminal failed,ErrorCode: %d", idwErrorCode);
                    }
                    else
                    {
                        System.Console.Write("No data from terminal returns!", "Error");
                    }
                }
                data += "]";
                //sw.Close();
                //fs.Close(); 
                axCZKEM1.EnableDevice(iMachineNumber, true);// enable the device
            }
            return data;
        }
        //Clear all attendance records from terminal
        public void BtnClearGLog_Click()
        {
            if (bIsConnected == false)
            {
                System.Console.Write("Please connect the device first", "Error");
                return;
            }
            int idwErrorCode = 0;

            axCZKEM1.EnableDevice(iMachineNumber, false);//disable the device
            if (axCZKEM1.ClearGLog(iMachineNumber))
            {
                axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
                System.Console.Write("All att Logs have been cleared from teiminal!", "Success");
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                System.Console.Write("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device
        }

        #endregion
        #region RealTime Events
        //If your fingerprint(or your card or face) passes the verification,this event will be triggered
        private void AxCZKEM1_OnAttTransactionEx(string sEnrollNumber, int iIsInValid, int iAttState, int iVerifyMethod, int iYear, int iMonth, int iDay, int iHour, int iMinute, int iSecond, int iWorkCode)
        {
            string url = "http://vegstore.utstar.com.cn:8081/send_msg"; /*"http://10.4.32.248:8500/api/sw_version"*/
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";
            //request.Headers.Add("Authorization", "Basic YXBpOlkycGpjMnhvY0N4b2MyMTVaMk56");
            //string data = "{\"iMachineNumber\":"+ iMachineNumber.ToString()+ ",\"sMachineName\":\"" + names[iMachineNumber-1] + "\",\"sEnrollNumber\":"+sEnrollNumber +
            //    ",\"Time\":\""+ iYear.ToString() + "-" + iMonth.ToString() + "-" + iDay.ToString() + " " +
            //    iHour.ToString() + ":" + iMinute.ToString() + ":" + iSecond.ToString()+ "\",\"VerifyMode\":"+
            //    iVerifyMethod.ToString()+ ",\"AttState\":"+iAttState.ToString()+ ",\"isInvalid\":"+ iIsInValid.ToString()+
            //    "}";
            string logTime = iYear.ToString() + "-" + iMonth.ToString() + "-" + iDay.ToString() + " " +
                            iHour.ToString() + ":" + iMinute.ToString() + ":" + iSecond.ToString();
            DateTime.TryParse(logTime, out DateTime logDateTime);
            int idwYear=0, idwMonth=0, idwDay=0, idwHour=0, idwMinute=0, idwSecond=0;
            axCZKEM1.GetDeviceTime(iMachineNumber, ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond);
            string machieTime = idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " +
                            idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString();
            DateTime.TryParse(machieTime, out DateTime machineDateTime);
            if (machineDateTime == DateTime.MinValue)
            {
                return;
            }
            long i = Convert.ToInt32((machineDateTime - logDateTime).TotalSeconds);
            if (i > 5)
            {
                return;
            }
            string data = String.Format("\"command\":\"send_msg\", \"access_token\":\"eyJ0eXAiOiJKV1QiLCJhbAbdOiJIUzI1NiJ9.eyJ1c2VyIUiiSFowMzg4MSJ9.iC29yeuDdd8YwtCk_ix2EZ1gBTNlxa3c5YPhCYUA2a\", \"userlist\":\"{0:s}\",\"agentid\":13,\"text\":\"时间: {1:0000}-{2:00}-{3:00} {4:00}:{5:00}:{6:00},地址:{7:s}\"", sEnrollNumber, iYear, iMonth, iDay, iHour, iMinute, iSecond, names[iMachineNumber - 1]);
            data = "{" + data + "}";
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());
            request.ContentLength = byteData.Length;
            try
            {
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    System.Diagnostics.Debug.WriteLine(reader.ReadToEnd());
                }

            }
            catch (Exception ex)
            {
                //System.Diagnostics.Debug.WriteLine(ex.Message);
                FileStream fs = new FileStream(logPath + "verifyError-" + iMachineNumber.ToString() + ".log", FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                string S = data + " failed to send to:" + ex.Message; 
                sw.WriteLine(S);
                sw.Close();
                fs.Close();
            }
        }
        #endregion
    }
}