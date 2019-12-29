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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Data;

namespace DatabaseManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        List<string> tables;

        public MainWindow()
        {
            InitializeComponent();
            tables = new List<string>();

            ConnectionToDatabase();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти?", "Выход", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                this.Close();
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();

            if (settingsWindow.ShowDialog() == true)
            {
                ConnectionToDatabase();
            }
        }

        private void ConnectionToDatabase()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlExpression = "SELECT * FROM sys.tables WHERE type_desc='USER_TABLE'";

                    SqlCommand command = new SqlCommand(sqlExpression, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    tables.Clear();

                    while (reader.Read())
                    {
                        tables.Add(reader.GetString(0));
                    }

                    ListOfTables.ItemsSource = tables;
                }
            }
            catch
            {
                MessageBox.Show("Проверьте правильность ввода строки подкючения");
            }
        }

        private void ListOfTables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sqlExpression = string.Format("SELECT * FROM {0}", ListOfTables.SelectedItem.ToString());

                SqlDataAdapter adapter = new SqlDataAdapter(sqlExpression, connection);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                Table.ItemsSource = ds.Tables[0].AsDataView();
                Table.Columns[0].IsReadOnly = true;
            }
        }
    }
}
