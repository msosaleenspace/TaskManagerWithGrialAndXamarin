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

    public class TabbedAddTaskViewModel
    {

        private HttpClient HttpClient { get; set; }

        private JsonSerializerOptions JsonSerializerOptions { get; set; }

        private bool LogoutInProcess { get; set; }

        public TabbedAddTaskViewModel(Label MessageLabel, bool logoutInProcess)
        {
            MessageLabel.Text = "";
            this.LogoutInProcess = logoutInProcess;
        }

        public async Task<bool> Logout(Label MessageLabel)
        {
            this.LogoutInProcess = true;
            MessageLabel.Text = "";

            this.SetHttpEnvironment();
            Uri uri = new Uri(string.Format("https://leenspacetaskmanager.herokuapp.com/api/logout", string.Empty));

            try
            {
                HttpResponseMessage response = null;

                HttpClient.DefaultRequestHeaders.Add("token", Preferences.Get("Token", ""));

                response = await HttpClient.DeleteAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    int j = 5;
                    Preferences.Set("Token", "");
                    Preferences.Set("UserId", "");
                    MessageLabel.Text = "Logout successful!";
                }
                else if (((int)response.StatusCode) == 400)
                {
                    MessageLabel.Text = "No token found!";
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseDTO>(responseContent);
                    MessageLabel.Text = responseDTO.errorMessage;
                }
            }
            catch (JsonException ex)
            {
                MessageLabel.Text = "Could not connect to server.";
            }
            catch (Exception ex)
            {
                MessageLabel.Text = ex.Message;
            }

            this.LogoutInProcess = false;

            return this.LogoutInProcess;
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