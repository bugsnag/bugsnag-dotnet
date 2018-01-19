using System.Linq;

namespace Bugsnag
{
  static class ConfigurationExtensions
  {
    public static bool InvalidReleaseStage(this IConfiguration configuration)
    {
      return configuration.NotifyReleaseStages != null &&
              !string.IsNullOrEmpty(configuration.ReleaseStage) &&
              !configuration.NotifyReleaseStages.Any(stage => stage == configuration.ReleaseStage);
    }
  }
}
