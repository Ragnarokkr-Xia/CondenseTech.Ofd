using System;
using SkiaSharp;

namespace CondenseTech.Ofd.Render.Helpers
{
    internal static class ScaleHelper
    {
        private const float INCH_PER_MM = 25.4f;

        private static float GetScaleFactor(int dpi) => dpi / INCH_PER_MM;

        internal static SKPoint ToScaled(this SKPoint original, int dpi)
        {
            float x = original.X * GetScaleFactor(dpi);
            float y = original.Y * GetScaleFactor(dpi);
            return new SKPoint(x, y);
        }

        internal static SKImageInfo ToScaled(this SKImageInfo original, int dpi)
        {
            int width = (int)Math.Ceiling(original.Width * GetScaleFactor(dpi));
            int height = (int)Math.Ceiling(original.Height * GetScaleFactor(dpi));
            return new SKImageInfo(width, height);
        }

        internal static SKRect ToScaled(this SKRect original, int dpi)
        {
            float left = original.Left.ToScaled(dpi);
            float top = original.Top.ToScaled(dpi);
            float right = original.Right.ToScaled(dpi);
            float bottom = original.Bottom.ToScaled(dpi);
            return new SKRect(left, top, right, bottom);
        }

        internal static float ToScaled(this float original, int dpi)
        {
            return original * GetScaleFactor(dpi);
        }

        internal static SKMatrix ToScaled(this SKMatrix original, int dpi)
        {
            float scaleFactor = GetScaleFactor(dpi);
            SKMatrix scaleMatrix = new SKMatrix(scaleFactor, 0, 0, 0, scaleFactor, 0, 0, 0, 1);
            SKMatrix resultMatrix = original.PostConcat(scaleMatrix);
            return resultMatrix;
        }
    }
}