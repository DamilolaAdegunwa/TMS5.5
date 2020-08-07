using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Text;

namespace Egoal.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson(this object obj, bool nullToEmpty = true)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            if (nullToEmpty)
            {
                StringBuilder sb = new StringBuilder();
                var jsonWriter = new NullJsonWriter(new StringWriter(sb));
                var jsonSerializer = new JsonSerializer();
                jsonSerializer.Serialize(jsonWriter, obj);

                return sb.ToString();
            }

            return JsonConvert.SerializeObject(obj);
        }

        public static T JsonToObject<T>(this string s)
        {
            return JsonConvert.DeserializeObject<T>(s);
        }

        public static bool IsJson(this string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                s = s.Trim().Trim('\r', '\n');
                if (s.Length > 1)
                {
                    return (s.StartsWith("{") && s.EndsWith("}")) || (s.StartsWith("[") && s.EndsWith("]"));
                }
            }

            return false;
        }

        public static string GetJsonKeyValue(this string json, string key)
        {
            int keyIndex = json.IndexOf(key);
            if (keyIndex == -1)
            {
                return string.Empty;
            }

            while (json[keyIndex - 1] != '"' || json[keyIndex + key.Length] != '"')
            {
                keyIndex = json.IndexOf(key, keyIndex + 1);
                if (keyIndex == -1)
                {
                    return string.Empty;
                }
            }

            int length = key.Length;
            int valueIndex = keyIndex + length + 2;
            char valueFirstChar = json[valueIndex];
            if (valueFirstChar == '{')
            {
                return GetObjectValue(valueIndex, json);
            }
            if (valueFirstChar == '"')
            {
                return GetStringValue(valueIndex, json);
            }

            return string.Empty;
        }

        private static string GetObjectValue(int index, string json)
        {
            int num = 0;
            StringBuilder stringBuilder = new StringBuilder();
            char ch;
            while (true)
            {
                ch = json[index];
                if (ch == '{')
                {
                    ++num;
                }
                if (ch == '}')
                {
                    --num;
                }
                if (num != 0)
                {
                    stringBuilder.Append(ch);
                    ++index;
                }
                else
                {
                    break;
                }
            }
            stringBuilder.Append(ch);
            return stringBuilder.ToString();
        }

        private static string GetStringValue(int index, string json)
        {
            StringBuilder stringBuilder = new StringBuilder();
            ++index;
            while (true)
            {
                char ch = json[index++];
                if (ch != '"')
                {
                    stringBuilder.Append(ch);
                }
                else
                {
                    break;
                }
            }
            return stringBuilder.ToString();
        }

        private class NullJsonWriter : JsonTextWriter
        {
            public NullJsonWriter(TextWriter textWriter)
                : base(textWriter)
            {
            }

            public override void WriteNull()
            {
                WriteValue(string.Empty);
            }
        }
    }

    public class DateConverter : IsoDateTimeConverter
    {
        public DateConverter()
        {
            DateTimeFormat = DateTimeExtensions.DateFormat;
        }
    }

    public class DateTimeConverter : IsoDateTimeConverter
    {
        public DateTimeConverter()
        {
            DateTimeFormat = DateTimeExtensions.DateTimeFormat;
        }
    }
}
