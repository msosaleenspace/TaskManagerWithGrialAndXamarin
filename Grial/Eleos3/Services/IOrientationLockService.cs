using UXDivers.Grial;
namespace Eleos3
{
    public enum OrientationLock
    {
        Unspecified = 0,
        Portrait,
        Landscape, // both landscape orientations
        ForwardLandscape,
        ReverseLandscape
    }

    /// <summary>
    /// This service allows setting a specific device orientation any time.
    /// </summary>
    public interface IOrientationLockService
    {
        /// <summary>
        /// Sets a specific device orientation. Device rotations will be ignored after calling this method.
        ///
        /// iOS considerations:
        ///   - the target <paramref name="orientationLock"/> needs to be enabled in the Info.plist beforehand, otherwise it will be ignored
        ///   - on IPads will work only if multitasking is disabled in the Info.plist ("Requires full screen" check needs to be true)
        ///     https://developer.apple.com/library/archive/documentation/WindowsViews/Conceptual/AdoptingMultitaskingOniPad/QuickStartForSlideOverAndSplitView.html
        /// </summary>
        void LockOrientation(OrientationLock orientationLock);

        /// <summary>
        /// Removes the orientation lock forced through a call to <see cref="LockOrientation(OrientationLock)" />.
        ///
        /// iOS considerations:
        ///   - if the device rotation is physically disabled by the device owner, even after calling this method the app will still be locked in the
        ///     orientation set through <see cref="LockOrientation(OrientationLock)" />.
        ///   - on the iPhone, the disabling the physical rotation means "portrait lock", so, to avoid your app being locked in landscape, it's good practice to 
        ///     go back to portrait before unlocking. The <paramref name="forcePortaitBeforeUnlockingOnIPhones"/> helps with that. It doesn't affect iPads.
        /// </summary>
        void UnlockOrientation(bool forcePortaitBeforeUnlockingOnIPhones = true);
    }
}
