using System.Collections.Generic;

namespace Model.Interface
{
    public interface ICreateDTO
    {
        string Name { get; set; }
        string Description { get; set; }
        IEnumerable<CreateTaskDTO> Tasks { get; set; }
    }
}
