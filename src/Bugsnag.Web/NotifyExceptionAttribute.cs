using System;
using System.Web;
using System.Web.Mvc;
using Bugsnag.Core;

namespace Bugsnag.Web
{
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public sealed class NotifyExceptionAttribute : HandleErrorAttribute
    {
        public static string ApiKey { get { return BugsnagSingleton.Client.Config.ApiKey; } }

        public NotifyExceptionAttribute(string apiKey)
        {
            BugsnagSingleton.Start(apiKey);
            BugsnagSingleton.Client.Config.BeforeNotify(error =>
            {
                if (HttpContext.Current != null && HttpContext.Current.Request != null && HttpContext.Current.Request.Params != null)
                {
                    var reqParams = HttpContext.Current.Request.Params;

                    for (int i = 0; i <= reqParams.Count - 1; i++)
                    {
                        var dataValues = String.Join("\n", reqParams.GetValues(i));
                        if (!String.IsNullOrEmpty(dataValues))
                            error.Metadata.AddToTab("Request", reqParams.GetKey(i), dataValues);
                    }

                    if (error.Context == null && HttpContext.Current.Request.Path != null)
                    {
                        error.Context = HttpContext.Current.Request.Path.ToString();
                    }
                }
                return true;
            });
        }

        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
            if (filterContext == null || filterContext.Exception == null)
                return;

            BugsnagSingleton.Notify(filterContext.Exception);
        }
    }
}
