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

        private bool AddTaskInProcess { get; set; }

        private Label AddTaskMessageLabel { get; set; }

        private Entry TaskNameEntry { get; set; }

        private DatePicker TaskDatePicker { get; set; }


        private bool UpdateTaskInProcess { get; set; }

        private Label UpdateTaskMessageLabel { get; set; }

        private Entry TaskIdEntryOnUpdate { get; set; }

        private Entry TaskNameEntryOnUpdate { get; set; }

        private DatePicker TaskDatePickerOnUpdate { get; set; }



        private bool DeleteTaskInProcess { get; set; }

        private Label DeleteTaskMessageLabel { get; set; }

        private Entry TaskIdEntryOnDelete { get; set; }

        public TabbedAddTaskViewModel(
            Label addTaskMessageLabel,
            bool addTaskInProcess,
            Entry taskNameEntry,
            DatePicker taskDatePicker,

            Label updateTaskMessageLabel,
            bool updateTaskInProcess,
            Entry taskIdEntryOnUpdate,
            Entry taskNameEntryOnUpdate,
            DatePicker taskDatePickerOnUpdate,

            Label deleteTaskMessageLabel,
            bool deleteTaskInProcess,
            Entry taskIdEntryOnDelete)
        {
            this.AddTaskMessageLabel = addTaskMessageLabel;
            this.AddTaskInProcess = addTaskInProcess;
            this.TaskNameEntry = taskNameEntry;
            this.TaskDatePicker = taskDatePicker;


            this.UpdateTaskMessageLabel = updateTaskMessageLabel;
            this.UpdateTaskInProcess = updateTaskInProcess;
            this.TaskIdEntryOnUpdate = taskIdEntryOnUpdate;
            this.TaskNameEntryOnUpdate = taskNameEntryOnUpdate;
            this.TaskDatePickerOnUpdate = taskDatePickerOnUpdate;


            this.DeleteTaskMessageLabel = deleteTaskMessageLabel;
            this.DeleteTaskInProcess = deleteTaskInProcess;
            this.TaskIdEntryOnDelete = taskIdEntryOnDelete;
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
                TodoTaskDTO todoTaskDTO = this.CreateTodoTaskDTOOnAddTask(userDTO);

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

        private TodoTaskDTO CreateTodoTaskDTOOnAddTask(UserDTO userDTO)
        {
            TodoTaskDTO todoTaskDTO = new TodoTaskDTO();
            todoTaskDTO.id = 0;
            todoTaskDTO.name = this.TaskNameEntry.Text;
            todoTaskDTO.date = new DateTime(this.TaskDatePicker.Date.Year, this.TaskDatePicker.Date.Month, this.TaskDatePicker.Date.Day);
            todoTaskDTO.user = userDTO;

            return todoTaskDTO;
        }

        private TodoTaskDTO CreateTodoTaskDTOOnUpdateTask(UserDTO userDTO)
        {
            TodoTaskDTO todoTaskDTO = new TodoTaskDTO();
            todoTaskDTO.id = Int32.Parse(this.TaskIdEntryOnUpdate.Text);
            todoTaskDTO.name = this.TaskNameEntryOnUpdate.Text;
            todoTaskDTO.date = this.TaskDatePickerOnUpdate.Date;
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

        public async Task<bool> UpdateTask()
        {
            this.UpdateTaskMessageLabel.Text = "";
            this.UpdateTaskInProcess = true;

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
                    this.TaskIdEntryOnUpdate .Text = "";
                    this.TaskNameEntryOnUpdate.Text = "";
                    this.TaskDatePickerOnUpdate.Date = DateTime.Today;
                    this.UpdateTaskMessageLabel.Text = "Task updated successfully!";
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseDTO>(responseContent);
                    this.UpdateTaskMessageLabel.Text = responseDTO.errorMessage;
                }
            }
            catch (ArgumentNullException ex)
            {
                this.UpdateTaskMessageLabel.Text = "Check your input data please!";
            }
            catch (NullReferenceException ex)
            {
                this.UpdateTaskMessageLabel.Text = "Check your input data please!";
            }
            catch (FormatException ex)
            {
                this.UpdateTaskMessageLabel.Text = "'Id' must be a number.";
            }
            catch (JsonException ex)
            {
                this.UpdateTaskMessageLabel.Text = "Could not connect to server.";
            }
            catch (Exception ex)
            {
                this.UpdateTaskMessageLabel.Text = ex.Message;
            }

            this.UpdateTaskInProcess = false;

            return this.UpdateTaskInProcess;
        }





        public async Task<bool> DeleteTodoTask()
        {
            this.DeleteTaskMessageLabel.Text = "";
            this.DeleteTaskInProcess = true;

            this.SetHttpEnvironment();

            try
            {
                int todoTaskId = Int32.Parse(this.TaskIdEntryOnDelete.Text);

                Uri uri = new Uri(string.Format("https://leenspacetaskmanager.herokuapp.com/api/tasks/" + todoTaskId, string.Empty));

                HttpResponseMessage response = null;

                HttpClient.DefaultRequestHeaders.Add("token", Preferences.Get("Token", ""));

                response = await HttpClient.DeleteAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    this.DeleteTaskMessageLabel.Text = "Task deleted successfully!";
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var responseDTO = System.Text.Json.JsonSerializer.Deserialize<ResponseDTO>(responseContent);
                    this.DeleteTaskMessageLabel.Text = responseDTO.errorMessage;
                }
            }
            catch (ArgumentNullException ex)
            {
                this.DeleteTaskMessageLabel.Text = "Check your input data please!";
            }
            catch (FormatException ex)
            {
                this.DeleteTaskMessageLabel.Text = "'Id' must be a number.";
            }
            catch (JsonException ex)
            {
                this.DeleteTaskMessageLabel.Text = "Could not connect to server.";
            }
            catch (Exception ex)
            {
                this.DeleteTaskMessageLabel.Text = ex.Message;
            }

            this.DeleteTaskInProcess = false;

            return this.DeleteTaskInProcess;
        }

    }

}