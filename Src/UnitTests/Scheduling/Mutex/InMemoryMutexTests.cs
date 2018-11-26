using System;
using System.Threading.Tasks;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Scheduling.Schedule.Mutex;
using UnitTests.Scheduling.Stubs;
using Xunit;

namespace UnitTests.Scheduling.Mutex
{
    public class InMemoryMutexTests
    {
        [Fact]
        public void InMemoryMutexLocksAndReleases()
        {
            IMutex mutex = new InMemoryMutex();
            string key = "mutexkey";
            string otherKey = "anotherkey";
            int timeout_24hours = 1440;

            bool firstTry = mutex.TryGetLock(key, timeout_24hours);
            bool secondTry = mutex.TryGetLock(key, timeout_24hours);

            bool lockOtherKey = mutex.TryGetLock(otherKey, timeout_24hours);

            mutex.Release(key);

            bool thirdTry = mutex.TryGetLock(key, timeout_24hours);

            Assert.True(firstTry);
            Assert.False(secondTry);
            Assert.True(lockOtherKey);
            Assert.True(thirdTry);
        }

        [Theory]
        [InlineData(1440, 60, false)]
        [InlineData(1440, 1439, false)]
        [InlineData(1440, 1441, true)]
        [InlineData(10, 9, false)]
        [InlineData(10, 10, true)]
        [InlineData(10, 11, true)]
        public void InMemoryMutexCanGetLockAfterTimeoutExpired(int timeoutInMinutes, int addTimeToNow, bool mutexShouldRelease)
        {
            IMutex mutex = new InMemoryMutex();
            string key = "mutexkey";
            bool wasReleased = false;

            mutex.TryGetLock(key, timeoutInMinutes);
            var futureTime = DateTime.UtcNow.AddMinutes(addTimeToNow);

            // Simulate time has passed        
            (mutex as InMemoryMutex).Using(new StaticUtcTimeStub(futureTime));

            wasReleased = mutex.TryGetLock(key, timeoutInMinutes);

            Assert.Equal(mutexShouldRelease, wasReleased);
        }
    }
}