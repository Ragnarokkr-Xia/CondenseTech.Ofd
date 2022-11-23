using CondenseTech.Ofd.Miscellaneous;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.X509;

namespace CondenseTech.Ofd.Validate.Model
{
    public class SeSeal
    {
        public SesSealInfo SealInfo { get; set; }
        public X509Certificate Certificate { get; set; }
        public DerObjectIdentifier SignAlgId { get; set; }
        public byte[] SignedValue { get; set; }

        public SeSeal(Asn1Sequence seSealSequence)
        {
            LoadSealInfo(seSealSequence);
            LoadCertificate(seSealSequence);
            LoadSignAlgId(seSealSequence);
            LoadSignedValue(seSealSequence);
        }

        private void LoadSignedValue(Asn1Sequence seSealSequence)
        {
            try
            {
                SignedValue = ((DerBitString)seSealSequence[3]).GetOctets();
            }
            catch
            {
                SignedValue = null;
            }
        }

        private void LoadSignAlgId(Asn1Sequence seSealSequence)
        {
            try
            {
                SignAlgId = (DerObjectIdentifier)seSealSequence[2];
            }
            catch
            {
                SignAlgId = null;
            }
        }

        private void LoadCertificate(Asn1Sequence seSealSequence)
        {
            try
            {
                Asn1OctetString certificateString =
                    (Asn1OctetString)ASN1Utility.ExtractInnerSingleObject((Asn1Object)seSealSequence[1]);
                byte[] certificateBytes = certificateString.GetOctets();
                Certificate = new X509CertificateParser().ReadCertificate(certificateBytes);
            }
            catch
            {
                Certificate = null;
            }
        }

        private void LoadSealInfo(Asn1Sequence seSealSequence)
        {
            try
            {
                Asn1Sequence sesSealInfoSequence = (Asn1Sequence)seSealSequence[0];
                SealInfo = new SesSealInfo(sesSealInfoSequence);
            }
            catch
            {
                SealInfo = null;
            }
        }
    }
}