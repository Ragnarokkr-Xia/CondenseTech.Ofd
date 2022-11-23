using System;
using System.Collections.Generic;
using System.Drawing;
using SkiaSharp;

namespace CondenseTech.Ofd.Render.Helpers
{
    internal static class CastHelper
    {
        internal static SKPoint ToSkiaPoint(this (float, float) original) =>
            new SKPoint(original.Item1, original.Item2);

        internal static SKImageInfo ToSkiaImageInfo(this RectangleF original) =>
            new SKImageInfo((int)Math.Ceiling(original.Width), (int)Math.Ceiling(original.Height));

        internal static SKImageInfo ToSkiaImageInfo(this Rectangle original) =>
            new SKImageInfo(original.Width, original.Height);

        internal static SKRect ToSkiaRect(this RectangleF original) =>
            new SKRect(original.Left, original.Top, original.Right, original.Bottom);

        internal static SKRect ToSkiaRect(this Rectangle original) =>
            new SKRect(original.Left, original.Top, original.Right, original.Bottom);

        internal static SKMatrix ToSkiaMatrix(this List<float> original)
        {
            if (original == null || original.Count != 6)
            {
                return SKMatrix.Identity;
            }
            SKMatrix originalMatrix = new SKMatrix(original[0], original[2], original[4], original[1], original[3], original[5], 0, 0, 1);
            return originalMatrix;
        }

        internal static SKColor ToSkiaColor(this Color color)
        {
            return new SKColor(color.R, color.G, color.B);
        }
    }
}