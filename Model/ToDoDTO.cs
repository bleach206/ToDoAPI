using System.Collections.Generic;

using Model.Interface;

namespace Model
{
    public class ToDoDTO : IToDoDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }      
        public IList<TaskDTO> Tasks { get; set; }
    }
}
