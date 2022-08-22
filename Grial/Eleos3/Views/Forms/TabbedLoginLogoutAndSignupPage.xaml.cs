using System;
using Xamarin.Forms;
using UXDivers.Grial;
using Eleos3.ViewModels.DemoApp;
using Eleos3.Domain;
using Acr.UserDialogs;
using System.ComponentModel;

namespace Eleos3
{
    public partial class TabbedLoginLogoutAndSignupPage : ContentPage
    {

        private TabbedLoginLogoutAndSignupPageViewModel TabbedLoginPageViewModel { get; set; }

        private bool LoginInProcess = false;

        private bool LogoutInProcess = false;

        private bool SignupInProcess = false;

        private TabbedLoginLogoutAndSignupPageObjects TabbedLoginPageObjects { get; set; }

        public TabbedLoginLogoutAndSignupPage()
        {
            InitializeComponent();

            this.TabbedLoginPageObjects = new TabbedLoginLogoutAndSignupPageObjects();

            this.TabbedLoginPageObjects.LoginInProcess = this.LoginInProcess;
            this.TabbedLoginPageObjects.MessageLabelLogin = this.MessageLabelLogin;
            this.TabbedLoginPageObjects.EmailAddressEntryLogin = this.EmailAddressEntryLogin;
            this.TabbedLoginPageObjects.PasswordEntryLogin = this.PasswordEntryLogin;
            this.TabbedLoginPageObjects.SignupInProcess = this.SignupInProcess;
            this.TabbedLoginPageObjects.MessageLabelSignup = this.MessageLabelSignup;
            this.TabbedLoginPageObjects.EmailAddressEntrySignup = this.EmailAddressEntrySignup;
            this.TabbedLoginPageObjects.PasswordEntrySignup = this.PasswordEntrySignup;
            this.TabbedLoginPageObjects.LogoutInProcess = this.LogoutInProcess;
            this.TabbedLoginPageObjects.LogoutMessageLabel = this.LogoutMessageLabel;

            this.TabbedLoginPageViewModel = new TabbedLoginLogoutAndSignupPageViewModel(this.TabbedLoginPageObjects);
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

        private async void OnLogoutBtnClicked(object sender, EventArgs e)
        {
            if (!this.LogoutInProcess)
            {
                UserDialogs.Instance.ShowLoading("Wait please...");
                this.LogoutInProcess = await this.TabbedLoginPageViewModel.Logout();
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
            this.LogoutMessageLabel.Text = "";
        }

        private async void OnCloseButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync();
        }



        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            var imageButton = sender as ImageButton;
            if (this.PasswordEntryLogin.IsPassword)
            {
                imageButton.Source = ImageSource.FromFile("dog.png");
                this.PasswordEntryLogin.IsPassword = false;
            }
            else
            {
                imageButton.Source = ImageSource.FromFile("dog.png");
                this.PasswordEntryLogin.IsPassword = true;
            }
        }



    }

    public class ShowPasswordTriggerAction : TriggerAction<ImageButton>, INotifyPropertyChanged
    {
        public string ShowIcon { get; set; }
        public string HideIcon { get; set; }

        bool _hidePassword = true;

        public bool HidePassword
        {
            set
            {
                if (_hidePassword != value)
                {
                    _hidePassword = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HidePassword)));
                }
            }
            get => _hidePassword;
        }

        protected override void Invoke(ImageButton sender)
        {
            sender.Source = HidePassword ? ShowIcon : HideIcon;
            HidePassword = !HidePassword;
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }

}