using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI.Models
{
    public enum LogType
    {
        Loggin = 0,
        Logout = 1,
        PaidLeave = 2,
        UnpaidLeave = 3,
        SickLeave = 4

    }
    public class Log
    {
        public string OID { get; set; }
        public DateTime DTCreated { get; set; }
        public LogType Type { get; set; }
        public Employee Resource { get; set; }
        private string _comment = "";
        public string Comment { 
            get { return _comment; } 
            set { _comment = value; } }
    }

    public class HolidayLog:Log
    {
        public HolidayLog()
        {
            Console.WriteLine();
            //this.DateFrom = ParseDateInComment(Comment);
            //this.DateTill = ParseDateInComment(Comment,1);
        }

        public HolidayLog(string Comment )
        {
            this.DateFrom = ParseDateInComment(Comment);
            this.DateTill = ParseDateInComment(Comment,1);
            this.Approved = Comment.Contains("[1]");
            
            TimeSpan durationTmsp = this.DateTill.Subtract(this.DateFrom);
            this.Hours = Math.Round(durationTmsp.TotalDays*9.5, 2);
        }
        //public bool Approved { get { return Comment.Contains("[0]"); } }
        public bool Approved { get; set; }
        private DateTime _dateFrom;
        public DateTime DateFrom { 
            get { return _dateFrom; } 
            set { _dateFrom = value; } }
        private DateTime _dateTill;
        public DateTime DateTill { 
            get { return _dateTill; } 
            set { _dateTill = value; } }

        public double Hours { get; set; }
        public DateTime ParseDateInComment(string comment, int dateF=0)
        {
            long ticks = 0;
            if (string.IsNullOrEmpty(comment))
                return DateTime.MinValue;
            string[] ss= comment.Split('[');
            string[] bb = null;
            if (ss.Length > 0)
                bb = ss[1].Replace("]","").Split('-');
            if(bb.Length>0)
            long.TryParse(bb[dateF], out ticks);
            return new DateTime(ticks);
        }
    }
}
