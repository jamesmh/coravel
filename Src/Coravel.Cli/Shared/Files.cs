using System.IO;

namespace Coravel.Cli.Shared
{
    public class Files
    {
        public static bool WriteFileIfNotCreatedYet(string path, string filename, string content)
        {
            var fullFilePath = path + "/" + filename;

            Directory.CreateDirectory(path);

            if (!File.Exists(fullFilePath))
            {
                using (var file = File.CreateText(fullFilePath))
                {
                    file.Write(content);
                }
                return true;
            }
            return false;
        }
    }
}