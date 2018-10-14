using System.Collections.Generic;
using System.Threading.Tasks;

using Model;
using Model.Interface;

namespace Service.Interface
{
    public interface IToDoService
    {
        Task<int> CreateToDo(int userId, ICreateDTO toDo);
        Task<ToDoDTO> GetToDoById(int id, int toDoId);
        Task<int> CreateToDoList(int userId, IEnumerable<ICreateDTO> toDoLists);
        Task<IEnumerable<ToDoDTO>> GetToDoByPaging(int userId, int skip = 1, int limit = 50, string searchString = "");
        Task<bool> UpdateToDoName(int id, IUpdateNameDTO updateNameDTO);
        Task<bool> UpdateToDoDescription(int id, IUpdateDescriptionDTO updateDescriptionDTO);
        Task<bool> UpdateToDo(int id, IToDoUpdateDTO toDo);
        Task<bool> Delete(int id, int userId);
    }
}
