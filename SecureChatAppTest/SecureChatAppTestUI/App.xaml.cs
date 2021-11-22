using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SecureChatAppTestUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0 && e.Args[0] == "-debug")
            {
                StartupUri = new Uri("/SecureChatAppTestUI;component/Views/DebugView.xaml", UriKind.Relative);
            }
            else
            {
                StartupUri = new Uri("/SecureChatAppTestUI;component/Views/IpEndpointView.xaml", UriKind.Relative);
            }
        }
    }
}
