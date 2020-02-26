using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAPI.Models;

namespace TestAPI.Services
{
    interface IProductionTask
    {
        bool ClaimTask(ProductionTask task, ProductionWorker worker);
        bool IsClaimed(ProductionTask task);
    }
}
