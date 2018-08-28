using System.Threading.Tasks;
using Coravel.Queuing.Interfaces;
using Demo.Invocables;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    public class SampleController : Controller
    {
        private IQueue _queue;

        public SampleController(IQueue queue)
        {
            this._queue = queue;
        }

        public IActionResult TriggerExpensiveStuff() {
            this._queue.QueueInvocable<DoExpensiveCalculationAndStore>();            
            return Ok();
        }
    }
}