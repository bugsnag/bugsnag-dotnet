using System.Windows;
using Bugsnag.Clients;

namespace Desktop.WPFApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            WPFClient.Start();
            base.OnStartup(e);

        }
    }
}
