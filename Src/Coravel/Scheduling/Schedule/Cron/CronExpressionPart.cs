using System;

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
        /// <param name="time"></param>
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
                    throw new MalformedCronExpressionException($"Cron entry '{expression}' is malformed.");
                }

                if (divisor == 0)
                {
                    throw new MalformedCronExpressionException($"Cron entry ${expression} is attempting division by zero.");
                }

                if (time == 0)
                {
                    time = replaceZeroWith;
                }

                return time % divisor == 0;
            }
            else
            {
                return new CronExpressionComplexPart(expression).CheckIfTimeIsDue(time);
            }
        }
    }
}