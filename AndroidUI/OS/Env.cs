namespace AndroidUI.OS
{
    public static class Env
    {
        public static string FindInPath(string fileName)
        {
            if (fileName.EndsWith(".exe"))
            {
                // do nothing
            }
            else if (fileName.EndsWith(".ex"))
            {
                fileName += "e";
            }
            else if (fileName.EndsWith(".e"))
            {
                fileName += "xe";
            }
            else if (fileName.EndsWith("."))
            {
                fileName += "exe";
            }
            else
            {
                fileName += ".exe";
            }

            if (File.Exists(fileName))
                return Path.GetFullPath(fileName);

            var values = Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }
    }
}
