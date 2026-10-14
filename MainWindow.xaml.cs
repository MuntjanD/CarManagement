using CarManagement.Models;
using CarManagement.Views;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CarManagement
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Part> PartsList { get; set; } = new();

        public MainWindow()
        {
            InitializeComponent();
            LoadCars();
        }

        private static readonly Dictionary<string, string[]> CarModels = new()
        {
            { "BMW", new[] { "320", "520", "X3", "X5", "M3", "M5" } },
            { "Audi", new[] { "A3", "A4", "A6", "Q5", "Q7", "S4" } },
            { "Volkswagen", new[] { "Golf", "Passat", "Tiguan", "Polo", "Arteon" } },
            { "Toyota", new[] { "Corolla", "Camry", "RAV4", "Yaris", "Auris" } },
            { "Mercedes", new[] { "C200", "E220", "GLC", "GLE", "S350" } },
            { "Honda", new[] { "Civic", "Accord", "CR-V", "Jazz" } },
            { "Volvo", new[] { "S60", "S90", "XC60", "XC90", "V60" } }
        };

        private static readonly string[] EngineTypes = { "Diesel", "Petrol", "Hybrid", "Electric" };

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            var rnd = new Random();
            int count = rnd.Next(50, 70);

            using (var connection = new SqliteConnection(App.ConnectionString))
            {
                connection.Open();

                for (int i = 0; i < count; i++)
                {
                    var brand = CarModels.Keys.ElementAt(rnd.Next(CarModels.Count));
                    var model = CarModels[brand][rnd.Next(CarModels[brand].Length)];
                    string number = $"LV-{rnd.Next(1000, 9999)}";

                    int year = rnd.Next(1998, 2024);
                    string engine = EngineTypes[rnd.Next(EngineTypes.Length)];
                    double volume = Math.Round(rnd.NextDouble() * 3.0 + 1.0, 1);
                    int kw = rnd.Next(60, 350);

                    string query = @"
                        INSERT INTO Cars (NumberPlate, Brand, Model, Year, EngineType, EngineVolume, Kilowatts)
                        VALUES (@n, @b, @m, @y, @e, @v, @k)";

                    using (var cmd = new SqliteCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@n", number);
                        cmd.Parameters.AddWithValue("@b", brand);
                        cmd.Parameters.AddWithValue("@m", model);
                        cmd.Parameters.AddWithValue("@y", year);
                        cmd.Parameters.AddWithValue("@e", engine);
                        cmd.Parameters.AddWithValue("@v", volume);
                        cmd.Parameters.AddWithValue("@k", kw);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            MessageBox.Show($"{count} random cars generated.");
            LoadCars();
        }

        private void LoadCars(string search = "")
        {
            var cars = new List<CarViewModel>();

            using (var connection = new SqliteConnection(App.ConnectionString))
            {
                connection.Open();

                string query = @"
                    SELECT * FROM Cars
                    WHERE 
                        (NumberPlate LIKE @search
                        OR Brand LIKE @search
                        OR Model LIKE @search)
                    AND
                        (@engine = 'All' OR EngineType = @engine)
                ";

                using (var cmd = new SqliteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@search", $"%{search}%");

                    string engine = EngineFilter.SelectedValue?.ToString() ?? "All";
                    cmd.Parameters.AddWithValue("@engine", engine);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cars.Add(new CarViewModel
                            {
                                Id = reader.GetInt32(0),
                                NumberPlate = reader.GetString(1),
                                Brand = reader.GetString(2),
                                Model = reader.GetString(3),
                                Year = reader.GetInt32(4),
                                EngineType = reader.GetString(5),
                                EngineVolume = reader.GetDouble(6),
                                Kilowatts = reader.GetInt32(7)
                            });
                        }
                    }
                }
            }

            CarsGrid.ItemsSource = cars;
            StatusText.Text = $"Loaded {cars.Count} cars";
        }

        private void OpenBrowser_Click(object sender, RoutedEventArgs e)
        {
            var browser = new BrowserWindow();
            browser.AddToListRequested += Browser_AddToListRequested;
            browser.Show();
        }

        private void Browser_AddToListRequested(Part part)
        {
            PartsList.Add(part);
            StatusText.Text = $"Added part: {part.Name}";
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadCars();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddCarWindow();
            if (window.ShowDialog() == true)
                LoadCars();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var car = CarsGrid.SelectedItem as CarViewModel;

            if (car == null)
            {
                MessageBox.Show("Please select a car to delete.");
                return;
            }

            var result = MessageBox.Show(
                $"Delete car {car.NumberPlate} ({car.Brand} {car.Model})?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            using (var connection = new SqliteConnection(App.ConnectionString))
            {
                connection.Open();

                string query = "DELETE FROM Cars WHERE Id=@Id";

                using (var cmd = new SqliteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", car.Id);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Car deleted successfully.");
            LoadCars();
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var car = CarsGrid.SelectedItem as CarViewModel;

            if (car == null)
            {
                MessageBox.Show("Please select a car to edit.");
                return;
            }

            var window = new EditCarWindow(car);
            if (window.ShowDialog() == true)
                LoadCars();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoadCars(SearchBox.Text);
        }

        private void EngineFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadCars(SearchBox.Text);
        }

        private void CarsGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var car = CarsGrid.SelectedItem as CarViewModel;
            if (car == null) return;

            var window = new CarDetailsWindow(car);
            window.ShowDialog();
        }
    }
}
