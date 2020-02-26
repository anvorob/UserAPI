using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI.Models
{
    public class Department
    {
        public string OID { get; set; }
        public string Name { get; set; }
        public DateTime DTCreated { get; set; }
        public DateTime DTModified { get; set; }
        public DateTime DTDeleted { get; set; }
    }
}
