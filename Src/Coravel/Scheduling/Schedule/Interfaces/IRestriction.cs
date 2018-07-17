using System;
namespace Coravel.Scheduling.Schedule.Interfaces
{
    public interface IRestriction
    {
        bool PassesRestrictions(DateTime utcNow);
    }
}
