using SecureChatAppTest.Components.Network;
using SecureChatAppTest.Network.Network.TCP;
using SecureChatAppTest.Network.Network.TCP.Client;
using SecureChatAppTest.Network.Network.TCP.Server;
using SecureChatAppTestUI.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SecureChatAppTestUI.Views
{
    /// <summary>
    /// Interaction logic for IpEndpointView.xaml
    /// </summary>
    public partial class IpEndpointView : Window
    {
        public static IConnector connector;
        public static ICommunicator communicator;
        private ChatView chatView;

        public IpEndpointView()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var window = (Window)sender;
            window.Closing -= Window_Closing;

            if (communicator != null)
                communicator.Stop();

            Show();
        }

        private void BeginSessionButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsSessionIdValid() && IsUsernameValid())
            {
                try
                {
                    if (((IpEndpointVM)DataContext).ServerRadioButton)
                    {
                        if (connector != null)
                            connector.Stop();

                        connector = new TcpServerConnector(CheckIpAddress(), CheckPortNumber());
                        communicator = new TcpSocketCommunicator(connector);
                    }
                    else
                    {
                        if (connector != null)
                            connector.Stop();

                        connector = new TcpClientConnector(CheckIpAddress(), CheckPortNumber());
                        communicator = new TcpSocketCommunicator(connector);
                    }

                    connector.Start();
                    IpEndPointWindowManager();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Address or port is not valid: {ex.Message}");
                } 
            }
            else
            {
                MessageBox.Show("> Please enter a value for both the Session ID and Username.\n\n> Both must only contain alphanumeric characters.\n\n> Session ID must be at least 10 characters long and the username must be at least 5 characters long");
            }
        }

        private void IpEndPointWindowManager()
        {
            ChatVM vm = new ChatVM(connector,
                                communicator,
                                ((IpEndpointVM)DataContext).ServerRadioButton,
                                ((IpEndpointVM)DataContext).SessionIdInput,
                                ((IpEndpointVM)DataContext).UsernameInput);
            chatView = new ChatView(vm);
            chatView.Closing += Window_Closing;
            Hide();
            chatView.Show();
        }

        private string CheckIpAddress()
        {
            IPAddress tempIp;

            if (((IpEndpointVM)DataContext).ClientRadioButton)
            {
                if (IPAddress.TryParse(((IpEndpointVM)DataContext).IpAddressInput, out tempIp))
                {
                    return tempIp.ToString();
                }
                else
                {
                    throw new ArgumentOutOfRangeException("IP Address is not valid");
                } 
            }
            else
            {
                if (IPAddress.TryParse(((IpEndpointVM)DataContext).IpComboBoxSelectedItem, out tempIp))
                {
                    return tempIp.ToString();
                }
                else
                {
                    throw new ArgumentOutOfRangeException("IP Address is not valid");
                }
            }
        }

        private int CheckPortNumber()
        {
            int tempPort;

            if (int.TryParse(((IpEndpointVM)DataContext).PortInput, out tempPort))
            {
                if (tempPort >= 1 && tempPort <= 65535)
                {
                    return tempPort;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Input was not between 1 - 65535");
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("Input was not an integer");
            }
        }

        private bool IsSessionIdValid()
        {
            var output = false;
            var sessionId = ((IpEndpointVM)DataContext).SessionIdInput;

            if (!string.IsNullOrEmpty(sessionId) && sessionId.Length >= 10 && IsInputAlphaNumeric(sessionId))
            {
                output = true;
            }

            return output;
        }

        private bool IsUsernameValid()
        {
            var output = false;
            var username = ((IpEndpointVM)DataContext).UsernameInput;

            if (!string.IsNullOrEmpty(username) && username.Length >= 5 && IsInputAlphaNumeric(username))
            {
                output = true;
            }

            return output;
        }

        private bool IsInputAlphaNumeric(string input)
        {
            var output = false;

            if (Regex.IsMatch(input, "[^0 - 9a - zA - Z]"))
            {
                output = true;
            }

            return output;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            BeginSessionButton_Click(null, null);
        }
    }
}
