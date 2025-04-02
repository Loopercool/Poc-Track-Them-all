using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;

namespace MyMauiApp.Platforms.Android
{
    public class NotificationHelper
    {
        private const string ChannelId = "location_channel"; // Consistent channel ID
        private readonly Context _context;

        public NotificationHelper(Context context)
        {
            _context = context;
        }

        public Notification BuildNotification()
        {
            // Create notification channel for Android 8.0+ (Oreo and above)
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(ChannelId, "Location Tracking", NotificationImportance.Default)
                {
                    Description = "Tracking location in background",
                    LockscreenVisibility = NotificationVisibility.Public // Ensure visibility on lock screen
                };
                var notificationManager = _context.GetSystemService(Context.NotificationService) as NotificationManager;
                notificationManager?.CreateNotificationChannel(channel);
            }

            // Build the notification with visibility and persistence
            var builder = new NotificationCompat.Builder(_context, ChannelId)
                .SetContentTitle("Location Tracking Active")
                .SetContentText("Your location is being tracked in the background.")
                .SetSmallIcon(Resource.Drawable.my_custom_icon) // Ensure this exists in Resources/drawable
                .SetOngoing(true) // Prevents swiping away, required for foreground service
                .SetPriority(NotificationCompat.PriorityDefault) // Default priority for visibility
                .SetAutoCancel(false); // Keeps it persistent

            return builder.Build();
        }
    }
}