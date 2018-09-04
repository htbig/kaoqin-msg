using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServer.Models
{
    public class User
    {
        public int idwFingerIndex { get; set; }
        public string sdwEnrollNumber { get; set; }
        public string sName { get; set; }
        public string sPassword { get; set; }
        public string sTmpData { get; set; }
        public int iPrivilege { get; set; }
        public bool bEnabled { get; set; }
        public int iFlag { get; set; }
    }
}