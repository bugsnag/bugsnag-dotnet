using System.Linq;

namespace Bugsnag
{
  /// <summary>
  /// Provide logic that can be derived from a configuration object.
  /// </summary>
  public static class ConfigurationExtensions
  {
    /// <summary>
    /// Determines if the combination of "ReleaseStage" and "NotifyReleaseStages" means
    /// that we should not send the error report
    /// </summary>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static bool ValidReleaseStage(this IConfiguration configuration)
    {
      if (configuration.NotifyReleaseStages == null)
      {
        return true;
      }

      return configuration.NotifyReleaseStages.Any(stage => stage == configuration.ReleaseStage);
    }
  }
}
