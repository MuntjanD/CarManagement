using CarManagement.Models;
using Microsoft.Web.WebView2.Core;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace CarManagement.Views
{
    public partial class BrowserWindow : Window
    {
        public event Action<Part>? PartImported;
        public event Action<Part>? AddToListRequested;

        public BrowserWindow()
        {
            InitializeComponent();
            InitBrowser();
        }

        private async void InitBrowser()
        {
            await Browser.EnsureCoreWebView2Async();
            Browser.Source = new Uri("https://www.trodo.lv/");
        }

        public async Task<Part> ImportFromTrodoReturnPart()
        {
            string name = await Browser.CoreWebView2.ExecuteScriptAsync(
                "document.querySelector('.product-name')?.innerText");

            string price = await Browser.CoreWebView2.ExecuteScriptAsync(
                "document.querySelector('.price')?.innerText");

            string brand = await Browser.CoreWebView2.ExecuteScriptAsync(
                "document.querySelector('.brand-name')?.innerText");

            string sku = await Browser.CoreWebView2.ExecuteScriptAsync(
                "document.querySelector('.product-code')?.innerText");

            string url = await Browser.CoreWebView2.ExecuteScriptAsync(
                "window.location.href");

            name = name.Trim('"');
            price = price.Trim('"');
            brand = brand.Trim('"');
            sku = sku.Trim('"');
            url = url.Trim('"');

            return new Part
            {
                SKU = sku,
                Name = name,
                Brand = brand,
                Price = price,
                Url = url
            };
        }

        private async void AddToList_Click(object sender, RoutedEventArgs e)
        {
            var part = await ImportFromTrodoReturnPart();
            AddToListRequested?.Invoke(part);
            this.Close();
        }

        private async void Import_Click(object sender, RoutedEventArgs e)
        {
            var part = await ImportFromTrodoReturnPart();
            PartImported?.Invoke(part);
            this.Close();
        }
        private async void ImportCart_Click(object sender, RoutedEventArgs e)
        {
            string json = await Browser.CoreWebView2.ExecuteScriptAsync(@"
        (function() {
            let items = [];
            document.querySelectorAll('.cart-item').forEach(item => {
                items.push({
                    name: item.querySelector('.cart-item-name')?.innerText,
                    brand: item.querySelector('.cart-item-brand')?.innerText,
                    price: item.querySelector('.cart-item-price')?.innerText,
                    url: item.querySelector('.cart-item-name')?.href
                });
            });
            return JSON.stringify(items);
        })();
    ");

            json = json.Trim('"').Replace("\\\"", "\"");

            var parts = System.Text.Json.JsonSerializer.Deserialize<List<Part>>(json);

            CartImported?.Invoke(parts);
            this.Close();
        }
        public event Action<List<Part>>? CartImported;


    }
}
