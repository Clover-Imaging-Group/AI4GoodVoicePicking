using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace AI4Good.Droid
{
    [Activity(Label = "AI4Good Conversational AI", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true,  ScreenOrientation = ScreenOrientation.Landscape)]
    public class SplashActivity : AppCompatActivity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            Log.Debug(TAG, "SplashActivity.OnCreate");

            // Create your application here
        }

        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(() => {
                Task.Delay(1000);
            });
            startupWork.ContinueWith(T =>
            {
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            }, TaskScheduler.FromCurrentSynchronizationContext());
            startupWork.Start();
        }
    }
}