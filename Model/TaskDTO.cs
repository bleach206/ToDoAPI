using Model.Interface;

namespace Model
{
    public class TaskDTO : ITaskDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
    }
}
