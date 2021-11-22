using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureChatAppTestUI.ViewModels
{
    public class DebugVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

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

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
