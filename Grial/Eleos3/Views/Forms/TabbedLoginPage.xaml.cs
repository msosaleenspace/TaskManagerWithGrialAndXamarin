using System;
using Xamarin.Forms;
using UXDivers.Grial;
using Eleos3.ViewModels.DemoApp;
using Eleos3.Domain;
using Acr.UserDialogs;

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

            TabbedLoginPageObjects tabbedLoginPageObjects = new TabbedLoginPageObjects();

            tabbedLoginPageObjects.LoginInProcess = this.LoginInProcess;
            tabbedLoginPageObjects.MessageLabelLogin = this.MessageLabelLogin;
            tabbedLoginPageObjects.EmailAddressEntryLogin = this.EmailAddressEntryLogin;
            tabbedLoginPageObjects.PasswordEntryLogin = this.PasswordEntryLogin;
            tabbedLoginPageObjects.SignupInProcess = this.SignupInProcess;
            tabbedLoginPageObjects.MessageLabelSignup = this.MessageLabelSignup;
            tabbedLoginPageObjects.EmailAddressEntrySignup = this.EmailAddressEntrySignup;
            tabbedLoginPageObjects.PasswordEntrySignup = this.PasswordEntrySignup;

            this.TabbedLoginPageViewModel = new TabbedLoginPageViewModel(tabbedLoginPageObjects);
        }

        private async void OnCloseButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync();
        }

        private async void OnLoginBtnClicked(object sender, EventArgs e)
        {
            if (!this.LoginInProcess)
            {
                UserDialogs.Instance.ShowLoading("Wait please...");
                this.LoginInProcess = await this.TabbedLoginPageViewModel.Login();
                UserDialogs.Instance.HideLoading();
            }
        }

        private async void OnSignupBtnClicked(object sender, EventArgs e)
        {
            if (!this.SignupInProcess)
            {
                UserDialogs.Instance.ShowLoading("Wait please...");
                this.SignupInProcess = await this.TabbedLoginPageViewModel.Signup();
                UserDialogs.Instance.HideLoading();
            }
        }

        private void OnTabChangingEvent(object sender, EventArgs e)
        {
            this.MessageLabelLogin.Text = "";
            this.EmailAddressEntryLogin.Text = "";
            this.PasswordEntryLogin.Text = "";
            this.MessageLabelSignup.Text = "";
            this.EmailAddressEntrySignup.Text = "";
            this.PasswordEntrySignup.Text = "";
        }

    }

}