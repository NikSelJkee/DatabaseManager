using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.IO;

namespace DatabaseManager
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            ConnectionStringBox.Text = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        private void CloseSettings_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void AcceptSettings_Click(object sender, RoutedEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStringsSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
            connectionStringsSection.ConnectionStrings["DefaultConnection"].ConnectionString = ConnectionStringBox.Text;
            config.Save();
            ConfigurationManager.RefreshSection("connectionStrings");

            this.DialogResult = true;
        }
    }
}
