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
    public partial class TabbedAddTaskPage : ContentPage
    {

        public TabbedAddTaskViewModel TabbedAddTaskViewModel { get; set; }

        private bool AddTaskInProcess = false;

        private bool UpdateTaskInProcess = false;

        public List<TodoTaskDTO> Tasks { get; set; }

        public ObservableCollection<TodoTaskDTO> List { get; } = new ObservableCollection<TodoTaskDTO>();

        public TabbedAddTaskPage()
        {
            InitializeComponent();
            this.TabbedAddTaskViewModel = new TabbedAddTaskViewModel(
                this.AddTaskMessageLabel,
                this.AddTaskInProcess,
                this.TaskNameEntry,
                this.TaskDatePicker,

                this.UpdateTaskMessageLabel,
                this.UpdateTaskInProcess,
                this.TaskIdEntryOnUpdate,
                this.TaskNameEntryOnUpdate,
                this.TaskDatePickerOnUpdate);
        }

        private async void OnCloseButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync();
        }

        private async void OnTappedTabGetTasksEvent(object sender, EventArgs args)
        {
            this.List.Clear();

            UserDialogs.Instance.ShowLoading("Wait please...");

            this.Tasks = await this.TabbedAddTaskViewModel.GetTasks();

            //TasksListView.ItemsSource = this.Tasks;

            for (int index = 0; index < this.Tasks.Count; index++)
            {
                this.List.Add(this.Tasks[index]);
            }

            BindingContext = this;

            UserDialogs.Instance.HideLoading();
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

        private void OnTabChangingEvent(object sender, EventArgs e)
        {
            this.TaskNameEntry.Text = "";
            this.TaskDatePicker.Date = DateTime.Today;
            this.AddTaskMessageLabel.Text = "";

            this.TaskIdEntryOnUpdate.Text = "";
            this.TaskNameEntryOnUpdate.Text = "";
            this.TaskDatePickerOnUpdate.Date = DateTime.Today;
            this.UpdateTaskMessageLabel.Text = "";
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
    }

}