using System;
namespace Eleos3.Domain
{
    public class TodoTaskDTO
    {

        public int id { get; set; }

        public string name { get; set; }

        public DateTime date { get; set; }

        public UserDTO user { get; set; }

        public string todoTaskData { get { return this.id + " - " + this.name.ToString(); } }

        public TodoTaskDTO()
        {

        }

    }

}