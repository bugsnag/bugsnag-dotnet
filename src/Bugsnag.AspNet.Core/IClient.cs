using System;
using Microsoft.AspNetCore.Http;

namespace Bugsnag.AspNet.Core
{
  /// <summary>
  /// The Bugsnag error reporting client.
  /// </summary>
  public interface IClient
  {
    /// <summary>
    /// Used to notify Bugsnag of an unhandled exception.
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="context"></param>
    void AutoNotify(Exception exception, HttpContext context);

    /// <summary>
    /// Used to leave breadcrumbs that will be attached to any errrors seen
    /// in the current request.
    /// </summary>
    Breadcrumbs Breadcrumbs { get; }
  }
}
