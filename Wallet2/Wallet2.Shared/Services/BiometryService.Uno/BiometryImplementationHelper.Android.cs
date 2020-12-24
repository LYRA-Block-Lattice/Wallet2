#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using AndroidX.Biometric;
using Android.OS;

namespace BiometryService
{
	public partial class BiometryImplementationHelper
	{
		public static BiometryImplementation GetBiometryImplementation()
		{
			// Biometry API is only available on devices running Android 6.0 and up.
			if (Build.VERSION.SdkInt >= BuildVersionCodes.M &&
				Application.Context.CheckSelfPermission(Android.Manifest.Permission.UseBiometric) == Permission.Granted)
			{
				var bm = BiometricManager.From(Application.Context);
				if (bm.CanAuthenticate() == BiometricManager.BiometricSuccess)
				{
					// TODO differenciation between BiometryImplementation.FingerprintId and BiometryImplementation.FaceId;
					return BiometryImplementation.FingerprintId;
				}
			}

			return BiometryImplementation.Unavailable;
		}
	}
}
#endif
