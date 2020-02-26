using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI.Models
{
    public class ExecutiveTask:Task
    {
        public KeyValuePair<OfficeWorker,bool> SignOff { get; set; }
        public string Comment { get; set; }
    }
}
