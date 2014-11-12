using System;
using System.Web.Mvc;
using System.Web;
using System.Collections.Generic;
using System.Linq;

namespace Bugsnag
{
    public class NotifyExceptionAttribute : HandleErrorAttribute
    {
        private Client Client { get; set; }

        public NotifyExceptionAttribute(Client client)
        {
            Client = client;
        }

        public override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            var error = new Event(filterContext.Exception);

            var reqParams = filterContext.HttpContext.Request.Params;

            for (int i = 0; i <= reqParams.Count - 1; i++)
            {
                var dataValues = String.Join("\n", reqParams.GetValues(i));
                if (!String.IsNullOrEmpty(dataValues))
                    error.MetaData.AddToTab("Request", reqParams.GetKey(i), dataValues);
            }

            Client.Notify(error);
        }
    }
}
