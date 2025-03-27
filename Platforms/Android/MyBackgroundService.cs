using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;
using Newtonsoft.Json;
using AndroidX.Core.App;
using Android.Content.PM;

namespace MyMauiApp.Platforms.Android
{
    [Service(Exported = true, ForegroundServiceType = ForegroundService.TypeLocation)]
    public class MyBackgroundService : Service
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        public override IBinder? OnBind(Intent? intent) => null;

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("MyBackgroundService", "Service started.");

            // Start foreground notification
            StartForeground(1, new NotificationCompat.Builder(this, "location_service")
                .SetContentTitle("Tracking Location")
                .SetContentText("Running in background")
                .SetSmallIcon(Resource.Drawable.my_custom_icon)
                .SetOngoing(true)
                .Build());

            // Start location updates
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));

                        if (location != null)
                        {
                            Log.Debug("MyBackgroundService", $"Location: {location.Latitude}, {location.Longitude}");
                            await SendLocationToApi(location.Latitude, location.Longitude);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("MyBackgroundService", $"Error: {ex.Message}");
                    }

                    await Task.Delay(10000); // Wait 10 sec
                }
            });

            return StartCommandResult.Sticky;
        }

        private async Task SendLocationToApi(double lat, double lon)
        {
            try
            {
                string apiUrl = "https://your-api-url.com/api/location";
                var json = JsonConvert.SerializeObject(new { Latitude = lat, Longitude = lon, Timestamp = DateTime.UtcNow });
                var response = await _httpClient.PostAsync(apiUrl, new StringContent(json, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                    Log.Debug("MyBackgroundService", "Location sent!");
                else
                    Log.Error("MyBackgroundService", $"API error: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                Log.Error("MyBackgroundService", $"API send error: {ex.Message}");
            }
        }

        public override void OnDestroy()
        {
            Log.Debug("MyBackgroundService", "Service stopped.");
            base.OnDestroy();
        }
    }
}
