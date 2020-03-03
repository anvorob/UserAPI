using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI.Models
{
    public class Shift
    {
        public DayOfWeekFlag DayOfWeekFlags { get; set; }

        private DateTime _dtStart { get; set; }
        public DateTime StartDate { get { return _dtStart.Date; } set { _dtStart = value.Add(_dtStart.TimeOfDay); } }
        public TimeSpan StartTime { get { return _dtStart.TimeOfDay; } set { _dtStart = _dtStart.Date.Add(value); } }

        private int _duration;
        public TimeSpan Duration { get { return TimeSpan.FromSeconds(_duration); } set { _duration = (int)Math.Round(value.TotalSeconds); } }

        public TimeSpan EndTime { get { return StartTime.Add(Duration); } }
        public EventCategory Category { get; set; }
        
    }

    public enum DayOfWeekFlag
    {
        None = 0,
        Sunday = 1,
        Monday = 2,
        Tuesday = 4,
        Wednesday = 8,
        Thursday = 16,
        Friday = 32,
        Saturday = 64,
        Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday,
        Weekend = Saturday | Sunday,
        AllDays = Monday | Tuesday | Wednesday | Thursday | Friday | Saturday | Sunday
    }

    public enum EventCategory { Unknown, CanWorkTime, WorkingTime, NonWorkingTime }
}
