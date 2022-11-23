using System.Linq;

namespace CondenseTech.Ofd.Render.Helpers
{
    internal static class FileHelper
    {
        internal static string GetExtensionInFilename(string fileName)
        {
            return fileName.Split('.').LastOrDefault();
        }
    }
}