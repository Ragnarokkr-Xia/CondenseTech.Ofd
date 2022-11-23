using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using CondenseTech.Ofd.BasicStructure;
using CondenseTech.Ofd.BasicType;
using CondenseTech.Ofd.Manager;

namespace CondenseTech.Ofd.Loader
{
    public class OfdEntryLoader
    {
        private OfdEntry OfdEntry { get; } = new OfdEntry();
        private XmlNamespaceManager NamespaceManager { get; set; }

        public OfdEntry Load(OfdContainer ofdContainer)
        {
            try
            {
                XmlDocument ofdEntryDocument = new XmlDocument();
                ST_Loc ofdEntryLocation = new ST_Loc("/OFD.xml");
                byte[] ofdEntryFileData = ofdContainer[ofdEntryLocation];
                if (ofdEntryFileData == null)
                    return null;
                using (MemoryStream ms = new MemoryStream(ofdEntryFileData))
                {
                    ofdEntryDocument.Load(ms);
                    NamespaceManager = new XmlNamespaceManager(ofdEntryDocument.NameTable);
                    var firstElement =
                        (from XmlNode elementNode in ofdEntryDocument.ChildNodes
                         where elementNode.NodeType == XmlNodeType.Element
                         select elementNode).FirstOrDefault();
                    if (firstElement != null && !NamespaceManager.HasNamespace(firstElement.Prefix))
                    {
                        NamespaceManager.AddNamespace(firstElement.Prefix, firstElement.NamespaceURI);
                    }

                    LoadRoot(ofdEntryDocument.SelectSingleNode("/ofd:OFD", NamespaceManager));
                    OfdEntry.DocBodyList =
                        LoadDocBodies(ofdEntryDocument.SelectNodes("/ofd:OFD/ofd:DocBody", NamespaceManager));
                }
                return OfdEntry;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    "Unable to load the specified file into an ofd entry, refer to the inner exception for more details.",
                    ex);
            }
        }

        private void LoadRoot(XmlNode rootNode)
        {
            if (rootNode != null)
            {
                string version = rootNode.Attributes?["Version"].InnerText;
                if (!string.IsNullOrWhiteSpace(version))
                {
                    OfdEntry.Version = version;
                }

                string docType = rootNode.Attributes?["DocType"]?.InnerText;
                if (!string.IsNullOrWhiteSpace(docType))
                {
                    OfdEntry.DocType = new DocType(docType);
                }
            }
        }

        private List<DocBody> LoadDocBodies(XmlNodeList docBodyNodes)
        {
            if (docBodyNodes != null)
            {
                List<DocBody> docBodyList = new List<DocBody>();
                foreach (XmlNode docBodyNode in docBodyNodes)
                {
                    docBodyList.Add(LoadDocBody(docBodyNode));
                }

                return docBodyList;
            }

            return null;
        }

        private DocBody LoadDocBody(XmlNode docBodyNode)
        {
            return new DocBody
            {
                DocInfo = LoadDocInfo(docBodyNode.SelectSingleNode("ofd:DocInfo", NamespaceManager)),
                DocRoot = LoadDocRoot(docBodyNode.SelectSingleNode("ofd:DocRoot", NamespaceManager)),
                SignaturesLocation = LoadSignatures(docBodyNode.SelectSingleNode("ofd:Signatures", NamespaceManager))
            };
        }

        private ST_Loc LoadSignatures(XmlNode signaturesNode)
        {
            if (signaturesNode != null)
            {
                return new ST_Loc(signaturesNode.InnerText);
            }

            return null;
        }

        private CT_DocInfo LoadDocInfo(XmlNode docInfoNode)
        {
            if (docInfoNode != null)
            {
                CT_DocInfo docInfo = new CT_DocInfo
                {
                    DocId = LoadDocId(docInfoNode.SelectSingleNode("ofd:DocID", NamespaceManager)),
                    Title = LoadTitle(docInfoNode.SelectSingleNode("ofd:Title", NamespaceManager)),
                    Author = LoadAuthor(docInfoNode.SelectSingleNode("ofd:Author", NamespaceManager)),
                    Subject = LoadSubject(docInfoNode.SelectSingleNode("ofd:Subject", NamespaceManager)),
                    Abstract = LoadAbstract(docInfoNode.SelectSingleNode("ofd:Abstract", NamespaceManager)),
                    Creator = LoadCreator(docInfoNode.SelectSingleNode("ofd:Creator", NamespaceManager)),
                    CreationDate =
                        LoadCreationDate(docInfoNode.SelectSingleNode("ofd:CreationDate", NamespaceManager)),
                    ModDate = LoadModDate(docInfoNode.SelectSingleNode("ofd:ModDate", NamespaceManager)),
                    DocUsage = LoadDocUsage(docInfoNode.SelectSingleNode("ofd:DocUsage", NamespaceManager)),
                    Cover = LoadCover(docInfoNode.SelectSingleNode("ofd:Cover", NamespaceManager)),
                    KeywordList = LoadKeywords(docInfoNode.SelectSingleNode("ofd:Keywords", NamespaceManager)),
                    CreatorVersion =
                        LoadCreatorVersion(docInfoNode.SelectSingleNode("ofd:CreatorVersion", NamespaceManager)),
                    CustomDataList = LoadCustomDatas(docInfoNode.SelectSingleNode("ofd:CustomDatas", NamespaceManager))
                };
                return docInfo;
            }

            return null;
        }

        private ST_Loc LoadDocRoot(XmlNode docRootNode)
        {
            if (docRootNode != null)
            {
                return new ST_Loc(docRootNode.InnerText);
            }

            return null;
        }

        private List<string> LoadKeywords(XmlNode keywordsNode)
        {
            if (keywordsNode != null)
            {
                List<string> keywords = new List<string>();
                XmlNodeList keywordNodes = keywordsNode.SelectNodes("ofd:Keyword", NamespaceManager);
                if (keywordNodes != null)
                {
                    foreach (XmlNode keywordNode in keywordNodes)
                    {
                        string keyword = LoadKeyword(keywordNode);
                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            keywords.Add(keyword);
                        }
                    }
                }

                return keywords;
            }

            return null;
        }

        private string LoadKeyword(XmlNode keywordNode)
        {
            return keywordNode?.InnerText;
        }

        private string LoadDocId(XmlNode docIdNode)
        {
            return docIdNode?.InnerText;
        }

        private string LoadTitle(XmlNode titleNode)
        {
            return titleNode?.InnerText;
        }

        private string LoadAuthor(XmlNode authorNode)
        {
            return authorNode?.InnerText;
        }

        private string LoadSubject(XmlNode subjectNode)
        {
            return subjectNode?.InnerText;
        }

        private string LoadAbstract(XmlNode abstractNode)
        {
            return abstractNode?.InnerText;
        }

        private DocUsage LoadDocUsage(XmlNode docUsageNode)
        {
            if (docUsageNode != null)
            {
                return new DocUsage(docUsageNode.InnerText);
            }

            return null;
        }

        private ST_Loc LoadCover(XmlNode coverNode)
        {
            if (coverNode != null)
            {
                return new ST_Loc(coverNode.InnerText);
            }

            return null;
        }

        private string LoadCreator(XmlNode creatorNode)
        {
            return creatorNode?.InnerText;
        }

        private string LoadCreatorVersion(XmlNode creatorVersionNode)
        {
            return creatorVersionNode?.InnerText;
        }

        private DateTime? LoadCreationDate(XmlNode creationDateNode)
        {
            if (creationDateNode != null)
            {
                string creationDateString = creationDateNode.InnerText;
                creationDateString = creationDateString.Replace("T", " ");
                return DateTime.Parse(creationDateString);
            }

            return null;
        }

        private DateTime? LoadModDate(XmlNode modDateNode)
        {
            if (modDateNode != null)
            {
                string creationDateString = modDateNode.InnerText;
                return DateTime.Parse(creationDateString);
            }

            return null;
        }

        private Dictionary<string, string> LoadCustomDatas(XmlNode customDatasNode)
        {
            if (customDatasNode != null)
            {
                Dictionary<string, string> customDataDictionary = new Dictionary<string, string>();
                XmlNodeList customDataNodes = customDatasNode.SelectNodes("ofd:CustomData", NamespaceManager);
                if (customDataNodes == null) return customDataDictionary;
                foreach (XmlNode customDataNode in customDataNodes)
                {
                    KeyValuePair<string, string>? customData = LoadCustomData(customDataNode);
                    if (customData != null && !customDataDictionary.ContainsKey(customData.Value.Key))
                    {
                        customDataDictionary.Add(customData.Value.Key, customData.Value.Value);
                    }
                }

                return customDataDictionary;
            }

            return null;
        }

        private KeyValuePair<string, string>? LoadCustomData(XmlNode customDataNode)
        {
            if (customDataNode?.Attributes == null)
            {
                return null;
            }

            string customDataKey = customDataNode.Attributes["Name"].InnerText;
            string customDataValue = customDataNode.InnerText;
            if (string.IsNullOrWhiteSpace(customDataKey) || string.IsNullOrWhiteSpace(customDataValue))
            {
                return null;
            }
            return new KeyValuePair<string, string>(customDataKey, customDataValue);
        }
    }
}