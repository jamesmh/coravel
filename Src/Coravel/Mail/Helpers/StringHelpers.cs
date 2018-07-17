using System.Text;

namespace Coravel.Mail.Helpers
{
    public static class StringHelpers
    {
        public static string ToSnakeCase(this string str) {
            StringBuilder builder = new StringBuilder();
            
            bool isFirst = true;
            foreach(char character in str) {
                bool prependSpace = !isFirst && char.IsUpper(character);

                isFirst = false;

                if(prependSpace)
                {
                    builder.Append(" " + character);                   
                }
                else {
                     builder.Append(character);
                }
            }

            return builder.ToString();
        }
    }
}