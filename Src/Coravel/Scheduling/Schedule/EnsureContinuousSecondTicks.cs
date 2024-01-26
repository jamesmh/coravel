using System;
using System.Collections.Generic;
using System.Linq;
using Coravel.Scheduling.Schedule.Helpers;

namespace Coravel.Scheduling.Schedule;

public class EnsureContinuousSecondTicks
{
    private DateTime previousTick;

    public EnsureContinuousSecondTicks(DateTime firstTick)
    {
        previousTick = firstTick;
    }

    /// <summary>
    /// Give this method when the next tick occurs and it will return any intermediary ticks that should
    /// have existed been the stored previous tick and the next one.
    /// </summary>
    /// <param name="nextTick"></param>
    /// <returns></returns>
    public IEnumerable<DateTime> GetTicksBetweenPreviousAndNext(DateTime nextTick)
    {
        // Starting at previousTick, we move ahead one second a time and record the next time until we get to the "nextTick".
        // Then we check if there are any missed ticks between the two.
        List<DateTime> missingTicks = null; // We don't want to commit any memory until we know for sure there's at least 1 missed tick.
        DateTime nextTickToTest = previousTick.PreciseUpToSecond().AddSeconds(1);
        while (nextTickToTest < nextTick.PreciseUpToSecond())
        {
            if (missingTicks is null)
            {
                missingTicks = new List<DateTime>();
            }
            missingTicks.Add(nextTickToTest);
            nextTickToTest = nextTickToTest.PreciseUpToSecond().AddSeconds(1);
        }

        return missingTicks ?? Enumerable.Empty<DateTime>();
    }

    public void SetNextTick(DateTime nextTick)
    {
        previousTick = nextTick;
    }
}
