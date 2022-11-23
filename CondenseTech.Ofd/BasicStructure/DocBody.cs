using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.BasicStructure
{
    public partial class DocBody
    {
        public CT_DocInfo DocInfo { get; set; } = null;
        public ST_Loc DocRoot { get; set; } = null;
        public ST_Loc SignaturesLocation { get; set; } = null;

        //TODO Versions
    }
}