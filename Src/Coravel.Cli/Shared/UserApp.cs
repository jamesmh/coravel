using System.IO;
using System.Linq;

namespace Coravel.Cli.Shared
{
    public class UserApp
    {
        public static string GetAppName() =>
            Directory.GetCurrentDirectory().Split('\\').Last();
    }
}