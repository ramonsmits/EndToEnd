namespace Utils.Runner
{
    using System;
    using System.IO;

    public class OutputDirectoryCreator
    {

        public string SetupOutputDirectory(string name)
        {
            var outputDirectory = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, name);

            if (Directory.Exists(outputDirectory) == false)
            {
                Directory.CreateDirectory(outputDirectory);
            }

            /*
            var directoryInfo = new DirectoryInfo(outputDirectory);

            foreach (var fileInfo in directoryInfo.GetFiles())
            {
                fileInfo.Delete();
            }

            foreach (var directory in directoryInfo.GetDirectories())
            {
                directory.Delete(true);
            }
            */
            return outputDirectory;
        }
    }
}