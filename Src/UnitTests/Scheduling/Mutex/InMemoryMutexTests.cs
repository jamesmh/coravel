using System.Threading.Tasks;
using Coravel.Scheduling.Schedule.Interfaces;
using Coravel.Scheduling.Schedule.Mutex;
using Xunit;

namespace UnitTests.Scheduling.Mutex
{
    public class InMemoryMutexTests
    {
        [Fact]
        public void InMemoryMutexLocks() {
            IMutex  mutex = new InMemoryMutex();
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
    }
}