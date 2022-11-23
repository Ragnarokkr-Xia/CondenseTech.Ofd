using System.Linq;
using System.Text.RegularExpressions;

namespace CondenseTech.Ofd.Miscellaneous
{
    public static class UnixPath
    {
        public static string GetParentDirectory(string path)
        {
            string[] pathParts = path.Split('/');
            return string.Join("/", pathParts.Take(pathParts.Length - 1));
        }

        public static string Combine(string basePath, string relativePath, char pathSeperator = '/')
        {
            if (relativePath.StartsWith(pathSeperator.ToString()))
            {
                return relativePath;
            }
            Regex relativePathRegex = new Regex($"(^\\.\\.{pathSeperator})|(^\\.{pathSeperator})");
            string[] basePathParts = basePath.Split(pathSeperator);
            int hierarchyIndex = basePathParts.Length;
            while (true)
            {
                Match match = relativePathRegex.Match(relativePath);
                if (match.Success)
                {
                    if (match.Value.Equals($"..{pathSeperator}"))
                        hierarchyIndex = hierarchyIndex - 1;
                    relativePath = relativePathRegex.Replace(relativePath, string.Empty, 1);
                }
                else
                {
                    break;
                }
            }
            return string.Join(pathSeperator.ToString(), basePathParts.Take(hierarchyIndex).Concat(new[] { relativePath }));
        }
    }
}