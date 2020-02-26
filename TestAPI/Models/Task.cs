using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAPI.Models
{
    public class Task
    {
        public string OID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AssignedTo { get; set; }
        public string Priority { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime DTCreated { get; set; }
        public DateTime DTModified { get; set; }
        public DateTime DTDeleted { get; set; }
        public bool Completed { get; set; }
    }
}
