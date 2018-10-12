using System.Collections.Generic;
using System.Threading.Tasks;

using Model;
using Model.Interface;

namespace Repository.Interface
{
    public interface IToDoRepository
    {
        Task<int> CreateToDo(ICreateDTO toDo);        
        Task<IToDoDTO> GetToDoById(int id);
        Task<IEnumerable<ToDoDTO>> GetToDoByPaging(int userId, int skip = 1, int limit = 50, string searchString = "");
        Task<bool> UpdateToDoName(int id, string name);
        Task<bool> UpdateToDoDescription(int id, string description);
        Task<bool> UpdateToDo(IToDoDTO toDo);
    }
}
