using System;
using System.Web;
using System.Web.Mvc;

namespace Bugsnag.Clients
{
    public static class WebMVCClient
    {
        [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
        public sealed class BugsnagExceptionHandler : HandleErrorAttribute
        {
            internal BugsnagExceptionHandler()
            {
            }

            public override void OnException(ExceptionContext filterContext)
            {
                base.OnException(filterContext);
                if (filterContext == null || filterContext.Exception == null)
                    return;

               Client.Notify(filterContext.Exception);
            }
        }

        private static BaseClient Client;

        static WebMVCClient()
        {
            Client = new BaseClient(ConfigurationStorage.XMLStorage.Settings);
            Client.Config.BeforeNotify(error =>
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

        public static BugsnagExceptionHandler ErrorHandler()
        {
            return new BugsnagExceptionHandler();
        }

        public static void Notify(Exception error)
        {
            Client.Notify(error);
        }
    }
}
