using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Essentials;
using UXDivers.Grial;

[assembly: Dependency(typeof(Eleos3.iOS.OrientationLockService))]

namespace Eleos3.iOS
{
    public class OrientationLockService : IOrientationLockService
    {
        private bool _orientationHasBeenLockedBefore;

        public void LockOrientation(OrientationLock orientationLock)
        {
            UIInterfaceOrientation target;
            UIInterfaceOrientationMask mask;

            _orientationHasBeenLockedBefore = true;

            switch (orientationLock)
            {
                case OrientationLock.Portrait:
                    target = UIInterfaceOrientation.Portrait;
                    mask = UIInterfaceOrientationMask.Portrait;
                    break;
                case OrientationLock.Landscape:
                    target = UIInterfaceOrientation.LandscapeRight;
                    mask = UIInterfaceOrientationMask.LandscapeLeft | UIInterfaceOrientationMask.LandscapeRight;
                    break;
                case OrientationLock.ForwardLandscape:
                    target = UIInterfaceOrientation.LandscapeRight;
                    mask = UIInterfaceOrientationMask.LandscapeRight;
                    break;
                case OrientationLock.ReverseLandscape:
                    target = UIInterfaceOrientation.LandscapeLeft;
                    mask = UIInterfaceOrientationMask.LandscapeLeft;
                    break;
                default:
                    _orientationHasBeenLockedBefore = false;
                    target = UIInterfaceOrientation.Unknown;
                    mask = UIInterfaceOrientationMask.All;
                    break;
            }

            var appDelegate = (AppDelegate)UIApplication.SharedApplication.Delegate;
            appDelegate.EnabledOrientations = mask;

            UIDevice.CurrentDevice.SetValueForKey(new NSNumber((int)target), new NSString("orientation"));
        }

        public void UnlockOrientation(bool forcePortaitBeforeUnlockingOnIPhones)
        {
            if (_orientationHasBeenLockedBefore &&
                forcePortaitBeforeUnlockingOnIPhones &&
                Device.Idiom == TargetIdiom.Phone &&
                DeviceDisplay.MainDisplayInfo.Orientation == DisplayOrientation.Landscape)
            {
                // If orientation has not been locked before do not force portait
                DeviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
                LockOrientation(OrientationLock.Portrait);
            }
            else
            {
                LockOrientation(OrientationLock.Unspecified);
            }
        }

        private void OnMainDisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            DeviceDisplay.MainDisplayInfoChanged -= OnMainDisplayInfoChanged;
            LockOrientation(OrientationLock.Unspecified);
        }
    }
}
