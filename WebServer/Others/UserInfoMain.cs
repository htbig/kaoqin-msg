/**********************************************************
 * Demo for Standalone SDK.Created by Darcy on Oct.15 2009*
***********************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using WebServer.Models;
using System.IO;
using System.Net;

using System.Threading;

namespace UserInfo
{
    public partial class UserInfoMain
    {
        public UserInfoMain(int machieNumber)
        {
            iMachineNumber = machieNumber;
            //InitializeComponent();
        }
        public static string[] names = { "name1", "name2", "name3", "name4", "name5", "name6", "name7", "name8" };
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
        private void tfn_trigger()
        {
            while (bIsConnected == true)
            {
                if (axCZKEM1.ReadRTLog(iMachineNumber))
                {
                    while (axCZKEM1.GetRTLog(iMachineNumber)) ;
                }
            }
        }
        //If your device supports the TCP/IP communications, you can refer to this.
        //when you are using the tcp/ip communication,you can distinguish different devices by their IP address.
        public void btnConnect_Click(string ip_addr/*object sender, EventArgs e*/)
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
                    axCZKEM1.OnAttTransactionEx += new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx);
                    //this.axCZKEM1.OnFingerFeature += new zkemkeeper._IZKEMEvents_OnFingerFeatureEventHandler(axCZKEM1_OnAttTransactionEx);
                    //this.axCZKEM1.OnEnrollFingerEx += new zkemkeeper._IZKEMEvents_OnEnrollFingerExEventHandler(axCZKEM1_OnAttTransactionEx);
                    //this.axCZKEM1.OnDeleteTemplate += new zkemkeeper._IZKEMEvents_OnDeleteTemplateEventHandler(axCZKEM1_OnAttTransactionEx);
                    //this.axCZKEM1.OnNewUser += new zkemkeeper._IZKEMEvents_OnNewUserEventHandler(axCZKEM1_OnAttTransactionEx);
                    //this.axCZKEM1.OnHIDNum += new zkemkeeper._IZKEMEvents_OnHIDNumEventHandler(axCZKEM1_OnAttTransactionEx);
                    //this.axCZKEM1.OnAlarm += new zkemkeeper._IZKEMEvents_OnAlarmEventHandler(axCZKEM1_OnAttTransactionEx);
                    //this.axCZKEM1.OnDoor += new zkemkeeper._IZKEMEvents_OnDoorEventHandler(axCZKEM1_OnAttTransactionEx);
                    //this.axCZKEM1.OnWriteCard += new zkemkeeper._IZKEMEvents_OnWriteCardEventHandler(axCZKEM1_OnAttTransactionEx);
                    //this.axCZKEM1.OnEmptyCard += new zkemkeeper._IZKEMEvents_OnEmptyCardEventHandler(axCZKEM1_OnAttTransactionEx);
                }
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
            }
            trigger_t = new Thread(tfn_trigger);
            trigger_t.Start();
        }

        #endregion

        /*************************************************************************************************
        * Before you refer to this demo,we strongly suggest you read the development manual deeply first.*
        * This part is for demonstrating operations with user(download/upload/delete/clear/modify).      *
        * ************************************************************************************************/
        #region UserInfo

        //Download user's 9.0 or 10.0 arithmetic fingerprint templates(in strings)
        //Only TFT screen devices with firmware version Ver 6.60 version later support function "GetUserTmpExStr" and "GetUserTmpEx".
        //'While you are using 9.0 fingerprint arithmetic and your device's firmware version is under ver6.60,you should use the functions "SSR_GetUserTmp" or 
        //"SSR_GetUserTmpStr" instead of "GetUserTmpExStr" or "GetUserTmpEx" in order to download the fingerprint templates.
        public void btnDownloadUserInfo_Click()
        {
            if (bIsConnected == false)
            {
                System.Console.Write("Please connect the device first!", "Error");
                return;
            }

            string sdwEnrollNumber = "";
            string sName = "";
            string sPassword = "";
            int iPrivilege = 0;
            bool bEnabled = false;

            int idwFingerIndex;
            string sTmpData = "";
            int iTmpLength = 0;
            int iFlag = 0;
            int iFaceIndex = 50;// 'the only possible parameter value
            string sTmpFaceData = "";
            int iFaceLength = 0;
            string sCardnumber = "";
            string S = "";
            S = "工号," + "姓名," + "指纹索引," + "指纹序列," + "等级," + "密码," + "使能," + "标记," + "人脸索引," + "人脸序列," + "人脸字节数," + "卡号";
            FileStream fs = new FileStream(logPath +"userInfo.csv", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(S);
            bool bHasFg = false;
            bool bHasFc = false;
            axCZKEM1.EnableDevice(iMachineNumber, false);
            axCZKEM1.ReadAllUserID(iMachineNumber);//read all the user information to the memory
            axCZKEM1.ReadAllTemplate(iMachineNumber);//read all the users' fingerprint templates to the memory
            while (axCZKEM1.SSR_GetAllUserInfo(iMachineNumber, out sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))//get all the users' information from the memory
            {
                for (idwFingerIndex = 0; idwFingerIndex < 10; idwFingerIndex++)
                {
                    if (axCZKEM1.GetUserTmpExStr(iMachineNumber, sdwEnrollNumber, idwFingerIndex, out iFlag, out sTmpData, out iTmpLength))//get the corresponding templates string and length from the memory
                    {
                        bHasFg = true;
                        S = sdwEnrollNumber + "," + sName + "," + idwFingerIndex + "," + sTmpData + "," + iPrivilege + "," + sPassword + "," + bEnabled + "," + iFlag;
                    }
                }
                if (axCZKEM1.GetUserFaceStr(iMachineNumber, sdwEnrollNumber, iFaceIndex, sTmpFaceData, iFaceLength)) {// 'get the face templates from the memory
                    if (bHasFg == false) {
                        S = sdwEnrollNumber + "," + sName + "," + "" + "," + "" + "," + iPrivilege + "," + sPassword + "," + bEnabled + "," + iFlag;
                    }
                    S = S + "," + iFaceIndex + "," + sTmpFaceData + "," + iFaceLength;
                    bHasFc = true;
                }
                if (axCZKEM1.GetStrCardNumber(out sCardnumber)) {// 'get the card number from the memory
                    if (bHasFg == false && bHasFc == false) {
                        S = sdwEnrollNumber + "," + sName + "," + "" + "," + "" + "," + iPrivilege + "," + sPassword + "," + bEnabled + "," + iFlag + "," + "" + "," + "" + "," + "";
                    } else if (bHasFc == false) {
                        S = S + "," + "" + "," + "" + "," + "";
                    }
                    S = S + "," + sCardnumber;
                 }
                sw.WriteLine(S);
                bHasFg = false;
                bHasFc = false;
            }
            sw.Close();
            fs.Close();
            axCZKEM1.EnableDevice(iMachineNumber, true);
        }

        //Upload the 9.0 or 10.0 fingerprint arithmetic templates to the device(in strings) in batches.
        //Only TFT screen devices with firmware version Ver 6.60 version later support function "SetUserTmpExStr" and "SetUserTmpEx".
        //While you are using 9.0 fingerprint arithmetic and your device's firmware version is under ver6.60,you should use the functions "SSR_SetUserTmp" or 
        //"SSR_SetUserTmpStr" instead of "SetUserTmpExStr" or "SetUserTmpEx" in order to upload the fingerprint templates.
        public void btnBatchUpdate_Click()
        {
            if (bIsConnected == false)
            {
                System.Console.Write("Please connect the device first!", "Error");
                return;
            }
            int idwErrorCode = 0;
            string sdwEnrollNumber = "";
            string sName = "";
            int idwFingerIndex = 0;
            string sTmpData = "";
            int iPrivilege = 0;
            string sPassword = "";
            string sEnabled = "";
            bool bEnabled = false;
            int iFlag = 1;
            bool bHasFace = false;
            int iFaceIndex = 0;
            string sTmpFaceData = "";
            int iTmpLength = 0;
            string[] sArray;
            StreamReader objReader = new StreamReader(logPath + "userInfo.csv");
            string sLine = "";
            sLine = objReader.ReadLine();
            sLine = objReader.ReadLine();
            int iUpdateFlag = 1;
            axCZKEM1.EnableDevice(iMachineNumber, false);
            if (axCZKEM1.BeginBatchUpdate(iMachineNumber, iUpdateFlag))//create memory space for batching data
            {
                string sLastEnrollNumber = "";//the former enrollnumber you have upload(define original value as 0)
                while (sLine != null)
                {
                    sArray = sLine.Split(',');
                    if (sArray.Length != 12)
                    {
                        System.Diagnostics.Debug.WriteLine("csv file not right");
                        return;
                    }
                    sdwEnrollNumber = sArray[0];
                    sName = sArray[1];
                    idwFingerIndex = int.Parse(sArray[2] == "" ? "0" : sArray[2]);
                    sTmpData = sArray[3];
                    iPrivilege = int.Parse(sArray[4]);
                    sPassword = sArray[5];
                    sEnabled = sArray[6];
                    iFlag = int.Parse(sArray[7]);
                    if (sArray[8] == "50")
                    {
                        bHasFace = true;
                        iFaceIndex = int.Parse(sArray[8]);
                        sTmpFaceData = sArray[9];
                        iTmpLength = int.Parse(sArray[10]);
                    }
                    //sCardnumber = long.Parse(sArray[11]);
                    if (sdwEnrollNumber != sLastEnrollNumber)
                    {
                        string hexString = sArray[11]; ;
                        int num = Int32.Parse(hexString, System.Globalization.NumberStyles.HexNumber);
                        axCZKEM1.SetStrCardNumber(num.ToString()); //Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
                        if (axCZKEM1.SSR_SetUserInfo(iMachineNumber, sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled))
                        {//upload user information to the device
                            axCZKEM1.SetUserTmpExStr(iMachineNumber, sdwEnrollNumber, idwFingerIndex, iFlag, sTmpData);// upload templates information to the device
                            if (bHasFace == true)
                            {
                                axCZKEM1.SetUserFaceStr(iMachineNumber, sdwEnrollNumber, iFaceIndex, sTmpFaceData, iTmpLength);//upload face templates information to the device
                                bHasFace = false;
                            }
                        }
                        else
                        {
                            axCZKEM1.GetLastError(ref idwErrorCode);
                            System.Diagnostics.Debug.WriteLine("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
                            axCZKEM1.EnableDevice(iMachineNumber, true);
                            return;
                        }
                    }
                    else//the current fingerprint and the former one belongs the same user,that is ,one user has more than one template
                    {
                        axCZKEM1.SetUserTmpExStr(iMachineNumber, sdwEnrollNumber, idwFingerIndex, iFlag, sTmpData);
                    }
                    sLastEnrollNumber = sdwEnrollNumber;//change the value of iLastEnrollNumber dynamicly
                    sLine = objReader.ReadLine();
                }
            }
            objReader.Close();
            axCZKEM1.BatchUpdate(iMachineNumber);//upload all the information in the memory
            axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
            axCZKEM1.EnableDevice(iMachineNumber, true);
            System.Diagnostics.Debug.WriteLine("Successfully upload fingerprint templates in batches , " + "total:" + "Success");
        }

        //Upload the 9.0 or 10.0 fingerprint arithmetic templates one by one(in strings)
        //Only TFT screen devices with firmware version Ver 6.60 version later support function "SetUserTmpExStr" and "SetUserTmpEx".
        //While you are using 9.0 fingerprint arithmetic and your device's firmware version is under ver6.60,you should use the functions "SSR_SetUserTmp" or 
        //"SSR_SetUserTmpStr" instead of "SetUserTmpExStr" or "SetUserTmpEx" in order to upload the fingerprint templates.
        public void btnUploadUserInfo_Click()
        {
            if (bIsConnected == false)
            {
                System.Console.Write("Please connect the device first!", "Error");
                return;
            }
            //int idwErrorCode = 0;
            string sdwEnrollNumber = "";
            string sName = "";
            int idwFingerIndex = 0;
            string sTmpData = "";
            int iPrivilege = 0;
            string sPassword = "";
            int iFlag = 0;
            bool  bHasFace = false;
            int iFaceIndex = 0; 
            string sEnabled = "";
            bool bEnabled = false;
            string sTmpFaceData="";
            int iTmpLength = 0;
            string[] sArray;
            StreamReader objReader = new StreamReader(logPath + "userInfo.csv");
            string sLine = "";
            sLine = objReader.ReadLine();
            sLine = objReader.ReadLine();
            axCZKEM1.EnableDevice(iMachineNumber, false);
            while (sLine != null)
            {
                sArray = sLine.Split(','); 
                if (sArray.Length != 12)
                {
                    System.Diagnostics.Debug.WriteLine("csv file not right");
                    return;
                }
                sdwEnrollNumber = sArray[0];
                sName = sArray[1];
                idwFingerIndex = int.Parse(sArray[2] == "" ? "0":sArray[2]);
                sTmpData = sArray[3];
                iPrivilege = int.Parse(sArray[4]);
                sPassword = sArray[5];
                sEnabled = sArray[6];
                iFlag = int.Parse(sArray[7]);
                if (sArray[8] == "50")
                {
                    bHasFace = true;
                    iFaceIndex = int.Parse (sArray[8]);
                    sTmpFaceData = sArray[9];
                    iTmpLength = int.Parse(sArray[10]);
                }
                axCZKEM1.SetStrCardNumber(sArray[11]); //Before you using function SetUserInfo,set the card number to make sure you can upload it to the device
                if (axCZKEM1.SSR_SetUserInfo(iMachineNumber, sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled)) {//upload user information to the device
                    axCZKEM1.SetUserTmpExStr(iMachineNumber, sdwEnrollNumber, idwFingerIndex, iFlag, sTmpData);// upload templates information to the device
                    if (bHasFace == true) {
                        axCZKEM1.SetUserFaceStr(iMachineNumber, sdwEnrollNumber, iFaceIndex, sTmpFaceData, iTmpLength);//upload face templates information to the device
                        bHasFace = false;
                    }
                }
                sLine = objReader.ReadLine();
            }
            objReader.Close();
            axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
            axCZKEM1.EnableDevice(iMachineNumber, true);
            System.Diagnostics.Debug.WriteLine("Successfully Upload fingerprint templates, " + "total:" + "Success");
        }

        public string btnGetGeneralLogData_Click(DateTime start_time, DateTime end_time) {
            if (bIsConnected == false)
            {
                System.Console.Write("Please connect the device first!", "Error");
                return "[]";
            }
            string sdwEnrollNumber = "";
            int idwVerifyMode;
            int idwInOutMode;
            int idwYear;
            int idwMonth;
            int idwDay;
            int idwHour;
            int idwMinute;
            int idwSecond;
            int idwWorkcode = 0;
            int idwErrorCode = 0;
            //string url = "http://10.4.32.248:8500/api/sw_version";
            //HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            //request.Method = "POST";
            //request.ContentType = "application/json";
            //request.Headers.Add("Authorization", "Basic YXBpOlkycGpjMnhvY0N4b2MyMTVaMk56");
            string data = "[";
            string S = "";
            S = "工号,验证方式,考勤状态,考勤时间,工作号,机器号\r\n";
            FileStream fs = new FileStream(logPath + "attLog-" + iMachineNumber.ToString() + ".csv", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(S);
            axCZKEM1.EnableDevice(iMachineNumber, false);// disable the device
            if (axCZKEM1.ReadGeneralLogData(iMachineNumber))
            {// read all the attendance records to the memory
                //get records from the memory
                while (axCZKEM1.SSR_GetGeneralLogData(iMachineNumber, out sdwEnrollNumber, out idwVerifyMode, out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))
                {
                    S = sdwEnrollNumber + "," + idwVerifyMode.ToString() + "," + idwInOutMode.ToString() + "," + idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " "
                    + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString() + "," + idwWorkcode.ToString() + "," + iMachineNumber.ToString();
                    sw.WriteLine(S);
                    if (start_time == DateTime.MinValue) { 
                        data += "{\"iMachineNumber\":" + iMachineNumber.ToString() + ",\"sMachineName\":\"" + names[iMachineNumber-1] + "\",\"sEnrollNumber\":\"" + sdwEnrollNumber +
                            "\",\"Time\":\"" + idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " +
                            idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString() + "\",\"VerifyMode\":" +
                            idwVerifyMode.ToString() + ",\"AttState\":" + idwInOutMode.ToString() +
                            "},";
                    }else
                    {
                        string logTime = idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " +
                            idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString();
                        DateTime logDateTime;
                        DateTime.TryParse(logTime, out logDateTime);
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
                //try
                //{
                //    byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());
                //    request.ContentLength = byteData.Length;
                //    using (Stream postStream = request.GetRequestStream())
                //    {
                //        postStream.Write(byteData, 0, byteData.Length);
                //    }

                //    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                //    {
                //        StreamReader reader = new StreamReader(response.GetResponseStream());
                //        System.Diagnostics.Debug.WriteLine(reader.ReadToEnd());
                //    }
                //}
                //    catch (Exception ex)
                //{
                //    System.Diagnostics.Debug.WriteLine(ex.Message);
                //}
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
            sw.Close();
            fs.Close(); 
            axCZKEM1.EnableDevice(iMachineNumber, true);// enable the device
            return data;
        }
        //Clear all attendance records from terminal
        public void btnClearGLog_Click()
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

        //Delete a certain user's fingerprint template of specified index
        //You shuold input the the user id and the fingerprint index you will delete
        //The difference between the two functions "SSR_DelUserTmpExt" and "SSR_DelUserTmp" is that the former supports 24 bits' user id.
        //private void btnSSR_DelUserTmpExt_Click(object sender, EventArgs e)
        //{
        //    if (bIsConnected == false)
        //    {
        //        MessageBox.Show("Please connect the device first!", "Error");
        //        return;
        //    }

        //    if (cbUserIDTmp.Text.Trim() == "" || cbFingerIndex.Text.Trim() == "")
        //    {
        //        MessageBox.Show("Please input the UserID and FingerIndex first!", "Error");
        //        return;
        //    }
        //    int idwErrorCode = 0;

        //    string sUserID = cbUserIDTmp.Text.Trim();
        //    int iFingerIndex = Convert.ToInt32(cbFingerIndex.Text.Trim());

        //    Cursor = Cursors.WaitCursor;
        //    if (axCZKEM1.SSR_DelUserTmpExt(iMachineNumber, sUserID, iFingerIndex))
        //    {
        //        axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
        //        MessageBox.Show("SSR_DelUserTmpExt,UserID:" + sUserID + " FingerIndex:" + iFingerIndex.ToString(), "Success");
        //    }
        //    else
        //    {
        //        axCZKEM1.GetLastError(ref idwErrorCode);
        //        MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
        //    }
        //    Cursor = Cursors.Default;
        //}

        //Clear all the fingerprint templates in the device(While the parameter DataFlag  of the Function "ClearData" is 2 )
        //private void btnClearDataTmps_Click(object sender, EventArgs e)
        //{
        //    if (bIsConnected == false)
        //    {
        //        MessageBox.Show("Please connect the device first!", "Error");
        //        return;
        //    }
        //    int idwErrorCode = 0;

        //    int iDataFlag = 2;

        //    Cursor = Cursors.WaitCursor;
        //    if (axCZKEM1.ClearData(iMachineNumber, iDataFlag))
        //    {
        //        axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
        //        MessageBox.Show("Clear all the fingerprint templates!", "Success");
        //    }
        //    else
        //    {
        //        axCZKEM1.GetLastError(ref idwErrorCode);
        //        MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
        //    }
        //    Cursor = Cursors.Default;
        //}

        //Delete all the user information in the device,while the related fingerprint templates will be deleted either. 
        //(While the parameter DataFlag  of the Function "ClearData" is 5 )
        //private void btnClearDataUserInfo_Click(object sender, EventArgs e)
        //{
        //    if (bIsConnected == false)
        //    {
        //        MessageBox.Show("Please connect the device first!", "Error");
        //        return;
        //    }
        //    int idwErrorCode = 0;

        //    int iDataFlag = 5;

        //    Cursor = Cursors.WaitCursor;
        //    if (axCZKEM1.ClearData(iMachineNumber, iDataFlag))
        //    {
        //        axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
        //        MessageBox.Show("Clear all the UserInfo data!", "Success");
        //    }
        //    else
        //    {
        //        axCZKEM1.GetLastError(ref idwErrorCode);
        //        MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
        //    }
        //    Cursor = Cursors.Default;
        //}

        //Delete a kind of data that some user has enrolled
        //The range of the Backup Number is from 0 to 9 and the specific meaning of Backup number is described in the development manual,pls refer to it.
        //private void btnDeleteEnrollData_Click(object sender, EventArgs e)
        //{
        //    if (bIsConnected == false)
        //    {
        //        MessageBox.Show("Please connect the device first!", "Error");
        //        return;
        //    }

        //    if (cbUserIDDE.Text.Trim() == "" || cbBackupDE.Text.Trim() == "")
        //    {
        //        MessageBox.Show("Please input the UserID and BackupNumber first!", "Error");
        //        return;
        //    }
        //    int idwErrorCode = 0;

        //    string sUserID = cbUserIDDE.Text.Trim();
        //    int iBackupNumber = Convert.ToInt32(cbBackupDE.Text.Trim());

        //    Cursor = Cursors.WaitCursor;
        //    if (axCZKEM1.SSR_DeleteEnrollData(iMachineNumber, sUserID, iBackupNumber))
        //    {
        //        axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
        //        MessageBox.Show("DeleteEnrollData,UserID=" + sUserID + " BackupNumber=" + iBackupNumber.ToString(), "Success");
        //    }
        //    else
        //    {
        //        axCZKEM1.GetLastError(ref idwErrorCode);
        //        MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
        //    }
        //    Cursor = Cursors.Default;
        //}

        //Clear all the administrator privilege(not clear the administrators themselves)
        //private void btnClearAdministrators_Click(object sender, EventArgs e)
        //{
        //    if (bIsConnected == false)
        //    {
        //        MessageBox.Show("Please connect the device first", "Error");
        //        return;
        //    }
        //    int idwErrorCode = 0;

        //    Cursor = Cursors.WaitCursor;
        //    if (axCZKEM1.ClearAdministrators(iMachineNumber))
        //    {
        //        axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
        //        MessageBox.Show("Successfully clear administrator privilege from teiminal!", "Success");
        //    }
        //    else
        //    {
        //        axCZKEM1.GetLastError(ref idwErrorCode);
        //        MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
        //    }
        //    Cursor = Cursors.Default;
        //}

        //Download users' face templates(in strings)(For TFT screen IFace series devices only)
        //private void btnDownLoadFace_Click(object sender, EventArgs e)
        //{
        //    if (bIsConnected == false)
        //    {
        //        MessageBox.Show("Please connect the device first!", "Error");
        //        return;
        //    }

        //    string sUserID = "";
        //    string sName = "";
        //    string sPassword = "";
        //    int iPrivilege = 0;
        //    bool bEnabled = false;
        //    int iFaceIndex = 50;//the only possible parameter value
        //    string sTmpData = "";
        //    int iLength = 0;

        //    lvFace.Items.Clear();
        //    lvFace.BeginUpdate();

        //    Cursor = Cursors.WaitCursor;
        //    axCZKEM1.EnableDevice(iMachineNumber, false);
        //    axCZKEM1.ReadAllUserID(iMachineNumber);//read all the user information to the memory

        //    while (axCZKEM1.SSR_GetAllUserInfo(iMachineNumber, out sUserID, out sName, out sPassword, out iPrivilege, out bEnabled))//get all the users' information from the memory
        //    {
        //        if (axCZKEM1.GetUserFaceStr(iMachineNumber, sUserID, iFaceIndex, ref sTmpData, ref iLength))//get the face templates from the memory
        //        {
        //            ListViewItem list = new ListViewItem();
        //            list.Text = sUserID;
        //            list.SubItems.Add(sName);
        //            list.SubItems.Add(sPassword);
        //            list.SubItems.Add(iPrivilege.ToString());
        //            list.SubItems.Add(iFaceIndex.ToString());
        //            list.SubItems.Add(sTmpData);
        //            list.SubItems.Add(iLength.ToString());
        //            if (bEnabled == true)
        //            {
        //                list.SubItems.Add("true");
        //            }
        //            else
        //            {
        //                list.SubItems.Add("false");
        //            }
        //            lvFace.Items.Add(list);
        //        }
        //    }
        //    axCZKEM1.EnableDevice(iMachineNumber, true);
        //    lvFace.EndUpdate();
        //    Cursor = Cursors.Default;
        //}

        //Upload users' face template(in strings)(For TFT screen IFace series devices only)
        //Uploading the face templates in batches is not supported temporarily.
        //private void btnUploadFace_Click(object sender, EventArgs e)
        //{
        //    if (bIsConnected == false)
        //    {
        //        MessageBox.Show("Please connect the device first!", "Error");
        //        return;
        //    }
        //    int idwErrorCode = 0;

        //    string sUserID = "";
        //    string sName = "";
        //    int iFaceIndex = 0;
        //    string sTmpData = "";
        //    int iLength = 0;
        //    int iPrivilege = 0;
        //    string sPassword = "";
        //    string sEnabled = "";
        //    bool bEnabled = false;

        //    Cursor = Cursors.WaitCursor;
        //    axCZKEM1.EnableDevice(iMachineNumber, false);
        //    for (int i = 0; i < lvFace.Items.Count; i++)
        //    {
        //        sUserID = lvFace.Items[i].SubItems[0].Text;
        //        sName = lvFace.Items[i].SubItems[1].Text;
        //        sPassword = lvFace.Items[i].SubItems[2].Text;
        //        iPrivilege = Convert.ToInt32(lvFace.Items[i].SubItems[3].Text);
        //        iFaceIndex = Convert.ToInt32(lvFace.Items[i].SubItems[4].Text);
        //        sTmpData = lvFace.Items[i].SubItems[5].Text;
        //        iLength = Convert.ToInt32(lvFace.Items[i].SubItems[6].Text);
        //        sEnabled = lvFace.Items[i].SubItems[7].Text;
        //        if (sEnabled == "true")
        //        {
        //            bEnabled = true;
        //        }
        //        else
        //        {
        //            bEnabled = false;
        //        }

        //        if (axCZKEM1.SSR_SetUserInfo(iMachineNumber, sUserID, sName, sPassword, iPrivilege, bEnabled))//face templates are part of users' information
        //        {
        //            axCZKEM1.SetUserFaceStr(iMachineNumber, sUserID, iFaceIndex, sTmpData, iLength);//upload face templates information to the device
        //        }
        //        else
        //        {
        //            axCZKEM1.GetLastError(ref idwErrorCode);
        //            MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
        //            Cursor = Cursors.Default;
        //            axCZKEM1.EnableDevice(iMachineNumber, true);
        //            return;
        //        }
        //    }

        //    axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
        //    Cursor = Cursors.Default;
        //    axCZKEM1.EnableDevice(iMachineNumber, true);
        //    MessageBox.Show("Successfully Upload the face templates, " + "total:" + lvFace.Items.Count.ToString(), "Success");
        //}


        //Delete a certain user's face template according to its id
        //private void btnDelUserFace_Click(object sender, EventArgs e)
        //{
        //    if (bIsConnected == false)
        //    {
        //        MessageBox.Show("Please connect the device first!", "Error");
        //        return;
        //    }

        //    if (cbUserID3.Text.Trim() == "")
        //    {
        //        MessageBox.Show("Please input the UserID first!", "Error");
        //        return;
        //    }
        //    int idwErrorCode = 0;

        //    string sUserID = cbUserID3.Text.Trim();
        //    int iFaceIndex = 50;

        //    Cursor = Cursors.WaitCursor;
        //    if (axCZKEM1.DelUserFace(iMachineNumber, sUserID, iFaceIndex))
        //    {
        //        axCZKEM1.RefreshData(iMachineNumber);
        //        MessageBox.Show("DelUserFace,UserID=" + sUserID, "Success");
        //    }
        //    else
        //    {
        //        axCZKEM1.GetLastError(ref idwErrorCode);
        //        MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
        //    }
        //    Cursor = Cursors.Default;

        //}

        //Download specified user's face template (in bytes array)    
        //You can refer to the part of "Udisk data Management" to learn how to manage the user's binary template(Get or Set)
        //private void btnGetUserFace_Click(object sender, EventArgs e)
        //{
        //    if (bIsConnected == false)
        //    {
        //        MessageBox.Show("Please connect the device first!", "Error");
        //        return;
        //    }

        //    if (cbUserID3.Text.Trim() == "")
        //    {
        //        MessageBox.Show("Please input the UserID first!", "Error");
        //        return;
        //    }
        //    int idwErrorCode = 0;

        //    string sUserID = cbUserID3.Text.Trim();
        //    int iFaceIndex = 50;//the only possible parameter value
        //    int iLength = 128 * 1024;//initialize the length(cannot be zero)
        //    byte[] byTmpData = new byte[iLength];

        //    Cursor = Cursors.WaitCursor;
        //    axCZKEM1.EnableDevice(iMachineNumber, false);

        //    if (axCZKEM1.GetUserFace(iMachineNumber, sUserID, iFaceIndex, ref byTmpData[0], ref iLength))
        //    {
        //        //Here you can manage the information of the face templates according to your request.(for example,you can sava them to the database)
        //        MessageBox.Show("GetUserFace,the  length of the bytes array byTmpData is " + iLength.ToString(), "Success");
        //    }
        //    else
        //    {
        //        axCZKEM1.GetLastError(ref idwErrorCode);
        //        MessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
        //    }

        //    axCZKEM1.EnableDevice(iMachineNumber, true);
        //    Cursor = Cursors.Default;
        //}

        //add by Darcy on Nov.23 2009
        //Add the existed userid to DropDownLists.
        //bool bAddControl = true;
        //private void UserIDTimer_Tick(object sender, EventArgs e)
        //{
        //    if (bIsConnected == false)
        //    {
        //        //cbUserIDDE.Items.Clear();
        //        //cbUserIDTmp.Items.Clear();
        //        bAddControl = true;
        //        return;
        //    }
        //    else
        //    {
        //        if (bAddControl == true)
        //        {
        //            string sEnrollNumber = "";
        //            string sName = "";
        //            string sPassword = "";
        //            int iPrivilege = 0;
        //            bool bEnabled = false;

        //            axCZKEM1.EnableDevice(iMachineNumber, false);
        //            axCZKEM1.ReadAllUserID(iMachineNumber);//read all the user information to the memory
        //            while (axCZKEM1.SSR_GetAllUserInfo(iMachineNumber, out sEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))
        //            {
        //                //cbUserIDDE.Items.Add(sEnrollNumber);
        //                //cbUserIDTmp.Items.Add(sEnrollNumber);
        //            }
        //            bAddControl = false;
        //            axCZKEM1.EnableDevice(iMachineNumber, true);
        //        }
        //        return;
        //    }
        //}
        #endregion
        #region RealTime Events
        //If your fingerprint(or your card or face) passes the verification,this event will be triggered
        private void axCZKEM1_OnAttTransactionEx(string sEnrollNumber, int iIsInValid, int iAttState, int iVerifyMethod, int iYear, int iMonth, int iDay, int iHour, int iMinute, int iSecond, int iWorkCode)
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
            string data = "{\"command\":\"send_msg\", \"access_token\":\"eyJ0eXAiOiJKV1QiLCJhbAbdOiJIUzI1NiJ9.eyJ1c2VyIUiiSFowMzg4MSJ9.iC29yeuDdd8YwtCk_ix2EZ1gBTNlxa3c5YPhCYUA2a\", \"userlist\":\""+sEnrollNumber+"\"," +
                "\"agentid\":13,\"text\":\"时间:"+ iYear.ToString() + "-" + iMonth.ToString() + "-" + iDay.ToString() + " " +
                iHour.ToString() + ":" + iMinute.ToString() + ":" + iSecond.ToString() +"\"}";
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
                System.Diagnostics.Debug.WriteLine(ex.Message);
                FileStream fs = new FileStream(logPath + "verifyError-" + iMachineNumber.ToString() + ".log", FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                string S = data + " failed to send to:" + ex.Message; 
                sw.WriteLine(S);
                sw.Close();
                fs.Close();
            }
        }
        //private void axCZKEM1_OnVerify(int iUserID)
        //{
        //    System.Diagnostics.Debug.WriteLine("verify has triggerd");
            //lbRTShow.Items.Add("RTEvent OnVerify Has been Triggered,Verifying...");
            //if (iUserID != -1)
            //{
            //    lbRTShow.Items.Add("Verified OK,the UserID is " + iUserID.ToString());
            //}
            //else
            //{
            //    lbRTShow.Items.Add("Verified Failed... ");
            //}
        //}
        #endregion
    }
}