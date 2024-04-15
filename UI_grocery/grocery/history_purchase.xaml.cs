using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace GroceriesApp
{
    public partial class HistoryPurchase : Page
    {
        private MySqlConnection connection;
        private string connectionString = "Server=mariadb.vamk.fi;Database=e2101098_Windows;Uid=e2101098;Pwd=cqgYeaFEN6A;";

        public HistoryPurchase()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Connect to the database
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                LoadMonths();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void LoadMonths()
        {
            try
            {
                // Fetch distinct months from the database
                string query = "SELECT DISTINCT MONTH(date_column) AS month FROM your_table_name;";
                MySqlCommand command = new MySqlCommand(query, connection);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();

                List<int> months = new List<int>();

                while (reader.Read())
                {
                    months.Add(Convert.ToInt32(reader["month"]));
                }

                // Set the text of the TextBox to display the available months
                MonthTextBox.Text = string.Join(", ", months);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }


        private void MonthTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = MonthTextBox.Text;
            if (!string.IsNullOrEmpty(input))
            {
                if (int.TryParse(input, out int selectedMonth))
                {
                    try
                    {
                        // Fetch invoices for the selected month
                        string query = $"SELECT * FROM your_table_name WHERE MONTH(date_column) = {selectedMonth};";
                        MySqlCommand command = new MySqlCommand(query, connection);
                        connection.Open();
                        MySqlDataReader reader = command.ExecuteReader();

                        // Clear previous invoices
                        InvoiceListBox.Items.Clear();

                        while (reader.Read())
                        {
                            // Add each invoice to the ListBox
                            InvoiceListBox.Items.Add(reader["invoice_details_column"]);
                        }

                        // Calculate total cost for the month
                        reader.Close();
                        query = $"SELECT SUM(cost_column) AS total_cost FROM your_table_name WHERE MONTH(date_column) = {selectedMonth};";
                        command.CommandText = query;
                        reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            decimal totalCost = Convert.ToDecimal(reader["total_cost"]);
                            TotalCostTextBlock.Text = $"Total Cost for Month {selectedMonth}: {totalCost.ToString("C")}";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                    finally
                    {
                        if (connection.State == ConnectionState.Open)
                        {
                            connection.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid month (1-12).");
                }
            }
        }


        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Add your menu button functionality here
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Add your logout button functionality here
        }

        private void MonthComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
