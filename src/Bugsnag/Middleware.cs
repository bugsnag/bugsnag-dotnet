namespace Bugsnag
{
  public delegate void Middleware(IConfiguration configuration, Report report);

  static class InternalMiddleware
  {
    public static Middleware ReleaseStageFilter = (c, r) => {
      r.Deliver = r.Deliver && !c.InvalidReleaseStage();
    };
  }
}
