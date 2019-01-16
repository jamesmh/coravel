using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coravel.Invocable;
using Coravel.Queuing.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace QueueWithCancellationTokens.Controllers
{
    public class QueueController : Controller
    {
        private static ConcurrentDictionary<Guid, CancellationTokenSource > _tokens = new ConcurrentDictionary<Guid, CancellationTokenSource >();

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IQueue _queue;

        public QueueController(IServiceScopeFactory scopeFactory, IQueue queue)
        {
            this._scopeFactory = scopeFactory;
            this._queue = queue;
        }

        [HttpGet]
        public IActionResult Start()
        {
            // Generate new token
           Guid newGuid = Guid.NewGuid();
           CancellationTokenSource  newToken = new CancellationTokenSource();

            _tokens.TryAdd(newGuid, newToken);

            // Launch invocable asyncronously in background.
            // This works because (a) IServiceScopeFactory and (b) IQueue are both singletons.
            // This is what IServiceScopedFactory is designed for.
            this._queue.QueueAsyncTask(async () => {
                using(var scope = this._scopeFactory.CreateScope())
                {
                    var invocable = scope.ServiceProvider.GetService(typeof(LongRunningTask)) as LongRunningTask;
                    invocable.SetToken(newToken.Token);
                    await invocable.Invoke();
                }
            });

            // Return guid to caller so he can cancel the Task later.
            return Json(newGuid);
        }

        [HttpGet]
        public IActionResult Stop([FromQuery] Guid guid)
        {
            CancellationTokenSource  token = _tokens.GetValueOrDefault(guid);
            token.Cancel();
            return Json("It worked!");
        }
    }

    public class LongRunningTask : IInvocable
    {
        private CancellationToken _token;
        public async Task Invoke()
        {
            while(!this._token.IsCancellationRequested)
            {
                Console.WriteLine("waiting...");
                await Task.Delay(1000);
            }
        }
        public void SetToken(CancellationToken token) => this._token = token;
    }
}