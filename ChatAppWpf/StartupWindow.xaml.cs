using System;
using System.Collections.Generic;
using System.Configuration;
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
using ChatApp;

namespace ChatAppWpf
{
    /// <summary>
    /// Логика взаимодействия для StartupWindow.xaml
    /// </summary>
    /// <inheritdoc cref="Window"/>
    public partial class StartupWindow : Window
    {
        public StartupWindow()
        {
            InitializeComponent();
        }

        private void ButtonServer_Click(object sender, RoutedEventArgs e)
        {
            ServerWindow win = new ServerWindow();
            win.Left = this.Left;
            win.Top = this.Top;
            win.Show();
            this.Close();
        }

        private void ButtonClient_Click(object sender, RoutedEventArgs e)
        {
            ClientWindow win = new ClientWindow();
            win.Left = this.Left;
            win.Top = this.Top;
            win.Show();
            this.Close();
        }

        private async void ButtonLogIn_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginTextBox.Text;
            var password = UserPassword.Password.GetHashCode();
            ConfigurationManager.AppSettings.Get("ServerAddress");
            ConfigurationManager.AppSettings.Get("ServerPort");

            /*Send a packet with authentication info to the server (username, password's hash code or any other secure string)
             *and check if the info matches with the one in the server's database.
             */

            //send the login-password pair to the server? then receive the answer and if the password matches the login, open the client window

            ClientWindow win = new ClientWindow();
            win.Left = this.Left;
            win.Top = this.Top;
            win.Show();
            this.Close();

            ClientEngine client = ClientEngine.Client;
            client.OnMessageReceived += (o, args) => {
                TextBlock msg = new TextBlock();
                msg.Text = $"{args.Author.UserName}: {args.Content}";
                msg.Style = Resources["ChatTextBlockStyle"] as Style;
                (win.ChatLog.Content as Grid).Children.Add(msg);
                win.ChatLog.ScrollToEnd(); 
            };//MessageBox.Show($"{args.Author.UserName} sent a message: {args.Content}"); };

            IPAddress ip = IPAddress.None;
            string address = ConfigurationManager.AppSettings.Get("ServerAddress");
            if (!IPAddress.TryParse(address, out ip))
            {
                MessageBox.Show(
                    "You should specify IP address of the server in App.congif file before launching the client.");
            }
            else
            {
                await client.Connect(ip, Convert.ToInt32(ConfigurationManager.AppSettings.Get("ServerPort")));
            }
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            //Environment.Exit(0);
            ServerWindow win = new ServerWindow();
            win.Left = this.Left;
            win.Top = this.Top;
            win.Show();
            this.Close();
        }

        private void UserPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter) ButtonLogIn_Click(sender, e);
        }

        private void RegistrationHyperText_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Registration!", "chat", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK);
        }
    }
}
