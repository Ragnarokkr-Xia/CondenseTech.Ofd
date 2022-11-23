using System;
using System.Drawing;
using CondenseTech.Ofd.BasicStructure;
using CondenseTech.Ofd.Manager;
using CondenseTech.Ofd.Render.Helpers;
using SkiaSharp;

namespace CondenseTech.Ofd.Render.ElementRenderHandlers
{
    internal class PathRenderHandler : BaseRenderHandler
    {
        public override Type RenderType => typeof(CT_Path);

        protected override SKSurface RenderElement(OfdContainer ofdContainer, DocumentResource documentResource, CT_GraphicUnit graphicUnit, int dpi, CT_DrawParam drawParam = null)
        {
            SKImageInfo overlayInfo = graphicUnit.Boundary.RectangleF.ToSkiaImageInfo().ToScaled(dpi);
            SKSurface overlaySurface = SKSurface.Create(overlayInfo);
            CT_Path ctPath = graphicUnit as CT_Path;
            using (SKCanvas overlayCanvas = overlaySurface.Canvas)
            {
                overlayCanvas.DrawColor(SKColors.Transparent);
                SKMatrix matrix = graphicUnit.CTM.ToSkiaMatrix().ToScaled(dpi);
                overlayCanvas.SetMatrix(matrix);
                Color strokeColor = ColorHelper.GetStrokeColor(ctPath, drawParam);
                float strokeWidth = (float)ctPath.LineWidth;
                SKPaint strokePaint = new SKPaint()
                {
                    Style = SKPaintStyle.StrokeAndFill,
                    Color = strokeColor.ToSkiaColor(),
                    IsStroke = true,
                    StrokeWidth = strokeWidth
                };
                SKPath path = new SKPath();
                GeneratePath(ctPath, path);
                overlayCanvas.DrawPath(path, strokePaint);
                overlayCanvas.ResetMatrix();
                return overlaySurface;
            }
        }

        private static void GeneratePath(CT_Path ctPath, SKPath path)
        {
            foreach (var pathOperation in ctPath.Operations)
            {
                if (pathOperation.Command.HasValue)
                {
                    switch (pathOperation.Command.Value)
                    {
                        case PathCommand.Start:
                            path.Reset();
                            var sPoint = (pathOperation.Operands[0], pathOperation.Operands[1]).ToSkiaPoint();
                            path.MoveTo(sPoint.X, sPoint.Y);
                            break;

                        case PathCommand.Move:
                            var mPoint = (pathOperation.Operands[0], pathOperation.Operands[1]).ToSkiaPoint();
                            path.MoveTo(mPoint.X, mPoint.Y);
                            break;

                        case PathCommand.Line:
                            var lPoint = (pathOperation.Operands[0], pathOperation.Operands[1]).ToSkiaPoint();
                            path.LineTo(lPoint.X, lPoint.Y);
                            break;

                        case PathCommand.CubicBezier:
                            var point1 = (pathOperation.Operands[0], pathOperation.Operands[1]).ToSkiaPoint();
                            var point2 = (pathOperation.Operands[2], pathOperation.Operands[3]).ToSkiaPoint();
                            var point3 = (pathOperation.Operands[4], pathOperation.Operands[5]).ToSkiaPoint();
                            path.CubicTo(point1.X, point1.Y, point2.X, point2.Y, point3.X, point3.Y);
                            break;

                        case PathCommand.Arc:
                            break;

                        case PathCommand.Close:
                            path.Close();
                            break;
                    }
                }
            }
        }
    }
}