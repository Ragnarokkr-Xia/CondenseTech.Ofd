using System;
using CondenseTech.Ofd.BasicStructure;
using CondenseTech.Ofd.Manager;
using CondenseTech.Ofd.Render.Helpers;
using SkiaSharp;

namespace CondenseTech.Ofd.Render.ElementRenderHandlers
{
    internal abstract class BaseRenderHandler
    {
        public abstract Type RenderType { get; }

        public void Render(SKCanvas canvas, OfdContainer ofdContainer, DocumentResource documentResource, CT_GraphicUnit graphicUnit, int dpi, CT_DrawParam drawParam = null)
        {
            if (graphicUnit.GetType() != RenderType)
            {
                return;
            }
            try
            {
                float x = graphicUnit.Boundary.RectangleF.X.ToScaled(dpi);
                float y = graphicUnit.Boundary.RectangleF.Y.ToScaled(dpi);
                using (SKSurface surface = RenderElement(ofdContainer, documentResource, graphicUnit, dpi, drawParam))
                {
                    canvas.DrawSurface(surface, x, y);
                }
            }
            catch
            {
                // supressed
            }
        }

        protected abstract SKSurface RenderElement(OfdContainer ofdContainer, DocumentResource documentResource, CT_GraphicUnit graphicUnit, int dpi, CT_DrawParam drawParam = null);
    }
}