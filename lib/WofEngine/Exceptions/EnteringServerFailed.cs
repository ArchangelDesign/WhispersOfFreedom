using System;
using System.Collections.Generic;
using System.Text;

namespace WofEngine.Exceptions
{
    public class EnteringServerFailed : Exception
    {
        public EnteringServerFailed(string message) : base(message) { }
    }
}
