using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.X509;

namespace CondenseTech.Ofd.Validate.Model
{
    public class SeSignature
    {
        public X509Certificate Certificate { get; set; }
        public TbsSign ToSign { get; set; }
        public DerObjectIdentifier SignatureAlgId { get; set; }
        public byte[] Signature { get; set; }

        public SeSignature(Asn1Sequence seSignatureSequence)
        {
            ToSign = new TbsSign((Asn1Sequence)seSignatureSequence[0]);
            byte[] certificateBytes = ((Asn1OctetString)seSignatureSequence[1]).GetOctets();
            Certificate = new X509CertificateParser().ReadCertificate(certificateBytes);
            SignatureAlgId = (DerObjectIdentifier)seSignatureSequence[2];
            Signature = ((DerBitString)seSignatureSequence[3]).GetOctets();
        }
    }
}