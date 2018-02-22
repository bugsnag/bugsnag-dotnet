using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Bugsnag.Sample.AspNet35
{
  public partial class Index : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      Singleton.Client.Breadcrumbs.Leave("Looks like the page loaded, that's a good start");
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
        Singleton.Client.Notify(exception);
      }
    }
  }
}
