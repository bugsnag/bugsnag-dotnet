using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bugsnag.Event
{
    public abstract class Event
    {
        public string Context { get; set; }
        public string GroupingHash { get; set; }
        public Severity Severity { get; set; }
        public MetaData MetaData { get; private set; }

        protected Event()
        {
            MetaData = new MetaData();
        }
    }
}
