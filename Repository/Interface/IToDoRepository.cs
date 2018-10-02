using System.Collections.Generic;
using System.Threading.Tasks;

using Model.Interface;

namespace Repository.Interface
{
    public interface IToDoRepository
    {
        Task<int> CreateToDo(ICreateDTO toDo);        
        Task<IToDoDTO> GetToDoById(int id);
        Task<IEnumerable<IToDoDTO>> GetToDoByPaging(string searchString = "", int skip = 1, int limit = 50);
        Task<bool> UpdateToDoName(int id, string name);
        Task<bool> UpdateToDoDescription(int id, string description);
        Task<bool> UpdateToDo(IToDoDTO toDo);
    }
}
