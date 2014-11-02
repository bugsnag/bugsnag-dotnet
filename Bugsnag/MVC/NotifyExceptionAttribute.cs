using System;
using System.Web.Mvc;
using System.Web;
using Bugsnag.Event;
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

            var error = new Error(filterContext.Exception);

            // Obtain a reference to the Request.Params collection.
            var pColl = filterContext.HttpContext.Request.Params;

            for (int i = 0; i <= pColl.Count - 1; i++)
            {
                var dataValues = String.Join("\n", pColl.GetValues(i));
                if (!String.IsNullOrEmpty(dataValues))
                    error.MetaData.AddToTab("Request", pColl.GetKey(i), dataValues);
            }

            Client.Notify(error);
        }
    }
}
