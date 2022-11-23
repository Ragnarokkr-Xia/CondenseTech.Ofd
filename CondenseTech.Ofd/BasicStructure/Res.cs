using System.Collections.Generic;

namespace CondenseTech.Ofd.BasicStructure
{
    public class Res
    {
        public List<CT_MultiMedia> MultiMedias { get; set; } = null;
        public List<CT_Font> Fonts { get; set; } = null;
        public List<CT_DrawParam> DrawParams { get; set; } = null;
    }
}