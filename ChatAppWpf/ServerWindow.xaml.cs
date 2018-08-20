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

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void ButtonConnectToServer_OnClick(object sender, RoutedEventArgs e)
        {
            ClientEngine client = new ClientEngine();
            await client.Connect(new IPAddress(0x7f000001));

        }

        private async void StartServerButton_OnClick(object sender, RoutedEventArgs e)
        {
            ServerEngine engine = new ServerEngine();
            engine.OnClientConnected += (o, args) => { MessageBox.Show("Somebody has just connected to the server!"); };
            await engine.StartAsync();
        }
    }
}
