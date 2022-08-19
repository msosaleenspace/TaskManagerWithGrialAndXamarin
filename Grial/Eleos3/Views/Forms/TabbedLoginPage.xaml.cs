using System;
using Xamarin.Forms;
using UXDivers.Grial;
using Eleos3.ViewModels.DemoApp;

namespace Eleos3
{
    public partial class TabbedLoginPage : ContentPage
    {

        public TabbedLoginPageViewModel TabbedLoginPageViewModel { get; set; }

        private bool LoginInProcess = false;

        private bool SignupInProcess = false;

        public TabbedLoginPage()
        {
            InitializeComponent();
            this.TabbedLoginPageViewModel = new TabbedLoginPageViewModel(this.MessageLabelLogin,
                this.EmailAddressEntryLogin,
                this.PasswordEntryLogin,
                this.LoginInProcess,

                this.MessageLabelSignup,
                this.EmailAddressEntrySignup,
                this.PasswordEntrySignup,
                this.SignupInProcess

                );

        }

        private async void OnCloseButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync();
        }

        private async void OnLoginBtnClicked(object sender, EventArgs e)
        {
            if (!this.LoginInProcess)
            {
                this.LoginInProcess = await this.TabbedLoginPageViewModel.Login();
            }

        }

        private async void OnSignupBtnClicked(object sender, EventArgs e)
        {
            if (!this.SignupInProcess)
            {
                this.SignupInProcess = await this.TabbedLoginPageViewModel.Signup();
            }

        }

    }

}