using System;
using System.Collections.Generic;
using System.Linq;
using Coravel.Scheduling.Schedule.Helpers;

namespace Coravel.Scheduling.Schedule.Cron
{
    /// <summary>
    /// Represents a cron expression "part" parser needed for determining when events ought to be due.
    /// </summary>
    public class CronExpressionPart
    {
        /// <summary>
        /// The cron expression used to determine when events are due.
        /// </summary>
        private string _expression;
        private int _timeParts;

        public CronExpressionPart(string expression, int timeParts)
        {
            this._expression = expression.Trim();
            this._timeParts = timeParts;
        }

        /// <summary>
        /// Based on the cron expression, is this DateTime due?
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public bool IsDue(int time)
        {
            return this.CronExpressionPartIsDue(time, this._expression, this._timeParts);
        }

        /// <summary>
        /// Generic method to check if a part of the cron expression is due.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="expression"></param>
        /// <param name="replaceZeroWith"></param>
        /// <returns></returns>
        private bool CronExpressionPartIsDue(int time, string expression, int replaceZeroWith)
        {
            if (expression == "*")
            {
                return true;
            }

            var isDivisibleUnit = expression.IndexOf("*/") > -1;

            if (isDivisibleUnit)
            {
                if (!int.TryParse(expression.Remove(0, 2), out var divisor))
                {
                    throw new Exception($"Cron entry '{expression}' is malformed.");
                }

                if (divisor == 0)
                {
                    throw new Exception($"Cron entry ${expression} is attempting division by zero.");
                }

                if (time == 0)
                {
                    time = replaceZeroWith;
                }

                return time % divisor == 0;
            }
            else
            {
                var values = this.GetCronIntArray(expression);
                return values.Contains(time);
            }
        }

        /// <summary>
        /// From the cron expression, get all the int values that are valid due times.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private IEnumerable<int> GetCronIntArray(string expression)
        {
            var isRange = expression.IndexOf('-') > -1;
            var isDelineatedArray = expression.IndexOf(',') > -1;

            if (isRange && isDelineatedArray)
            {
                throw new Exception($"Cron expression '{expression}' has mixed entry type.");
            }

            if (isRange)
            {
                return this.GetRange(expression);
            }
            else if (isDelineatedArray)
            {
                return this.GetDelineatedArray(expression);
            }
            else
            {
                return GetSpecifiedInt(expression);
            }
        }

        /// <summary>
        /// Get value from a cron expression with a single value.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static IEnumerable<int> GetSpecifiedInt(string expression)
        {
            bool parsed = int.TryParse(expression, out var parsedValue);

            if (!parsed)
            {
                throw new Exception($"Cron entry '{expression}' is malformed.");
            }

            return new int[] { parsedValue };
        }

        /// <summary>
        /// Get values from cron expression like '5,4,3'.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private IEnumerable<int> GetDelineatedArray(string expression)
        {
            var delineatedValues = expression.Split(',');
            foreach (var val in delineatedValues)
            {
                if (!int.TryParse(val, out var parsedValue))
                {
                    throw new Exception($"Cron entry '{expression}' is malformed.");
                }

                yield return parsedValue;
            }
        }

        /// <summary>
        /// Get values from cron expressio range (e.g.true '5-10').
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private IEnumerable<int> GetRange(string expression)
        {
            // e.g. "5-10"
            var range = expression.Split('-');
            bool firstParsed = int.TryParse(range[0], out var first);
            bool secondParsed = int.TryParse(range[1], out var second);

            if (!(firstParsed && secondParsed))
            {
                throw new Exception($"Cron expression ${expression} is malformed.");
            }

            return this.Between(first, second);
        }

        /// <summary>
        /// Get all int between (including) first and second.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private IEnumerable<int> Between(int first, int second)
        {
            for (int i = first; i <= second; i++)
            {
                yield return i;
            }
        }
    }
}