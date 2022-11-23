using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.BasicStructure
{
    public class CT_Image : CT_GraphicUnit
    {
        public CT_Image()
        {
        }

        public CT_Image(CT_GraphicUnit GraphicUnit)
        {
            if (GraphicUnit == null) return;
            Id = GraphicUnit.Id;
            Boundary = GraphicUnit.Boundary;
            Name = GraphicUnit.Name;
            Visible = GraphicUnit.Visible;
            LineWidth = GraphicUnit.LineWidth;
            Alpha = GraphicUnit.Alpha;
            CTM = GraphicUnit.CTM;
        }

        public ST_ID ResourceId { get; set; } = null;
    }
}