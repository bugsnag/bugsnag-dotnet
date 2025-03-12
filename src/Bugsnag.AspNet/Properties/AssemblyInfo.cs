using System;
using System.Web;

[assembly: CLSCompliant(true)]
[assembly: PreApplicationStartMethod(typeof(Bugsnag.AspNet.HttpModuleAutoLoad), "Attach")]

