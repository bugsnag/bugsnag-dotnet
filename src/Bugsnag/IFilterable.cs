using System.Collections;

namespace Bugsnag
{
  /// <summary>
  /// An interface to mark payload objects as having filtering applied to them.
  /// </summary>
  public interface IFilterable : IDictionary
  {
  }
}
