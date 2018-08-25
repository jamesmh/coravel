using System.Threading.Tasks;
using Coravel.Invocable;

namespace TestMvcApp.Models
{
    public class TestInvocable : IInvocable
    {
        private TestInvocableStaticRunCounter _counter;

        public TestInvocable(TestInvocableStaticRunCounter counter) {
            this._counter = counter;
        }

        public Task Invoke()
        {
            this._counter.Increment();
            return Task.CompletedTask;
        }
    }

    public class TestInvocableStaticRunCounter {
        public static int RunCount = 0;

        public void Increment() => RunCount++;
    }
}