using System;
using System.Web;
using System.Web.Http.Filters;

namespace Bugsnag.Clients
{
    public static class WebAPIClient
    {
        [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
        public sealed class BugsnagExceptionHandler : ExceptionFilterAttribute
        {
            internal BugsnagExceptionHandler()
            {
            }

            public override void OnException(HttpActionExecutedContext context)
            {
                base.OnException(context);
                if (context == null || context.Exception == null)
                    return;

                Client.Notify(context.Exception);
            }
        }

        private static BaseClient Client;
        public static Configuration Config;

        static WebAPIClient()
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

                    if (error.Context == null && HttpContext.Current.Request.Path != null)
                    {
                        error.Context = HttpContext.Current.Request.Path.ToString();
                    }
                }
                return true;
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
            if (Client.Config.AutoNotify)
                Client.Notify(error);
        }
    }
}
