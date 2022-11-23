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
    public class AttachmentListLoader
    {
        private List<CT_Attachment> AttachmentList { get; set; }
        private XmlNamespaceManager NamespaceManager { get; set; }

        private string CurrentDirectory { get; set; }

        public List<CT_Attachment> Load(ST_Loc attachmentsFilename, OfdContainer container)
        {
            CurrentDirectory = UnixPath.GetParentDirectory(attachmentsFilename);
            byte[] attachmentsFileData = container[attachmentsFilename];
            if (attachmentsFileData == null)
                return null;
            using (MemoryStream ms = new MemoryStream(attachmentsFileData))
            {
                XmlDocument attachmentsDocument = new XmlDocument();
                try
                {
                    attachmentsDocument.Load(ms);
                    NamespaceManager = new XmlNamespaceManager(attachmentsDocument.NameTable);
                    var firstElement =
                        (from XmlNode elementNode in attachmentsDocument.ChildNodes
                         where elementNode.NodeType == XmlNodeType.Element
                         select elementNode).FirstOrDefault();
                    if (firstElement != null && !NamespaceManager.HasNamespace(firstElement.Prefix))
                    {
                        NamespaceManager.AddNamespace(firstElement.Prefix, firstElement.NamespaceURI);
                    }

                    AttachmentList =
                        LoadAttachments(attachmentsDocument.SelectNodes("/ofd:Attachments/ofd:Attachment",
                            NamespaceManager));
                    return AttachmentList;
                }
                catch
                {
                    return null;
                }
            }
        }

        private List<CT_Attachment> LoadAttachments(XmlNodeList attachmentNodeList)
        {
            if (attachmentNodeList != null && attachmentNodeList.Count > 0)
            {
                List<CT_Attachment> attachmentList = new List<CT_Attachment>();
                foreach (XmlNode attachmentNode in attachmentNodeList)
                {
                    CT_Attachment attachment = LoadAttachment(attachmentNode);
                    if (attachment != null)
                    {
                        attachmentList.Add(attachment);
                    }
                }

                return attachmentList;
            }
            return null;
        }

        private CT_Attachment LoadAttachment(XmlNode customTagNode)
        {
            string idString = customTagNode?.Attributes?["ID"]?.InnerText;
            string nameString = customTagNode?.Attributes?["Name"]?.InnerText;
            if (!string.IsNullOrWhiteSpace(idString) && !string.IsNullOrWhiteSpace(nameString))
            {
                ST_Loc fileLocation = LoadAttachmentFileLocation(customTagNode);
                if (fileLocation != null)
                {
                    return new CT_Attachment()
                    {
                        Id = new ST_ID(idString),
                        Name = nameString,
                        FileLocation = fileLocation
                    };
                }
            }

            return null;
        }

        private ST_Loc LoadAttachmentFileLocation(XmlNode attachmentNode)
        {
            XmlNode fileLocationNode = attachmentNode.SelectSingleNode("ofd:FileLoc", NamespaceManager);
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