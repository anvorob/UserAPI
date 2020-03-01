using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI.Models
{
    public class UserBatch
    {
        public List<User> UserList = new List<User>();
    }

    public class ReturnObject
    {
        public int CurrPage { get; set; }
        public int TotalPages { get; set; }
        public int Limit { get; set; }
        public List<User> UserList { get; set; }
        public int NumOfRecords { get; set; }

    }

    public class ReturnMessage
    {
        public string Message { get; set; }
        public ReturnMessage(string message)
        {
            Message = message;
        }
    }

    public class ShiftLog
    {
        public DateTime TimeFrom { get; set; }
        public DateTime TimeTill { get; set; }
        public double Hours { get; set; }

    }
}
