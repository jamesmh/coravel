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

        public IActionResult Remember()
        {
            Func<string> bigData = () =>
            {
                Thread.Sleep(500); // Simulate some work.
                return "I AM A BIG PEICE OF DATA!";
            };

            var content = Content(
                this._cache.Remember("bigdata", bigData, TimeSpan.FromSeconds(10))
            );

            return content;
        }

        public async Task<IActionResult> RememberAsync()
        {

            Func<Task<string>> bigData = async () =>
            {
                await Task.Delay(500);
                return "I AM A BIG PEICE OF DATA!";
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
                return "I AM A BIG PEICE OF DATA!";
            };

            var content = Content(
                this._cache.Forever("bigdataforever", bigData)
            );

            return content;
        }
    }
}