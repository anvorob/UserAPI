using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAPI.Models;
namespace TestAPI.Services
{
    interface IExecutiveTask
    {
        bool SignOffBy(ExecutiveTask task, OfficeWorker worker);
        string AddComment(ExecutiveTask task, string comment);
    }
}
