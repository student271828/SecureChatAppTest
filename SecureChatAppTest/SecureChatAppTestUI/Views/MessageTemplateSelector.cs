using SecureChatAppTestUI.Models;
using SecureChatAppTestUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SecureChatAppTestUI.Views
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is ChatMessage)
            {
                if(item is IncommingMessage)
                {
                    return element.FindResource("IncommingMessageTemplate") as DataTemplate;
                }
                else if (item is OutgoingMessage)
                {
                    return element.FindResource("OutgoingMessageTemplate") as DataTemplate;
                }
            }

            return null;
        }
    }
}
