using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.BasicStructure
{
    public class CT_MultiMedia
    {
        public ST_ID Id { get; set; } = null;
        public MultiMediaType Type { get; set; } = null;
        public string Format = null;

        public ST_Loc MediaFile { get; set; } = null;
    }
}