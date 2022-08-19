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

        private TabbedLoginPageObjects TabbedLoginPageObjects { get; set; }

        private Label LogoutMessageLabel { get; set; }

        private bool LogoutInProcess { get; set; }

        public TabbedLoginPageViewModel(TabbedLoginPageObjects tabbedLoginPageObjects)
        {
            this.TabbedLoginPageObjects = tabbedLoginPageObjects;

            this.TabbedLoginPageObjects.MessageLabelLogin.Text = "";
            this.TabbedLoginPageObjects.MessageLabelSignup.Text = "";
            this.LogoutMessageLabel = this.TabbedLoginPageObjects.LogoutMessageLabel;
            this.LogoutInProcess = this.TabbedLoginPageObjects.LogoutInProcess;

        }

        public async Task<bool> Login()
        {
            this.TabbedLoginPageObjects.LoginInProcess = true;
            this.TabbedLoginPageObjects.MessageLabelLogin.Text = "";

            this.SetHttpEnvironment();

            string AnEmailAddress = this.TabbedLoginPageObjects.EmailAddressEntryLogin.Text;
            string Apassword = this.TabbedLoginPageObjects.PasswordEntryLogin.Text;

            Uri uri = new Uri(string.Format("https://leenspacetaskmanager.herokuapp.com/api/login?email=" + AnEmailAddress + "&password=" + Apassword, string.Empty));

            try
            {
                if (String.IsNullOrWhiteSpace(this.TabbedLoginPageObjects.EmailAddressEntryLogin.Text) || String.IsNullOrWhiteSpace(this.TabbedLoginPageObjects.PasswordEntryLogin.Text))
                {
                    this.TabbedLoginPageObjects.MessageLabelLogin.Text = "Check your input data please!";
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
                        this.TabbedLoginPageObjects.MessageLabelLogin.Text = "You are now logged!";
                    }
                    else
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseDTO>(responseContent);
                        this.TabbedLoginPageObjects.MessageLabelLogin.Text = responseDTO.errorMessage;
                    }
                }
            }
            catch (JsonException ex)
            {
                this.TabbedLoginPageObjects.MessageLabelLogin.Text = "Could not connect to server.";
            }
            catch (Exception ex)
            {
                this.TabbedLoginPageObjects.MessageLabelLogin.Text = ex.GetType().ToString();
            }

            this.TabbedLoginPageObjects.LoginInProcess = false;

            return this.TabbedLoginPageObjects.LoginInProcess;
        }

        public async Task<bool> Signup()
        {
            this.TabbedLoginPageObjects.MessageLabelSignup.Text = "";
            this.TabbedLoginPageObjects.SignupInProcess = true;

            try
            {
                if (String.IsNullOrWhiteSpace(this.TabbedLoginPageObjects.EmailAddressEntrySignup.Text) || String.IsNullOrWhiteSpace(this.TabbedLoginPageObjects.PasswordEntrySignup.Text))
                {
                    this.TabbedLoginPageObjects.MessageLabelSignup.Text = "Check your input data please!";
                }
                else
                {
                    UserDTO userDTO = new UserDTO();
                    userDTO.id = 0;
                    userDTO.emailAddress = this.TabbedLoginPageObjects.EmailAddressEntrySignup.Text;
                    userDTO.password = this.TabbedLoginPageObjects.PasswordEntrySignup.Text;

                    this.SetHttpEnvironment();
                    Uri uri = new Uri(string.Format("https://leenspacetaskmanager.herokuapp.com/api/users", string.Empty));

                    string json = System.Text.Json.JsonSerializer.Serialize<UserDTO>(userDTO, JsonSerializerOptions);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = null;

                    response = await HttpClient.PostAsync(uri, content);

                    if (response.IsSuccessStatusCode)
                    {
                        this.TabbedLoginPageObjects.EmailAddressEntrySignup.Text = "";
                        this.TabbedLoginPageObjects.PasswordEntrySignup.Text = "";
                        this.TabbedLoginPageObjects.MessageLabelSignup.Text = "User created succesfully!";
                    }
                    else
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var responseDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseDTO>(responseContent);
                        this.TabbedLoginPageObjects.MessageLabelSignup.Text = responseDTO.errorMessage;
                    }
                }
            }
            catch (JsonException ex)
            {
                this.TabbedLoginPageObjects.MessageLabelSignup.Text = "Could not connect to server";
            }
            catch (Exception ex)
            {
                this.TabbedLoginPageObjects.MessageLabelSignup.Text = ex.Message;
            }

            this.TabbedLoginPageObjects.SignupInProcess = false;

            return this.TabbedLoginPageObjects.SignupInProcess;
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

        public async Task<bool> Logout()
        {
            this.LogoutInProcess = true;
            this.LogoutMessageLabel.Text = "";

            this.SetHttpEnvironment();
            Uri uri = new Uri(string.Format("https://leenspacetaskmanager.herokuapp.com/api/logout", string.Empty));

            try
            {
                HttpResponseMessage response = null;

                HttpClient.DefaultRequestHeaders.Add("token", Preferences.Get("Token", ""));

                response = await HttpClient.DeleteAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    Preferences.Set("Token", "");
                    Preferences.Set("UserId", "");
                    this.LogoutMessageLabel.Text = "Logout successful!";
                }
                else if (((int)response.StatusCode) == 400)
                {
                    this.LogoutMessageLabel.Text = "No token found!";
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseDTO>(responseContent);
                    this.LogoutMessageLabel.Text = responseDTO.errorMessage;
                }
            }
            catch (JsonException ex)
            {
                this.LogoutMessageLabel.Text = "Could not connect to server.";
            }
            catch (Exception ex)
            {
                this.LogoutMessageLabel.Text = ex.Message;
            }

            this.LogoutInProcess = false;

            return this.LogoutInProcess;
        }

    }

}