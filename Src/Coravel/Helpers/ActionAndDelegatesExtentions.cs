using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Coravel.Helpers
{
    public static class ActionAndDelegatesExtentions
    {
        public static bool IsThisAsync(this Delegate d)
        {
            return d.Method.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
        }

        public static bool IsThisAsync(this Action action)
        {
            return action.Method.IsDefined(typeof(AsyncStateMachineAttribute),
                                           false);
        }

        public static bool IsThisAsync(this Func<Action> action)
        {
            return action.Method.IsDefined(typeof(AsyncStateMachineAttribute),
                                           false);
        }
    }
}
