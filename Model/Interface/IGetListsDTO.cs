namespace Model.Interface
{
    public interface IGetListsDTO
    {        
        int Skip { get; set; }
        int Limit { get; set; }
        string SearchString { get; set; }
    }
}
