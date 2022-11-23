using System;
using System.Drawing;
using System.Linq;
using CondenseTech.Ofd.BasicStructure;
using CondenseTech.Ofd.Manager;
using CondenseTech.Ofd.Render.Helpers;
using SkiaSharp;

namespace CondenseTech.Ofd.Render.ElementRenderHandlers
{
    internal class TextRenderHandler : BaseRenderHandler
    {
        public override Type RenderType => typeof(CT_Text);

        protected override SKSurface RenderElement(OfdContainer ofdContainer, DocumentResource documentResource, CT_GraphicUnit graphicUnit, int dpi, CT_DrawParam drawParam = null)
        {
            SKImageInfo overlayInfo = graphicUnit.Boundary.RectangleF.ToSkiaImageInfo().ToScaled(dpi);
            SKSurface overlaySurface = SKSurface.Create(overlayInfo);
            CT_Text ctText = graphicUnit as CT_Text;
            CT_Font ctFont = documentResource.Fonts.FirstOrDefault(f => f.Id.Id == ctText.Font.Id);

            using (SKCanvas overlayCanvas = overlaySurface.Canvas)
            {
                overlayCanvas.DrawColor(SKColors.Transparent);
                SKMatrix matrix = graphicUnit.CTM.ToSkiaMatrix().ToScaled(dpi);
                overlayCanvas.SetMatrix(matrix);

                if (ctFont != null && ctText.Size.HasValue)
                {
                    float fontSize = (float)ctText.Size.Value;
                    SKFont font = FontHelper.InstanceFont(ctFont.FontName, fontSize);
                    CT_Color fillColor = ColorHelper.GetFillColor(ctText, drawParam);
                    CT_Color strokeColor = ColorHelper.GetStrokeColor(ctText, drawParam);
                    foreach (var textCode in ctText.TextCodeList)
                    {
                        RenderTextCode(overlayCanvas, textCode, font, strokeColor, fillColor);
                    }
                }

                overlayCanvas.ResetMatrix();
                return overlaySurface;
            }
        }

        private static void RenderTextCode(SKCanvas g, TextCode textCode, SKFont font, CT_Color strokeColor, CT_Color fillColor)
        {
            float textCodeX = textCode.X.HasValue ? (float)textCode.X.Value : 0;
            float textCodeY = textCode.Y.HasValue ? (float)textCode.Y.Value : 0;
            PointF point = new PointF(textCodeX, textCodeY);
            if (textCode.DeltaX?.List != null || textCode.DeltaY?.List != null)
            {
                RenderTextCodeWithDeltaArray(g, textCode, point, font, strokeColor, fillColor);
            }
            else
            {
                RenderTextCodeWithoutDeltaArray(g, textCode, point, font, strokeColor, fillColor);
            }
        }

        private static void RenderTextCodeWithoutDeltaArray(SKCanvas g, TextCode textCode, PointF textPoint, SKFont font,
            CT_Color strokeColor, CT_Color fillColor)
        {
            SKTextBlob text = SKTextBlob.Create(textCode.Text, font);
            SKPaint fillPaint = new SKPaint()
            {
                Color = fillColor.Color.ToSkiaColor(),
                IsStroke = false
            };

            g.DrawText(text, textPoint.X, textPoint.Y, fillPaint);

            if (strokeColor != null)
            {
                SKPaint strokePaint = new SKPaint()
                {
                    Color = strokeColor.Color.ToSkiaColor(),
                    IsStroke = true
                };
                g.DrawText(text, textPoint.X, textPoint.Y, strokePaint);
            }
        }

        private static void RenderTextCodeWithDeltaArray(SKCanvas g, TextCode textCode, PointF textPoint, SKFont font,
            CT_Color strokeColor, CT_Color fillColor)
        {
            var deltaX = textCode.DeltaX?.List;
            var deltaY = textCode.DeltaY?.List;
            int charCount = 0;
            PointF point = new PointF(textPoint.X, textPoint.Y);
            foreach (var textChar in textCode.Text)
            {
                float charDx = (deltaX != null && charCount < deltaX.Count)
                    ? (float)deltaX[charCount] : 0;
                float charDy = (deltaY != null && charCount < deltaY.Count)
                    ? (float)deltaY[charCount] : 0;
                SKTextBlob text = SKTextBlob.Create(textChar.ToString(), font);
                SKPaint fillPaint = new SKPaint()
                {
                    Color = fillColor.Color.ToSkiaColor(),
                    IsStroke = false
                };

                g.DrawText(text, point.X, point.Y, fillPaint);

                if (strokeColor != null)
                {
                    SKPaint strokePaint = new SKPaint()
                    {
                        Color = strokeColor.Color.ToSkiaColor(),
                        IsStroke = true
                    };
                    g.DrawText(text, point.X, point.Y, strokePaint);
                }

                point = new PointF(point.X + charDx, point.Y + charDy);
                charCount += 1;
            }
        }
    }
}