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
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Eleos3.ViewModels.DemoApp
{

    public class TabbedAddTaskViewModel
    {

        private HttpClient HttpClient { get; set; }

        private JsonSerializerOptions JsonSerializerOptions { get; set; }

        private bool LogoutInProcess { get; set; }

        private Label LogoutMessageLabel { get; set; }

        private bool AddTaskInProcess { get; set; }

        private Label AddTaskMessageLabel { get; set; }

        private Entry TaskNameEntry { get; set; }

        private DatePicker TaskDatePicker { get; set; }

        public TabbedAddTaskViewModel(Label logoutMessageLabel,
            bool logoutInProcess,
            Label addTaskMessageLabel,
            bool addTaskInProcess,
            Entry taskNameEntry,
            DatePicker taskDatePicker)
        {
            this.LogoutMessageLabel = logoutMessageLabel;
            this.LogoutInProcess = logoutInProcess;
            this.AddTaskMessageLabel = addTaskMessageLabel;
            this.AddTaskInProcess = addTaskInProcess;
            this.TaskNameEntry = taskNameEntry;
            this.TaskDatePicker = taskDatePicker;
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
                    int j = 5;
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

        private void SetHttpEnvironment()
        {
            this.HttpClient = new HttpClient();
            this.JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<bool> AddTask()
        {
            this.AddTaskMessageLabel.Text = "";
            this.AddTaskInProcess = true;

            this.SetHttpEnvironment();
            Uri uri = new Uri(string.Format("https://leenspacetaskmanager.herokuapp.com/api/tasks", string.Empty));

            try
            {
                UserDTO userDTO = this.CreateUserDTO();
                TodoTaskDTO todoTaskDTO = this.CreateTodoTaskDTO(userDTO);

                string json = System.Text.Json.JsonSerializer.Serialize<TodoTaskDTO>(todoTaskDTO, this.JsonSerializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;

                this.HttpClient.DefaultRequestHeaders.Add("token", Preferences.Get("Token", ""));

                response = await this.HttpClient.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    this.TaskNameEntry.Text = "";
                    this.TaskDatePicker.Date = DateTime.Today;
                    this.AddTaskMessageLabel.Text = "Task added successfully!";
                }
                else if ((int)response.StatusCode == 400)
                {
                    this.AddTaskMessageLabel.Text = "Check your input data please.";
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseDTO>(responseContent);
                    this.AddTaskMessageLabel.Text = responseDTO.errorMessage;
                }

            }
            catch (NullReferenceException ex)
            {
                this.AddTaskMessageLabel.Text = "Check your input data please.";
            }
            catch (JsonException ex)
            {
                this.AddTaskMessageLabel.Text = "Could not connect to server.";
            }
            catch (Exception ex)
            {
                this.AddTaskMessageLabel.Text = ex.Message;
            }

            this.AddTaskInProcess = false;

            return this.AddTaskInProcess;
        }

        private UserDTO CreateUserDTO()
        {
            UserDTO userDTO = new UserDTO();
            userDTO.id = Int32.Parse(Preferences.Get("UserId", ""));
            userDTO.emailAddress = "";
            userDTO.password = "";

            return userDTO;
        }

        private TodoTaskDTO CreateTodoTaskDTO(UserDTO userDTO)
        {
            TodoTaskDTO todoTaskDTO = new TodoTaskDTO();
            todoTaskDTO.id = 0;
            todoTaskDTO.name = this.TaskNameEntry.Text;
            todoTaskDTO.date = new DateTime(this.TaskDatePicker.Date.Year, this.TaskDatePicker.Date.Month, this.TaskDatePicker.Date.Day);
            todoTaskDTO.user = userDTO;

            return todoTaskDTO;
        }


        public async Task<List<TodoTaskDTO>> GetTasks()
        {
            List<TodoTaskDTO> todoTasksDTO = new List<TodoTaskDTO>();
            this.SetHttpEnvironment();

            Uri uri = new Uri(string.Format("https://leenspacetaskmanager.herokuapp.com/api/tasks", string.Empty));

            try
            {
                HttpResponseMessage response = null;

                HttpClient.DefaultRequestHeaders.Add("token", Preferences.Get("Token", ""));

                response = await HttpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    todoTasksDTO = System.Text.Json.JsonSerializer.Deserialize<List<TodoTaskDTO>>(content, JsonSerializerOptions);
                    int j = 1;
                }
            }
            catch (Exception ex)
            {
                
            }

            return todoTasksDTO;
        }

    }

}