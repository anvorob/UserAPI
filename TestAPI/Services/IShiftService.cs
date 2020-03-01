using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAPI.Models;
namespace TestAPI.Services
{
    public interface IShiftService
    {
        bool AddShift(Shift shift);
        bool UpdateShift(Shift shift);
        List<Shift> GetShifts();
        bool DeleteShift(string shiftID);
    }
}
