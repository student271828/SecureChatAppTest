using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SecureChatAppTestUI.ViewModels
{
    class IpEndpointVM : INotifyPropertyChanged
    {
        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;
        private readonly Dispatcher _dispatcher;

        private string _ipAddressInput;
        public string IpAddressInput
        {
            get { return _ipAddressInput; }
            set
            {
                if (_ipAddressInput == value)
                {
                    return;
                }
                _ipAddressInput = value;
                RaisePropertyChanged(nameof(IpAddressInput));
            }
        }

        private string _isIpAddressTextBoxVisible;
        public string IsIpAddressTextBoxVisible
        {
            get { return _isIpAddressTextBoxVisible; }
            set
            {
                if (_isIpAddressTextBoxVisible == value)
                {
                    return;
                }
                _isIpAddressTextBoxVisible = value;
                RaisePropertyChanged(nameof(IsIpAddressTextBoxVisible));
            }
        }

        private string _isIpAddressComboBoxVisible;
        public string IsIpAddressComboBoxVisible
        {
            get { return _isIpAddressComboBoxVisible; }
            set
            {
                if (_isIpAddressComboBoxVisible == value)
                {
                    return;
                }
                _isIpAddressComboBoxVisible = value;
                RaisePropertyChanged(nameof(IsIpAddressComboBoxVisible));
            }
        }

        private ObservableCollection<string> _ipAddressComboBox;
        public ObservableCollection<string> IpAddressComboBox
        {
            get { return _ipAddressComboBox; }
            set
            {
                if (_ipAddressComboBox == value)
                {
                    return;
                }
                _ipAddressComboBox = value;
                RaisePropertyChanged(nameof(IpAddressComboBox));
            }
        }

        private string _ipAddressComboBoxSelectedValue;
        public string IpComboBoxSelectedItem
        {
            get { return _ipAddressComboBoxSelectedValue; }
            set
            {
                if (_ipAddressComboBoxSelectedValue == value)
                {
                    return;
                }
                _ipAddressComboBoxSelectedValue = value;
                RaisePropertyChanged(nameof(IpComboBoxSelectedItem));
            }
        }

        private string _portInput;
        public string PortInput
        {
            get { return _portInput; }
            set
            {
                if (_portInput == value)
                {
                    return;
                }
                _portInput = value;
                RaisePropertyChanged(nameof(PortInput));
            }
        }

        private bool _serverRadioButton;
        public bool ServerRadioButton
        {
            get { return _serverRadioButton; }
            set
            {
                if (_serverRadioButton == value)
                {
                    return;
                }
                _serverRadioButton = value;
                RaisePropertyChanged(nameof(ServerRadioButton));
                DetermineIpInputBox(value);
            }
        }

        private bool _clientRadioButton;
        public bool ClientRadioButton
        {
            get { return _clientRadioButton; }
            set
            {
                if (_clientRadioButton == value)
                {
                    return;
                }
                _clientRadioButton = value;
                RaisePropertyChanged(nameof(ClientRadioButton));
            }
        }

        private string _sessionIdInput;
        public string SessionIdInput
        {
            get { return _sessionIdInput; }
            set
            {
                if (_sessionIdInput == value)
                {
                    return;
                }
                _sessionIdInput = value;
                RaisePropertyChanged(nameof(SessionIdInput));
            }
        }

        private string _usernameInput;
        public string UsernameInput
        {
            get => _usernameInput;
            set
            {
                if (_usernameInput == value)
                    return;
                _usernameInput = value;
                RaisePropertyChanged(nameof(UsernameInput));
            }
        }

        //private string _sessionInvalidText;
        //public string SessionInvalidText
        //{
        //    get => _sessionInvalidText;
        //    set
        //    {
        //        if (_sessionInvalidText == value)
        //            return;
        //        _sessionInvalidText = value;
        //        RaisePropertyChanged(nameof(SessionInvalidText));
        //    }
        //}

        #endregion

        public IpEndpointVM()
        {
            _dispatcher = Application.Current.Dispatcher;
            //SessionInvalidText = "";
            ServerRadioButton = true;
            ClientRadioButton = false;
            IpAddressInput = "";
            IpComboBoxSelectedItem = "";
            PortInput = "";
            SessionIdInput = "";
            UsernameInput = "";

            IsIpAddressComboBoxVisible = "Visible";
            IsIpAddressTextBoxVisible = "Hidden";
            IpAddressComboBox = GetNetworkInterfaces();

        }

        #region Methods

        private void DetermineIpInputBox(bool server)
        {
            if (server)
            {
                IsIpAddressComboBoxVisible = "Visible";
                IsIpAddressTextBoxVisible = "Hidden";
            }
            else
            {
                IsIpAddressComboBoxVisible = "Hidden";
                IsIpAddressTextBoxVisible = "Visible";
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            if (!_dispatcher.CheckAccess())
            {
                _dispatcher.BeginInvoke(new Action(() => RaisePropertyChanged(propertyName)));
                return;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<string> GetNetworkInterfaces()
        {
            List<string> adapterStringList = new List<string>();

            var localIps = new List<IPAddress>();
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                var ips =
                    nic.GetIPProperties().UnicastAddresses
                        .Select(uni => uni.Address)
                        .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToList();

                localIps.AddRange(ips);
            }

            foreach (var ip in localIps)
            {
                adapterStringList.Add(ip.ToString());
            }

            ObservableCollection<string> output = new ObservableCollection<string>(adapterStringList);
            return output;
        }

        #endregion
    }
}
