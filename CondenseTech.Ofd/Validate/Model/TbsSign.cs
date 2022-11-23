using System;
using CondenseTech.Ofd.Miscellaneous;
using Org.BouncyCastle.Asn1;

namespace CondenseTech.Ofd.Validate.Model
{
    public class TbsSign
    {
        private Asn1Sequence Sequence { get; set; }

        public byte[] GetEncoded()
        {
            return Sequence.GetEncoded();
        }

        public SeSeal ESeal { get; set; }
        public int Version { get; set; }
        public DateTime TimeInfo { get; set; }
        public byte[] DataHash { get; set; }
        public string PropertyInfo { get; set; }

        public TbsSign(Asn1Sequence tbsSignSequence)
        {
            Sequence = tbsSignSequence;
            Version = ((DerInteger)Sequence[0]).IntValueExact;
            ESeal = new SeSeal((Asn1Sequence)Sequence[1]);
            TimeInfo = ASN1Utility.GetDateTime(Sequence[2]);
            DataHash = ((DerBitString)Sequence[3]).GetOctets();
            PropertyInfo = ((DerIA5String)Sequence[4]).GetString();
        }
    }
}