using System;

namespace Coravel.Scheduling.Schedule.Restrictions
{
    public class CustomRestrictions : IRestrictions
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