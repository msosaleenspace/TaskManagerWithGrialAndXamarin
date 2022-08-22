using System;
using Xamarin.Forms;

namespace Eleos3.Domain
{
	public class TabbedTaskManagementPageObjects
	{

        public bool AddTaskInProcess { get; set; }

        public Label AddTaskMessageLabel { get; set; }

        public Entry TaskNameEntry { get; set; }

        public DatePicker TaskDatePicker { get; set; }

        public bool UpdateTaskInProcess { get; set; }

        public Label UpdateTaskMessageLabel { get; set; }

        public Entry TaskIdEntryOnUpdate { get; set; }

        public Entry TaskNameEntryOnUpdate { get; set; }

        public DatePicker TaskDatePickerOnUpdate { get; set; }

        public bool DeleteTaskInProcess { get; set; }

        public Label DeleteTaskMessageLabel { get; set; }

        public Entry TaskIdEntryOnDelete { get; set; }

        public TabbedTaskManagementPageObjects()
		{
		}

	}

}