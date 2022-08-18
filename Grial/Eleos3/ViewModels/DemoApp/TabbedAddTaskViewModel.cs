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

        public TabbedAddTaskViewModel(Label MessageLabel)
        {
            MessageLabel.Text = "";
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

                HttpClient.DefaultRequestHeaders.Add("token", "bjghjghjhhvg"/*Preferences.Get("Token", "")*/);

                response = await HttpClient.DeleteAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    Preferences.Set("Token", "");
                    Preferences.Set("UserId", "");
                    MessageLabel.Text = "Logout successful!";
                    int j = 1;
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseDTO>(responseContent);
                    MessageLabel.Text = responseDTO.errorMessage;
                    int k = 1;

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