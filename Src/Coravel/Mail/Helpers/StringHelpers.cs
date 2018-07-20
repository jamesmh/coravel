using System;
using System.Linq;
using System.Text;

namespace Coravel.Mail.Helpers
{
    public static class StringHelpers
    {
        public static string ToSnakeCase(this string str)
        {
            StringBuilder builder = new StringBuilder();

            var charSpan = str.AsSpan();

            // Always add the first upper case char without prepending a space.
            builder.Append(charSpan[0]);

            foreach (char character in charSpan.Slice(1))
            {
                if (char.IsUpper(character))
                {
                    builder.Append(" " + character);
                }
                else
                {
                    builder.Append(character);
                }
            }

            return builder.ToString();
        }

        public static string RemoveLastOccuranceOfWord(this string str, string word)
        {
            var span = str.AsSpan();
            var indexOfLastSpace = span.LastIndexOf(' ');
            var indexOfLastWord = indexOfLastSpace + 1;
            var lastWord = span.Slice(indexOfLastWord);

            if(lastWord.SequenceEqual(word.AsSpan()))
            {
                return span.Slice(0, indexOfLastSpace).ToString();
            }
            else {
                return str;
            }
        }
    }
}