using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Security.Cryptography;

namespace Egoal.Cryptography.RSA
{
    public class RSAKeyHelper
    {
        public static RSAParameters CreatePKCS1PrivateKey(string privateKey)
        {
            privateKey = RSAPemFormater.FormatPKCS1PrivateKey(privateKey);

            PemReader pemReader = new PemReader(new StringReader(privateKey));
            if (!(pemReader.ReadObject() is AsymmetricCipherKeyPair asymmetricCipherKeyPair))
            {
                throw new Exception("Private key format is incorrect");
            }

            var rsaPrivateCrtKeyParameters = (RsaPrivateCrtKeyParameters)asymmetricCipherKeyPair.Private;

            var rsap = new RSAParameters();
            rsap.Modulus = rsaPrivateCrtKeyParameters.Modulus.ToByteArrayUnsigned();
            rsap.Exponent = rsaPrivateCrtKeyParameters.PublicExponent.ToByteArrayUnsigned();
            rsap.P = rsaPrivateCrtKeyParameters.P.ToByteArrayUnsigned();
            rsap.Q = rsaPrivateCrtKeyParameters.Q.ToByteArrayUnsigned();
            rsap.DP = rsaPrivateCrtKeyParameters.DP.ToByteArrayUnsigned();
            rsap.DQ = rsaPrivateCrtKeyParameters.DQ.ToByteArrayUnsigned();
            rsap.InverseQ = rsaPrivateCrtKeyParameters.QInv.ToByteArrayUnsigned();
            rsap.D = rsaPrivateCrtKeyParameters.Exponent.ToByteArrayUnsigned();

            return rsap;
        }

        public static RSAParameters CreatePKCS1PublicKey(string publicKey)
        {
            publicKey = RSAPemFormater.FormatPublicKey(publicKey);

            PemReader pemReader = new PemReader(new StringReader(publicKey));
            if (!(pemReader.ReadObject() is RsaKeyParameters rsaKeyParameters))
            {
                throw new Exception("Public key format is incorrect");
            }

            var rsap = new RSAParameters();
            rsap.Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned();
            rsap.Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned();

            return rsap;
        }

        public static RSAParameters CreatePKCS8PrivateKey(string privateKey)
        {
            privateKey = RSAPemFormater.RemovePKCS8PrivateKeyFormat(privateKey);

            var rsaPrivateCrtKeyParameters = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));

            var rsap = new RSAParameters();
            rsap.Modulus = rsaPrivateCrtKeyParameters.Modulus.ToByteArrayUnsigned();
            rsap.Exponent = rsaPrivateCrtKeyParameters.PublicExponent.ToByteArrayUnsigned();
            rsap.P = rsaPrivateCrtKeyParameters.P.ToByteArrayUnsigned();
            rsap.Q = rsaPrivateCrtKeyParameters.Q.ToByteArrayUnsigned();
            rsap.DP = rsaPrivateCrtKeyParameters.DP.ToByteArrayUnsigned();
            rsap.DQ = rsaPrivateCrtKeyParameters.DQ.ToByteArrayUnsigned();
            rsap.InverseQ = rsaPrivateCrtKeyParameters.QInv.ToByteArrayUnsigned();
            rsap.D = rsaPrivateCrtKeyParameters.Exponent.ToByteArrayUnsigned();

            return rsap;
        }

        public static RSAParameters CreatePKCS8PublicKey(string publicKey)
        {
            publicKey = RSAPemFormater.RemovePublicKeyFormat(publicKey);

            var rsaKeyParameters = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));

            var rsap = new RSAParameters();
            rsap.Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned();
            rsap.Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned();

            return rsap;
        }
    }
}
