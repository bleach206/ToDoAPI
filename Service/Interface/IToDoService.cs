using System.Collections.Generic;
using System.Threading.Tasks;

using Model.Interface;

namespace Service.Interface
{
    public interface IToDoService
    {
        Task<int> CreateToDo(ICreateDTO toDo);
        Task<IEnumerable<IToDoDTO>> GetAllList(string searchString = "", int skip = 1, int limit = 50);
    }
}
