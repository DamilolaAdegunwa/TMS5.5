namespace Egoal.Cryptography.RSA
{
    public class RSAPemFormater
    {
        public static string FormatPKCS1PrivateKey(string str)
        {
            return PemFormater.Format(str, "-----BEGIN RSA PRIVATE KEY-----", "-----END RSA PRIVATE KEY-----");
        }

        public static string RemovePKCS1PrivateKeyFormat(string str)
        {
            return PemFormater.RemoveFormat(str, "-----BEGIN RSA PRIVATE KEY-----", "-----END RSA PRIVATE KEY-----");
        }

        public static string FormatPKCS8PrivateKey(string str)
        {
            return PemFormater.Format(str, "-----BEGIN PRIVATE KEY-----", "-----END PRIVATE KEY-----");
        }

        public static string RemovePKCS8PrivateKeyFormat(string str)
        {
            return PemFormater.RemoveFormat(str, "-----BEGIN PRIVATE KEY-----", "-----END PRIVATE KEY-----");
        }

        public static string FormatPublicKey(string str)
        {
            return PemFormater.Format(str, "-----BEGIN PUBLIC KEY-----", "-----END PUBLIC KEY-----");
        }

        public static string RemovePublicKeyFormat(string str)
        {
            return PemFormater.RemoveFormat(str, "-----BEGIN PUBLIC KEY-----", "-----END PUBLIC KEY-----");
        }
    }
}
