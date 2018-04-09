using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatAppWpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        public ClientWindow()
        {
            InitializeComponent();
        }

        private void ButtonSendMessage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBoxMessage_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBoxMessage.Text == "Input your message here...")
            {
                TextBoxMessage.Clear();
            }
        }

        private void TextBoxMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && TextBoxMessage.Text.Length > 0)
            {
                if (TextBoxMessage.Text.Trim(' ').Length == 0)
                {
                    TextBoxMessage.Clear();
                }
                else
                {
                    ChatLog.Content += TextBoxMessage.Text + '\n';
                    ChatLog.ScrollToBottom();
                    TextBoxMessage.Clear();
                }
            }
        }

        private void TextBoxMessage_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TextBoxMessage.Text == string.Empty || TextBoxMessage.Text.Length == 0)
            {
                TextBoxMessage.Text = "Input your message here...";
            }
        }
    }
}
