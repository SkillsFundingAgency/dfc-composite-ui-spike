using System;
using System.Collections.Generic;
using System.Linq;
using Ncs.Prototype.Web.Web2.Data;

namespace Ncs.Prototype.Web.Web2.Services
{
    public class TradeService : ITradeService
    {
        private readonly List<Trade> _trades;

        public TradeService()
        {
            _trades = new List<Trade>();

            _trades.Add(new Trade()
            {
                Id = 1,
                Description = "General bar/pub work including the serving of alcohol",
                MinimumAge = 18,
                Title = "Bar staff",
                City = "Coventry",
                Category = "Social"
            });

            _trades.Add(new Trade()
            {
                Id = 2,
                Description = "General serving of food including the serving of alcohol",
                MinimumAge = 16,
                Title = "Waiting staff",
                City = "Coventry",
                Category = "Social"
            });

            _trades.Add(new Trade()
            {
                Id = 3,
                Description = "General security including external site patrolling / security",
                MinimumAge = 21,
                Title = "Security guard",
                City = "Solihull",
                Category = "Security"
            });

            _trades.Add(new Trade()
            {
                Id = 4,
                Description = "General security including site security / patrolling",
                MinimumAge = 21,
                Title = "Night watchman",
                City = "Solihull",
                Category = "Security"
            });

            _trades.Add(new Trade()
            {
                Id = 5,
                Description = "Car park attendant, including collection of monies and site security",
                MinimumAge = 21,
                Title = "Car park attendant",
                City = "Banbury",
                Category = "Security"
            });

            _trades.Add(new Trade()
            {
                Id = 6,
                Description = "Providing day to day assistance to a qualified plumber",
                MinimumAge = 16,
                Title = "Plumber's mate",
                City = "Solihull",
                Category = "Labourer"
            });

            _trades.Add(new Trade()
            {
                Id = 7,
                Description = "Providing day to day assistance to a qualified electrician",
                MinimumAge = 16,
                Title = "Electrician's mate",
                City = "Coventry",
                Category = "Labourer"
            });

            _trades.Add(new Trade()
            {
                Id = 8,
                Description = "Providing day to day assistance to a qualified brick layer",
                MinimumAge = 16,
                Title = "Bricklayer's mate",
                City = "Banbury",
                Category = "Labourer"
            });

        }


        public List<Trade> GetTrades(string city = null, string category = null, bool filter16Plus = false, bool filter18Plus = false, bool filter21Plus = false)
        {
            var results = _trades;

            if (!string.IsNullOrEmpty(city))
            {
                results = results.Where(x => x.City == city).ToList();
            }

            if (!string.IsNullOrEmpty(category))
            {
                results = results.Where(x => x.Category == category).ToList();
            }

            if (filter16Plus)
            {
                results = results.Where(x => x.MinimumAge >= 16).ToList();
            }

            if (filter18Plus)
            {
                results = results.Where(x => x.MinimumAge >= 18).ToList();
            }

            if (filter21Plus)
            {
                results = results.Where(x => x.MinimumAge >= 21).ToList();
            }

            return results;
        }

        public Trade GetTrade(int id)
        {
            return _trades.FirstOrDefault(x => x.Id == id);
        }
    }
}
