using System.IO;

namespace Coravel.Cli.Shared
{
    public class Files
    {
        public static void WriteFileIfNotCreatedYet(string path, string filename, string content)
        {
            string fullFilePath = path + "/" + filename;

            Directory.CreateDirectory(path);

            if (!File.Exists(fullFilePath))
            {
                using (var file = File.CreateText(fullFilePath))
                {
                    file.Write(content);
                }
            }
        }
    }
}