using Bugsnag.Payload;
using Microsoft.AspNetCore.Http;

namespace Bugsnag.AspNet.Core
{
  static class ReportExtensions
  {
    /// <summary>
    /// Store the httpContext in an error report context.
    /// </summary>
    /// <param name="reportContext"></param>
    /// <param name="httpContext"></param>
    public static void HttpContext(this ReportContext reportContext, HttpContext httpContext)
    {
      reportContext.AddToPayload("bugsnag.aspnet.httpcontext", httpContext);
    }

    /// <summary>
    /// Retrieve the httpContext from an error report context.
    /// </summary>
    /// <param name="reportContext"></param>
    /// <returns></returns>
    public static HttpContext HttpContext(this ReportContext reportContext)
    {
      return reportContext["bugsnag.aspnet.httpcontext"] as HttpContext;
    }
  }
}
