using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SchedulerDemo.Models;
using Coravel.Queuing.Interfaces;

namespace SchedulerDemo.Controllers
{
    public class HomeController : Controller
    {
        IQueue _queue;

        public HomeController(IQueue queue) {
            this._queue = queue;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        // Home/QueueTask
        public IActionResult QueueTask() {
            this._queue.QueueTask(() => Console.WriteLine("This was queued!"));
            return Ok();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

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
