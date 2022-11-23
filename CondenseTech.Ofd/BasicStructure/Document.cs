using System.Collections.Generic;
using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.BasicStructure
{
    public class Document
    {
        public CT_CommonData CommonData { get; set; } = null;

        public List<PagePseudo> PageList { get; set; } = null;
        public ST_Loc CustomTagsLocation { get; set; } = null;
        public ST_Loc AttachmentsLocation { get; set; } = null;
    }
}