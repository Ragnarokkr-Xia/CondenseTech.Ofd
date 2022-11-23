using System.Collections.Generic;
using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.BasicStructure
{
    public class CT_GraphicUnit
    {
        public ST_ID Id { get; set; } = null;
        public ST_Box Boundary { get; set; } = null;
        public string Name { get; set; } = null;
        public bool Visible { get; set; } = true;

        public List<float> CTM { get; set; } = null;
        public double LineWidth { get; set; } = 0.353;

        public int Alpha { get; set; } = 255;
    }
}