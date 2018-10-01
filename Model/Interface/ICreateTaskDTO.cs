namespace Model.Interface
{
    public interface ICreateTaskDTO
    {
        string Name { get; set; }
        bool IsCompleted { get; set; }
    }
}
