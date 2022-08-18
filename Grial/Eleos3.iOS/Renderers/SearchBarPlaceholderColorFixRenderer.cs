using System;
using UXDivers.Grial;
using Xamarin.Forms;

namespace Eleos3.iOS
{
	/// <summary>
    /// IOS 13 has this problem when setting the placeholder color of UISearchBar:
	/// https://stackoverflow.com/questions/57820342/ios-13-set-uisearchtextfield-placeholder-color
    /// The error is submitted here altough is not a Xamarin.Forms issue:
	/// https://github.com/xamarin/Xamarin.Forms/issues/7695
    /// Once this is fixed, this renderer can be completely removed, see declaration in AssemblyInfo.cs
	/// </summary>
	public class SearchBarPlaceholderColorFixRenderer : SearchBarRenderer
	{
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (Element != null)
            {
                var color = Element.PlaceholderColor;
                Element.PlaceholderColor = Color.Default;
                Element.PlaceholderColor = color;
            }
        }
    }
}
