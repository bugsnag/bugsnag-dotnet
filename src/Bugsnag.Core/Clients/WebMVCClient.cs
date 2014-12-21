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
                if (Config.AutoNotify)
                    Client.Notify(filterContext.Exception);
            }
        }

        public static Configuration Config;
        private static BaseClient Client;

        static WebMVCClient()
        {
            Client = new BaseClient(ConfigurationStorage.ConfigSection.Settings);
            Config = Client.Config;
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

                    if (String.IsNullOrEmpty(error.Context) && HttpContext.Current.Request.Path != null)
                    {
                        error.Context = HttpContext.Current.Request.Path.ToString();
                    }

                    if (String.IsNullOrEmpty(error.UserId))
                    {
                        if (!String.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
                        {
                            error.UserId = HttpContext.Current.User.Identity.Name;
                        }
                        else if (HttpContext.Current.Session != null && !String.IsNullOrEmpty(HttpContext.Current.Session.SessionID))
                        {
                            error.UserId = HttpContext.Current.Session.SessionID;
                        }
                        else
                        {
                            error.UserId = HttpContext.Current.Request.UserHostAddress;
                        }
                    }
                }
            });
        }

        public static void Start()
        {

        }

        public static BugsnagExceptionHandler ErrorHandler()
        {
            return new BugsnagExceptionHandler();
        }

        public static void Notify(Exception error)
        {
            Client.Notify(error);
        }

        public static void Notify(Exception error, Metadata metadata)
        {
            Client.Notify(error, metadata);
        }

        public static void Notify(Exception error, Severity severity)
        {
            Client.Notify(error, severity);
        }

        public static void Notify(Exception error, Severity severity, Metadata metadata)
        {
            Client.Notify(error, severity, metadata);
        }
    }
}
