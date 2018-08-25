using System.Threading.Tasks;
using Coravel.Scheduling.Schedule;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TestMvcApp.Models;

namespace TestMvcApp.Controllers
{
    public class InvocableController : Controller
    {
        private IScheduler _scheduler;

        public InvocableController(IScheduler scheduler)
        {
            this._scheduler = scheduler;
        }

        public async Task<IActionResult> RunInvocableScheduledTask()
        {
            int currentCount = TestInvocableStaticRunCounter.RunCount;
            await (this._scheduler as Scheduler).RunSchedulerAsync();
            // if no equal throw error
            int expectedCount = currentCount + 1;

            if (expectedCount != TestInvocableStaticRunCounter.RunCount)
            {
                throw new System.Exception("RunInvocableScheduledTask test failed.");
            }

            return Ok();
        }
    }
}