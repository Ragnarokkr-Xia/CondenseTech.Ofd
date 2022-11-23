using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.BasicStructure
{
    public class CT_Font
    {
        public ST_ID Id { get; set; } = null;
        public string FontName { get; set; } = null;
        public string FamilyName { get; set; } = null;
        public string Charset { get; set; } = "unicode";
        public bool Italic { get; set; } = false;
        public bool Bold { get; set; } = false;
        public bool Serif { get; set; } = false;
        public bool FixedWidth { get; set; } = false;
    }
}