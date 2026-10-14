using CarManagement;
using Microsoft.Data.Sqlite;
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

namespace CarManagement
{
    public partial class AddCarWindow : Window
    {
        public AddCarWindow()
        {
            InitializeComponent();
        }

        private void AddCar_Click(object sender, RoutedEventArgs e)
        {
            using (var connection = new SqliteConnection(App.ConnectionString))
            {
                connection.Open();

                string query = @"
                INSERT INTO Cars (NumberPlate, Brand, Model, Year, EngineType, EngineVolume, Kilowatts)
                VALUES (@NumberPlate, @Brand, @Model, @Year, @EngineType, @EngineVolume, @Kilowatts)";

                using (var cmd = new SqliteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@NumberPlate", NumberPlateBox.Text);
                    cmd.Parameters.AddWithValue("@Brand", BrandBox.Text);
                    cmd.Parameters.AddWithValue("@Model", ModelBox.Text);
                    cmd.Parameters.AddWithValue("@Year", int.Parse(YearBox.Text));
                    cmd.Parameters.AddWithValue("@EngineType", EngineTypeBox.Text);
                    cmd.Parameters.AddWithValue("@EngineVolume", double.Parse(EngineVolumeBox.Text));
                    cmd.Parameters.AddWithValue("@Kilowatts", int.Parse(KilowattsBox.Text));

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Car added successfully");
            this.DialogResult = true;
            this.Close();
        }
    }
}
