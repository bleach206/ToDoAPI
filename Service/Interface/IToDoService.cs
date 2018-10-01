using System.Threading.Tasks;

using Model.Interface;

namespace Service.Interface
{
    public interface IToDoService
    {
        Task<int> CreateToDo(ICreateDTO toDo);
    }
}
