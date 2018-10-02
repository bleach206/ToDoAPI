using System.Collections.Generic;
using System.Threading.Tasks;

using Model.Interface;

namespace Repository.Interface
{
    public interface IToDoRepository
    {
        Task<int> CreateToDo(ICreateDTO toDo);
        Task<IEnumerable<IToDoDTO>> GetAllList(string searchString = "", int skip = 1, int limit = 50);
    }
}
