namespace Coravel.Scheduling.Schedule.Interfaces
{
    public interface IScheduleRestriction
    {
         IScheduleRestriction Monday();
         IScheduleRestriction Tuesday();
         IScheduleRestriction Wednesday();
         IScheduleRestriction Thursday();
         IScheduleRestriction Friday();
         IScheduleRestriction Saturday();
         IScheduleRestriction Sunday();
         IScheduleRestriction Weekday();
         IScheduleRestriction Weekend();
    }
}