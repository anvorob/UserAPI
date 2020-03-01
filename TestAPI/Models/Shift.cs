using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI.Models
{
    public class Shift
    {
        public string dayOfTheWeekFrom { get; set; }
        public string dayOfTheWeekTill { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
    }
}
