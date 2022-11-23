using System.Collections.Generic;
using System.Linq;
using CondenseTech.Ofd.BasicStructure;
using CondenseTech.Ofd.BasicType;
using CondenseTech.Ofd.Loader;

namespace CondenseTech.Ofd.Manager
{
    public class OfdManager
    {
        public List<DocumentResource> DocumentResourceList { get; set; } = new List<DocumentResource>();
        public OfdContainer OfdContainer { get; private set; }

        public OfdManager(string ofdFileName)
        {
            OfdContainer = new OfdContainer(ofdFileName);
            LoadDocBodies();
        }

        public OfdManager(byte[] ofdFileData)
        {
            OfdContainer = new OfdContainer(ofdFileData);
            LoadDocBodies();
        }

        private void LoadDocBodies()
        {
            OfdEntryLoader entryLoader = new OfdEntryLoader();
            var ofdEntry = entryLoader.Load(OfdContainer);
            foreach (var docBody in ofdEntry.DocBodyList)
            {
                DocumentResource documentResource = new DocumentResource();
                var document = new DocumentLoader().Load(docBody.DocRoot, OfdContainer);
                documentResource.Document = document;
                if (document.CommonData.DocumentResList != null)
                {
                    foreach (var documentRes in document.CommonData.DocumentResList)
                    {
                        var res = new ResLoader().Load(documentRes, OfdContainer);
                        if (res.MultiMedias != null)
                        {
                            documentResource.MultiMedias = documentResource.MultiMedias.Concat(res.MultiMedias).ToList();
                        }

                        if (res.Fonts != null)
                        {
                            documentResource.Fonts = documentResource.Fonts.Concat(res.Fonts).ToList();
                        }
                        if (res.DrawParams != null)
                        {
                            documentResource.DrawParams = documentResource.DrawParams.Concat(res.DrawParams).ToList();
                        }
                    }
                }

                if (document.CommonData.PublicResList != null)
                {
                    foreach (var publicRes in document.CommonData.PublicResList)
                    {
                        var res = new ResLoader().Load(publicRes, OfdContainer);
                        if (res.MultiMedias != null)
                        {
                            documentResource.MultiMedias = documentResource.MultiMedias.Concat(res.MultiMedias).ToList();
                        }

                        if (res.Fonts != null)
                        {
                            documentResource.Fonts = documentResource.Fonts.Concat(res.Fonts).ToList();
                        }
                        if (res.DrawParams != null)
                        {
                            documentResource.DrawParams = documentResource.DrawParams.Concat(res.DrawParams).ToList();
                        }
                    }
                }

                if (document.CommonData.TemplatePageList != null)
                {
                    foreach (var templatePseudo in document.CommonData.TemplatePageList)
                    {
                        Page targetPage = new PageLoader().Load(templatePseudo.BaseLoc, OfdContainer);
                        if (targetPage != null)
                        {
                            documentResource.TemplatePages.Add(new KeyValuePair<ST_ID, Page>(templatePseudo.Id, targetPage));
                        }
                    }
                }

                if (document.PageList != null)
                {
                    foreach (var pagePseudo in document.PageList)
                    {
                        Page targetPage = new PageLoader().Load(pagePseudo.BaseLoc, OfdContainer);
                        if (targetPage != null)
                        {
                            documentResource.Pages.Add(new KeyValuePair<ST_ID, Page>(pagePseudo.ID, targetPage));
                        }
                    }
                }

                if (document.CustomTagsLocation != null)
                {
                    var customTagsPseudoLocations = new CustomTagsPseudoLoader().Load(document.CustomTagsLocation, OfdContainer);
                    if (customTagsPseudoLocations != null)
                    {
                        documentResource.CustomTagPseudoList =
                            documentResource.CustomTagPseudoList.Concat(customTagsPseudoLocations).ToList();
                    }
                }
                if (document.AttachmentsLocation != null)
                {
                    var attachmentLocations = new AttachmentListLoader().Load(document.AttachmentsLocation, OfdContainer);
                    if (attachmentLocations != null)
                    {
                        documentResource.AttachmentList =
                            documentResource.AttachmentList.Concat(attachmentLocations).ToList();
                    }
                }

                if (docBody.SignaturesLocation != null)
                {
                    List<SignaturePseudo> signaturePseudoList =
                        SignaturePseudoListLoader.Load(docBody.SignaturesLocation, OfdContainer);
                    foreach (var signaturePseudo in signaturePseudoList)
                    {
                        Signature signature =
                            SignatureLoader.Load(signaturePseudo.BaseLocation, OfdContainer);
                        if (signature != null)
                        {
                            signature.Type = signaturePseudo.Type;
                            documentResource.Signatures.Add(new KeyValuePair<ST_ID, Signature>(signaturePseudo.Id, signature));
                        }
                    }
                }

                DocumentResourceList.Add(documentResource);
            }
        }
    }
}