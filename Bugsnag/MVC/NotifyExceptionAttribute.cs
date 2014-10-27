using System.Web.Mvc;

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
            Client.Notify(filterContext.Exception);
        }
    }
}
