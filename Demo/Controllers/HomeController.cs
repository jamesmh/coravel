using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Demo.Models;
using Coravel.Queuing.Interfaces;
using System.Threading;
using Coravel.Scheduling.Schedule.Interfaces;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        IQueue _queue;
        IScheduler _scheduler;

        public HomeController(IQueue queue, IScheduler scheduler) {
            this._queue = queue;
            this._scheduler = scheduler;
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
            Thread.Sleep(5000);
            this._queue.QueueTask(() => Console.WriteLine("This was queued!"));
            Console.WriteLine("Task Queued");
            return Ok();
        }
        
        public IActionResult QueueTaskAsync() {
            this._queue.QueueAsyncTask(async() => {
                await Task.Delay(1000);
                Console.WriteLine("This was queued!");
                await Task.Delay(1000);
            });
            Console.WriteLine("Task Queued");
            return Ok();
        }

        public IActionResult ScheduleTask() {
            this._scheduler.Schedule(
                () => Console.WriteLine("Scheduled dynamically from http controller.")
            )
            .EveryMinute();

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
