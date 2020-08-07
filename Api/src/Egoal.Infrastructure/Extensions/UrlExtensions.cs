using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Web;

namespace Egoal.Extensions
{
    public static class UrlExtensions
    {
        public static string UrlCombine(this string url, params string[] args)
        {
            StringBuilder sbUrl = new StringBuilder(url.TrimEnd('/'));

            foreach (var arg in args)
            {
                sbUrl.Append("/").Append(arg.Trim('/'));
            }

            return sbUrl.ToString();
        }

        public static string ToUrlArgs(this object obj, bool ignoreNull = false)
        {
            StringBuilder args = new StringBuilder();

            PropertyInfo[] propertys = obj.GetType().GetProperties();
            foreach (var property in propertys)
            {
                var value = property.GetValue(obj);

                if (value == null)
                {
                    if (!ignoreNull)
                    {
                        args.Append(property.Name).Append("=").Append("&");
                    }
                }
                else
                {
                    args.Append(property.Name).Append("=").Append(value.ToString()).Append("&");
                }
            }

            return args.ToString().Trim('&');
        }

        public static T FromUrlArgs<T>(this string s)
        {
            return s.FromUrlArgs().ToJson().JsonToObject<T>();
        }

        public static Dictionary<string, string> FromUrlArgs(this string s)
        {
            var nameValueCollection = new Dictionary<string, string>();
            var nameValuePairs = s.Split('&');
            foreach (var nameValuePair in nameValuePairs)
            {
                if (nameValuePair.IsNullOrEmpty()) continue;

                var nameValue = nameValuePair.Split('=');
                if (nameValue.IsNullOrEmpty()) continue;

                if (nameValue.Length == 1)
                {
                    nameValueCollection.Add(nameValue[0], string.Empty);
                }
                else if (nameValue.Length == 2)
                {
                    nameValueCollection.Add(nameValue[0], nameValue[1]);
                }
            }

            return nameValueCollection;
        }

        public static string UrlEncode(this string s)
        {
            return HttpUtility.UrlEncode(s, Encoding.UTF8);
        }

        public static string UrlDecode(this string s)
        {
            return HttpUtility.UrlDecode(s);
        }
    }
}
