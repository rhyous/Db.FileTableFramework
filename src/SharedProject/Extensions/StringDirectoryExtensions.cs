using System.Collections.Generic;

namespace Rhyous.Db.FileTableFramework.Extensions
{
    public static class StringDirectoryExtensions
    {
        public static string GetRelativePath(this string fullPath, string root)
        {
            int index = 0;
            do
            {
                root = root.Substring(index);
                if (fullPath.StartsWith(root))
                    return fullPath.Remove(0, root.Length);
            } while ((index = GetNextIndex(root)) > 0);
            return fullPath;
        }

        private static int GetNextIndex(string root)
        {
            int index = root.IndexOfAny(@"\/".ToCharArray());
            return index == 0 ? 1 : index;
        }

        public static string[] SplitByDirectory(this string path)
        {
            var splitChars = @"\/".ToCharArray();
            return path.Trim().Trim(splitChars).Split(splitChars);
        }
    }
}
