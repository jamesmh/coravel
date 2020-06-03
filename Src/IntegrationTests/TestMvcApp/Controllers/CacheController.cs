using System;
using System.Threading.Tasks;
using Coravel.Cache.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TestMvcApp.Controllers
{
    [Route("Cache")]
    public class CacheController : Controller
    {
        private ICache _cache;

        public CacheController(ICache cache)
        {
            this._cache = cache;
        }

        [Route("Remember")]
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

        [Route("RememberAsync")]
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

        [Route("Forever")]
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

        [Route("ForeverAsync")]
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

        [Route("HasAsync")]
        public async Task<IActionResult> HasAsync()
        {
            var content = Content(
                (await this._cache.HasAsync("Remember")).ToString()
            );
            return content;
        }

        [Route("GetAsync")]
        public async Task<IActionResult> GetAsync()
        {
            var content = Content(
                (await this._cache.GetAsync<string>("Remember")).ToString()
            );
            content.Content += "True";
            return content;
        }

        [Route("Forget")]
        public IActionResult Forget(string key)
        {
            this._cache.Forget(key);
            return Ok();
        }

        [Route("Flush")]
        public IActionResult Flush()
        {
            this._cache.Flush();
            return Ok();
        }
    }
}