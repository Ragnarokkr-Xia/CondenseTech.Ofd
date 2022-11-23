using System.Xml;
using CondenseTech.Ofd.BasicStructure;
using CondenseTech.Ofd.BasicType;
using CondenseTech.Ofd.Miscellaneous;

namespace CondenseTech.Ofd.Loader.GraphicUnit
{
    internal abstract class BaseGraphicUnitLoader<T> where T : CT_GraphicUnit, new()
    {
        public T Load(XmlNode xmlNode, XmlNamespaceManager namespaceManager)
        {
            if (xmlNode == null)
            {
                return null;
            }
            CT_GraphicUnit graphicUnit = new T();
            LoadGraphicUnit(xmlNode, ref graphicUnit);

            T result = graphicUnit as T;
            Load0(xmlNode, namespaceManager, ref result);
            return result;
        }

        protected abstract void Load0(XmlNode xmlNode, XmlNamespaceManager namespaceManager, ref T graphicUnit);

        private void LoadGraphicUnit(XmlNode graphicUnitNode, ref CT_GraphicUnit graphicUnit)
        {
            if (graphicUnitNode?.Attributes == null)
            {
                graphicUnit = null;
                return;
            }

            string boundaryString = graphicUnitNode.Attributes["Boundary"]?.InnerText;
            if (string.IsNullOrWhiteSpace(boundaryString))
            {
                graphicUnit = null;
                return;
            }

            graphicUnit.Boundary = new ST_Box(boundaryString);
            LoadGraphicUnitId(graphicUnitNode, ref graphicUnit);
            LoadGraphicUnitName(graphicUnitNode, ref graphicUnit);
            LoadGraphicUnitVisible(graphicUnitNode, ref graphicUnit);
            LoadGraphicUnitLineWidth(graphicUnitNode, ref graphicUnit);
            LoadGraphicUnitAlpha(graphicUnitNode, ref graphicUnit);
            LoadGraphicUnitCTM(graphicUnitNode, ref graphicUnit);
        }

        private void LoadGraphicUnitId(XmlNode graphicUnitNode, ref CT_GraphicUnit graphicUnit)
        {
            ST_ID id = ST_ID.GetIdInAttribute(graphicUnitNode);
            if (id != null)
            {
                graphicUnit.Id = id;
            }
        }

        private void LoadGraphicUnitCTM(XmlNode graphicUnitNode, ref CT_GraphicUnit graphicUnit)
        {
            string ctmString = graphicUnitNode.Attributes?["CTM"]?.InnerText;
            if (!string.IsNullOrWhiteSpace(ctmString))
            {
                graphicUnit.CTM = new ST_Array<float>(ctmString);
            }
        }

        private void LoadGraphicUnitAlpha(XmlNode graphicUnitNode, ref CT_GraphicUnit graphicUnit)
        {
            string alphaString = graphicUnitNode.Attributes?["Alpha"]?.InnerText;
            if (!string.IsNullOrWhiteSpace(alphaString))
            {
                graphicUnit.Alpha = int.Parse(alphaString);
            }
        }

        private void LoadGraphicUnitLineWidth(XmlNode graphicUnitNode, ref CT_GraphicUnit graphicUnit)
        {
            string lineWidthString = graphicUnitNode.Attributes?["LineWidth"]?.InnerText;
            if (!string.IsNullOrWhiteSpace(lineWidthString))
            {
                graphicUnit.LineWidth = double.Parse(lineWidthString);
            }
        }

        private void LoadGraphicUnitVisible(XmlNode graphicUnitNode, ref CT_GraphicUnit graphicUnit)
        {
            string visibleString = graphicUnitNode.Attributes?["Visible"]?.InnerText;
            if (!string.IsNullOrWhiteSpace(visibleString))
            {
                graphicUnit.Visible = LoadBool(visibleString);
            }
        }

        private void LoadGraphicUnitName(XmlNode graphicUnitNode, ref CT_GraphicUnit graphicUnit)
        {
            string nameString = graphicUnitNode.Attributes?["Name"]?.InnerText;
            if (!string.IsNullOrWhiteSpace(nameString))
            {
                graphicUnit.Name = nameString;
            }
        }

        protected bool LoadBool(string raw)
        {
            return "true".Equals(raw.ToLower());
        }

        protected CT_Color LoadAttributeColor(XmlNode xmlNode, string attributeName)
        {
            string colorString = xmlNode?.Attributes?[attributeName]?.InnerText;
            if (!string.IsNullOrWhiteSpace(colorString))
            {
                return new CT_Color(colorString);
            }
            return null;
        }

        protected CT_Color LoadAttributeColor(XmlNode xmlNode)
        {
            string colorString = xmlNode?.Attributes?["Value"]?.InnerText;
            if (!string.IsNullOrWhiteSpace(colorString))
            {
                string alphaString = xmlNode?.Attributes?["Alpha"]?.InnerText;
                if (!string.IsNullOrWhiteSpace(alphaString))
                {
                    byte alpha = ValueUtility.TryConvert2Byte(alphaString, 255);
                    return new CT_Color(colorString, alpha);
                }
                else
                {
                    return new CT_Color(colorString);
                }
            }
            return null;
        }
    }
}