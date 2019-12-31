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
        string sqlExpression;

        SqlDataAdapter adapter;
        SqlCommandBuilder builder;
        DataSet ds;

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

                    sqlExpression = "SELECT * FROM sys.tables WHERE type_desc='USER_TABLE'";

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

                sqlExpression = string.Format("SELECT * FROM {0}", ListOfTables.SelectedItem.ToString());

                adapter = new SqlDataAdapter(sqlExpression, connection);
                ds = new DataSet();
                adapter.Fill(ds);
                Table.ItemsSource = ds.Tables[0].AsDataView();
                Table.Columns[0].IsReadOnly = true;
            }
        }

        private void Table_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    sqlExpression = string.Format("SELECT * FROM {0}", ListOfTables.SelectedItem.ToString());

                    adapter = new SqlDataAdapter(sqlExpression, connection);
                    builder = new SqlCommandBuilder(adapter);

                    adapter.UpdateCommand = builder.GetUpdateCommand();
                    adapter.Update(ds);
                }
            }
            catch
            {
                MessageBox.Show("Убедитесь, что в вашей таблице определен первичный ключ");
            }
        }
    }
}
