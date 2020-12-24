using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Views;
using Android;
using Plugin.Fingerprint;

namespace Wallet2.Droid
{
	[Activity(
			MainLauncher = true,
			ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
			WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
		)]
	public class MainActivity : Windows.UI.Xaml.ApplicationActivity
	{
		readonly string[] PermissionsReq =
		{
			Manifest.Permission.UseFingerprint,
			Manifest.Permission.UseBiometric
        };

		protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

			CrossFingerprint.SetCurrentActivityResolver(() => this);

			Xamarin.Essentials.Platform.Init(Application);
			ZXing.Net.Mobile.Forms.Android.Platform.Init();
		}

        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);

			CrossFingerprint.SetCurrentActivityResolver(() => this);

			Xamarin.Essentials.Platform.Init(Application);
			ZXing.Net.Mobile.Forms.Android.Platform.Init();
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
		{
			global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}


	}
}

