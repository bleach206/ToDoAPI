using System.Collections.Generic;

namespace Model.Interface
{
    public interface IToDoDTO
    {
        int Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        IList<TaskDTO> Tasks { get; set; }     
    }
}
