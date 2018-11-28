using System;
using System.Collections.Generic;

namespace Coravel.Scheduling.Schedule.Cron
{
    public class CronExpression
    {
        private string _minutes;
        private string _hours;
        private string _days;
        private string _months;
        private string _weekdays;

        public CronExpression(string expression) {
            var values = expression.Split(' ');
            if(values.Length != 5){
                throw new Exception($"Cron expression '{expression}' is malformed.");
            }

            this._minutes = values[0];
            this._hours = values[1];
            this._days = values[2];
            this._months = values[3];
            this._weekdays = values[4];
        }

        public CronExpression AppendWeekDay(DayOfWeek day) {
            int intDay = (int) day;

            if(this._weekdays == "*"){
                this._weekdays = intDay.ToString();
            }
            else {
                this._weekdays += "," + intDay.ToString();
            }

            return this;
        }

        public bool IsDue(DateTime time) {            
            return new CronExpressionPart(this._minutes, 60).IsDue(time.Minute)
                && new CronExpressionPart(this._hours, 24).IsDue(time.Hour)
                && new CronExpressionPart(this._days, 31).IsDue(time.Day)
                && new CronExpressionPart(this._months, 12).IsDue(time.Month)
                && new CronExpressionPart(this._weekdays, 7).IsDue((int) time.DayOfWeek);
        }
    }
}