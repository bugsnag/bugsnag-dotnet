using System;
#if NET45
using System.Security;
using System.Web;
#endif

[assembly: CLSCompliant(false)]

#if NET45
[assembly: PreApplicationStartMethod(typeof(Bugsnag.AspNet.HttpModuleAutoLoad), "Attach")]
#endif
