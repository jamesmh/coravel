namespace Coravel.Queuing
{
    public class QueueMetrics
    {
        private int _runningCount = 0;
        private int _waitingCount = 0;

        public QueueMetrics(int runningCount, int waitingCount)
        {
            this._runningCount = runningCount;
            this._waitingCount = waitingCount;
        }

        public int WaitingCount()
        {
            return this._waitingCount;
        }

        public int RunningCount()
        {
            return this._runningCount;
        }
    }
}