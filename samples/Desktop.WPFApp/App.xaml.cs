using Bugsnag.Clients;
using System.Collections.Generic;
using System.Windows;

namespace Desktop.WPFApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //WPFClient.Config.SetUser("2222", "cccc@dddd.com", "CCcc Dddd");
            //WPFClient.Config.FilePrefixes = new string[] {@"e:\GitHub\Bugsnag-NET\"};

            base.OnStartup(e);

        }
    }
}
