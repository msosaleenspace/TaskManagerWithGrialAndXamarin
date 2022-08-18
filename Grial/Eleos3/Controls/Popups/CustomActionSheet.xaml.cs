using System;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using UXDivers.Grial;

namespace Eleos3
{
    public partial class CustomActionSheet : PopupPage
    {
        public CustomActionSheet()
        {
            InitializeComponent();
        }

        private void OnClose(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
    }
}
