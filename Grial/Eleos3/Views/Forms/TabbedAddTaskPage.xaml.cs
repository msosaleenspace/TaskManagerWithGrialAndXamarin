using System;
using Xamarin.Forms;
using UXDivers.Grial;
using Eleos3.ViewModels.DemoApp;

namespace Eleos3
{
    public partial class TabbedAddTaskPage : ContentPage
    {

        public TabbedAddTaskViewModel TabbedAddTaskViewModel { get; set; }

        private bool LogoutInProcess = false;

        private bool AddTaskInProcess = false;

        public TabbedAddTaskPage()
        {
            InitializeComponent();
            this.TabbedAddTaskViewModel = new TabbedAddTaskViewModel(this.LogoutMessageLabel,
                this.LogoutInProcess,
                this.AddTaskMessageLabel,
                this.AddTaskInProcess,
                this.TaskNameEntry,
                this.TaskDatePicker);
        }

        private async void OnCloseButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PopModalAsync();
        }

        private async void OnAddTaskBtnClicked(object sender, EventArgs e)
        {
            if (!this.AddTaskInProcess)
            {
                this.AddTaskInProcess = await this.TabbedAddTaskViewModel.AddTask();
            }

        }

        private async void OnDeleteTaskBtnClicked(object sender, EventArgs e)
        {
            if (!this.LogoutInProcess)
            {
                this.LogoutInProcess = await this.TabbedAddTaskViewModel.Logout();
            }

        }

    }

}