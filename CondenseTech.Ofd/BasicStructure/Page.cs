using System.Collections.Generic;
using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.BasicStructure
{
    public class Page
    {
        public CT_PageArea Area { get; set; } = null;
        public List<TemplatePseudo> Templates { get; set; } = null;
        public Content Content { get; set; } = null;
        public List<ST_Loc> PageResList { get; set; } = null;
    }
}