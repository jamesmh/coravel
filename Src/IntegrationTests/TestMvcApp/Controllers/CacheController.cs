using System;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Cache.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TestMvcApp.Controllers
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
            bool wasCached = false;
            string BigData()
            {
                wasCached = true;
                return "Was Cached:";
            };

            var content = Content(
                this._cache.Remember("Remember", BigData, TimeSpan.FromSeconds(5))
            );

            content.Content += wasCached.ToString();

            return content;
        }

        public async Task<IActionResult> RememberAsync()
        {
            bool wasCached = false;
            async Task<string> BigData()
            {
                wasCached = true;
                await Task.Delay(0);
                 return "Was Cached:";
            };

            var content = Content(
                await this._cache.RememberAsync("RememberAsync", BigData, TimeSpan.FromSeconds(10))
            );

            content.Content += wasCached.ToString();

            return content;
        }

        public IActionResult Forever()
        {
            bool wasCached = false;
            string BigData()
            {
                wasCached = true;
                return "Was Cached:";
            };

            var content = Content(
                this._cache.Forever("Forever", BigData)
            );

            content.Content += wasCached.ToString();

            return content;
        }

        public async Task<IActionResult> ForeverAsync()
        {
            bool wasCached = false;
            async Task<string> BigData()
            {
                wasCached = true;
                await Task.Delay(0);
                return "Was Cached:";
            };

            var content = Content(
                await this._cache.ForeverAsync("ForeverAsync", BigData)
            );

            content.Content += wasCached.ToString();

            return content;
        }

        public IActionResult Forget(string key) {
            this._cache.Forget(key);
            return Ok();
        }

        public IActionResult Flush() {
            this._cache.Flush();
            return Ok();
        }
    }
}