using System.Collections.Generic;
using System.Linq;
using CondenseTech.Ofd.BasicType;

namespace CondenseTech.Ofd.BasicStructure
{
    public class DocumentResource
    {
        public Document Document { get; set; }
        public List<CT_Font> Fonts { get; set; } = new List<CT_Font>();
        public List<CT_MultiMedia> MultiMedias { get; set; } = new List<CT_MultiMedia>();
        public List<CT_DrawParam> DrawParams { get; set; } = new List<CT_DrawParam>();
        public List<KeyValuePair<ST_ID, Page>> TemplatePages { get; set; } = new List<KeyValuePair<ST_ID, Page>>();

        public List<KeyValuePair<ST_ID, Page>> Pages { get; set; } = new List<KeyValuePair<ST_ID, Page>>();

        public List<CustomTagPseudo> CustomTagPseudoList { get; set; } = new List<CustomTagPseudo>();

        public List<KeyValuePair<ST_ID, Signature>> Signatures { get; set; } = new List<KeyValuePair<ST_ID, Signature>>();
        public List<CT_Attachment> AttachmentList { get; set; } = new List<CT_Attachment>();

        public CT_Font FindFont(ST_ID fontId)
        {
            return Fonts.FirstOrDefault(F => F.Id.Id == fontId.Id);
        }

        public Page FindTemplate(ST_ID templateId)
        {
            return TemplatePages.Where(tp => tp.Key.Id == templateId.Id).Select(tp => tp.Value).FirstOrDefault();
        }

        public Page FindPage(ST_ID pageId)
        {
            return Pages.Where(p => p.Key.Id == pageId.Id).Select(p => p.Value).FirstOrDefault();
        }
    }
}