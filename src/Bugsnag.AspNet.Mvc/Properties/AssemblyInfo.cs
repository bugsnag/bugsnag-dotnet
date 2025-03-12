using System;
using System.Web;

[assembly: CLSCompliant(true)]
[assembly: PreApplicationStartMethod(typeof(Bugsnag.AspNet.Mvc.AutoLoad), "Attach")]

