using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;

namespace MyMauiApp.Platforms.Android
{
    public class NotificationHelper
    {
        private const string ChannelId = "location_channel";
        private readonly Context _context;

        public NotificationHelper(Context context)
        {
            _context = context;
        }

        public Notification BuildNotification()
        {
            // Create notification channel on Android Oreo and above.
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(ChannelId, "Location Tracking", NotificationImportance.Default)
                {
                    Description = "Tracking location in background"
                };
                var notificationManager = (NotificationManager)_context.GetSystemService(Context.NotificationService);
                notificationManager?.CreateNotificationChannel(channel);
            }

            // Build the notification using your custom icon.
            // Use the fully qualified reference: global::Resource.Drawable.my_custom_icon
            var builder = new NotificationCompat.Builder(_context, ChannelId)
                .SetContentTitle("Location Tracking Active")
                .SetContentText("Your location is being tracked in the background.")
                .SetSmallIcon(Resource.Drawable.my_custom_icon)
                .SetOngoing(true);

            return builder.Build();
        }
    }
}
