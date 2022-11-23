using System;
using System.IO;
using System.Linq;
using CondenseTech.JBig2Decoder;
using CondenseTech.Ofd.BasicStructure;
using CondenseTech.Ofd.BasicType;
using CondenseTech.Ofd.Manager;
using CondenseTech.Ofd.Render.Helpers;
using SkiaSharp;

namespace CondenseTech.Ofd.Render.ElementRenderHandlers
{
    internal class ImageRenderHandler : BaseRenderHandler
    {
        public override Type RenderType => typeof(CT_Image);

        protected override SKSurface RenderElement(OfdContainer ofdContainer, DocumentResource documentResource, CT_GraphicUnit graphicUnit, int dpi, CT_DrawParam drawParam = null)
        {
            SKImageInfo overlayInfo = graphicUnit.Boundary.RectangleF.ToSkiaImageInfo().ToScaled(dpi);
            SKSurface overlaySurface = SKSurface.Create(overlayInfo);
            CT_Image ctImage = graphicUnit as CT_Image;
            using (SKCanvas overlayCanvas = overlaySurface.Canvas)
            {
                overlayCanvas.DrawColor(SKColors.Transparent);
                CT_MultiMedia multiMedia = documentResource.MultiMedias.FirstOrDefault(mm => mm.Id == ctImage.ResourceId.Id);
                ST_Loc imageLoc = multiMedia?.MediaFile;
                string imageFormat = multiMedia?.Format;
                if (imageLoc == null)
                    return overlaySurface;
                if (string.IsNullOrWhiteSpace(imageFormat))
                    imageFormat = FileHelper.GetExtensionInFilename(imageLoc);
                byte[] imageData = ofdContainer[imageLoc];
                if (imageData == null)
                    return overlaySurface;
                SKMatrix matrix = graphicUnit.CTM.ToSkiaMatrix().ToScaled(dpi);
                overlayCanvas.SetMatrix(matrix);
                using (SKImage image = ReadImage(imageData, imageFormat))
                {
                    float widthPercent = overlayCanvas.LocalClipBounds.Width / image.Width;
                    float heightPercent = overlayCanvas.LocalClipBounds.Height / image.Height;
                    if (Math.Min(widthPercent, heightPercent) < 0.5f)
                    {
                        overlayCanvas.ResetMatrix();
                        SKRect targetRect = new SKRect(0, 0, overlayInfo.Width, overlayInfo.Height);
                        overlayCanvas.DrawImage(image, targetRect);
                    }
                    else
                    {
                        overlayCanvas.DrawImage(image, 0, 0);
                    }
                }
                overlayCanvas.ResetMatrix();
                return overlaySurface;
            }
        }

        private static SKImage ReadImage(byte[] imageData, string imageFormat)
        {
            switch (imageFormat.ToUpper())
            {
                case "JBIG2":
                case "JB2":
                case "GBIG2":
                    byte[] imageDecodedData = new JBIG2StreamDecoder().DecodeJBIG2(imageData, SKEncodedImageFormat.Jpeg);
                    using (MemoryStream fs = new MemoryStream(imageDecodedData))
                    {
                        return SKImage.FromEncodedData(fs);
                    }

                default:
                    using (MemoryStream fs = new MemoryStream(imageData))
                    {
                        return SKImage.FromEncodedData(fs);
                    }
            }
        }
    }
}