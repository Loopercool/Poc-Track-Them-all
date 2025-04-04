﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace MyMauiApp.Platforms.Android
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation |
                               ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Start the background service
            var serviceIntent = new Intent(this, typeof(MyBackgroundService));
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                StartForegroundService(serviceIntent);
            else
                StartService(serviceIntent);
        }
    }
}