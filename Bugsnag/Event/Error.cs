using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bugsnag.Event
{    
    public class Error : Event
    {
        public Exception Exception { get; private set; }
        public bool? IsRuntimeEnding { get; private set; }

        public Error(Exception exp, bool? runtimeEnding = null): base()
        {
            Exception = exp;
            IsRuntimeEnding = runtimeEnding;
            Severity = Severity.Error;
        }
    }
}
