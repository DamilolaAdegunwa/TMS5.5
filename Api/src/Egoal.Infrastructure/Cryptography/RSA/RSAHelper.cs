using System;
using System.Security.Cryptography;

namespace Egoal.Cryptography.RSA
{
    public class RSAHelper
    {
        public static string SignData(byte[] buffer, object halg, int dwKeySize, RSAParameters key)
        {
            return SignData(buffer, halg, dwKeySize, new CspParameters { Flags = CspProviderFlags.UseMachineKeyStore }, key);
        }

        public static string SignData(byte[] buffer, object halg, int dwKeySize, CspParameters parameters, RSAParameters key)
        {
            using (var rsa = new RSACryptoServiceProvider(dwKeySize, parameters))
            {
                rsa.ImportParameters(key);

                return Convert.ToBase64String(rsa.SignData(buffer, halg));
            }
        }

        public static bool VerifyData(byte[] buffer, object halg, string signature, RSAParameters key)
        {
            return VerifyData(buffer, halg, Convert.FromBase64String(signature), key);
        }

        public static bool VerifyData(byte[] buffer, object halg, byte[] signature, RSAParameters key)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(key);

                return rsa.VerifyData(buffer, halg, signature);
            }
        }
    }
}
