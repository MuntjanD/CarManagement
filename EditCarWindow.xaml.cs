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
using Microsoft.Data.Sqlite;


namespace CarManagement
{
    public partial class EditCarWindow : Window
    {
        private CarViewModel _car;

        public EditCarWindow(CarViewModel car)
        {
            InitializeComponent();
            _car = car;

            // Заполняем поля
            NumberPlateBox.Text = car.NumberPlate;
            BrandBox.Text = car.Brand;
            ModelBox.Text = car.Model;
            YearBox.Text = car.Year.ToString();
            EngineTypeBox.Text = car.EngineType;
            EngineVolumeBox.Text = car.EngineVolume.ToString();
            KilowattsBox.Text = car.Kilowatts.ToString();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            using (var connection = new SqliteConnection(App.ConnectionString))
            {
                connection.Open();

                string query = @"
                UPDATE Cars
                SET NumberPlate=@NumberPlate,
                    Brand=@Brand,
                    Model=@Model,
                    Year=@Year,
                    EngineType=@EngineType,
                    EngineVolume=@EngineVolume,
                    Kilowatts=@Kilowatts
                WHERE Id=@Id";

                using (var cmd = new SqliteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", _car.Id);
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

            MessageBox.Show("Car updated successfully");
            this.DialogResult = true;
            this.Close();
        }
    }
}
