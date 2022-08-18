using System;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using UXDivers.Grial;

namespace Eleos3
{
    public partial class SimpleDialog : PopupPage
    {
        public SimpleDialog()
        {
            InitializeComponent();
        }

        private void OnClose(object sender, EventArgs e)
        {
            PopupNavigation.Instance.PopAsync();
        }
    }
}
