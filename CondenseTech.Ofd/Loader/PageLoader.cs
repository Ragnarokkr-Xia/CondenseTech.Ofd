using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using CondenseTech.Ofd.BasicStructure;
using CondenseTech.Ofd.BasicType;
using CondenseTech.Ofd.Manager;
using CondenseTech.Ofd.Miscellaneous;

namespace CondenseTech.Ofd.Loader
{
    public class PageLoader
    {
        private Page Page { get; set; } = new Page();
        private XmlNamespaceManager NamespaceManager { get; set; }

        private string CurrentDirectory { get; set; }
        private double LastX { get; set; } = 0;
        private double LastY { get; set; } = 0;

        public Page Load(ST_Loc pageXmlFileName, OfdContainer container)
        {
            CurrentDirectory = UnixPath.GetParentDirectory(pageXmlFileName);
            byte[] pageXmlFileData = container[pageXmlFileName];
            if (pageXmlFileData == null)
                return null;
            using (MemoryStream ms = new MemoryStream(pageXmlFileData))
            {
                XmlDocument pageDocument = new XmlDocument();
                try
                {
                    pageDocument.Load(ms);
                    NamespaceManager = new XmlNamespaceManager(pageDocument.NameTable);
                    var firstElement =
                        (from XmlNode elementNode in pageDocument.ChildNodes
                         where elementNode.NodeType == XmlNodeType.Element
                         select elementNode).FirstOrDefault();
                    if (firstElement != null && !NamespaceManager.HasNamespace(firstElement.Prefix))
                    {
                        NamespaceManager.AddNamespace(firstElement.Prefix, firstElement.NamespaceURI);
                    }

                    Page.Area = LoadPageArea(pageDocument.SelectSingleNode("/ofd:Page/ofd:Area", NamespaceManager));
                    Page.Templates =
                        LoadTemplates(pageDocument.SelectNodes("/ofd:Page/ofd:Template", NamespaceManager));
                    Page.Content =
                        LoadContent(pageDocument.SelectSingleNode("/ofd:Page/ofd:Content", NamespaceManager));
                    return Page;
                }
                catch
                {
                    return null;
                }
            }
        }

        private CT_PageArea LoadPageArea(XmlNode pageAreaNode)
        {
            if (pageAreaNode != null)
            {
                CT_PageArea pageArea = new CT_PageArea()
                {
                    PhysicalBox = LoadBox(pageAreaNode.SelectSingleNode("ofd:PhysicalBox", NamespaceManager)),
                    ApplicationBox = LoadBox(pageAreaNode.SelectSingleNode("ofd:ApplicationBox", NamespaceManager)),
                    ContentBox = LoadBox(pageAreaNode.SelectSingleNode("ofd:ContentBox", NamespaceManager)),
                    BleedBox = LoadBox(pageAreaNode.SelectSingleNode("ofd:BleedBox", NamespaceManager))
                };
                return pageArea;
            }
            return null;
        }

        private ST_Box LoadBox(XmlNode boxNode)
        {
            if (boxNode != null && !string.IsNullOrWhiteSpace(boxNode.InnerText))
            {
                return new ST_Box(boxNode.InnerText);
            }
            return null;
        }

        private List<TemplatePseudo> LoadTemplates(XmlNodeList templateNodes)
        {
            if (templateNodes != null)
            {
                List<TemplatePseudo> templateList = new List<TemplatePseudo>();
                foreach (XmlNode templateNode in templateNodes)
                {
                    TemplatePseudo template = LoadTemplate(templateNode);
                    if (template != null)
                    {
                        templateList.Add(template);
                    }
                }

                return templateList;
            }

            return null;
        }

        private TemplatePseudo LoadTemplate(XmlNode templateNode)
        {
            if (templateNode?.Attributes != null)
            {
                string idString = templateNode.Attributes["TemplateID"]?.InnerText;
                string zOrderString = templateNode.Attributes["ZOrder"]?.InnerText;
                if (!string.IsNullOrWhiteSpace(idString))
                    return new TemplatePseudo()
                    {
                        Id = new ST_ID(idString),
                        ZOrder = string.IsNullOrWhiteSpace(zOrderString) ? new LayerType(LayerTypeEnum.Background) : new LayerType(zOrderString)
                    };
            }
            return null;
        }

        private Content LoadContent(XmlNode contentNode)
        {
            if (contentNode != null)
            {
                Content content = new Content();
                XmlNodeList layerNodes = contentNode.SelectNodes("ofd:Layer", NamespaceManager);
                if (layerNodes != null && layerNodes.Count != 0)
                {
                    content.Layers = LoadLayers(layerNodes);
                }
                return content;
            }
            return null;
        }

        private List<Layer> LoadLayers(XmlNodeList layerNodes)
        {
            if (layerNodes != null)
            {
                List<Layer> layers = new List<Layer>();
                foreach (XmlNode layerNode in layerNodes)
                {
                    Layer layer = LoadLayer(layerNode);
                    if (layer != null)
                    {
                        layers.Add(layer);
                    }
                }

                return layers;
            }
            return null;
        }

        private Layer LoadLayer(XmlNode layerNode)
        {
            if (layerNode != null)
            {
                ST_ID id = ST_ID.GetIdInAttribute(layerNode);
                string drawParamId = XmlUtility.LoadNodeAttributeInnerText(layerNode, "DrawParam");
                Layer layer = new Layer()
                {
                    GraphicUnits = LoadPageObjects(layerNode.ChildNodes)
                };
                if (id != null)
                {
                    layer.Id = id;
                }

                if (!string.IsNullOrWhiteSpace(drawParamId))
                {
                    layer.DrawParamId = new ST_ID(drawParamId);
                }

                return layer;
            }

            return null;
        }

        private List<CT_GraphicUnit> LoadPageObjects(XmlNodeList pageObjectNodes)
        {
            if (pageObjectNodes != null)
            {
                List<CT_GraphicUnit> pageObjects = new List<CT_GraphicUnit>();
                foreach (XmlNode pageObjectNode in pageObjectNodes)
                {
                    CT_GraphicUnit pageObject = LoadPageObject(pageObjectNode);
                    if (pageObject != null)
                    {
                        pageObjects.Add(pageObject);
                    }
                }

                return pageObjects;
            }
            return null;
        }

        private CT_GraphicUnit LoadPageObject(XmlNode graphicUnitNode)
        {
            if (graphicUnitNode != null)
            {
                switch (graphicUnitNode.Name)
                {
                    case "ofd:ImageObject":
                        return LoadImageObject(graphicUnitNode);

                    case "ofd:TextObject":
                        return LoadTextObject(graphicUnitNode);

                    case "ofd:PathObject":
                        return LoadPathObject(graphicUnitNode);
                }
            }
            return null;
        }

        private CT_Text LoadTextObject(XmlNode textObjectNode)
        {
            if (textObjectNode?.Attributes != null)
            {
                string fontString = textObjectNode.Attributes["Font"]?.InnerText;
                string sizeString = textObjectNode.Attributes["Size"]?.InnerText;

                if (!string.IsNullOrWhiteSpace(fontString) && !string.IsNullOrWhiteSpace(sizeString))
                {
                    CT_Text textObject = new CT_Text(LoadGraphicUnit(textObjectNode))
                    {
                        Font = new ST_ID(fontString),
                        Size = double.Parse(sizeString),
                        TextCodeList = LoadTextCodes(textObjectNode.SelectNodes("ofd:TextCode", NamespaceManager))
                    };
                    LoadTextObjectReadDirection(textObjectNode, ref textObject);
                    LoadTextObjectCharDirection(textObjectNode, ref textObject);
                    LoadTextObjectWeight(textObjectNode, ref textObject);
                    LoadTextObjectItalic(textObjectNode, ref textObject);
                    LoadTextObjectStroke(textObjectNode, ref textObject);
                    LoadTextObjectFill(textObjectNode, ref textObject);
                    LoadTextObjectHScale(textObjectNode, ref textObject);
                    LoadTextObjectFillColor(textObjectNode, ref textObject);
                    return textObject;
                }
            }
            return null;
        }

        private void LoadTextObjectFillColor(XmlNode textObjectNode, ref CT_Text textObject)
        {
            CT_Color fillColor = LoadAttributeColor(textObjectNode.SelectSingleNode("ofd:FillColor", NamespaceManager), "Value");
            if (fillColor != null)
            {
                textObject.FillColor = fillColor;
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

        private CT_GraphicUnit LoadGraphicUnit(XmlNode graphicUnitNode)
        {
            if (graphicUnitNode?.Attributes != null)
            {
                string boundaryString = graphicUnitNode.Attributes["Boundary"]?.InnerText;

                if (!string.IsNullOrWhiteSpace(boundaryString))
                {
                    CT_GraphicUnit graphicUnit = new CT_GraphicUnit
                    {
                        Boundary = new ST_Box(boundaryString),
                    };
                    LoadGraphicUnitId(graphicUnitNode, ref graphicUnit);
                    LoadGraphicUnitName(graphicUnitNode, ref graphicUnit);
                    LoadGraphicUnitVisible(graphicUnitNode, ref graphicUnit);
                    LoadGraphicUnitLineWidth(graphicUnitNode, ref graphicUnit);
                    LoadGraphicUnitAlpha(graphicUnitNode, ref graphicUnit);
                    LoadGraphicUnitCTM(graphicUnitNode, ref graphicUnit);
                    return graphicUnit;
                }
            }

            return null;
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

        private bool LoadBool(string raw)
        {
            switch (raw.ToLower())
            {
                case "true":
                    return true;
            }

            return false;
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
                        UpdateTextCodeCoordinate(textCode);
                        CompleteTextCodeCoordinate(ref textCode);
                        textCodes.Add(textCode);
                    }
                }
                return textCodes;
            }
            return null;
        }

        private void UpdateTextCodeCoordinate(TextCode textCode)
        {
            if (textCode.X.HasValue)
            {
                LastX = textCode.X.Value;
            }
            if (textCode.Y.HasValue)
            {
                LastY = textCode.Y.Value;
            }
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

        private CT_Image LoadImageObject(XmlNode imageObjectNode)
        {
            if (imageObjectNode != null)
            {
                string resourceIdString = imageObjectNode.Attributes?["ResourceID"]?.InnerText;

                if (!string.IsNullOrWhiteSpace(resourceIdString))
                {
                    CT_Image image = new CT_Image(LoadGraphicUnit(imageObjectNode))
                    {
                        ResourceId = new ST_ID(resourceIdString)
                    };
                    return image;
                }
            }
            return null;
        }

        private CT_Path LoadPathObject(XmlNode pathObjectNode)
        {
            if (pathObjectNode != null)
            {
                CT_Path pathObject = new CT_Path(LoadGraphicUnit(pathObjectNode))
                {
                    Operations =
                        LoadPathObjectAbbreviatedData(pathObjectNode.SelectSingleNode("ofd:AbbreviatedData",
                            NamespaceManager))
                };
                LoadPathObjectFill(pathObjectNode, ref pathObject);
                LoadPathObjectStroke(pathObjectNode, ref pathObject);
                LoadPathObjectRule(pathObjectNode, ref pathObject);
                LoadPathObjectFillColor(pathObjectNode.SelectSingleNode("ofd:FillColor", NamespaceManager), ref pathObject);
                LoadPathObjectStrokeColor(pathObjectNode.SelectSingleNode("ofd:StrokeColor", NamespaceManager), ref pathObject);
                return pathObject;
            }
            return null;
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
            CT_Color color = LoadAttributeColor(strokeColorNode, "Value");
            if (color != null)
            {
                pathObject.StrokeColor = color;
            }
        }

        private void LoadPathObjectFillColor(XmlNode fillColorNode, ref CT_Path pathObject)
        {
            CT_Color color = LoadAttributeColor(fillColorNode, "Value");
            if (color != null)
            {
                pathObject.FillColor = color;
            }
        }

        private CT_Color LoadAttributeColor(XmlNode node, string attributeName)
        {
            string colorString = node?.Attributes?[attributeName]?.InnerText;
            if (!string.IsNullOrWhiteSpace(colorString))
            {
                return new CT_Color(colorString);
            }
            return null;
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