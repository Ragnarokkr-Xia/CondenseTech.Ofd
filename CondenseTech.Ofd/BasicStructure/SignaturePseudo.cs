using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.BasicStructure
{
    public class SignaturePseudo
    {
        public ST_ID Id { get; set; } = null;
        public ST_Loc BaseLocation { get; set; } = null;
        public SignatureType Type { get; set; } = SignatureType.Seal;
    }
}