﻿using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Ncs.Prototype.Web.Web2.Data;
using Ncs.Prototype.Web.Web2.Models;
using Ncs.Prototype.Web.Web2.Services;

namespace Ncs.Prototype.Web.Web2.Controllers
{
    public class TradeController : Controller
    {
        private readonly ITradeService _tradeService;

        public TradeController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }

        [HttpGet]
        public IActionResult Index(string category, string filter)
        {
            var vm = new TradeIndexViewModel();
            string city = ((string.Compare(filter, "Mine", true) == 0) ? GetCity() : string.Empty);
            bool filter16Plus = (string.Compare(filter, "16Plus", true) == 0);
            bool filter18Plus = (string.Compare(filter, "18Plus", true) == 0);
            bool filter21Plus = (string.Compare(filter, "21Plus", true) == 0);

            vm.Trades = _tradeService.GetTrades(city, category, filter16Plus, filter18Plus, filter21Plus);

            if (DateTime.Now.Second >= 45)
            {
                throw new System.Exception("kaboom");
            }
            if (DateTime.Now.Second > 10 && DateTime.Now.Second < 15)
            {
                System.Threading.Thread.Sleep(new TimeSpan(0, 0, 13));
            }

            return View(vm);
        }

        [HttpGet]
        public IActionResult Sidebar()
        {
            var vm = new SidebarViewModel();
            var trades = _tradeService.GetTrades();

            var categories = trades.GroupBy(g => g.Category)
                .Select(s => new Data.Category()
                {
                    Name = s.Key,
                    TradeCount = s.Count()
                }
                )
                .OrderBy(o => o.Name)
                .ToList();

            vm.Categories = categories;

            return View(vm);
        }

        [HttpGet]
        public IActionResult Navbar()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var vm = _tradeService.GetTrade(id);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Trade trade)
        {
            if (ModelState.IsValid)
            {
                bool isAuthenticated = User.Identity.IsAuthenticated;

                return RedirectToAction(nameof(Index));
            }

            return View(trade);
        }

        private string GetCity()
        {
            var result = string.Empty;
            var emailAddress = string.Empty;

            if (User.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                emailAddress = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
            }

            if (!string.IsNullOrWhiteSpace(emailAddress))
            {
                emailAddress = emailAddress.ToLower();
                if (emailAddress == "ilyasdhin@gmail.com")
                {
                    result = "Solihull";
                }
                else if (emailAddress == "ianshangout99@gmail.com")
                {
                    result = "Coventry";
                }
            }
            return result;
        }
    }
}