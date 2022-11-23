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
    public static class SignaturePseudoListLoader
    {
        public static List<SignaturePseudo> Load(ST_Loc signaturePseudoXmlFileName, OfdContainer container)
        {
            string currentDirectory = UnixPath.GetParentDirectory(signaturePseudoXmlFileName);
            byte[] signaturePseudoXmlFileData = container[signaturePseudoXmlFileName];
            if (signaturePseudoXmlFileData == null)
                return null;
            using (MemoryStream ms = new MemoryStream(signaturePseudoXmlFileData))
            {
                XmlDocument signaturePseudoDocument = new XmlDocument();
                try
                {
                    signaturePseudoDocument.Load(ms);
                    var nsManager = new XmlNamespaceManager(signaturePseudoDocument.NameTable);
                    var firstElement =
                        (from XmlNode elementNode in signaturePseudoDocument.ChildNodes
                         where elementNode.NodeType == XmlNodeType.Element
                         select elementNode).FirstOrDefault();
                    if (firstElement != null && !nsManager.HasNamespace(firstElement.Prefix))
                    {
                        nsManager.AddNamespace(firstElement.Prefix, firstElement.NamespaceURI);
                    }

                    List<SignaturePseudo> signaturePseudoList = new List<SignaturePseudo>();
                    XmlNodeList signaturePseudoNodes =
                        signaturePseudoDocument.SelectNodes("/ofd:Signatures/ofd:Signature", nsManager);
                    if (signaturePseudoNodes != null)
                    {
                        foreach (XmlNode signaturePseudoNode in signaturePseudoNodes)
                        {
                            SignaturePseudo signaturePseudo =
                                LoadSignaturePseudo(signaturePseudoNode, currentDirectory);
                            if (signaturePseudo != null)
                            {
                                signaturePseudoList.Add(signaturePseudo);
                            }
                        }
                    }

                    return signaturePseudoList;
                }
                catch
                {
                    return null;
                }
            }
        }

        private static SignaturePseudo LoadSignaturePseudo(XmlNode signaturePseudoNode, string currentDirectory)
        {
            ST_ID signaturePseudoId = ST_ID.GetIdInAttribute(signaturePseudoNode);
            string signaturePseudoLocationString = signaturePseudoNode?.Attributes?["BaseLoc"]?.InnerText;

            if (signaturePseudoId != null && signaturePseudoLocationString != null)
            {
                SignaturePseudo signaturePseudo = new SignaturePseudo()
                {
                    Id = signaturePseudoId,
                    BaseLocation = new ST_Loc(signaturePseudoLocationString)
                    {
                        CurrentDirectory = currentDirectory
                    }
                };
                LoadSignaturePseudo(signaturePseudoNode, ref signaturePseudo);
                return signaturePseudo;
            }

            return null;
        }

        private static void LoadSignaturePseudo(XmlNode signaturePseudoNode, ref SignaturePseudo signaturePseudo)
        {
            string signaturePseudoTypeString = signaturePseudoNode?.Attributes?["Type"]?.InnerText;
            if (!string.IsNullOrWhiteSpace(signaturePseudoTypeString))
            {
                switch (signaturePseudoTypeString.Trim().ToUpper())
                {
                    case "SEAL":
                        signaturePseudo.Type = SignatureType.Seal;
                        break;

                    case "SIGN":
                        signaturePseudo.Type = SignatureType.Sign;
                        break;
                }
            }
        }
    }
}