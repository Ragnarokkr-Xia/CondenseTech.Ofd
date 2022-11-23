using System;
using System.Collections.Generic;
using CondenseTech.Ofd.Validate.Model;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Security.Certificates;
using Org.BouncyCastle.X509;

namespace CondenseTech.Ofd.Miscellaneous
{
    public static class CryptoUtility
    {
        public static readonly Dictionary<string, CryptoMethod> CryptoMethodDictionary = new Dictionary<string, CryptoMethod>()
        {
            {"1.2.156.10197.1.401", CryptoMethod.Sm3DigestWithoutKey},
            {"1.2.156.10197.1.501", CryptoMethod.Sm3WithSm2},
            {"1.2.840.113549.2.5", CryptoMethod.Md5WithRsa},
            {"1.2.840.113549.1.1.4", CryptoMethod.RsaWithMd5}
        };

        public static byte[] Sm3DigestWithoutKey(byte[] content)
        {
            SM3Digest sm3Digest = new SM3Digest();
            sm3Digest.BlockUpdate(content, 0, content.Length);
            byte[] ret = new byte[sm3Digest.GetDigestSize()];
            sm3Digest.DoFinal(ret, 0);
            return ret;
        }

        public static bool GeneralVerifySignature(byte[] content, byte[] sign, AsymmetricKeyParameter publicKey, string signMethod)
        {
            ISigner signer = SignerUtilities.GetSigner(signMethod);
            signer.Init(false, publicKey);
            signer.BlockUpdate(content, 0, content.Length);
            return signer.VerifySignature(sign);
        }

        public static bool CheckCertificateSelfSigned(X509Certificate certificate)
        {
            return certificate.IssuerDN.Equivalent(certificate.SubjectDN);
        }

        public static bool CheckCertificateValidity(X509Certificate certificate, DateTime signDate)
        {
            try
            {
                certificate.CheckValidity(signDate);
                return true;
            }
            catch (CertificateExpiredException)
            {
                return false;
            }
            catch (CertificateNotYetValidException)
            {
                return false;
            }
        }

        public static bool CheckSubjectCertificateIssuerDnEquivalent(X509Certificate subjectCertificate,
            X509Certificate issuerCertificate)
        {
            return subjectCertificate.IssuerDN.Equivalent(issuerCertificate.SubjectDN);
        }

        public static bool VerifySubjectCertificateWithSingleIssuerCertificate(
            X509Certificate subjectCertificate, X509Certificate issuerCertificate,
            DateTime? subjectSignDate = null, DateTime? issuerSignDate = null)
        {
            if (CheckCertificateSelfSigned(subjectCertificate))
            {
                return false;
            }
            if (CheckSubjectCertificateIssuerDnEquivalent(subjectCertificate, issuerCertificate))
            {
                try
                {
                    subjectCertificate.Verify(issuerCertificate.GetPublicKey());
                    return true;
                }
                catch (InvalidKeyException)
                {
                    return false;
                }
                catch (CertificateException)
                {
                    return false;
                }
            }
            if (subjectSignDate != null)
            {
                if (!CheckCertificateValidity(subjectCertificate, subjectSignDate.Value))
                {
                    return false;
                }
            }
            if (issuerSignDate != null)
            {
                if (!CheckCertificateValidity(issuerCertificate, issuerSignDate.Value))
                {
                    return false;
                }
            }

            return false;
        }

        public static bool VerifySubjectCertificateWithMultipleIssuerCertificates(X509Certificate subjectCertificate,
            List<X509Certificate> issuerCertificates, DateTime? subjectSignDate = null, DateTime? issuerSignDate = null)
        {
            foreach (var issuerCertificate in issuerCertificates)
            {
                if (VerifySubjectCertificateWithSingleIssuerCertificate(subjectCertificate, issuerCertificate, subjectSignDate, issuerSignDate))
                    return true;
            }

            return false;
        }
    }
}