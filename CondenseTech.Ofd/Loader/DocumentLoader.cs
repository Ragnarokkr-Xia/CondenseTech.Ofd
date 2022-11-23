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
    public class DocumentLoader
    {
        private Document Document { get; } = new Document();
        private XmlNamespaceManager NamespaceManager { get; set; }

        private string CurrentDirectory { get; set; }

        public Document Load(ST_Loc documentXmlFileName, OfdContainer container)
        {
            CurrentDirectory = UnixPath.GetParentDirectory(documentXmlFileName);
            byte[] documentXmlFileData = container[documentXmlFileName];
            if (documentXmlFileData == null)
                return null;
            using (MemoryStream ms = new MemoryStream(documentXmlFileData))
            {
                XmlDocument documentDocument = new XmlDocument();
                try
                {
                    documentDocument.Load(ms);
                    NamespaceManager = new XmlNamespaceManager(documentDocument.NameTable);
                    var firstElement =
                        (from XmlNode elementNode in documentDocument.ChildNodes
                         where elementNode.NodeType == XmlNodeType.Element
                         select elementNode).FirstOrDefault();
                    if (firstElement != null && !NamespaceManager.HasNamespace(firstElement.Prefix))
                    {
                        NamespaceManager.AddNamespace(firstElement.Prefix, firstElement.NamespaceURI);
                    }

                    Document.PageList =
                        LoadPages(documentDocument.SelectSingleNode("/ofd:Document/ofd:Pages", NamespaceManager));
                    Document.CommonData =
                        LoadCommonData(documentDocument.SelectSingleNode("/ofd:Document/ofd:CommonData",
                            NamespaceManager));
                    Document.CustomTagsLocation =
                        LoadCustomTagsLocation(
                            documentDocument.SelectSingleNode("/ofd:Document/ofd:CustomTags", NamespaceManager));
                    Document.AttachmentsLocation =
                        LoadAttachmentsLocation(documentDocument.SelectSingleNode("/ofd:Document/ofd:Attachments",
                            NamespaceManager));
                    return Document;
                }
                catch
                {
                    return null;
                }
            }
        }

        private List<PagePseudo> LoadPages(XmlNode pagesNode)
        {
            if (pagesNode != null)
            {
                List<PagePseudo> pageList = new List<PagePseudo>();
                XmlNodeList pageNodes = pagesNode.SelectNodes("ofd:Page", NamespaceManager);
                if (pageNodes == null) return pageList;
                foreach (XmlNode pageNode in pageNodes)
                {
                    PagePseudo page = LoadPage(pageNode);
                    if (page != null)
                    {
                        pageList.Add(page);
                    }
                }

                return pageList;
            }

            return null;
        }

        private PagePseudo LoadPage(XmlNode pageNode)
        {
            if (pageNode?.Attributes == null)
            {
                return null;
            }

            string idString = pageNode.Attributes["ID"].InnerText;
            string baseLocString = pageNode.Attributes["BaseLoc"].InnerText;
            if (string.IsNullOrWhiteSpace(idString) || string.IsNullOrWhiteSpace(baseLocString))
            {
                return null;
            }

            return new PagePseudo()
            {
                BaseLoc = new ST_Loc(baseLocString)
                {
                    CurrentDirectory = CurrentDirectory
                },
                ID = new ST_ID(idString)
            };
        }

        private CT_CommonData LoadCommonData(XmlNode commonDataNode)
        {
            if (commonDataNode != null)
            {
                CT_CommonData commonData = new CT_CommonData
                {
                    MaxUnitId =
                    LoadMaxUnitID(commonDataNode.SelectSingleNode("ofd:MaxUnitID", NamespaceManager)),
                    PublicResList =
                    LoadPublicResList(commonDataNode.SelectNodes("ofd:PublicRes", NamespaceManager)),
                    DocumentResList =
                    LoadDocumentResList(commonDataNode.SelectNodes("ofd:DocumentRes", NamespaceManager)),
                    DefaultColorSpace =
                    LoadDefaultColorSpace(commonDataNode.SelectSingleNode("ofd:DefaultCS", NamespaceManager)),
                    TemplatePageList =
                    LoadTemplatePageList(commonDataNode.SelectNodes("ofd:TemplatePage", NamespaceManager)),
                    PageArea = LoadPageArea(commonDataNode.SelectSingleNode("ofd:PageArea", NamespaceManager))
                };
                return commonData;
            }

            return null;
        }

        private ST_ID LoadMaxUnitID(XmlNode maxUnitIdNode)
        {
            if (!string.IsNullOrWhiteSpace(maxUnitIdNode?.InnerText))
            {
                return new ST_ID(maxUnitIdNode.InnerText);
            }

            return null;
        }

        private List<ST_Loc> LoadPublicResList(XmlNodeList publicResNodes)
        {
            if (publicResNodes != null && publicResNodes.Count > 0)
            {
                List<ST_Loc> publicResList = new List<ST_Loc>();
                foreach (XmlNode publicResNode in publicResNodes)
                {
                    ST_Loc publicRes = LoadPublicRes(publicResNode);
                    if (publicRes != null)
                    {
                        publicResList.Add(publicRes);
                    }
                }

                return publicResList;
            }

            return null;
        }

        private ST_Loc LoadPublicRes(XmlNode publicResNode)
        {
            if (publicResNode != null && !string.IsNullOrWhiteSpace(publicResNode.InnerText))
            {
                return new ST_Loc(publicResNode.InnerText)
                {
                    CurrentDirectory = CurrentDirectory
                };
            }

            return null;
        }

        private List<ST_Loc> LoadDocumentResList(XmlNodeList documentResNodes)
        {
            if (documentResNodes != null)
            {
                List<ST_Loc> documentResList = new List<ST_Loc>();
                foreach (XmlNode documentResNode in documentResNodes)
                {
                    ST_Loc documentRes = LoadDocumentRes(documentResNode);
                    if (documentRes != null)
                    {
                        documentResList.Add(documentRes);
                    }
                }

                return documentResList;
            }

            return null;
        }

        private ST_Loc LoadDocumentRes(XmlNode documentResNode)
        {
            if (documentResNode != null && !string.IsNullOrWhiteSpace(documentResNode.InnerText))
            {
                return new ST_Loc(documentResNode.InnerText)
                {
                    CurrentDirectory = CurrentDirectory
                };
            }

            return null;
        }

        private uint? LoadDefaultColorSpace(XmlNode defaultColorSpaceNode)
        {
            if (defaultColorSpaceNode != null)
            {
                return uint.Parse(defaultColorSpaceNode.InnerText);
            }

            return null;
        }

        private List<TemplatePseudo> LoadTemplatePageList(XmlNodeList templatePageNodes)
        {
            if (templatePageNodes != null)
            {
                List<TemplatePseudo> templatePageList = new List<TemplatePseudo>();
                foreach (XmlNode templatePageNode in templatePageNodes)
                {
                    TemplatePseudo templatePage = LoadTemplatePage(templatePageNode);
                    if (templatePage != null)
                    {
                        templatePageList.Add(templatePage);
                    }
                }

                return templatePageList;
            }

            return null;
        }

        private TemplatePseudo LoadTemplatePage(XmlNode templatePageNode)
        {
            if (templatePageNode?.Attributes != null)
            {
                string idString = templatePageNode.Attributes["ID"]?.InnerText;
                string nameString = templatePageNode.Attributes["Name"]?.InnerText;
                string zOrderString = templatePageNode.Attributes["ZOrder"]?.InnerText;
                string baseLocString = templatePageNode.Attributes["BaseLoc"]?.InnerText;
                if (!string.IsNullOrWhiteSpace(idString) && !string.IsNullOrWhiteSpace(baseLocString))
                    return new TemplatePseudo()
                    {
                        Id = new ST_ID(idString),
                        BaseLoc = new ST_Loc(baseLocString)
                        {
                            CurrentDirectory = CurrentDirectory,
                        },
                        Name = string.IsNullOrWhiteSpace(nameString) ? null : nameString,
                        ZOrder = string.IsNullOrWhiteSpace(zOrderString) ? new LayerType(LayerTypeEnum.Background) : new LayerType(zOrderString)
                    };
            }
            return null;
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

        private ST_Loc LoadCustomTagsLocation(XmlNode customTagsNode)
        {
            if (!string.IsNullOrWhiteSpace(customTagsNode?.InnerText))
            {
                return new ST_Loc(customTagsNode.InnerText)
                {
                    CurrentDirectory = CurrentDirectory
                };
            }

            return null;
        }

        private ST_Loc LoadAttachmentsLocation(XmlNode attachmentsNode)
        {
            if (!string.IsNullOrWhiteSpace(attachmentsNode?.InnerText))
            {
                return new ST_Loc(attachmentsNode.InnerText)
                {
                    CurrentDirectory = CurrentDirectory
                };
            }

            return null;
        }
    }
}