using Bugsnag.Payload;
using System.Web;

namespace Bugsnag.AspNet
{
  static class ReportExtensions
  {
    public static void HttpContext(this ReportContext reportContext, HttpContextBase httpContext)
    {
      reportContext.AddToPayload("bugsnag.aspnet.httpcontext", httpContext);
    }

    public static HttpContextBase HttpContext(this ReportContext reportContext)
    {
      return reportContext["bugsnag.aspnet.httpcontext"] as HttpContextBase;
    }
  }
}
