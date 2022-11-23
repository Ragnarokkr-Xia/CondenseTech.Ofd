using System.Collections.Generic;
using System.Xml;
using CondenseTech.Ofd.BasicStructure;
using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.Loader.GraphicUnit
{
    /// <summary>
    /// TextObject
    /// </summary>
    internal class TextLoader : BaseGraphicUnitLoader<CT_Text>
    {
        protected override void Load0(XmlNode xmlNode, XmlNamespaceManager namespaceManager, ref CT_Text ctText)
        {
            if (xmlNode.Attributes == null)
            {
                return;
            }
            string fontString = xmlNode.Attributes["Font"]?.InnerText;
            string sizeString = xmlNode.Attributes["Size"]?.InnerText;

            if (string.IsNullOrWhiteSpace(fontString) || string.IsNullOrWhiteSpace(sizeString))
            {
                return;
            }

            ctText.Font = new ST_ID(fontString);
            ctText.Size = double.Parse(sizeString);
            ctText.TextCodeList = LoadTextCodes(xmlNode.SelectNodes("ofd:TextCode", namespaceManager));
            LoadTextObjectReadDirection(xmlNode, ref ctText);
            LoadTextObjectCharDirection(xmlNode, ref ctText);
            LoadTextObjectWeight(xmlNode, ref ctText);
            LoadTextObjectItalic(xmlNode, ref ctText);
            LoadTextObjectStroke(xmlNode, ref ctText);
            LoadTextObjectFill(xmlNode, ref ctText);
            LoadTextObjectHScale(xmlNode, ref ctText);
            LoadTextObjectFillColor(xmlNode, ref ctText, namespaceManager);
            LoadTextObjectStrokeColor(xmlNode, ref ctText, namespaceManager);
        }

        private List<TextCode> LoadTextCodes(XmlNodeList textCodeNodes)
        {
            if (textCodeNodes != null && textCodeNodes.Count > 0)
            {
                List<TextCode> textCodes = new List<TextCode>();

                foreach (XmlNode textCodeNode in textCodeNodes)
                {
                    TextCode textCode = LoadTextCode(textCodeNode);
                    if (textCode != null)
                    {
                        CompleteTextCodeCoordinate(ref textCode);
                        textCodes.Add(textCode);
                    }
                }
                return textCodes;
            }
            return null;
        }

        private void CompleteTextCodeCoordinate(ref TextCode textCode)
        {
            //Not standard
            if (!textCode.X.HasValue)
            {
                //TextCode.X = LastX;
                textCode.X = 0;
            }
            if (!textCode.Y.HasValue)
            {
                //TextCode.Y = LastY;
                textCode.Y = 0;
            }
        }

        private TextCode LoadTextCode(XmlNode textCodeNode)
        {
            if (textCodeNode != null)
            {
                TextCode textCode = new TextCode
                {
                    DeltaX = LoadAttributeDeltaArray(textCodeNode, "DeltaX"),
                    DeltaY = LoadAttributeDeltaArray(textCodeNode, "DeltaY"),
                    Text = textCodeNode.InnerText
                };
                string xString = textCodeNode.Attributes?["X"]?.InnerText;
                string yString = textCodeNode.Attributes?["Y"]?.InnerText;
                if (!string.IsNullOrWhiteSpace(xString))
                {
                    textCode.X = double.Parse(xString);
                }
                if (!string.IsNullOrWhiteSpace(yString))
                {
                    textCode.Y = double.Parse(yString);
                }

                return textCode;
            }

            return null;
        }

        private ST_Array<double> LoadAttributeDeltaArray(XmlNode node, string attributeName)
        {
            string deltaString = node.Attributes?[attributeName]?.InnerText;
            if (!string.IsNullOrWhiteSpace(deltaString))
            {
                return new ST_Array<double>(ST_Array<double>.NormalizeDeltaArrayString(deltaString));
            }
            return null;
        }

        private void LoadTextObjectFillColor(XmlNode textObjectNode, ref CT_Text textObject, XmlNamespaceManager namespaceManager)
        {
            CT_Color fillColor = LoadAttributeColor(textObjectNode.SelectSingleNode("ofd:FillColor", namespaceManager));
            if (fillColor != null)
            {
                textObject.FillColor = fillColor;
            }
        }

        private void LoadTextObjectStrokeColor(XmlNode textObjectNode, ref CT_Text textObject, XmlNamespaceManager namespaceManager)
        {
            CT_Color strokeColor = LoadAttributeColor(textObjectNode.SelectSingleNode("ofd:StrokeColor", namespaceManager));
            if (strokeColor != null)
            {
                textObject.StrokeColor = strokeColor;
            }
        }

        private void LoadTextObjectHScale(XmlNode textObjectNode, ref CT_Text textObject)
        {
            string hScaleString = textObjectNode.Attributes?["HScale"]?.InnerText;
            if (!string.IsNullOrWhiteSpace(hScaleString))
            {
                textObject.HScale = double.Parse(hScaleString);
            }
        }

        private void LoadTextObjectFill(XmlNode textObjectNode, ref CT_Text textObject)
        {
            string fillString = textObjectNode.Attributes?["Fill"]?.InnerText;
            if (!string.IsNullOrWhiteSpace(fillString))
            {
                textObject.Fill = LoadBool(fillString);
            }
        }

        private void LoadTextObjectStroke(XmlNode textObjectNode, ref CT_Text textObject)
        {
            string strokeString = textObjectNode.Attributes?["Stroke"]?.InnerText;
            if (!string.IsNullOrWhiteSpace(strokeString))
            {
                textObject.Stroke = LoadBool(strokeString);
            }
        }

        private void LoadTextObjectItalic(XmlNode textObjectNode, ref CT_Text textObject)
        {
            string italicString = textObjectNode.Attributes?["Italic"]?.InnerText;
            if (!string.IsNullOrWhiteSpace(italicString))
            {
                textObject.Italic = LoadBool(italicString);
            }
        }

        private void LoadTextObjectWeight(XmlNode textObjectNode, ref CT_Text textObject)
        {
            string weightString = textObjectNode.Attributes?["Weight"]?.InnerText;
            if (!string.IsNullOrWhiteSpace(weightString))
            {
                textObject.Weight = int.Parse(weightString);
            }
        }

        private void LoadTextObjectReadDirection(XmlNode textObjectNode, ref CT_Text textObject)
        {
            string readDirectionString = textObjectNode.Attributes?["ReadDirection"]?.InnerText;
            if (!string.IsNullOrWhiteSpace(readDirectionString))
            {
                textObject.ReadDirection = int.Parse(readDirectionString);
            }
        }

        private void LoadTextObjectCharDirection(XmlNode textObjectNode, ref CT_Text textObject)
        {
            string charDirectionString = textObjectNode.Attributes?["CharDirection"]?.InnerText;
            if (!string.IsNullOrWhiteSpace(charDirectionString))
            {
                textObject.CharDirection = int.Parse(charDirectionString);
            }
        }
    }
}