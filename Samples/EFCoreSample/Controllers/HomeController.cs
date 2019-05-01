using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.Mvc;
using test.Invocables;
using test.Models;

namespace test.Controllers
{
    public class HomeController : Controller
    {
        private IQueue _queue;

        public HomeController(IQueue queue)
        {
            this._queue = queue;
        }

        public IActionResult Index()
        {
            this._queue.QueueInvocable<TestInvocable>();
            this._queue.QueueInvocable<TestInvocable>();
            this._queue.QueueInvocable<TestInvocable>();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
