using System;
#if NET45 || NET40
using System.Web;
#endif

[assembly: CLSCompliant(true)]

#if NET45 || NET40
[assembly: PreApplicationStartMethod(typeof(Bugsnag.AspNet.HttpModuleAutoLoad), "Attach")]
#endif
