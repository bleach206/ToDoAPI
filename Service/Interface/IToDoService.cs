using System.Collections.Generic;
using System.Threading.Tasks;

using Model;
using Model.Interface;

namespace Service.Interface
{
    public interface IToDoService
    {
        Task<int> CreateToDo(ICreateDTO toDo);
        Task<IToDoDTO> GetToDoById(int id);
        Task<IEnumerable<ToDoDTO>> GetToDoByPaging(int userId, int skip = 1, int limit = 50, string searchString = "");
        Task<bool> UpdateToDoName(int id, string name);
        Task<bool> UpdateToDoDescription(int id, string description);
        Task<bool> UpdateToDo(IToDoDTO toDo);
    }
}
