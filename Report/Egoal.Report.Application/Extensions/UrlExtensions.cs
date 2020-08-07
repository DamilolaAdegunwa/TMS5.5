using System.Reflection;
using System.Text;

namespace Egoal.Report.Extensions
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
    }
}
