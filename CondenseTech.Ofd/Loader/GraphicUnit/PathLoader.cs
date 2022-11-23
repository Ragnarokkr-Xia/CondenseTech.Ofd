using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using CondenseTech.Ofd.BasicStructure;

namespace CondenseTech.Ofd.Loader.GraphicUnit
{
    /// <summary>
    /// PathObject
    /// </summary>
    internal class PathLoader : BaseGraphicUnitLoader<CT_Path>
    {
        protected override void Load0(XmlNode xmlNode, XmlNamespaceManager namespaceManager, ref CT_Path ctPath)
        {
            var abbreviatedDataNode = xmlNode.SelectSingleNode("ofd:AbbreviatedData", namespaceManager);
            ctPath.Operations = LoadPathObjectAbbreviatedData(abbreviatedDataNode);
            LoadPathObjectFill(xmlNode, ref ctPath);
            LoadPathObjectStroke(xmlNode, ref ctPath);
            LoadPathObjectRule(xmlNode, ref ctPath);
            LoadPathObjectFillColor(xmlNode.SelectSingleNode("ofd:FillColor", namespaceManager), ref ctPath);
            LoadPathObjectStrokeColor(xmlNode.SelectSingleNode("ofd:StrokeColor", namespaceManager), ref ctPath);
        }

        private void LoadPathObjectStroke(XmlNode pathObjectNode, ref CT_Path pathObject)
        {
            string strokeString = pathObjectNode.Attributes?["Stroke"]?.InnerText;
            if (!string.IsNullOrWhiteSpace(strokeString))
            {
                pathObject.Stroke = LoadBool(strokeString);
            }
        }

        private void LoadPathObjectFill(XmlNode pathObjectNode, ref CT_Path pathObject)
        {
            string fillString = pathObjectNode.Attributes?["Fill"]?.InnerText;
            if (!string.IsNullOrWhiteSpace(fillString))
            {
                pathObject.Fill = LoadBool(fillString);
            }
        }

        private void LoadPathObjectRule(XmlNode pathObjectNode, ref CT_Path pathObject)
        {
            string ruleString = pathObjectNode.Attributes?["Rule"]?.InnerText;
            if (!string.IsNullOrWhiteSpace(ruleString))
            {
                switch (ruleString)
                {
                    case "NoneZero":
                        pathObject.Rule = FillRule.NonZero;
                        break;

                    case "Even-Odd":
                        pathObject.Rule = FillRule.EvenOdd;
                        break;
                }
            }
        }

        private void LoadPathObjectStrokeColor(XmlNode strokeColorNode, ref CT_Path pathObject)
        {
            CT_Color color = LoadAttributeColor(strokeColorNode);
            if (color != null)
            {
                pathObject.StrokeColor = color;
            }
        }

        private void LoadPathObjectFillColor(XmlNode fillColorNode, ref CT_Path pathObject)
        {
            CT_Color color = LoadAttributeColor(fillColorNode);
            if (color != null)
            {
                pathObject.FillColor = color;
            }
        }

        private List<PathOperation> LoadPathObjectAbbreviatedData(XmlNode abbreviatedDataNode)
        {
            string abbreviatedDataString = abbreviatedDataNode?.InnerText;
            if (!string.IsNullOrWhiteSpace(abbreviatedDataString))
            {
                List<PathOperation> pathOperations = new List<PathOperation>();
                Regex oRegex = new Regex("[SMLQBAC] ([0-9\\.]+[ ]*)+");
                while (true)
                {
                    var oMatch = oRegex.Match(abbreviatedDataString);
                    if (!oMatch.Success)
                        break;
                    var pathOperation = PathOperation.DeAbbreviatePathOperation(oMatch.Value.Trim());
                    if (pathOperation != null)
                    {
                        pathOperations.Add(pathOperation);
                    }
                    abbreviatedDataString = oRegex.Replace(abbreviatedDataString, string.Empty, 1);
                }

                return pathOperations;
            }
            return null;
        }
    }
}