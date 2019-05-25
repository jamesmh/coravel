using System;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Cache.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    public class CacheController : Controller
    {
        private ICache _cache;

        public CacheController(ICache cache)
        {
            this._cache = cache;
        }

        public class t
        {
            public string name { get; set; }
            public DateTimeOffset setAt { get; set; }
        }

        public IActionResult Remember()
        {
            Func<t> bigData = () =>
            {
                Thread.Sleep(500); // Simulate some work.
                return new t
                {
                    name = "James",
                    setAt = DateTimeOffset.UtcNow
                };
            };

            t model = this._cache.Remember("bigdata", bigData, TimeSpan.FromSeconds(10));

            var content = Content(
                $"name = {model.name}, number = {model.setAt}"
            );

            return content;
        }

        public async Task<IActionResult> RememberAsync()
        {

            Func<Task<string>> bigData = async () =>
            {
                await Task.Delay(500);
                return "I AM A BIG PEICE OF DATA!" + DateTime.UtcNow;
            };

            var content = Content(
                await this._cache.RememberAsync("bigdataasync", bigData, TimeSpan.FromSeconds(10))
            );

            return content;
        }

        public IActionResult Forever()
        {
            Func<string> bigData = () =>
            {
                Thread.Sleep(500); // Simulate some work.
                return "I AM A BIG PEICE OF DATA!" + DateTime.UtcNow;
            };

            var content = Content(
                this._cache.Forever("bigdataforever", bigData)
            );

            return content;
        }

        public async Task<IActionResult> ForeverAsync()
        {
            Func<Task<string>> bigData = async () =>
            {
                await Task.Delay(500); // Simulate some work.
                return "I AM A BIG PEICE OF DATA!" + DateTime.UtcNow;
            };

            var content = Content(
                await this._cache.ForeverAsync("bigdataforever", bigData)
            );

            return content;
        }

        public IActionResult Flush()
        {
            this._cache.Flush();

            var content = Content(
                "flushed"
            );

            return content;
        }

        public IActionResult Forget([FromQuery] string key)
        {
            this._cache.Forget(key);

            var content = Content(
                "forgot"
            );

            return content;
        }
    }
}