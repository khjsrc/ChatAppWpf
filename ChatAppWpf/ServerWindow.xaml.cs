using System;
using System.Collections.Generic;
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

        private async void ButtonConnectToServer_OnClick(object sender, RoutedEventArgs e)
        {
            ClientEngine client = ClientEngine.Client;
            await client.Connect(new IPAddress(new byte[] {127, 0, 0, 1}), 5813);
        }

        private async void StartServerButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Starting the server side...");
            ServerEngine engine = new ServerEngine(5813);
            engine.OnClientConnected += (o, args) => { MessageBox.Show("Somebody has just connected to the server!"); };
            engine.OnMessageReceived += (o, args) => { MessageBox.Show($"{args.Author.UserName} said: {args.Content}"); };
            await engine.StartAsync();
        }

        private async void SendMessageButton_OnClick(object sender, RoutedEventArgs e)
        {
            ClientEngine client = ClientEngine.Client;
            await client.SendMessageAsync(new Message("testing string", "Kappa"));
        }
    }
}
