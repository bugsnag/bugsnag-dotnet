using System;
using System.Web;

[assembly: CLSCompliant(true)]

#if NET462
[assembly: PreApplicationStartMethod(typeof(Bugsnag.AspNet.HttpModuleAutoLoad), "Attach")]
#endif
