using CarManagement.Views;
using HtmlAgilityPack;
using Microsoft.Data.Sqlite;
using System;
using System.Net.Http;
using System.Windows;
using CarManagement.Models;

namespace CarManagement
{
    public partial class AddPartWindow : Window
    {
        private int _carId;

        public AddPartWindow(int carId)
        {
            InitializeComponent();
            _carId = carId;
        }

        private async void Fetch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sku = SkuBox.Text.Trim();
                if (string.IsNullOrEmpty(sku))
                {
                    MessageBox.Show("Enter SKU first.");
                    return;
                }

                string url = $"https://lv.e-cat.intercars.eu/ru/search?query={sku}";
                UrlBox.Text = url;

                var http = new HttpClient();
                http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");

                var html = await http.GetStringAsync(url);

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                NameBox.Text = doc.DocumentNode.SelectSingleNode("//div[@class='product-name']/a")?.InnerText.Trim() ?? "Not found";
                BrandBox.Text = doc.DocumentNode.SelectSingleNode("//div[@class='product-brand']/span")?.InnerText.Trim() ?? "Unknown";

                var priceNode = doc.DocumentNode.SelectSingleNode("//span[contains(@class,'price')]");
                PriceBox.Text = priceNode?.InnerText.Replace("€", "").Trim() ?? "0";

                var linkNode = doc.DocumentNode.SelectSingleNode("//div[@class='product-name']/a");
                if (linkNode != null)
                {
                    string productUrl = linkNode.GetAttributeValue("href", "");
                    if (!productUrl.StartsWith("http"))
                        productUrl = "https://lv.e-cat.intercars.eu" + productUrl;

                    UrlBox.Text = productUrl;
                }

                MessageBox.Show("Imported from e-cat.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            using (var connection = new SqliteConnection(App.ConnectionString))
            {
                connection.Open();

                string query = @"
                INSERT INTO Parts (CarId, SKU, Name, Price, Brand, Url)
                VALUES (@carId, @sku, @name, @price, @brand, @url)";

                using (var cmd = new SqliteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@carId", _carId);
                    cmd.Parameters.AddWithValue("@sku", SkuBox.Text);
                    cmd.Parameters.AddWithValue("@name", NameBox.Text);
                    cmd.Parameters.AddWithValue("@price", double.Parse(PriceBox.Text));
                    cmd.Parameters.AddWithValue("@brand", BrandBox.Text);
                    cmd.Parameters.AddWithValue("@url", UrlBox.Text);

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Part saved.");
            this.DialogResult = true;
            this.Close();
        }

        private void ImportFromTrodo_Click(object sender, RoutedEventArgs e)
        {
            var browser = new BrowserWindow();
            browser.PartImported += Browser_PartImported;
            browser.Show();
        }

        private void Browser_PartImported(Part part)
        {
            SkuBox.Text = part.SKU;
            NameBox.Text = part.Name;
            BrandBox.Text = part.Brand;
            PriceBox.Text = part.Price;
            UrlBox.Text = part.Url;
        }
    }
}
