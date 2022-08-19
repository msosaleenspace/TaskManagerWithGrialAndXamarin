using System;
namespace Eleos3.Domain
{
	public class SessionDTO
	{
        public string token { get; set; }

        public int userId { get; set; }

        public SessionDTO()
        {
            this.token = "";
        }
    }
}

