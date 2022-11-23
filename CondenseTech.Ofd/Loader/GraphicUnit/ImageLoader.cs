using System.Xml;
using CondenseTech.Ofd.BasicStructure;
using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.Loader.GraphicUnit
{
    /// <summary>
    /// ImageObject
    /// </summary>
    internal class ImageLoader : BaseGraphicUnitLoader<CT_Image>
    {
        protected override void Load0(XmlNode xmlNode, XmlNamespaceManager namespaceManager, ref CT_Image graphicUnit)
        {
            string resourceIdString = xmlNode.Attributes?["ResourceID"]?.InnerText;

            if (!string.IsNullOrWhiteSpace(resourceIdString))
            {
                graphicUnit.ResourceId = new ST_ID(resourceIdString);
            }
        }
    }
}
