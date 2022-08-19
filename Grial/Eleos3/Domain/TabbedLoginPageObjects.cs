using System;
using Xamarin.Forms;

namespace Eleos3.Domain
{
	public class TabbedLoginPageObjects
	{

        public bool LoginInProcess { get; set; }

        public Label MessageLabelLogin { get; set; }

        public Entry EmailAddressEntryLogin { get; set; }

        public Entry PasswordEntryLogin { get; set; }

        public bool SignupInProcess { get; set; }

        public Label MessageLabelSignup { get; set; }

        public Entry EmailAddressEntrySignup { get; set; }

        public Entry PasswordEntrySignup { get; set; }

        public TabbedLoginPageObjects()
		{
		}

	}

}