using Android.Content;
using Eleos3;
using Eleos3.Droid.Renderers;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using UXDivers.Grial;

[assembly: ExportRenderer(typeof(ExtendedCarouselView), typeof(ExtendedCarouselViewRenderer))]

namespace Eleos3.Droid.Renderers
{
    /// <summary>
    /// This renderer is to ensure that in Android the ExtendeCarouselView recieves
    /// every scroll notification required for animations (including the idle state).
    /// </summary>
    public class ExtendedCarouselViewRenderer : CarouselViewRenderer
    {
        private ExtendedCarouselView _carousel;

        public ExtendedCarouselViewRenderer(Context context): base(context)
        {
        }

        public override void OnScrolled(int dx, int dy)
        {
            base.OnScrolled(dx, dy);

            var isHorizontal = _carousel.ItemsLayout.Orientation == ItemsLayoutOrientation.Horizontal;
            double offset = isHorizontal ? ComputeHorizontalScrollOffset() : ComputeVerticalScrollOffset();
            offset /= DeviceDisplay.MainDisplayInfo.Density;
            _carousel.HandleScrollChange(offset);
        }

        public override void OnScrollStateChanged(int state)
        {
            base.OnScrollStateChanged(state);

            if (state == ScrollStateIdle)
            {
                _carousel.AndroidScrollEnd();
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ItemsView> elementChangedEvent)
        {
            base.OnElementChanged(elementChangedEvent);

            _carousel = elementChangedEvent.NewElement as ExtendedCarouselView;
        }

        protected override void ScrollTo(ScrollToRequestEventArgs args)
        {
            // Workaround for this issue: https://github.com/xamarin/Xamarin.Forms/issues/13296
            if (_carousel.CurrentItem != null)
            {
                base.ScrollTo(args);
            }
        }
    }
}
