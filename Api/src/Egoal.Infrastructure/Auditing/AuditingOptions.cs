using System;
using System.Collections.Generic;

namespace Egoal.Auditing
{
    public class AuditingOptions
    {
        public bool IsEnabled { get; set; }
        public bool IsEnabledForAnonymousUsers { get; set; }
        public List<Type> IgnoredTypes { get; }
        public bool SaveReturnValues { get; set; }

        public AuditingOptions()
        {
            IgnoredTypes = new List<Type>();
        }
    }
}
