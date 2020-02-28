using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAPI.Models;
namespace TestAPI.Services
{
    public interface ILogService
    {
        void AddLog(Log log);
        List<Log> GetLogs(string guid, DateTime from, DateTime till);
    }
}
