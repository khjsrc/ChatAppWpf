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
    /// Логика взаимодействия для ServerWindow.xaml
    /// </summary>
    public partial class ServerWindow : Window
    {
        public ServerWindow()
        {
            InitializeComponent();
        }

        private void ButtonExit_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region shit code for tests
        private async void ButtonConnectToServer_OnClick(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "Client";

            ClientEngine client = ClientEngine.Client;
            client.OnMessageReceived += (o, args) => { MessageBox.Show($"{args.Author.UserName} sent a message: {args.Content}"); };

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

        private async void StartServerButton_OnClick(object sender, RoutedEventArgs e)
        {
            StatusTextBlock.Text = "Server";

            ServerEngine server = ServerEngine.Server;
            server.OnClientConnected += async (o, args) =>
            {
                await server.GreetNewcomerAsync();
                MessageBox.Show("Somebody has just connected to the server!");
            };
            server.OnMessageReceived += (o, args) => { MessageBox.Show($"{args.Author.UserName} said: {args.Content}"); };
            await server.StartAsync(Convert.ToInt32(ConfigurationManager.AppSettings.Get("ServerPort")));
        }

        private async void SendMessageButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClientEngine client = ClientEngine.Client;
            await client.SendMessageAsync(new Message("testing string", "Kappa"));
        }

        private async void SendBroadcastButton_OnClick(object sender, RoutedEventArgs e)
        {
            ServerEngine server = ServerEngine.Server;
            await server.BroadcastAsync(new Message("this is broadcast message", "ServerName"));
        }
        #endregion
    }
}
