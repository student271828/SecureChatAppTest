using SecureChatAppTest.Components.Network;
using SecureChatAppTest.Network.Network.NetworkDebug;
using SecureChatAppTestUI.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
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
    /// Interaction logic for DebugChatView.xaml
    /// </summary>
    public partial class DebugView : Window
    {
        public DebugView()
        {
            InitializeComponent();
        }

        private void BeginSessionButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();

            var debugConnector1 = new DebugConnector();
            var debugConnector2 = new DebugConnector();

            debugConnector1.OtherConnector = debugConnector2;
            debugConnector2.OtherConnector = debugConnector1;

            debugConnector1.Start();
            debugConnector2.Start();

            var debugCommunicator1 = new DebugCommunicator();
            var debugCommunicator2 = new DebugCommunicator();

            debugCommunicator1.OtherCommunicator = debugCommunicator2;
            debugCommunicator2.OtherCommunicator = debugCommunicator1;

            var vm1 = new ChatVM(debugConnector1, debugCommunicator1, true, "12345", "Alice");
            ChatView chatView1 = new ChatView(vm1);
            chatView1.Show();

            var vm2 = new ChatVM(debugConnector2, debugCommunicator2, false, "12345", "Bob");
            ChatView chatView2 = new ChatView(vm2);
            chatView2.Show();
        }
    }
}
