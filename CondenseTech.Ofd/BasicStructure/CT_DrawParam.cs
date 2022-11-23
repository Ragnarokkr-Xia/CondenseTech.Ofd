using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.BasicStructure
{
    public class CT_DrawParam
    {
        public ST_ID Id { get; set; } = null;
        public double LineWidth { get; set; } = 0.353;
        public double DashOffset { get; set; } = 0;

        public double MiterLimit { get; set; } = 4.234;
        public CT_Color StrokeColor { get; set; } = null;
        public CT_Color FillColor { get; set; } = null;
    }
}