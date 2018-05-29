using System.Collections.Generic;

namespace Bugsnag.Payload
{
  /// <summary>
  /// Represents the "app" key in the error report payload.
  /// </summary>
  public class App : Dictionary<string, object>, IFilterable
  {
    public App(IConfiguration configuration) : this(configuration.AppVersion, configuration.ReleaseStage, configuration.AppType)
    {

    }

    public App(string version, string releaseStage, string type)
    {
      this.AddToPayload("version", version);
      this.AddToPayload("releaseStage", releaseStage);
      this.AddToPayload("type", type);
    }
  }
}
