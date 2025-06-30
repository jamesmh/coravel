using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coravel.Queuing
{
    public class QueueOptions
    {
        /// <summary>
        /// This determines how often (in seconds) the queue host will consume all pending tasks.
        /// </summary>
        public int? ConsummationDelay { get; set; }

        public int GetConsummationDelay(IConfiguration configuration)
        {
            if (ConsummationDelay.HasValue) return ConsummationDelay.Value;

            var configurationSection = configuration.GetSection("Coravel:Queue:ConsummationDelay");
            bool couldParseDelay = int.TryParse(configurationSection.Value, out var parsedDelay);
            return couldParseDelay ? parsedDelay : 30;
        }
    }
}
