using System;
using Eleos3;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UXDivers.Grial;

[assembly: ExportRenderer(typeof(ExtendedCarouselView), typeof(Eleos3.iOS.ExtendedCarouselViewRenderer))]

namespace Eleos3.iOS
{
    /// <summary>
    /// This renderer is used to fix layout problems on orientation changes with the CarouselView when Loop == false
    /// </summary>
    public class ExtendedCarouselViewRenderer : CarouselViewRenderer
    {
        private bool _forceScrollFix;

        public ExtendedCarouselViewRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<CarouselView> e)
        {
            if (e.OldElement != null)
            {
                e.OldElement.SizeChanged -= OnElementSizeChanged;
            }

            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                e.NewElement.SizeChanged += OnElementSizeChanged;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (Element != null)
            {
                Element.SizeChanged -= OnElementSizeChanged;
            }

            base.Dispose(disposing);
        }

        protected override void ScrollToRequested(object sender, ScrollToRequestEventArgs args)
        {
            if (_forceScrollFix)
            {
                Element.Loop = true;
            }

            base.ScrollToRequested(sender, args);

            if (_forceScrollFix)
            {
                Element.Loop = false;
            }
        }

        private void OnElementSizeChanged(object sender, EventArgs e)
        {
            _forceScrollFix = !Element.Loop;
            if (_forceScrollFix)
            {
                Device.BeginInvokeOnMainThread(() => _forceScrollFix = false);
            }
        }
    }
}
