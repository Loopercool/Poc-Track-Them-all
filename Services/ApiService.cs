using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MyMauiApp.Services
{
    public static class ApiService
    {
        // You can reuse a static HttpClient instance.
        private static readonly HttpClient client = new HttpClient();

        public static async Task SendLocationAsync(double latitude, double longitude)
        {
            // Create an anonymous object (or a proper model class) to represent your data.
            var locationData = new
            {
                Latitude = latitude,
                Longitude = longitude,
                Timestamp = DateTime.UtcNow
            };

            // Replace with your actual API endpoint.
            string apiUrl = "https://yourapi.com/api/locations";

            // POST the JSON data to your API.
            var response = await client.PostAsJsonAsync(apiUrl, locationData);

            // Throw an exception if the call was not successful.
            response.EnsureSuccessStatusCode();
        }
    }
}
