using System.Drawing;
using CondenseTech.Ofd.BasicStructure;

namespace CondenseTech.Ofd.Render.Helpers
{
    internal static class ColorHelper
    {
        internal static CT_Color GetFillColor(CT_Text ctText, CT_DrawParam drawParam)
        {
            if (ctText.FillColor != null)
            {
                return ctText.FillColor;
            }

            if (drawParam?.FillColor != null)
            {
                return drawParam.FillColor;
            }
            return new CT_Color(Color.Black);
        }

        internal static CT_Color GetStrokeColor(CT_Text ctText, CT_DrawParam drawParam)
        {
            if (ctText.StrokeColor != null)
            {
                return ctText.StrokeColor;
            }

            if (drawParam?.StrokeColor != null)
            {
                return drawParam.StrokeColor;
            }
            return null;
        }

        internal static CT_Color GetFillColor(CT_Path ctPath, CT_DrawParam drawParam)
        {
            if (ctPath.FillColor != null)
            {
                return ctPath.FillColor;
            }

            if (drawParam?.FillColor != null)
            {
                return drawParam.FillColor;
            }
            return new CT_Color(Color.Transparent);
        }

        internal static CT_Color GetStrokeColor(CT_Path ctPath, CT_DrawParam drawParam)
        {
            if (ctPath.StrokeColor != null)
            {
                return ctPath.StrokeColor;
            }

            if (drawParam?.StrokeColor != null)
            {
                return drawParam.StrokeColor;
            }
            return new CT_Color(Color.Black);
        }
    }
}