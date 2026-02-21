using GooseShop.Models;
using System.Net.Http.Json;

namespace GooseShop.Services
{
    public class NovaPoshtaService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public NovaPoshtaService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            // Якщо ви хочете брати ключ з appsettings.json, використовуйте назву секції, наприклад "NovaPoshtaApiKey"
            // Або залиште константу, якщо не хочете морочитися з файлом конфігурації
            _apiKey = config["NovaPoshtaApiKey"] ?? "bbc234cd14ef22bdc1d89f39be858103";
        }

        public async Task<List<NPCity>> GetCities(string search)
        {
            var payload = new
            {
                apiKey = _apiKey,
                modelName = "Address",
                calledMethod = "getCities",
                methodProperties = new
                {
                    FindByString = search,
                    Limit = "20"
                }
            };

            // Використовуємо саме _httpClient, який отримали через конструктор
            var response = await _httpClient.PostAsJsonAsync("https://api.novaposhta.ua/v2.0/json/", payload);
            var result = await response.Content.ReadFromJsonAsync<NPResponse<NPCity>>();

            return result?.Data ?? new List<NPCity>();
        }

        public async Task<List<NPWarehouse>> GetAllWarehouses()
        {
            var request = new
            {
                apiKey = _apiKey,
                modelName = "Address",
                calledMethod = "getWarehouses",
                methodProperties = new { } // Порожній об'єкт завантажить всі відділення України
            };

            var response = await _httpClient.PostAsJsonAsync("https://api.novaposhta.ua/v2.0/json/", request);
            var result = await response.Content.ReadFromJsonAsync<NPResponse<NPWarehouse>>();

            return result?.Data ?? new List<NPWarehouse>();
        }
    }
}