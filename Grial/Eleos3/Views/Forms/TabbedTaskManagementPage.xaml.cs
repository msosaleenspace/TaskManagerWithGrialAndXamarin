using System;
using Xamarin.Forms;
using UXDivers.Grial;
using Eleos3.ViewModels.DemoApp;
using Eleos3.Domain;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.Threading;
using Acr.UserDialogs;
using System.Numerics;

namespace Eleos3
{
    public partial class TabbedTaskManagementPage : ContentPage
    {

        private TabbedTaskManagementViewModel TabbedAddTaskViewModel { get; set; }

        private bool AddTaskInProcess = false;

        private bool UpdateTaskInProcess = false;

        private bool DeleteTaskInProcess = false;

        private List<TodoTaskDTO> Tasks = new List<TodoTaskDTO>();

        public ObservableCollection<TodoTaskDTO> TasksCollection { get; } = new ObservableCollection<TodoTaskDTO>();

        private TabbedTaskManagementPageObjects TabbedAddTaskPageObjects { get; set; }

        public TabbedTaskManagementPage()
        {
            InitializeComponent();

            this.TabbedAddTaskPageObjects = new TabbedTaskManagementPageObjects();

            this.TabbedAddTaskPageObjects.AddTaskInProcess = this.AddTaskInProcess;
            this.TabbedAddTaskPageObjects.AddTaskMessageLabel = this.AddTaskMessageLabel;
            this.TabbedAddTaskPageObjects.TaskNameEntry = this.TaskNameEntry;
            this.TabbedAddTaskPageObjects.TaskDatePicker = this.TaskDatePicker;

            this.TabbedAddTaskPageObjects.UpdateTaskMessageLabel = this.UpdateTaskMessageLabel;
            this.TabbedAddTaskPageObjects.UpdateTaskInProcess = this.UpdateTaskInProcess;
            this.TabbedAddTaskPageObjects.TaskIdEntryOnUpdate = this.TaskIdEntryOnUpdate;
            this.TabbedAddTaskPageObjects.TaskNameEntryOnUpdate = this.TaskNameEntryOnUpdate;
            this.TabbedAddTaskPageObjects.TaskDatePickerOnUpdate = this.TaskDatePickerOnUpdate;

            this.TabbedAddTaskPageObjects.DeleteTaskMessageLabel = this.DeleteTaskMessageLabel;
            this.TabbedAddTaskPageObjects.DeleteTaskInProcess = this.DeleteTaskInProcess;
            this.TabbedAddTaskPageObjects.TaskIdEntryOnDelete = this.TaskIdEntryOnDelete;

            this.TabbedAddTaskViewModel = new TabbedTaskManagementViewModel(this.TabbedAddTaskPageObjects);
        }

        private async void OnAddTaskBtnClicked(object sender, EventArgs e)
        {
            if (!this.AddTaskInProcess)
            {
                UserDialogs.Instance.ShowLoading("Wait please...");
                this.AddTaskInProcess = await this.TabbedAddTaskViewModel.AddTask();
                UserDialogs.Instance.HideLoading();
            }
        }

        private async void OnUpdateTaskBtnClicked(object sender, EventArgs e)
        {
            if (!this.UpdateTaskInProcess)
            {
                UserDialogs.Instance.ShowLoading("Wait please...");
                this.UpdateTaskInProcess = await this.TabbedAddTaskViewModel.UpdateTask();
                UserDialogs.Instance.HideLoading();
            }
        }

        private async void OnDeleteTaskBtnClicked(object sender, EventArgs e)
        {
            if (!this.DeleteTaskInProcess)
            {
                UserDialogs.Instance.ShowLoading("Wait please...");
                this.DeleteTaskInProcess = await this.TabbedAddTaskViewModel.DeleteTodoTask();
                UserDialogs.Instance.HideLoading();
            }
        }

        private async void OnTappedTabGetTasksEvent(object sender, EventArgs args)
        {
            this.TasksCollection.Clear();

            UserDialogs.Instance.ShowLoading("Wait please...");

            this.Tasks = await this.TabbedAddTaskViewModel.GetTasks();

            for (int index = 0; index < this.Tasks.Count; index++)
            {
                this.TasksCollection.Add(this.Tasks[index]);
            }

            BindingContext = this;

            UserDialogs.Instance.HideLoading();
        }

        private void OnTabChangingEvent(object sender, EventArgs e)
        {
            this.TaskNameEntry.Text = "";
            this.TaskDatePicker.Date = DateTime.Today;
            this.AddTaskMessageLabel.Text = "";

            this.TaskIdEntryOnUpdate.Text = "";
            this.TaskNameEntryOnUpdate.Text = "";
            this.TaskDatePickerOnUpdate.Date = DateTime.Today;
            this.UpdateTaskMessageLabel.Text = "";

            this.TaskIdEntryOnDelete.Text = "";
            this.DeleteTaskMessageLabel.Text = "";
        }

        private async void OnCloseButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync();
        }

    }

}