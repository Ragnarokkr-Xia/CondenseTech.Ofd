using System.Collections.Generic;
using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.BasicStructure
{
    public class CT_Text : CT_GraphicUnit
    {
        public CT_Text()
        {
        }

        public CT_Text(CT_GraphicUnit graphicUnit)
        {
            if (graphicUnit == null) return;
            Id = graphicUnit.Id;
            Boundary = graphicUnit.Boundary;
            Name = graphicUnit.Name;
            Visible = graphicUnit.Visible;
            LineWidth = graphicUnit.LineWidth;
            Alpha = graphicUnit.Alpha;
            CTM = graphicUnit.CTM;
        }

        public ST_ID Font { get; set; } = null;

        public double? Size { get; set; } = null;
        public bool Stroke { get; set; } = false;
        public bool Fill { get; set; } = true;
        public double HScale { get; set; } = 1;
        public int ReadDirection { get; set; } = 0;
        public int CharDirection { get; set; } = 0;
        public int Weight { get; set; } = 400;
        public bool Italic { get; set; } = false;

        public CT_Color FillColor = null;

        public CT_Color StrokeColor = null;

        //TODO StrokeColor
        public List<TextCode> TextCodeList { get; set; } = null;
    }
}