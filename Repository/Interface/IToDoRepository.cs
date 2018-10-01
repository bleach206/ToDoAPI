using System.Threading.Tasks;

using Model.Interface;

namespace Repository.Interface
{
    public interface IToDoRepository
    {
        Task<int> CreateToDo(ICreateDTO toDo);
    }
}
