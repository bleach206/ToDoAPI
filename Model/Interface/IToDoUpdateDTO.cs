namespace Model.Interface
{
    public interface IToDoUpdateDTO
    {      
        int UserId { get; set; }
        string Name { get; set; }
        string Description { get; set; }   
    }
}
