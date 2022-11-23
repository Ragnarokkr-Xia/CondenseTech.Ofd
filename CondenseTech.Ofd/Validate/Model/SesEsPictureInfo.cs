using Org.BouncyCastle.Asn1;

namespace CondenseTech.Ofd.Validate.Model
{
    public class SesEsPictureInfo
    {
        public string Type { get; set; }
        public byte[] Data { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public SesEsPictureInfo(Asn1Sequence sesEsPictureInfoSequence)
        {
            Type = sesEsPictureInfoSequence[0].ToString();
            Data = ((Asn1OctetString)sesEsPictureInfoSequence[1]).GetOctets();
            Width = ((DerInteger)sesEsPictureInfoSequence[2]).IntValueExact;
            Height = ((DerInteger)sesEsPictureInfoSequence[3]).IntValueExact;
        }
    }
}