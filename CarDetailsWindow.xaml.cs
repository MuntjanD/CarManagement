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
    public partial class CarDetailsWindow : Window
    {
        private CarViewModel _car;

        public CarDetailsWindow(CarViewModel car)
        {
            InitializeComponent();
            _car = car;

            LoadCarInfo();
            LoadParts();
            LoadWorkHistory();
            LoadMaintenance();
        }



        private void LoadCarInfo()
        {
            PlateText.Text = _car.NumberPlate;
            BrandText.Text = _car.Brand;
            ModelText.Text = _car.Model;
        }

        private void LoadMaintenance()
        {
            // SELECT * FROM Maintenance WHERE CarId = _car.Id
        }

        private void LoadWorkHistory()
        {
            // SELECT * FROM WorkHistory WHERE CarId = _car.Id
        }

        private void LoadInvoices()
        {
            // SELECT * FROM Invoice WHERE CarId = _car.Id
        }
        private void AddMaintenance_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add Maintenance clicked");
        }

        private void AddWork_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add Work clicked");
        }

        private void GenerateInvoice_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Generate Invoice clicked");
        }
        private void LoadParts()
        {
            var list = new List<PartViewModel>();

            using (var connection = new SqliteConnection(App.ConnectionString))
            {
                connection.Open();

                string query = "SELECT SKU, Name, Price, Brand, Url FROM Parts WHERE CarId = @id";

                using (var cmd = new SqliteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", _car.Id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new PartViewModel
                            {
                                SKU = reader.GetString(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDouble(2),
                                Brand = reader.GetString(3),
                                Url = reader.GetString(4)
                            });
                        }
                    }
                }
            }

            PartsGrid.ItemsSource = list;
        }

        public class PartViewModel 
        { 
            public string SKU { get; set; } 
            public string Name { get; set; } 
            public double Price { get; set; } 
            public string Brand { get; set; } 
            public string Url { get; set; } 
        }
        private void AddPart_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddPartWindow(_car.Id);
            if (window.ShowDialog() == true)
                LoadParts();
        }



    }
}