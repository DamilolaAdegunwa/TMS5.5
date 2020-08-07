using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Egoal.Report.Json
{
    public class IgnoreContractResolver : DefaultContractResolver
    {
        private string[] ignoreProperties = null;

        public IgnoreContractResolver(string[] ignoreProperties)
        {
            this.ignoreProperties = ignoreProperties;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization);

            return properties.Where(p => !ignoreProperties.Any(ignore => ignore.ToUpper() == p.PropertyName.ToUpper())).ToList();
        }
    }
}
