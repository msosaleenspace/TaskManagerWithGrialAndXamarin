using System;
using Xamarin.Forms;
using UXDivers.Grial;

namespace Eleos3
{
    public partial class TabbedLoginPage : ContentPage
    {
        public TabbedLoginPage()
        {
            InitializeComponent();
        }

        private async void OnCloseButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync();
        }
    }
}
