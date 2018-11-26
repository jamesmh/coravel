using System.Reflection;

namespace Coravel.Mailer.Mail.Helpers
{
    public static class ReflectionHelpers
    {
        public static object GetPropOrFieldValue(this object me, string memberName)
        {
            var type = me.GetType();
            var member = type.GetProperty(memberName) as MemberInfo ?? type.GetField(memberName);

            if (member is PropertyInfo prop)
                return prop.GetValue(me);
            if (member is FieldInfo field)
                return field.GetValue(me);
            return null;
        }
    }
}