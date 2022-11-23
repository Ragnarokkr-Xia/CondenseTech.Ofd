using System.Xml;
using CondenseTech.Ofd.BasicStructure;
using CondenseTech.Ofd.Loader.GraphicUnit;

namespace CondenseTech.Ofd.Loader
{
    public static class GraphicUnitLoader
    {
        private static ImageLoader imageLoader = new ImageLoader();
        private static PathLoader pathLoader = new PathLoader();
        private static TextLoader textLoader = new TextLoader();

        public static CT_Image InstanceImageUnit(XmlNode imageNode, XmlNamespaceManager namespaceManager)
        {
            return imageLoader.Load(imageNode, namespaceManager);
        }

        public static CT_Path InstancePathUnit(XmlNode pathNode, XmlNamespaceManager namespaceManager)
        {
            return pathLoader.Load(pathNode, namespaceManager);
        }

        public static CT_Text InstanceTextUnit(XmlNode textNode, XmlNamespaceManager namespaceManager)
        {
            return textLoader.Load(textNode, namespaceManager);
        }
    }
}