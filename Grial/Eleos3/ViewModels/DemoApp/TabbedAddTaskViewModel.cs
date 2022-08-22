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

        private TabbedAddTaskPageObjects TabbedAddTaskPageObjects { get; set; }

        public TabbedAddTaskViewModel(TabbedAddTaskPageObjects tabbedAddTaskPageObjects)
        {
            this.TabbedAddTaskPageObjects = tabbedAddTaskPageObjects;
        }

        public async Task<bool> AddTask()
        {
            this.TabbedAddTaskPageObjects.AddTaskMessageLabel.Text = "";
            this.TabbedAddTaskPageObjects.AddTaskInProcess = true;

            this.SetHttpEnvironment();
            Uri uri = new Uri(string.Format("https://leenspacetaskmanager.herokuapp.com/api/tasks", string.Empty));

            try
            {
                UserDTO userDTO = this.CreateUserDTO();
                TodoTaskDTO todoTaskDTO = this.CreateTodoTaskDTOOnAddTask(userDTO);

                string json = System.Text.Json.JsonSerializer.Serialize<TodoTaskDTO>(todoTaskDTO, this.JsonSerializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;

                this.HttpClient.DefaultRequestHeaders.Add("token", Preferences.Get("Token", ""));

                response = await this.HttpClient.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    this.TabbedAddTaskPageObjects.TaskNameEntry.Text = "";
                    this.TabbedAddTaskPageObjects.TaskDatePicker.Date = DateTime.Today;
                    this.TabbedAddTaskPageObjects.AddTaskMessageLabel.Text = "Task added successfully!";
                }
                else if ((int)response.StatusCode == 400)
                {
                    this.TabbedAddTaskPageObjects.AddTaskMessageLabel.Text = "Check your input data please.";
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseDTO>(responseContent);
                    this.TabbedAddTaskPageObjects.AddTaskMessageLabel.Text = responseDTO.errorMessage;
                }

            }
            catch (NullReferenceException ex)
            {
                this.TabbedAddTaskPageObjects.AddTaskMessageLabel.Text = "Check your input data please.";
            }
            catch (JsonException ex)
            {
                this.TabbedAddTaskPageObjects.AddTaskMessageLabel.Text = "Could not connect to server.";
            }
            catch (Exception ex)
            {
                this.TabbedAddTaskPageObjects.AddTaskMessageLabel.Text = ex.Message;
            }

            this.TabbedAddTaskPageObjects.AddTaskInProcess = false;

            return this.TabbedAddTaskPageObjects.AddTaskInProcess;
        }

        public async Task<bool> UpdateTask()
        {
            this.TabbedAddTaskPageObjects.UpdateTaskMessageLabel.Text = "";
            this.TabbedAddTaskPageObjects.UpdateTaskInProcess = true;

            this.SetHttpEnvironment();

            Uri uri = new Uri(string.Format("https://leenspacetaskmanager.herokuapp.com/api/tasks", string.Empty));

            try
            {
                UserDTO userDTO = this.CreateUserDTO();
                TodoTaskDTO todoTaskDTO = this.CreateTodoTaskDTOOnUpdateTask(userDTO);

                string json = System.Text.Json.JsonSerializer.Serialize<TodoTaskDTO>(todoTaskDTO, this.JsonSerializerOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = null;

                this.HttpClient.DefaultRequestHeaders.Add("token", Preferences.Get("Token", ""));

                response = await this.HttpClient.PutAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    this.TabbedAddTaskPageObjects.TaskIdEntryOnUpdate .Text = "";
                    this.TabbedAddTaskPageObjects.TaskNameEntryOnUpdate.Text = "";
                    this.TabbedAddTaskPageObjects.TaskDatePickerOnUpdate.Date = DateTime.Today;
                    this.TabbedAddTaskPageObjects.UpdateTaskMessageLabel.Text = "Task updated successfully!";
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseDTO>(responseContent);
                    this.TabbedAddTaskPageObjects.UpdateTaskMessageLabel.Text = responseDTO.errorMessage;
                }
            }
            catch (ArgumentNullException ex)
            {
                this.TabbedAddTaskPageObjects.UpdateTaskMessageLabel.Text = "Check your input data please!";
            }
            catch (NullReferenceException ex)
            {
                this.TabbedAddTaskPageObjects.UpdateTaskMessageLabel.Text = "Check your input data please!";
            }
            catch (FormatException ex)
            {
                this.TabbedAddTaskPageObjects.UpdateTaskMessageLabel.Text = "'Id' must be a number.";
            }
            catch (JsonException ex)
            {
                this.TabbedAddTaskPageObjects.UpdateTaskMessageLabel.Text = "Could not connect to server.";
            }
            catch (Exception ex)
            {
                this.TabbedAddTaskPageObjects.UpdateTaskMessageLabel.Text = ex.Message;
            }

            this.TabbedAddTaskPageObjects.UpdateTaskInProcess = false;

            return this.TabbedAddTaskPageObjects.UpdateTaskInProcess;
        }

        public async Task<bool> DeleteTodoTask()
        {
            this.TabbedAddTaskPageObjects.DeleteTaskMessageLabel.Text = "";
            this.TabbedAddTaskPageObjects.DeleteTaskInProcess = true;

            this.SetHttpEnvironment();

            try
            {
                int todoTaskId = Int32.Parse(this.TabbedAddTaskPageObjects.TaskIdEntryOnDelete.Text);

                Uri uri = new Uri(string.Format("https://leenspacetaskmanager.herokuapp.com/api/tasks/" + todoTaskId, string.Empty));

                HttpResponseMessage response = null;

                HttpClient.DefaultRequestHeaders.Add("token", Preferences.Get("Token", ""));

                response = await HttpClient.DeleteAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    this.TabbedAddTaskPageObjects.DeleteTaskMessageLabel.Text = "Task deleted successfully!";
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseDTO>(responseContent);
                    this.TabbedAddTaskPageObjects.DeleteTaskMessageLabel.Text = responseDTO.errorMessage;
                }
            }
            catch (ArgumentNullException ex)
            {
                this.TabbedAddTaskPageObjects.DeleteTaskMessageLabel.Text = "Check your input data please!";
            }
            catch (FormatException ex)
            {
                this.TabbedAddTaskPageObjects.DeleteTaskMessageLabel.Text = "'Id' must be a number.";
            }
            catch (JsonException ex)
            {
                this.TabbedAddTaskPageObjects.DeleteTaskMessageLabel.Text = "Could not connect to server.";
            }
            catch (Exception ex)
            {
                this.TabbedAddTaskPageObjects.DeleteTaskMessageLabel.Text = ex.Message;
            }

            this.TabbedAddTaskPageObjects.DeleteTaskInProcess = false;

            return this.TabbedAddTaskPageObjects.DeleteTaskInProcess;
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

        private void SetHttpEnvironment()
        {
            this.HttpClient = new HttpClient();
            this.JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        private UserDTO CreateUserDTO()
        {
            UserDTO userDTO = new UserDTO();
            userDTO.id = Int32.Parse(Preferences.Get("UserId", ""));
            userDTO.emailAddress = "";
            userDTO.password = "";

            return userDTO;
        }

        private TodoTaskDTO CreateTodoTaskDTOOnAddTask(UserDTO userDTO)
        {
            TodoTaskDTO todoTaskDTO = new TodoTaskDTO();
            todoTaskDTO.id = 0;
            todoTaskDTO.name = this.TabbedAddTaskPageObjects.TaskNameEntry.Text;
            todoTaskDTO.date = new DateTime(this.TabbedAddTaskPageObjects.TaskDatePicker.Date.Year, this.TabbedAddTaskPageObjects.TaskDatePicker.Date.Month, this.TabbedAddTaskPageObjects.TaskDatePicker.Date.Day);
            todoTaskDTO.user = userDTO;

            return todoTaskDTO;
        }

        private TodoTaskDTO CreateTodoTaskDTOOnUpdateTask(UserDTO userDTO)
        {
            TodoTaskDTO todoTaskDTO = new TodoTaskDTO();
            todoTaskDTO.id = Int32.Parse(this.TabbedAddTaskPageObjects.TaskIdEntryOnUpdate.Text);
            todoTaskDTO.name = this.TabbedAddTaskPageObjects.TaskNameEntryOnUpdate.Text;
            todoTaskDTO.date = this.TabbedAddTaskPageObjects.TaskDatePickerOnUpdate.Date;
            todoTaskDTO.user = userDTO;

            return todoTaskDTO;
        }

    }

}