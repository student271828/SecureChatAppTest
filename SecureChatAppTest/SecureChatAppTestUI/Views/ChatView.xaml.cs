using SecureChatAppTestUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
    /// Interaction logic for ChatView.xaml
    /// </summary>
    public partial class ChatView : Window, IDisposable
    {
        public ChatView() : this(new ChatVM(null, null, false, null, null))
        {
        }

        public ChatView(ChatVM vm)
        {
            DataContext = vm;
            vm.PropertyChanged += Vm_PropertyChanged;
            InitializeComponent();
            ((INotifyCollectionChanged)ChatItemsControl.Items).CollectionChanged += ChatView_CollectionChanged;

            vm.IsClosingChanged += Vm_IsClosingChanged;
            if (vm.IsClosing)
            {
                Dispose();
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var vm = (ChatVM)sender;
            if (e.PropertyName == nameof(vm.OtherUsername)){
                Title = $"Chatting With {vm.OtherUsername}";
            }
        }

        private void Vm_IsClosingChanged(object sender, EventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(() => Vm_IsClosingChanged(sender, e)));
                return;
            }

            ChatVM vm = (ChatVM)DataContext;
            if (vm.IsClosing)
            {
                Dispose();
            }
        }

        private void ChatView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ChatScrollViewer.ScrollToEnd();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var tb = (TextBox)sender;
            var vm = (ChatVM)DataContext;
            var isShift = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;
            if (e.Key == Key.Enter && !isShift)
            {
                vm?.SendButtonCommand?.Execute(null);
            }
            if(Keyboard.IsKeyDown(Key.Enter) && isShift)
            {
                if(vm?.ChatEntryTextBox != null)
                {
                    vm.ChatEntryTextBox += Environment.NewLine;
                    tb.CaretIndex = tb.Text.Length;
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.ScrollToEnd();
        }

        private void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var textbox = (TextBox)sender;
            textbox.Focus();
        }

        public void Dispose()
        {
            var vm = (ChatVM)DataContext;
            vm.IsClosingChanged -= Vm_IsClosingChanged;
            vm.Dispose();
            Close();
        }
    }
}
