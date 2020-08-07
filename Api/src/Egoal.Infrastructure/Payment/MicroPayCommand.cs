using System;
using System.Collections.Generic;
using System.Text;

namespace Egoal.Payment
{
    public class MicroPayCommand : PayCommandBase
    {
        public string AuthCode { get; set; }
    }
}
