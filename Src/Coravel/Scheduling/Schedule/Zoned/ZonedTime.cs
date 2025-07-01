using System;

namespace Coravel.Scheduling.Schedule.Zoned
{
    public class ZonedTime
    {
        private TimeZoneInfo _info;
        
        public TimeZoneInfo TimeZoneInfo => _info;
        
        public ZonedTime(TimeZoneInfo info)
        {
            this._info = info;
        }

        public static ZonedTime AsUTC()
        {
            return new ZonedTime(TimeZoneInfo.Utc);
        }

        public DateTime Convert(DateTime time)
        {
            if(this._info == TimeZoneInfo.Utc)
            {
                return time;
            }
            else {
                return TimeZoneInfo.ConvertTime(time, this._info);
            }
        }
    }
}