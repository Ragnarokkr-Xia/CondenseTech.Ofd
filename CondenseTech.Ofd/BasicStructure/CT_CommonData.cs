using System.Collections.Generic;
using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.BasicStructure
{
    public partial class CT_CommonData
    {
        public const uint DEFAULT_DEFAULT_CS = 0;
    }

    public partial class CT_CommonData
    {
        public ST_ID MaxUnitId { get; set; } = new ST_ID();
        public List<ST_Loc> PublicResList { get; set; } = null;
        public List<ST_Loc> DocumentResList { get; set; } = null;
        public uint? DefaultColorSpace { get; set; } = DEFAULT_DEFAULT_CS;

        public CT_PageArea PageArea { get; set; } = new CT_PageArea();
        public List<TemplatePseudo> TemplatePageList { get; set; } = null;
    }
}