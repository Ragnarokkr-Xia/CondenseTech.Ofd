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
    public class CustomTagsPseudoLoader
    {
        private List<CustomTagPseudo> CustomTags { get; set; }
        private XmlNamespaceManager NamespaceManager { get; set; }

        private string CurrentDirectory { get; set; }

        public List<CustomTagPseudo> Load(ST_Loc customTagsFileName, OfdContainer container)
        {
            CurrentDirectory = UnixPath.GetParentDirectory(customTagsFileName);
            byte[] customTagsFileData = container[customTagsFileName];
            if (customTagsFileData == null)
                return null;
            using (MemoryStream ms = new MemoryStream(customTagsFileData))
            {
                XmlDocument customTagPseudosDocument = new XmlDocument();
                try
                {
                    customTagPseudosDocument.Load(ms);
                    NamespaceManager = new XmlNamespaceManager(customTagPseudosDocument.NameTable);
                    var firstElement =
                        (from XmlNode elementNode in customTagPseudosDocument.ChildNodes
                         where elementNode.NodeType == XmlNodeType.Element
                         select elementNode).FirstOrDefault();
                    if (firstElement != null && !NamespaceManager.HasNamespace(firstElement.Prefix))
                    {
                        NamespaceManager.AddNamespace(firstElement.Prefix, firstElement.NamespaceURI);
                    }

                    CustomTags =
                        LoadCustomTags(customTagPseudosDocument.SelectNodes("/ofd:CustomTags/ofd:CustomTag",
                            NamespaceManager));
                    return CustomTags;
                }
                catch
                {
                    return null;
                }
            }
        }

        private List<CustomTagPseudo> LoadCustomTags(XmlNodeList customTagNodeList)
        {
            if (customTagNodeList != null && customTagNodeList.Count > 0)
            {
                List<CustomTagPseudo> customTagPseudoList = new List<CustomTagPseudo>();
                foreach (XmlNode customTagNode in customTagNodeList)
                {
                    CustomTagPseudo customTag = LoadCustomTag(customTagNode);
                    if (customTag != null)
                    {
                        customTagPseudoList.Add(customTag);
                    }
                }

                return customTagPseudoList;
            }
            return null;
        }

        private CustomTagPseudo LoadCustomTag(XmlNode customTagNode)
        {
            string typeIdString = customTagNode?.Attributes?["TypeID"]?.InnerText;
            if (!string.IsNullOrWhiteSpace(typeIdString))
            {
                ST_Loc fileLocation = LoadCustomTagFileLocation(customTagNode);
                if (fileLocation != null)
                {
                    return new CustomTagPseudo()
                    {
                        TypeId = typeIdString,
                        FileLocation = fileLocation
                    };
                }
            }

            return null;
        }

        private ST_Loc LoadCustomTagFileLocation(XmlNode customTagNode)
        {
            XmlNode fileLocationNode = customTagNode.SelectSingleNode("ofd:FileLoc", NamespaceManager);
            string fileLocationString = fileLocationNode?.InnerText;
            if (!string.IsNullOrWhiteSpace(fileLocationString))
            {
                return new ST_Loc(fileLocationString)
                {
                    CurrentDirectory = CurrentDirectory
                };
            }
            return null;
        }
    }
}