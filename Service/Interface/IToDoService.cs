using System.Collections.Generic;
using System.Threading.Tasks;

using Model.Interface;

namespace Service.Interface
{
    public interface IToDoService
    {
        Task<int> CreateToDo(ICreateDTO toDo);
        Task<IToDoDTO> GetToDoById(int id);
        Task<IEnumerable<IToDoDTO>> GetToDoByPaging(string searchString = "", int skip = 1, int limit = 50);
    }
}
