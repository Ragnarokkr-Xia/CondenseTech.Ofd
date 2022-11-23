using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.BasicStructure
{
    public class TextCode
    {
        public double? X { get; set; } = null;
        public double? Y { get; set; } = null;
        public ST_Array<double> DeltaX = null;
        public ST_Array<double> DeltaY = null;
        public string Text { get; set; } = null;
    }
}