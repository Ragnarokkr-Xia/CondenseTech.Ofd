using System.IO;
using CondenseTech.Ofd.Validate.Model;
using Org.BouncyCastle.Asn1;

namespace CondenseTech.Ofd.Miscellaneous
{
    public static class SignatureUtility
    {
        public static SeSignature LoadSeSignature(byte[] signatureFileData)
        {
            Asn1Sequence signatureSequence = Asn1Sequence.GetInstance(signatureFileData);
            return new SeSignature(signatureSequence);
        }

        public static SeSignature LoadSeSignature(Stream signatureFileStream)
        {
            byte[] signatureFileData = new byte[signatureFileStream.Length];
            signatureFileStream.Read(signatureFileData, 0, signatureFileData.Length);
            return LoadSeSignature(signatureFileData);
        }
    }
}