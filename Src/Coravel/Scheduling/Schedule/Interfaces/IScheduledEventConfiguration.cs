namespace Coravel.Scheduling.Schedule.Interfaces
{
    /// <summary>
    /// Provides methods for configuring how a scheduled event behaves.
    /// This contains all public facing configuration methods - i.e. this defines the 
    /// fluent api for the scheduled events.
    /// </summary>
    public interface IScheduledEventConfiguration
    {
        /// <summary>
        /// Restrict task to run on Mondays.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration Monday();

        /// <summary>
        /// Restrict task to run on Tuesdays.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration Tuesday();

        /// <summary>
        /// Restrict task to run on Wednesdays.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration Wednesday();

        /// <summary>
        /// Restrict task to run on Thursdays.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration Thursday();

        /// <summary>
        /// Restrict task to run on Fridays.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration Friday();

        /// <summary>
        /// Restrict task to run on Saturdays.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration Saturday();

        /// <summary>
        /// Restrict task to run on Sundays.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration Sunday();

        /// <summary>
        /// Restrict task to run on weekdays (Monday - Friday).
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration Weekday();

        /// <summary>
        /// Restrict task to run on weekends (Saturday and Sunday).
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration Weekend();

        /// <summary>
        /// If this event has not completed from the last time it was invoked, and is due again,
        /// it will be prevented from running.
        /// </summary>
        /// <returns></returns>
        IScheduledEventConfiguration PreventOverlapping(string uniqueIdentifier);


        // TODO: Figure out how to launch long running tasks NOT using Task.Run() - since this causes the task to get killed
        //       on app stopping. Try to launch it as a new IHostedService that runs this task once?
        //
        // /// <summary>
        // /// When your task is CPU intensive and/or takes an extended period to complete (~ longer than a few minutes),
        // /// Coravel will fire this task using a separate thread.
        // /// 
        // /// Note that using many long running tasks will eat up your app's available threads.
        // /// </summary>
        // /// <returns></returns>
        // IScheduledEventConfiguration AsLongRunning();
    }
}