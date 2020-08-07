using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Egoal.Report.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static T ToObject<T>(this NameValueCollection collection)
        {
            var values = new Dictionary<string, string>();
            foreach (var key in collection.AllKeys)
            {
                values.Add(key, collection[key]);
            }

            var json = JsonConvert.SerializeObject(values);

            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
