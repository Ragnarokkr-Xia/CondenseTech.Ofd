using Org.BouncyCastle.Asn1;

namespace CondenseTech.Ofd.Validate.Model
{
    public class SesHeader
    {
        public SesHeader(Asn1Sequence sesHeaderSequence)
        {
            Id = sesHeaderSequence[0].ToString();
            Version = ((DerInteger)sesHeaderSequence[1]).IntValueExact;
            VId = sesHeaderSequence[1].ToString();
        }

        public string Id { get; set; } = "ES";
        public int Version { get; set; } = 0;

        public string VId { get; set; } = string.Empty;
    }
}