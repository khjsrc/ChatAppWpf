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
using ChatApp;
using System.Net;
using System.Configuration;

namespace ChatAppWpf
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class ClientWindow : Window
    {
        private string username;
        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                if (string.IsNullOrEmpty(username))
                {
                    username = value;
                }
            }
        }
        public ClientWindow()
        {
            InitializeComponent();
            ClientEngine client = ClientEngine.Client;
            client.OnMessageReceived += (o, args) => {
                Grid grid = ChatLog.Content as Grid;
                TextBlock msg = new TextBlock
                {
                    Text = $"({args.TimeOfCreation:hh:MM:ss}){args.Author.UserName}: {args.Content}",
                    Style = Application.Current.Resources["ChatTextBlockStyle"] as Style
                };

                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                Grid.SetRow(msg, grid.RowDefinitions.Count - 1);
                grid.Children.Add(msg);

                ChatLog.ScrollToEnd();
            };

            IPAddress ip = IPAddress.None;
            string address = ConfigurationManager.AppSettings.Get("ServerAddress");
            if (!IPAddress.TryParse(address, out ip))
            {
                MessageBox.Show(
                    "You should specify IP address of the server in App.congif file before launching the client.");
            }
            else
            {
                client.Connect(ip, Convert.ToInt32(ConfigurationManager.AppSettings.Get("ServerPort")));
            }
        }

        private void ButtonSendMessage_OnClick(object sender, RoutedEventArgs e) //will remove that button soon-ish
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Style = Resources["ChatTextBlockStyle"] as Style;
            textBlock.Text = "testing dynamic grid with chat messages";
            ChatGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            //ChatGrid.RowDefinitions.Count;
            Grid.SetRow(textBlock, ChatGrid.RowDefinitions.Count - 1);
            ChatGrid.Children.Add(textBlock);
            ChatLog.ScrollToEnd();
        }

        private void TextBoxMessage_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBoxMessage.Text == "Input your message here...")
            {
                TextBoxMessage.Clear();
            }
        }

        private async void TextBoxMessage_KeyDown(object sender, KeyEventArgs e)
        {
            ClientEngine client = ClientEngine.Client;

            if (e.Key == Key.Enter && TextBoxMessage.Text.Length > 0)
            {
                string message = string.Empty;
                if (TextBoxMessage.Text.Trim(' ').Length == 0)
                {
                    TextBoxMessage.Clear();
                }
                else
                {
                    message = TextBoxMessage.Text;
                    ChatLog.ScrollToBottom();
                    TextBoxMessage.Clear();
                }
                await client.SendMessageAsync(new Message(message, Username));
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
