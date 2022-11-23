using System;
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
    public static class SignatureLoader
    {
        public static Signature Load(ST_Loc signatureXmlFileName, OfdContainer container)
        {
            string currentDirectory = UnixPath.GetParentDirectory(signatureXmlFileName); ;

            byte[] signatureXmlFileData = container[signatureXmlFileName];
            if (signatureXmlFileData == null)
                return null;
            using (MemoryStream ms = new MemoryStream(signatureXmlFileData))
            {
                XmlDocument signatureDocument = new XmlDocument();
                try
                {
                    signatureDocument.Load(ms);
                    var nsManager = new XmlNamespaceManager(signatureDocument.NameTable);
                    var firstElement =
                        (from XmlNode elementNode in signatureDocument.ChildNodes
                         where elementNode.NodeType == XmlNodeType.Element
                         select elementNode).FirstOrDefault();
                    if (firstElement != null && !nsManager.HasNamespace(firstElement.Prefix))
                    {
                        nsManager.AddNamespace(firstElement.Prefix, firstElement.NamespaceURI);
                    }

                    XmlNode signatureNode = signatureDocument.SelectSingleNode("/ofd:Signature", nsManager);
                    Signature signature = LoadSignature(signatureNode, currentDirectory, nsManager);
                    signature.FileLocation = signatureXmlFileName;
                    return signature;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static Signature LoadSignature(XmlNode signatureNode, string currentDirectory, XmlNamespaceManager nsManager)
        {
            if (signatureNode != null)
            {
                Signature signature = new Signature();
                if (signature.SignedInfo == null)
                {
                    signature.SignedInfo = new SignedInfo();
                }

                XmlNode signedInfoNode = signatureNode.SelectSingleNode("ofd:SignedInfo", nsManager);
                if (signedInfoNode != null)
                {
                    signature.SignedInfo = new SignedInfo
                    {
                        Provider = LoadProvider(signedInfoNode.SelectSingleNode("ofd:Provider", nsManager)),
                        SignatureDateTime = LoadSignatureDateTime(
                            signedInfoNode.SelectSingleNode("ofd:SignatureDateTime", nsManager)),
                        SignatureMethod = LoadSignatureMethod(
                            signedInfoNode.SelectSingleNode("ofd:SignatureMethod", nsManager)),
                        References = LoadReferences(
                            signedInfoNode.SelectSingleNode("ofd:References", nsManager),
                            currentDirectory,
                            nsManager),
                        StampAnnotations = LoadStampAnnotations(signedInfoNode.SelectNodes("ofd:StampAnnot", nsManager))
                    };
                }
                signature.SignedValue = LoadSignedValue(signatureNode.SelectSingleNode("ofd:SignedValue", nsManager), currentDirectory);

                return signature;
            }
            return null;
        }

        public static Provider LoadProvider(XmlNode providerNode)
        {
            string providerNameString = XmlUtility.LoadNodeAttributeInnerText(providerNode, "ProviderName");
            if (providerNameString != null)
            {
                return new Provider()
                {
                    ProviderName = providerNameString,
                    Company = XmlUtility.LoadNodeAttributeInnerText(providerNode, "Company"),
                    Version = XmlUtility.LoadNodeAttributeInnerText(providerNode, "Version")
                };
            }

            return null;
        }

        public static string LoadSignatureMethod(XmlNode signatureMethodNode)
        {
            return XmlUtility.LoadNodeInnerText(signatureMethodNode);
        }

        public static DateTime LoadSignatureDateTime(XmlNode signatureDateTimeNode)
        {
            string dateTimeString = XmlUtility.LoadNodeInnerText(signatureDateTimeNode);
            return ASN1Utility.GetDateTime(dateTimeString);
        }

        public static References LoadReferences(XmlNode referencesNode, string currentDirectory,
            XmlNamespaceManager nsManager)
        {
            if (referencesNode != null)
            {
                References references = new References();
                references.CheckMethod = LoadCheckMethod(referencesNode);
                if (references.Reference == null)
                {
                    references.Reference = new List<Reference>();
                }

                XmlNodeList referenceNodes = referencesNode.SelectNodes("ofd:Reference", nsManager);
                if (referenceNodes != null)
                {
                    foreach (XmlNode referenceNode in referenceNodes)
                    {
                        Reference reference = LoadReference(referenceNode, currentDirectory, nsManager);
                        if (reference != null)
                        {
                            references.Reference.Add(reference);
                        }
                    }
                    return references;
                }
            }

            return null;
        }

        public static string LoadCheckMethod(XmlNode referencesNode)
        {
            return XmlUtility.LoadNodeAttributeInnerText(referencesNode, "CheckMethod");
        }

        public static Reference LoadReference(XmlNode referenceNode, string currentDirectory,
            XmlNamespaceManager nsManager)
        {
            if (referenceNode != null)
            {
                string fileRefString = XmlUtility.LoadNodeAttributeInnerText(referenceNode, "FileRef");
                byte[] checkValue = LoadCheckValue(referenceNode.SelectSingleNode("ofd:CheckValue", nsManager));
                if (!string.IsNullOrWhiteSpace(fileRefString) && checkValue != null)
                {
                    return new Reference()
                    {
                        CheckValue = checkValue,
                        FileRef = new ST_Loc(fileRefString)
                        {
                            CurrentDirectory = currentDirectory
                        }
                    };
                }
            }

            return null;
        }

        public static byte[] LoadCheckValue(XmlNode checkValueNode)
        {
            string checkValueString = XmlUtility.LoadNodeInnerText(checkValueNode);
            if (!string.IsNullOrWhiteSpace(checkValueString))
            {
                try
                {
                    return Convert.FromBase64String(checkValueString);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public static List<StampAnnotation> LoadStampAnnotations(XmlNodeList stampAnnotationNodes)
        {
            if (stampAnnotationNodes != null)
            {
                List<StampAnnotation> stampAnnotations = new List<StampAnnotation>();
                foreach (XmlNode stampAnnotationNode in stampAnnotationNodes)
                {
                    StampAnnotation stampAnnotation = LoadStampAnnotation(stampAnnotationNode);
                    if (stampAnnotation != null)
                    {
                        stampAnnotations.Add(stampAnnotation);
                    }
                }

                return stampAnnotations;
            }

            return null;
        }

        public static StampAnnotation LoadStampAnnotation(XmlNode stampAnnotationNode)
        {
            ST_ID id = ST_ID.GetIdInAttribute(stampAnnotationNode);
            string pageRefString = XmlUtility.LoadNodeAttributeInnerText(stampAnnotationNode, "PageRef");
            string boundryString = XmlUtility.LoadNodeAttributeInnerText(stampAnnotationNode, "Boundary");
            if (id != null && !string.IsNullOrWhiteSpace(pageRefString) && !string.IsNullOrWhiteSpace(boundryString))
            {
                string clipString = XmlUtility.LoadNodeAttributeInnerText(stampAnnotationNode, "Clip");
                StampAnnotation stampAnnotation = new StampAnnotation()
                {
                    Id = id,
                    Boundary = new ST_Box(boundryString),
                    PageRef = new ST_ID(pageRefString)
                };
                if (!string.IsNullOrWhiteSpace(clipString))
                {
                    stampAnnotation.Clip = new ST_Box(clipString);
                }

                return stampAnnotation;
            }

            return null;
        }

        public static ST_Loc LoadSignedValue(XmlNode signedValueNode, string currentDirectory)
        {
            string signedValueString = XmlUtility.LoadNodeInnerText(signedValueNode);
            if (!string.IsNullOrWhiteSpace(signedValueString))
            {
                return new ST_Loc(signedValueString)
                {
                    CurrentDirectory = currentDirectory
                };
            }

            return null;
        }
    }
}