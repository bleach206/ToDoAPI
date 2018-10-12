namespace Model.Interface
{
    public interface IToDoDTO : ICacheType
    {
        int Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }       
    }
}
