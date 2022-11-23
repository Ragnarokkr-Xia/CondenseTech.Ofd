using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CondenseTech.Ofd.BasicStructure;
using CondenseTech.Ofd.BasicType;
using CondenseTech.Ofd.Manager;
using CondenseTech.Ofd.Miscellaneous;
using CondenseTech.Ofd.Render.ElementRenderHandlers;
using CondenseTech.Ofd.Render.Helpers;
using CondenseTech.Ofd.Validate.Model;
using SkiaSharp;

namespace CondenseTech.Ofd.Render
{
    public static class RenderUtility
    {
        private static Dictionary<Type, BaseRenderHandler> renders = new Dictionary<Type, BaseRenderHandler>();

        static RenderUtility()
        {
            InstanceRenders();
        }

        public static byte[] RenderImage(OfdManager manager, DocumentResource documentResource, ST_ID pageId, int dpi, SKEncodedImageFormat imageFormat)
        {
            if (documentResource == null)
            {
                return null;
            }
            OfdContainer container = manager.OfdContainer;
            Page page = documentResource.Pages.Where(p => p.Key.Id == pageId.Id).Select(p => p.Value).FirstOrDefault();
            if (page == null)
            {
                return null;
            }
            CT_PageArea pageArea = page.Area ?? documentResource?.Document?.CommonData?.PageArea;
            if (pageArea == null)
            {
                return null;
            }
            SKImageInfo imageInfo = pageArea.ContentBox.RectangleF.ToSkiaImageInfo().ToScaled(dpi);
            using (SKSurface surface = SKSurface.Create(imageInfo))
            {
                using (SKCanvas g = surface.Canvas)
                {
                    if (imageFormat != SKEncodedImageFormat.Png)
                    {
                        g.DrawColor(SKColors.White);
                    }

                    RenderTemplates(g, documentResource, container, page, dpi);
                    RenderLayers(g, documentResource, container, page, pageId, dpi);
                    using (SKImage image = surface.Snapshot())
                    {
                        using (SKData data = image.Encode(imageFormat, 100))
                        {
                            using (Stream stream = data.AsStream())
                            {
                                byte[] buffer = new byte[stream.Length];
                                stream.Read(buffer, 0, buffer.Length);
                                return buffer;
                            }
                        }
                    }
                }
            }
        }

        private static void RenderLayers(SKCanvas g, DocumentResource documentResource, OfdContainer container, Page page, ST_ID pageId, int dpi)
        {
            foreach (var pageLayer in page.Content.Layers)
            {
                CT_DrawParam drawParam = null;
                if (pageLayer.DrawParamId != null)
                {
                    drawParam = documentResource.DrawParams.FirstOrDefault(t => t.Id == pageLayer.DrawParamId.Id);
                }

                RenderLayerOnCanvas(container, pageLayer, documentResource, g, dpi, drawParam);
                RenderStampAnnotationsOnCanvas(pageId, documentResource, g, container, dpi);
            }
        }

        private static void RenderTemplates(SKCanvas g, DocumentResource documentResource, OfdContainer container, Page page, int dpi)
        {
            foreach (var templatePseudo in page.Templates)
            {
                var templatePage = documentResource.TemplatePages.Where(tp => tp.Key.Id == templatePseudo.Id.Id).Select(tp => tp.Value).FirstOrDefault();
                if (templatePage != null)
                {
                    foreach (var templateLayer in templatePage.Content.Layers)
                    {
                        CT_DrawParam drawParam = null;
                        if (templateLayer.DrawParamId != null)
                        {
                            drawParam = documentResource.DrawParams.FirstOrDefault(t => t.Id == templateLayer.DrawParamId);
                        }
                        RenderLayerOnCanvas(container, templateLayer, documentResource, g, dpi, drawParam);
                    }
                }
            }
        }

        private static void RenderLayerOnCanvas(OfdContainer container, Layer layer, DocumentResource documentResource, SKCanvas g, int dpi, CT_DrawParam drawParam)
        {
            foreach (var graphicUnit in layer.GraphicUnits)
            {
                Type type = graphicUnit.GetType();
                BaseRenderHandler render = renders.ContainsKey(type) ? renders[type] : null;
                render?.Render(g, container, documentResource, graphicUnit, dpi, drawParam);
            }
        }

        private static void RenderStampAnnotationsOnCanvas(ST_ID pageId, DocumentResource documentResource, SKCanvas g, OfdContainer container, int dpi)
        {
            foreach (var signatureWithId in documentResource.Signatures)
            {
                Signature signature = signatureWithId.Value;
                if (signature == null)
                {
                    continue;
                }
                byte[] seSignatureData = container[signature.SignedValue];
                if (seSignatureData == null)
                    return;
                try
                {
                    SeSignature seSignature = SignatureUtility.LoadSeSignature(seSignatureData);
                    SesEsPictureInfo stampPictureInfo = seSignature.ToSign.ESeal.SealInfo.Picture;
                    if (stampPictureInfo.Data != null)
                    {
                        switch (stampPictureInfo.Type.ToUpper())
                        {
                            case "OFD":
                                OfdManager manager = new OfdManager(stampPictureInfo.Data);
                                if (manager.DocumentResourceList.Count > 0)
                                {
                                    var tempDoc = manager.DocumentResourceList[0];
                                    if (tempDoc.Pages.Count > 0)
                                    {
                                        byte[] imageBuffer = RenderImage(manager, tempDoc, tempDoc.Pages[0].Key, dpi,
                                            SKEncodedImageFormat.Png);
                                        SKImage image = SKImage.FromEncodedData(imageBuffer);
                                        List<StampAnnotation> presentStampAnnotations = signature.SignedInfo
                                            .StampAnnotations
                                            .Where(stampAnnotation => stampAnnotation.PageRef == pageId.Id).ToList();
                                        foreach (var stampAnnotation in presentStampAnnotations)
                                        {
                                            SKRect dest = stampAnnotation.Boundary.RectangleF.ToSkiaRect()
                                                .ToScaled(dpi);
                                            g.DrawImage(image, dest);
                                        }
                                    }
                                }

                                break;
                        }
                    }
                }
                catch
                {
                    return;
                }
            }
        }

        private static void InstanceRenders()
        {
            try
            {
                var types = Assembly.GetExecutingAssembly().GetTypes();
                foreach (var type in types)
                {
                    if (typeof(BaseRenderHandler).IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        var render = Assembly.GetExecutingAssembly().CreateInstance(type.FullName) as BaseRenderHandler;
                        renders[render.RenderType] = render;
                    }
                }
            }
            catch
            {
                BaseRenderHandler imageRender = new ImageRenderHandler();
                BaseRenderHandler pathRender = new PathRenderHandler();
                BaseRenderHandler textRender = new TextRenderHandler();
                renders[imageRender.RenderType] = imageRender;
                renders[pathRender.RenderType] = pathRender;
                renders[textRender.RenderType] = textRender;
            }
        }
    }
}