using System.Threading.Tasks;
using YetAnotherTwitchBot.Models;

namespace YetAnotherTwitchBot.Interfaces
{
    public interface IExecutiveOrderService
    {
        ExecutiveOrder GetRandom();
        ExecutiveOrder Get(string OrderID);
        Task Init();
    }
}