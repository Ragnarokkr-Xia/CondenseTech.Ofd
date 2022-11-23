using System.Drawing;
using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.BasicStructure
{
    public class CT_Color
    {
        private byte alpha = 255;

        public CT_Color()
        {
        }

        public CT_Color(string raw)
        {
            ST_Array<int> colorArray = new ST_Array<int>(raw);
            if (colorArray.List.Count == 3)
            {
                ColorArray = colorArray;
            }
        }

        public CT_Color(string raw, byte alpha)
        {
            ST_Array<int> colorArray = new ST_Array<int>(raw);
            if (colorArray.List.Count == 3)
            {
                ColorArray = colorArray;
            }
            this.alpha = alpha;
        }

        public CT_Color(Color color)
        {
            this.Color = color;
        }

        public Color Color
        {
            get => ColorArray == null ? Color.Empty : Color.FromArgb(alpha, ColorArray.List[0], ColorArray.List[1], ColorArray.List[2]);
            set => ColorArray = new ST_Array<int>(string.Join(" ", value.R, value.G, value.B));
        }

        private ST_Array<int> ColorArray { get; set; } = null;

        public static implicit operator Color(CT_Color ctColor)
        {
            return ctColor.Color;
        }

        public static implicit operator string(CT_Color ctColor)
        {
            return $"{ctColor.Color.R:X2}{ctColor.Color.G:X2}{ctColor.Color.B:X2}";
        }
    }
}