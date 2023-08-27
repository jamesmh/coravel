using System;

namespace Coravel.Scheduling.Schedule.Zoned;

internal sealed class ZonedTime
{
    private readonly TimeZoneInfo _info;
    public ZonedTime(TimeZoneInfo info) => _info = info;

    public static ZonedTime AsUTC()
    {
        return new ZonedTime(TimeZoneInfo.Utc);
    }

    public DateTime Convert(DateTime time)
    {
        if(_info == TimeZoneInfo.Utc)
        {
            return time;
        }
        else {
            return TimeZoneInfo.ConvertTime(time, _info);
        }
    }
}