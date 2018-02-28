using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Bugsnag.AspNet;

namespace Bugsnag.Sample.AspNet35
{
  public partial class Index : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      AspNet.Client.Current.Breadcrumbs.Leave("Page_Load");
    }

    protected void Unhandled(object sender, EventArgs e)
    {
      throw new NotImplementedException();
    }

    protected void Handled(object sender, EventArgs e)
    {
      try
      {
        throw new Exception("This could be bad");
      }
      catch (Exception exception)
      {
        AspNet.Client.Current.NotifyWithHttpContext(exception);
      }
    }
  }
}
