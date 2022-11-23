using System;
using System.Collections.Generic;
using System.Linq;
using CondenseTech.Ofd.BasicStructure;
using CondenseTech.Ofd.BasicType;
using CondenseTech.Ofd.Manager;
using CondenseTech.Ofd.Miscellaneous;
using CondenseTech.Ofd.Validate.Exception;
using CondenseTech.Ofd.Validate.Model;
using Org.BouncyCastle.Crypto;

namespace CondenseTech.Ofd.Validate
{
    public static class Validator
    {
        public static List<ST_Loc> Validate(OfdManager manager, DocumentResource documentResource)
        {
            OfdContainer container = manager.OfdContainer;
            List<ST_Loc> modifiedFiles = new List<ST_Loc>();
            foreach (var signatureWithId in documentResource.Signatures)
            {
                Signature signature = signatureWithId.Value;
                ST_Loc signatureDataLocation = signature.SignedValue;
                byte[] signatureData = container[signatureDataLocation];
                if (signatureData == null)
                    return null;
                try
                {
                    SeSignature seSignature = SignatureUtility.LoadSeSignature(signatureData);
                    ValidateToSignInSeSignature(seSignature);
                    ValidateSignatureXmlInTbsSign(seSignature.ToSign, signature, container);
                    modifiedFiles = modifiedFiles.Concat(ValidateReferences(signature.SignedInfo.References, container))
                        .ToList();
                }
                catch (SignatureDistortedException ex)
                {
                    throw new ValidateTerminatedException("Validation terminated, refer to the inner exception for more details.", ex);
                }
                catch (CertificateRevokedException ex)
                {
                    throw new ValidateTerminatedException("Validation terminated, refer to the inner exception for more details.", ex);
                }
            }
            return modifiedFiles;
        }

        private static List<ST_Loc> ValidateReferences(References references, OfdContainer container)
        {
            string cryptoMethodString = references.CheckMethod;
            List<ST_Loc> modifiedFiles = new List<ST_Loc>();
            if (CryptoUtility.CryptoMethodDictionary.ContainsKey(cryptoMethodString))
            {
                CryptoMethod cryptoMethod = CryptoUtility.CryptoMethodDictionary[cryptoMethodString];
                foreach (var reference in references.Reference)
                {
                    try
                    {
                        byte[] content = container[reference.FileRef];
                        byte[] realHash = null;
                        byte[] desiredHash = reference.CheckValue;
                        switch (cryptoMethod)
                        {
                            case CryptoMethod.Sm3DigestWithoutKey:
                                realHash = CryptoUtility.Sm3DigestWithoutKey(content);
                                break;
                        }

                        if (realHash == null || desiredHash == null || !realHash.SequenceEqual(desiredHash))
                        {
                            modifiedFiles.Add(reference.FileRef);
                        }
                    }
                    catch { }
                }
            }
            return modifiedFiles;
        }

        private static void ValidateSignatureXmlInTbsSign(TbsSign toSign, Signature signature, OfdContainer container)
        {
            SeSeal seal = toSign.ESeal;
            string signAlgIdString = seal.SignAlgId.Id;
            if (!CryptoUtility.CryptoMethodDictionary.ContainsKey(signAlgIdString))
            {
                throw new NotImplementedException($"{signAlgIdString} is not supported.");
            }

            CryptoMethod signatureXmlVerifyMethod = CryptoUtility.CryptoMethodDictionary[signAlgIdString];
            byte[] content = container[signature.FileLocation];
            byte[] desiredHash = toSign.DataHash;
            byte[] realHash = null;
            switch (signatureXmlVerifyMethod)
            {
                case CryptoMethod.Sm3WithSm2:
                    realHash = CryptoUtility.Sm3DigestWithoutKey(content);
                    break;
            }
            if (realHash == null || desiredHash == null || !realHash.SequenceEqual(desiredHash))
            {
                throw new SignatureDistortedException($"{signature.FileLocation} is distorted and cannot be trusted.");
            }
        }

        private static void ValidateToSignInSeSignature(SeSignature seSignature)
        {
            byte[] content = seSignature.ToSign.GetEncoded();
            byte[] desiredSignature = seSignature.Signature;
            AsymmetricKeyParameter signingPublicKey = seSignature.Certificate.GetPublicKey();
            string signingMethodString = seSignature.SignatureAlgId.Id;
            if (!CryptoUtility.CryptoMethodDictionary.ContainsKey(signingMethodString))
            {
                throw new NotImplementedException($"{signingMethodString} is not supported.");
            }
            CryptoMethod signatureVerifyMethod = CryptoUtility.CryptoMethodDictionary[signingMethodString];
            bool signatureVerifyResult = false;
            switch (signatureVerifyMethod)
            {
                case CryptoMethod.Sm3WithSm2:
                    signatureVerifyResult =
                        CryptoUtility.GeneralVerifySignature(content, desiredSignature, signingPublicKey,
                            signatureVerifyMethod.ToString());
                    break;
            }
            if (!signatureVerifyResult)
            {
                throw new SignatureDistortedException("Signature data has been distorted.");
            }
        }
    }
}