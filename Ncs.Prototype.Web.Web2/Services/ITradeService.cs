using System.Collections.Generic;
using Ncs.Prototype.Web.Web2.Data;

namespace Ncs.Prototype.Web.Web2.Services
{
    public interface ITradeService
    {
        Trade GetTrade(int id);
        List<Trade> GetTrades(string city = null, string category = null, bool filter16Plus = false, bool filter18Plus = false, bool filter21Plus = false);
    }
}