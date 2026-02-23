using GooseShop.Models;
using GooseShop.Data;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;

namespace GooseShop.Services
{
    public class NovaPoshtaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public NovaPoshtaService(HttpClient httpClient, IConfiguration config, IDbContextFactory<AppDbContext> dbFactory)
        {
            _httpClient = httpClient;
            _dbFactory = dbFactory;
            _apiKey = config["NovaPoshtaApiKey"] ?? "bbc234cd14ef22bdc1d89f39be858103";
        }

        // Пошук міст залишаємо через API (вони рідко міняються і їх мало)
        public async Task<List<NPCity>> GetCities(string search)
        {
            var payload = new
            {
                apiKey = _apiKey,
                modelName = "Address",
                calledMethod = "getCities",
                methodProperties = new { FindByString = search, Limit = "20" }
            };

            var response = await _httpClient.PostAsJsonAsync("https://api.novaposhta.ua/v2.0/json/", payload);
            var result = await response.Content.ReadFromJsonAsync<NPResponse<NPCity>>();
            return result?.Data ?? new List<NPCity>();
        }

        // МЕТОД ДЛЯ ОНОВЛЕННЯ КЕШУ (Запускаємо при виборі міста)
        public async Task RefreshWarehouseCache(string cityRef)
        {
            using var context = _dbFactory.CreateDbContext();

            // 1. Отримуємо всі відділення конкретного міста з API
            var payload = new
            {
                apiKey = _apiKey,
                modelName = "Address",
                calledMethod = "getWarehouses",
                methodProperties = new { CityRef = cityRef }
            };

            var response = await _httpClient.PostAsJsonAsync("https://api.novaposhta.ua/v2.0/json/", payload);
            var result = await response.Content.ReadFromJsonAsync<NPResponse<NPWarehouse>>();

            if (result?.Data != null && result.Success)
            {
                // 2. Видаляємо старий кеш для цього міста
                var oldEntries = context.CachedWarehouses.Where(w => w.CityRef == cityRef);
                context.CachedWarehouses.RemoveRange(oldEntries);

                // 3. Додаємо нові з обробкою номера
                foreach (var wh in result.Data)
                {
                    // Витягуємо тільки цифри для поля Number (Відділення №22 -> 22)
                    var onlyDigits = new string(wh.Description.Where(char.IsDigit).ToArray());
                    int.TryParse(onlyDigits, out int whNumber);

                    context.CachedWarehouses.Add(new CachedWarehouse
                    {
                        CityRef = cityRef,
                        Ref = wh.Ref,
                        Description = wh.Description,
                        Number = whNumber,
                        // Визначаємо тип: якщо в описі є "Поштомат" - маркуємо як Postomat
                        Type = wh.Description.Contains("Поштомат", StringComparison.OrdinalIgnoreCase)
                               ? "Postomat" : "Warehouse"
                    });
                }

                await context.SaveChangesAsync();
            }
        }
        public async Task<List<NPWarehouse>> GetAllWarehouses()
        {
            var request = new
            {
                apiKey = _apiKey,
                modelName = "Address",
                calledMethod = "getWarehouses",
                methodProperties = new { } // Порожні властивості = вся Україна
            };

            var response = await _httpClient.PostAsJsonAsync("https://api.novaposhta.ua/v2.0/json/", request);
            var result = await response.Content.ReadFromJsonAsync<NPResponse<NPWarehouse>>();

            return result?.Data ?? new List<NPWarehouse>();
        }
    }
}