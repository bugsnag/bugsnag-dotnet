namespace Bugsnag
{
  /// <summary>
  /// Represents all of the possible breadcrumb types that the Bugsnag API supports.
  /// </summary>
  public enum BreadcrumbType
  {
    Navigation,

    Request,

    Process,

    Log,

    User,

    State,

    Error,

    Manual,
  }
}
