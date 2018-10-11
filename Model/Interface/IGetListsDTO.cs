namespace Model.Interface
{
    public interface IGetListsDTO
    {
        int Id { get; set; }
        int Skip { get; set; }
        int Limit { get; set; }
    }
}
