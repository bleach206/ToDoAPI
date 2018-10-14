using System.Collections.Generic;
using System.Threading.Tasks;

using Model;
using Model.Interface;

namespace Repository.Interface
{
    public interface IToDoRepository
    {
        Task<int> CreateToDo(int userId, ICreateDTO toDo);
        Task<int> CreateToDoList(int userId, IEnumerable<ICreateDTO> toDoLists);
        Task<ToDoDTO> GetToDoById(int id, int toDoId);
        Task<IEnumerable<ToDoDTO>> GetToDoByPaging(int userId, int skip = 1, int limit = 50, string searchString = "");
        Task<bool> UpdateToDoName(int id, IUpdateNameDTO updateNameDTO);
        Task<bool> UpdateToDoDescription(int id, IUpdateDescriptionDTO updateDescriptionDTO);
        Task<bool> UpdateToDo(int id, IToDoUpdateDTO toDo);
        Task<bool> Delete(int id, int userId);
    }
}
