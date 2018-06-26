namespace Coravel.Scheduling.Schedule.Interfaces
{
    /// <summary>
    /// Provides methods to restrict scheduled tasks to only run on certain day(s).
    /// </summary>
    public interface IScheduleRestriction
    {
        /// <summary>
        /// Restrict task to run on Mondays.
        /// </summary>
        /// <returns></returns>
         IScheduleRestriction Monday();

         /// <summary>
         /// Restrict task to run on Tuesdays.
         /// </summary>
         /// <returns></returns>
         IScheduleRestriction Tuesday();

         /// <summary>
         /// Restrict task to run on Wednesdays.
         /// </summary>
         /// <returns></returns>
         IScheduleRestriction Wednesday();

         /// <summary>
         /// Restrict task to run on Thursdays.
         /// </summary>
         /// <returns></returns>
         IScheduleRestriction Thursday();

         /// <summary>
         /// Restrict task to run on Fridays.
         /// </summary>
         /// <returns></returns>
         IScheduleRestriction Friday();

         /// <summary>
         /// Restrict task to run on Saturdays.
         /// </summary>
         /// <returns></returns>
         IScheduleRestriction Saturday();

         /// <summary>
         /// Restrict task to run on Sundays.
         /// </summary>
         /// <returns></returns>
         IScheduleRestriction Sunday();

         /// <summary>
         /// Restrict task to run on weekdays (Monday - Friday).
         /// </summary>
         /// <returns></returns>
         IScheduleRestriction Weekday();

         /// <summary>
         /// Restrict task to run on weekends (Saturday and Sunday).
         /// </summary>
         /// <returns></returns>
         IScheduleRestriction Weekend();
    }
}