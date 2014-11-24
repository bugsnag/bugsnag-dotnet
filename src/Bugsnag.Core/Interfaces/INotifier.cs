
namespace Bugsnag.Core
{
    public interface INotifier
    {
        void Send(Event errorEvent);
    }
}
