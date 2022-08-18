using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;
using UXDivers.Grial;

namespace Eleos3
{
    public partial class SimpleDialogNoTitleInverse : PopupPage
    {
        public SimpleDialogNoTitleInverse()
        {
            InitializeComponent();
        }

        private void OnClose(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
    }
}
