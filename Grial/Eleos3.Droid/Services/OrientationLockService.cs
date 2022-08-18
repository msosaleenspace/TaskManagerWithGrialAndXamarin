using Android.Content.PM;
using Xamarin.Essentials;
using Xamarin.Forms;
using UXDivers.Grial;

[assembly: Dependency(typeof(Eleos3.Droid.OrientationLockService))]

namespace Eleos3.Droid
{
    public class OrientationLockService : IOrientationLockService
    {
        public void LockOrientation(OrientationLock orientationLock)
        {
            ScreenOrientation target;

            switch (orientationLock)
            {
                case OrientationLock.Portrait:
                    target = ScreenOrientation.Portrait;
                    break;
                case OrientationLock.Landscape:
                    target = ScreenOrientation.SensorLandscape;
                    break;
                case OrientationLock.ForwardLandscape:
                    target = ScreenOrientation.Landscape;
                    break;
                case OrientationLock.ReverseLandscape:
                    target = ScreenOrientation.ReverseLandscape;
                    break;
            default:
                target = ScreenOrientation.Unspecified;
                    break;
            }

            Platform.CurrentActivity.RequestedOrientation = target;
        }

        public void UnlockOrientation(bool forcePortaitBeforeUnlockingOnIPhones)
        {
            LockOrientation(OrientationLock.Unspecified);
        }
    }
}
