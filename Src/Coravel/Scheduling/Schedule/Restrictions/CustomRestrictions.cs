using System;
using Coravel.Scheduling.Schedule.Interfaces;

namespace Coravel.Scheduling.Schedule.Restrictions
{
    public class CustomRestrictions : IRestriction
    {
        private Func<bool> func;

        public CustomRestrictions(){

        }

        public CustomRestrictions(Func<bool> func)
        {
            this.func = func;
        }

        public void SetRestriction(Func<bool> func)
        {
            this.func = func;
        }

        public bool PassesRestrictions(DateTime utcNow)
        {
            return func?.Invoke() ?? true;
        }
    }
}