using System;
using System.Linq;
using Coravel.Scheduling.Schedule;
using Xunit;

namespace CoravelUnitTests.Scheduling;

public class EnsureContinuousSecondTicksTests
{
    public const int SecondsInMinute = 60;
    public const int SecondsInHour = 60 * 60;
    public const int SecondsInDay = 60 * 60 * 24;
    
    [Theory]
    // 1 second difference: no missing ticks
    [InlineData("2024-01-12T12:00:00.0000000Z", "2024-01-12T12:00:01.0000000Z")]
    [InlineData("2024-01-12T12:00:00.2000000Z", "2024-01-12T12:00:01.2000000Z")]
    [InlineData("2024-01-12T12:00:00.0000000Z", "2024-01-12T12:00:01.9990000Z")]
    [InlineData("2009-05-22T22:22:22.0000000Z", "2009-05-22T22:22:23.9999999Z")]
    
    // Having less than a second between: no missing ticks
    [InlineData("2009-05-22T22:22:22.0000000Z", "2009-05-22T22:22:22.9999999Z")]
    [InlineData("2009-05-22T23:59:59.0000000Z", "2009-05-22T23:59:59.9999999Z")]
    
    // Negative ticks
    [InlineData("2009-05-22T23:59:59.9999999Z", "2009-05-22T23:59:59.0000000Z")]
    [InlineData("2009-05-22T23:59:59.9999999Z", "2009-05-22T23:59:58.9999999Z")]
    [InlineData("2009-05-22T23:59:59.9999999Z", "2009-05-22T23:58:59.9999999Z")]
    [InlineData("2009-05-22T23:59:59.9999999Z", "2009-05-22T22:59:59.9999999Z")]
    [InlineData("2009-05-22T23:59:59.9999999Z", "2009-05-21T23:59:59.9999999Z")]
    [InlineData("2009-05-22T23:59:59.9999999Z", "2009-04-22T23:59:59.9999999Z")]
    [InlineData("2009-05-22T23:59:59.9999999Z", "2008-05-22T23:59:59.9999999Z")]
    public void ShouldNotHaveMissingTicks(string previousTick, string nextTick)
    {
        var previous = DateTime.Parse(previousTick);
        var next = DateTime.Parse(nextTick);
        
        var sut = new EnsureContinuousSecondTicks(previous);
        var missingTicks = sut.GetTicksBetweenPreviousAndNext(next);

        Assert.Empty(missingTicks);
    }
    
    [Theory]
    // Same day but different seconds
    [InlineData("2024-01-12T12:00:00.0000000Z", "2024-01-12T12:00:02.0000000Z", 1)]
    [InlineData("2024-01-12T12:00:00.9999999Z", "2024-01-12T12:00:02.0000000Z", 1)]
    [InlineData("2024-01-12T12:00:00.0000000Z", "2024-01-12T12:00:03.0000000Z", 2)]
    [InlineData("2024-01-12T12:00:00.0000000Z", "2024-01-12T12:00:04.0000000Z", 3)]
    [InlineData("2024-01-12T12:00:00.0000000Z", "2024-01-12T12:00:04.9999999Z", 3)]
    [InlineData("2024-01-12T12:00:00.0000000Z", "2024-01-12T12:00:59.9999999Z", 58)]
    
    // Different seconds and minutes
    [InlineData("2024-01-12T12:00:00.0000000Z", "2024-01-12T12:01:02.0000000Z", 61)]
    [InlineData("2024-01-12T12:00:00.0000000Z", "2024-01-12T12:01:02.5500000Z", 61)]
    [InlineData("2024-01-12T12:00:00.0000000Z", "2024-01-12T12:01:02.9999998Z", 61)]
    
    // Different seconds, minutes and/or hours
    [InlineData("2024-01-12T12:00:00.0000000Z", "2024-01-12T13:00:00.0000000Z", SecondsInHour - 1)]
    [InlineData("2024-01-12T12:00:00.0000000Z", "2024-01-12T13:00:00.9999999Z", SecondsInHour - 1)]
    [InlineData("2024-01-12T12:00:00.0000000Z", "2024-01-12T14:10:22.9999999Z", (SecondsInHour * 2) + (SecondsInMinute * 10) + 22 - 1)]
    
    // Different day but same month
    [InlineData("2024-01-12T13:00:00.0000000Z", "2024-01-13T13:00:00.0000000Z", SecondsInDay - 1)]
    [InlineData("2024-01-12T12:00:00.0000000Z", "2024-01-17T14:10:22.9999999Z", (SecondsInDay * 5) + (SecondsInHour * 2) + (SecondsInMinute * 10) + 22 - 1)]
    
    // Different month
    [InlineData("2024-01-01T13:00:00.0000000Z", "2024-02-01T13:00:00.0000000Z", SecondsInDay * 31 - 1)]
    [InlineData("2024-01-01T13:00:00.0000000Z", "2024-03-01T13:00:00.0000000Z", (SecondsInDay * 31) + (SecondsInDay * 29) - 1)]
    [InlineData("2024-01-01T13:00:00.0000000Z", "2024-03-01T13:00:00.9999999Z", (SecondsInDay * 31) + (SecondsInDay * 29) - 1)]
    
    // Different year (testing large ranges and moving from dec to jan)
    [InlineData("2023-12-31T13:00:00.0000000Z", "2024-01-01T13:00:00.0000000Z", SecondsInDay - 1)]
    [InlineData("2023-12-31T13:00:00.0000000Z", "2024-01-01T13:00:00.9456743Z", SecondsInDay - 1)]
    
    // Entire year (not realistic but tests the logic).
    [InlineData("2022-12-31T13:00:00.0000000Z", "2023-12-31T13:00:00.0000000Z", SecondsInDay * 365 - 1)]
    [InlineData("2022-12-31T13:00:00.9999999Z", "2023-12-31T13:00:00.0000000Z", SecondsInDay * 365 - 1)]
    [InlineData("2022-12-31T13:00:00.0000000Z", "2023-12-31T13:00:00.9999999Z", SecondsInDay * 365 - 1)]

    public void ShouldHaveMissingTicks(string previousTick, string nextTick, int expectedMissingTicks)
    {
        var previous = DateTime.Parse(previousTick).ToUniversalTime();
        var next = DateTime.Parse(nextTick).ToUniversalTime();
        
        var sut = new EnsureContinuousSecondTicks(previous);
        var missingTicks = sut.GetTicksBetweenPreviousAndNext(next);
        
        Assert.Equal(expectedMissingTicks, missingTicks.Count());
    }
    
    [Fact]
    public void MissingTicksAreExpectedTimes()
    {
        var previous = DateTime.Parse("2024-01-12T12:00:00.9994443Z").ToUniversalTime();
        var next = DateTime.Parse("2024-01-12T12:00:05.2345633Z").ToUniversalTime();
        
        var sut = new EnsureContinuousSecondTicks(previous);
        var missingTicks = sut.GetTicksBetweenPreviousAndNext(next).ToArray();
        
        Assert.Equal(4, missingTicks.Count());
        Assert.Equal(DateTime.Parse("2024-01-12T12:00:01.0000000Z").ToUniversalTime(), missingTicks[0]);
        Assert.Equal(DateTime.Parse("2024-01-12T12:00:02.0000000Z").ToUniversalTime(), missingTicks[1]);
        Assert.Equal(DateTime.Parse("2024-01-12T12:00:03.0000000Z").ToUniversalTime(), missingTicks[2]);
        Assert.Equal(DateTime.Parse("2024-01-12T12:00:04.0000000Z").ToUniversalTime(), missingTicks[3]);
        
        // Set the next tick and test that the next check works.
        sut.SetNextTick(DateTime.Parse("2024-01-12T12:00:06.2345633Z").ToUniversalTime());
        missingTicks = sut.GetTicksBetweenPreviousAndNext(DateTime.Parse("2024-01-12T12:00:10.9999888Z").ToUniversalTime()).ToArray();
        
        Assert.Equal(3, missingTicks.Count());
        Assert.Equal(DateTime.Parse("2024-01-12T12:00:07.0000000Z").ToUniversalTime(), missingTicks[0]);
        Assert.Equal(DateTime.Parse("2024-01-12T12:00:08.0000000Z").ToUniversalTime(), missingTicks[1]);
        Assert.Equal(DateTime.Parse("2024-01-12T12:00:09.0000000Z").ToUniversalTime(), missingTicks[2]);
    }
}
