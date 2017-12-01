using System;
using System.Collections.Generic;
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

namespace ChatAppWpf
{
    /// <summary>
    /// Логика взаимодействия для StartupWindow.xaml
    /// </summary>
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
    }
}
