using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using CondenseTech.Ofd.BasicStructure;
using CondenseTech.Ofd.BasicType;
using CondenseTech.Ofd.Manager;
using CondenseTech.Ofd.Miscellaneous;

namespace CondenseTech.Ofd.Loader
{
    public class ResLoader
    {
        private Res Res { get; set; } = new Res();
        private XmlNamespaceManager NamespaceManager { get; set; }

        private string BaseDirectory { get; set; } = string.Empty;
        private string CurrentDirectory { get; set; }

        private string ResDirectory { get; set; }

        public Res Load(ST_Loc resXmlFileName, OfdContainer container)
        {
            CurrentDirectory = UnixPath.GetParentDirectory(resXmlFileName);
            byte[] resXmlFileData = container[resXmlFileName];
            if (resXmlFileData == null)
            {
                return null;
            }
            using (MemoryStream ms = new MemoryStream(resXmlFileData))
            {
                XmlDocument resDocument = new XmlDocument();
                try
                {
                    resDocument.Load(ms);
                    NamespaceManager = new XmlNamespaceManager(resDocument.NameTable);
                    var firstElement =
                        (from XmlNode elementNode in resDocument.ChildNodes
                         where elementNode.NodeType == XmlNodeType.Element
                         select elementNode).FirstOrDefault();
                    if (firstElement != null && !NamespaceManager.HasNamespace(firstElement.Prefix))
                    {
                        NamespaceManager.AddNamespace(firstElement.Prefix, firstElement.NamespaceURI);
                    }

                    ResDirectory = LoadResDirectory(resDocument.SelectSingleNode("/ofd:Res", NamespaceManager));
                    Res.MultiMedias =
                        LoadMultiMedias(resDocument.SelectSingleNode("/ofd:Res/ofd:MultiMedias", NamespaceManager));
                    Res.Fonts = LoadFonts(resDocument.SelectSingleNode("/ofd:Res/ofd:Fonts", NamespaceManager));
                    Res.DrawParams =
                        LoadDrawParams(resDocument.SelectSingleNode("/ofd:Res/ofd:DrawParams", NamespaceManager));
                    return Res;
                }
                catch
                {
                    return null;
                }
            }
        }

        public List<CT_MultiMedia> LoadMultiMedias(XmlNode multiMediasNode)
        {
            if (multiMediasNode != null)
            {
                List<CT_MultiMedia> multiMediaList = new List<CT_MultiMedia>();
                XmlNodeList multiMediaNodes = multiMediasNode.SelectNodes("ofd:MultiMedia", NamespaceManager);
                if (multiMediaNodes != null)
                {
                    foreach (XmlNode multiMediaNode in multiMediaNodes)
                    {
                        CT_MultiMedia multiMedia = LoadMultiMedia(multiMediaNode);
                        if (multiMedia != null)
                        {
                            multiMediaList.Add(multiMedia);
                        }
                    }
                }
                return multiMediaList;
            }
            return null;
        }

        public List<CT_DrawParam> LoadDrawParams(XmlNode drawParamsNode)
        {
            if (drawParamsNode != null)
            {
                List<CT_DrawParam> drawParamList = new List<CT_DrawParam>();
                XmlNodeList drawParamNodes = drawParamsNode.SelectNodes("ofd:DrawParam", NamespaceManager);
                if (drawParamNodes != null)
                {
                    foreach (XmlNode drawParaNode in drawParamNodes)
                    {
                        CT_DrawParam drawParam = LoadDrawParam(drawParaNode);
                        if (drawParam != null)
                        {
                            drawParamList.Add(drawParam);
                        }
                    }
                }
                return drawParamList;
            }
            return null;
        }

        public CT_DrawParam LoadDrawParam(XmlNode drawParamNode)
        {
            if (drawParamNode?.Attributes != null)
            {
                ST_ID id = ST_ID.GetIdInAttribute(drawParamNode);
                if (id != null)
                {
                    CT_DrawParam drawParam = new CT_DrawParam()
                    {
                        Id = id
                    };
                    LoadDrawParamLineWidth(drawParamNode, ref drawParam);
                    LoadDrawParamDashOffset(drawParamNode, ref drawParam);
                    LoadDrawParamMiterLimit(drawParamNode, ref drawParam);
                    LoadDrawParamFillColor(drawParamNode, ref drawParam);
                    LoadDrawParamStrokeColor(drawParamNode, ref drawParam);
                    return drawParam;
                }
            }
            return null;
        }

        private static void LoadDrawParamLineWidth(XmlNode drawParamNode, ref CT_DrawParam drawParam)
        {
            string lineWidthString = XmlUtility.LoadNodeAttributeInnerText(drawParamNode, "LineWidth");
            if (!string.IsNullOrWhiteSpace(lineWidthString))
            {
                try
                {
                    double lineWidth = double.Parse(lineWidthString);
                    drawParam.LineWidth = lineWidth;
                }
                catch
                {
                }
            }
        }

        private static void LoadDrawParamDashOffset(XmlNode drawParamNode, ref CT_DrawParam drawParam)
        {
            string dashOffsetString = XmlUtility.LoadNodeAttributeInnerText(drawParamNode, "DashOffset");
            if (!string.IsNullOrWhiteSpace(dashOffsetString))
            {
                try
                {
                    double dashOffset = double.Parse(dashOffsetString);
                    drawParam.DashOffset = dashOffset;
                }
                catch
                {
                }
            }
        }

        private static void LoadDrawParamMiterLimit(XmlNode drawParamNode, ref CT_DrawParam drawParam)
        {
            string miterLimitString = XmlUtility.LoadNodeAttributeInnerText(drawParamNode, "MiterLimit");
            if (!string.IsNullOrWhiteSpace(miterLimitString))
            {
                try
                {
                    double miterLimit = double.Parse(miterLimitString);
                    drawParam.MiterLimit = miterLimit;
                }
                catch
                {
                }
            }
        }

        public CT_Color LoadDrawParamFillColor(XmlNode drawParaNode, ref CT_DrawParam drawParam)
        {
            XmlNode fillColorNode = drawParaNode.SelectSingleNode("ofd:FillColor", NamespaceManager);
            string fillColorString = XmlUtility.LoadNodeAttributeInnerText(fillColorNode, "Value");
            if (!string.IsNullOrWhiteSpace(fillColorString))
            {
                drawParam.FillColor = new CT_Color(fillColorString);
            }

            return null;
        }

        public CT_Color LoadDrawParamStrokeColor(XmlNode drawParaNode, ref CT_DrawParam drawParam)
        {
            XmlNode strokeColorNode = drawParaNode.SelectSingleNode("ofd:StrokeColor", NamespaceManager);
            string strokeColorString = XmlUtility.LoadNodeAttributeInnerText(strokeColorNode, "Value");
            if (!string.IsNullOrWhiteSpace(strokeColorString))
            {
                drawParam.StrokeColor = new CT_Color(strokeColorString);
            }

            return null;
        }

        public CT_MultiMedia LoadMultiMedia(XmlNode multiMediaNode)
        {
            if (multiMediaNode?.Attributes != null)
            {
                string idString = multiMediaNode.Attributes["ID"].InnerText;
                string typeString = multiMediaNode.Attributes["Type"].InnerText;
                string formatString = multiMediaNode.Attributes["Format"]?.InnerText;
                if (!string.IsNullOrWhiteSpace(idString) && !string.IsNullOrWhiteSpace(typeString))
                {
                    ST_Loc mediaFileLoc =
                        LoadMediaFile(multiMediaNode.SelectSingleNode("ofd:MediaFile", NamespaceManager));
                    if (mediaFileLoc != null)
                    {
                        return new CT_MultiMedia()
                        {
                            Format = formatString,
                            Id = new ST_ID(idString),
                            Type = new MultiMediaType(typeString),
                            MediaFile = mediaFileLoc
                        };
                    }
                }
            }
            return null;
        }

        public List<CT_Font> LoadFonts(XmlNode fontsNode)
        {
            if (fontsNode != null)
            {
                List<CT_Font> fontList = new List<CT_Font>();
                XmlNodeList fontNodes = fontsNode.SelectNodes("ofd:Font", NamespaceManager);
                if (fontNodes != null && fontNodes.Count > 0)
                {
                    foreach (XmlNode fontNode in fontNodes)
                    {
                        CT_Font font = LoadFont(fontNode);
                        if (font != null)
                        {
                            fontList.Add(font);
                        }
                    }
                }
                return fontList;
            }
            return null;
        }

        public CT_Font LoadFont(XmlNode fontNode)
        {
            if (fontNode?.Attributes != null)
            {
                string fontNameString = fontNode.Attributes["FontName"]?.InnerText;
                string idString = fontNode.Attributes["ID"]?.InnerText;
                if (!string.IsNullOrWhiteSpace(fontNameString) && !string.IsNullOrWhiteSpace(idString))
                {
                    return new CT_Font()
                    {
                        Id = new ST_ID(idString),
                        FontName = fontNameString
                    };
                }
            }
            return null;
        }

        public ST_Loc LoadMediaFile(XmlNode mediaFileNode)
        {
            if (mediaFileNode != null && !string.IsNullOrWhiteSpace(mediaFileNode.InnerText))
            {
                return new ST_Loc(mediaFileNode.InnerText)
                {
                    CurrentDirectory = string.IsNullOrWhiteSpace(ResDirectory) ? CurrentDirectory : ResDirectory,
                };
            }

            return null;
        }

        public ST_Loc LoadResDirectory(XmlNode resNode)
        {
            if (resNode?.Attributes != null)
            {
                string resDirectory = resNode.Attributes["BaseLoc"]?.InnerText;
                if (!string.IsNullOrWhiteSpace(resDirectory))
                {
                    return new ST_Loc(resDirectory)
                    {
                        CurrentDirectory = CurrentDirectory
                    };
                }
            }

            return null;
        }
    }
}