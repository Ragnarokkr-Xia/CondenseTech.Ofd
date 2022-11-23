using System.Xml;

namespace CondenseTech.Ofd.Miscellaneous
{
    public static class XmlUtility
    {
        public static string LoadNodeInnerText(XmlNode node)
        {
            return node?.InnerText;
        }

        public static string LoadNodeAttributeInnerText(XmlNode node, string attributeName)
        {
            return node?.Attributes?[attributeName]?.InnerText;
        }
    }
}