using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI.Models
{
    public enum LogType
    {
        Loggin = 0,
        Logout = 1

    }
    public class Log
    {
        public string OID { get; set; }
        public DateTime DTCreated { get; set; }
        public LogType Type { get; set; }
        public Employee Resource { get; set; }
        public string Comment { get; set; }
    }
}
