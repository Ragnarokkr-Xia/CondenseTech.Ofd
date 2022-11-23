using Org.BouncyCastle.Asn1;

namespace CondenseTech.Ofd.Validate.Model
{
    public class SesSealInfo
    {
        public SesHeader Header { get; set; }
        public string EsId { get; set; }

        public SesEsPropertyInfo Property { get; set; }
        public SesEsPictureInfo Picture { get; set; }

        public SesSealInfo(Asn1Sequence sesSealInfoSequence)
        {
            Header = LoadHeader(sesSealInfoSequence);

            EsId = LoadEsId(sesSealInfoSequence);

            Property = LoadProperty(sesSealInfoSequence);

            Picture = LoadPicture(sesSealInfoSequence);
        }

        private SesEsPictureInfo LoadPicture(Asn1Sequence sesSealInfoSequence)
        {
            try
            {
                Asn1Sequence sesEsPictureInfoSequence = (Asn1Sequence)sesSealInfoSequence[3];
                return new SesEsPictureInfo(sesEsPictureInfoSequence);
            }
            catch
            {
                return null;
            }
        }

        private SesEsPropertyInfo LoadProperty(Asn1Sequence sesSealInfoSequence)
        {
            try
            {
                Asn1Sequence sesEsPropertyInfoSequence = (Asn1Sequence)sesSealInfoSequence[2];
                return new SesEsPropertyInfo(sesEsPropertyInfoSequence);
            }
            catch
            {
                return null;
            }
        }

        private string LoadEsId(Asn1Sequence sesSealInfoSequence)
        {
            try
            {
                return sesSealInfoSequence[1].ToString();
            }
            catch
            {
                return null;
            }
        }

        private SesHeader LoadHeader(Asn1Sequence sesSealInfoSequence)
        {
            try
            {
                Asn1Sequence sesHeaderSequence = (Asn1Sequence)sesSealInfoSequence[0];
                return new SesHeader(sesHeaderSequence);
            }
            catch
            {
                return null;
            }
        }
    }
}