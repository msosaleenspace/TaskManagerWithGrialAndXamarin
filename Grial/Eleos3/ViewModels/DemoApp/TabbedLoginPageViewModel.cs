using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Text;
using Eleos3.Domain;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using JsonException = System.Text.Json.JsonException;

namespace Eleos3.ViewModels.DemoApp
{

    public class TabbedLoginPageViewModel
    {

        private HttpClient HttpClient { get; set; }

        private JsonSerializerOptions JsonSerializerOptions { get; set; }

        private bool LoginInProcess { get; set; }

        private Label MessageLabelLogin { get; set; }

        private Entry EmailAddressEntryLogin { get; set; }

        private Entry PasswordEntryLogin { get; set; }

        private bool SignupInProcess { get; set; }

        private Label MessageLabelSignup { get; set; }

        private Entry EmailAddressEntrySignup { get; set; }

        private Entry PasswordEntrySignup { get; set; }

        public TabbedLoginPageViewModel(Label messageLabelLogin,
            Entry emailAddressEntryLogin,
            Entry passwordEntryLogin,
            bool loginInProcess,
            Label messageLabelSignup,
            Entry emailAddressEntrySignup,
            Entry passwordEntrySignup,
            bool signupInProcess)
        {
            this.MessageLabelLogin = messageLabelLogin;
            this.EmailAddressEntryLogin = emailAddressEntryLogin;
            this.PasswordEntryLogin = passwordEntryLogin;
            this.LoginInProcess = loginInProcess;

            this.MessageLabelSignup = messageLabelSignup;
            this.EmailAddressEntrySignup = emailAddressEntrySignup;
            this.PasswordEntrySignup = passwordEntrySignup;
            this.SignupInProcess = signupInProcess;

            this.MessageLabelLogin.Text = "";
            this.MessageLabelSignup.Text = "";
        }

        public async Task<bool> Login()
        {
            this.LoginInProcess = true;
            this.MessageLabelLogin.Text = "";

            this.SetHttpEnvironment();

            string AnEmailAddress = EmailAddressEntryLogin.Text;
            string Apassword = PasswordEntryLogin.Text;

            Uri uri = new Uri(string.Format("https://leenspacetaskmanager.herokuapp.com/api/login?email=" + AnEmailAddress + "&password=" + Apassword, string.Empty));

            try
            {
                if (String.IsNullOrWhiteSpace(EmailAddressEntryLogin.Text) || String.IsNullOrWhiteSpace(PasswordEntryLogin.Text))
                {
                    this.MessageLabelLogin.Text = "Check your input data please!";
                }
                else
                {
                    HttpResponseMessage response = null;

                    response = await HttpClient.PostAsync(uri, null);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();

                        var sessionDTO = System.Text.Json.JsonSerializer.Deserialize<SessionDTO>(responseContent);

                        Preferences.Set("Token", sessionDTO.token);
                        Preferences.Set("UserId", sessionDTO.userId.ToString());
                        this.MessageLabelLogin.Text = "You are now logged!";
                    }
                    else
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseDTO>(responseContent);
                        this.MessageLabelLogin.Text = responseDTO.errorMessage;
                    }
                }
            }
            catch (JsonException ex)
            {
                this.MessageLabelLogin.Text = "Could not connect to server.";
            }
            catch (Exception ex)
            {
                this.MessageLabelLogin.Text = ex.GetType().ToString();
            }

            this.LoginInProcess = false;

            return this.LoginInProcess;
        }

        public async Task<bool> Signup()
        {
            this.MessageLabelSignup.Text = "";
            this.SignupInProcess = true;

            try
            {
                if (String.IsNullOrWhiteSpace(this.EmailAddressEntrySignup.Text) || String.IsNullOrWhiteSpace(this.PasswordEntrySignup.Text))
                {
                    this.MessageLabelSignup.Text = "Check your input data please!";
                }
                else
                {
                    UserDTO userDTO = new UserDTO();
                    userDTO.id = 0;
                    userDTO.emailAddress = this.EmailAddressEntrySignup.Text;
                    userDTO.password = this.PasswordEntrySignup.Text;

                    this.SetHttpEnvironment();
                    Uri uri = new Uri(string.Format("https://leenspacetaskmanager.herokuapp.com/api/users", string.Empty));

                    string json = System.Text.Json.JsonSerializer.Serialize<UserDTO>(userDTO, JsonSerializerOptions);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = null;

                    response = await HttpClient.PostAsync(uri, content);

                    if (response.IsSuccessStatusCode)
                    {
                        this.EmailAddressEntrySignup.Text = "";
                        this.PasswordEntrySignup.Text = "";
                        this.MessageLabelSignup.Text = "User created succesfully!";
                    }
                    else
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseDTO>(responseContent);
                        this.MessageLabelSignup.Text = responseDTO.errorMessage;
                    }
                }
            }
            catch (JsonException ex)
            {
                this.MessageLabelSignup.Text = "Could not connect to server";
            }
            catch (Exception ex)
            {
                this.MessageLabelSignup.Text = ex.Message;
            }

            this.SignupInProcess = false;

            return this.SignupInProcess;
        }

        private void SetHttpEnvironment()
        {
            this.HttpClient = new HttpClient();
            this.JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

    }

}