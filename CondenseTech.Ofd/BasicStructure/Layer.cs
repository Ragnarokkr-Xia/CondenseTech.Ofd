using System.Collections.Generic;
using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.BasicStructure
{
    public class Layer
    {
        public ST_ID DrawParamId { get; set; } = null;
        public ST_ID Id { get; set; } = null;
        public List<CT_GraphicUnit> GraphicUnits = null;
    }
}