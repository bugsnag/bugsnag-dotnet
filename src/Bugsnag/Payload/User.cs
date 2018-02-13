using System.Collections.Generic;

namespace Bugsnag.Payload
{
  public class User : Dictionary<string, string>
  {
    public string Id
    {
      get
      {
        return this.Get("id");
      }
      set
      {
        this.AddToPayload("id", value);
      }
    }

    public string Name
    {
      get
      {
        return this.Get("name");
      }
      set
      {
        this.AddToPayload("name", value);
      }
    }

    public string Email
    {
      get
      {
        return this.Get("email");
      }
      set
      {
        this.AddToPayload("email", value);
      }
    }
  }
}
