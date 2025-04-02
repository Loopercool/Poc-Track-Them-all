using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;
using Newtonsoft.Json;
using Android.Content.PM;

namespace MyMauiApp.Platforms.Android
{
    [Service(Exported = true, ForegroundServiceType = ForegroundService.TypeLocation)]
    public class MyBackgroundService : Service
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public override IBinder? OnBind(Intent? intent) => null;

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            Log.Debug("MyBackgroundService", "Service started.");

            var notificationHelper = new NotificationHelper(this);
            StartForeground(1, notificationHelper.BuildNotification());

            Task.Run(async () =>
            {
                while (!_cts.IsCancellationRequested)
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
                        Log.Error("MyBackgroundService", $"Error fetching location: {ex.Message}");
                    }

                    await Task.Delay(10000, _cts.Token);
                }
            }, _cts.Token);

            return StartCommandResult.Sticky;
        }

        private async Task SendLocationToApi(double lat, double lon)
        {
            try
            {
                string apiUrl = "http://10.0.2.2:5008/api/locations"; // Emulator: 10.0.2.2, Physical device: your IP
                var locationRecord = new
                {
                    Latitude = lat,
                    Longitude = lon,
                    Timestamp = DateTime.UtcNow
                };
                var json = JsonConvert.SerializeObject(locationRecord);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                Log.Debug("MyBackgroundService", $"Sending to API: {json}");
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                    Log.Debug("MyBackgroundService", "Location sent successfully!");
                else
                    Log.Error("MyBackgroundService", $"API error: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }
            catch (HttpRequestException ex)
            {
                Log.Error("MyBackgroundService", $"API connection error: {ex.Message} - Inner: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                Log.Error("MyBackgroundService", $"API send error: {ex.Message}");
            }
        }

        public override void OnDestroy()
        {
            _cts.Cancel();
            Log.Debug("MyBackgroundService", "Service stopped.");
            base.OnDestroy();
        }
    }
}