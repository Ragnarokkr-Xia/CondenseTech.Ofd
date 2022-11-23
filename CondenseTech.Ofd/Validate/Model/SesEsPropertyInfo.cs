using System;
using CondenseTech.Ofd.Miscellaneous;
using Org.BouncyCastle.Asn1;

namespace CondenseTech.Ofd.Validate.Model
{
    public class SesEsPropertyInfo
    {
        public int Type { get; set; }
        public string Name { get; set; }
        public CertListType CertListType { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ValidStart { get; set; }
        public DateTime ValidEnd { get; set; }

        public SesEsPropertyInfo(Asn1Sequence sesEsPropertyInfoSequence)
        {
            Type = ((DerInteger)sesEsPropertyInfoSequence[0]).IntValueExact;
            Name = sesEsPropertyInfoSequence[1].ToString();
            object certListTypeOrCertListAssume = sesEsPropertyInfoSequence[2];
            int createDateIndex = 4;
            if (certListTypeOrCertListAssume is Asn1Sequence)
            {
                createDateIndex = 3;
            }
            else if (certListTypeOrCertListAssume is DerInteger)
            {
                CertListType = (CertListType)((DerInteger)sesEsPropertyInfoSequence[2]).IntValueExact;
            }

            int validStartIndex = createDateIndex + 1;
            int validEndIndex = validStartIndex + 1;
            CreateDate = ASN1Utility.GetDateTime(sesEsPropertyInfoSequence[createDateIndex]);
            ValidStart = ASN1Utility.GetDateTime(sesEsPropertyInfoSequence[validStartIndex]);
            ValidEnd = ASN1Utility.GetDateTime(sesEsPropertyInfoSequence[validEndIndex]);
        }
    }

    public enum CertListType
    {
        CertList = 1, CertDigestList = 2
    }
}