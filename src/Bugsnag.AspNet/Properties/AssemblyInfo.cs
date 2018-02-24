using System;
using System.Web;

[assembly: CLSCompliant(true)]

#if NET45 || NET40
[assembly: PreApplicationStartMethod(typeof(Bugsnag.AspNet.HttpModuleAutoLoad), "Attach")]
#endif
