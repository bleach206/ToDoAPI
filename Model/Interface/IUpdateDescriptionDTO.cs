namespace Model.Interface
{
    public interface IUpdateDescriptionDTO
    {
        int UserId { get; set; }
        string Description { get; set; }
    }
}
