using System;
using System.Collections.Generic;
using System.Text;

namespace Egoal.Payment
{
    public class H5PayCommand : PayCommandBase
    {
        public string WapUrl { get; set; }
        public string WapName { get; set; }
    }
}
