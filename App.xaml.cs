using Microsoft.Data.Sqlite;
using System;
using System.IO;
using System.Windows;

namespace CarManagement
{
    public partial class App : Application
    {
        public static string ConnectionString = "Data Source=cars.db";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                string createPartsTable = @"
        CREATE TABLE IF NOT EXISTS Parts (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            CarId INTEGER,
            SKU TEXT,
            Name TEXT,
            Price REAL,
            Brand TEXT,
            Url TEXT,
            FOREIGN KEY(CarId) REFERENCES Cars(Id)
        );";

                using (var cmd = new SqliteCommand(createPartsTable, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }


        private void InitializeDatabase()
        {
            string dbFile = "cars.db";

            if (!File.Exists(dbFile))
            {
                File.WriteAllBytes(dbFile, Array.Empty<byte>());
            }

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();

                string createTable = @"
                CREATE TABLE IF NOT EXISTS Cars (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    NumberPlate TEXT NOT NULL UNIQUE,
                    Brand TEXT NOT NULL,
                    Model TEXT NOT NULL,
                    Year INTEGER NOT NULL,
                    EngineType TEXT NOT NULL,
                    EngineVolume REAL NOT NULL,
                    Kilowatts INTEGER NOT NULL
                );";

                using (var cmd = new SqliteCommand(createTable, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                string check = "SELECT COUNT(*) FROM Cars";
                using (var cmd = new SqliteCommand(check, connection))
                {
                    long count = (long)cmd.ExecuteScalar();

                    if (count == 0)
                    {
                        string insert = @"
                        INSERT INTO Cars (NumberPlate, Brand, Model, Year, EngineType, EngineVolume, Kilowatts) VALUES
                        ('LV-1234', 'BMW', '530d', 2018, 'Diesel', 3.0, 195),
                        ('JT-9876', 'Audi', 'A6', 2016, 'Petrol', 2.0, 140),
                        ('MR-5522', 'Volkswagen', 'Golf', 2014, 'Diesel', 1.6, 77),
                        ('TR-4411', 'Toyota', 'Corolla', 2020, 'Hybrid', 1.8, 90);";

                        using (var insertCmd = new SqliteCommand(insert, connection))
                        {
                            insertCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}
