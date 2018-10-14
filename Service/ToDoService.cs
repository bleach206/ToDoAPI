using System.Collections.Generic;
using System.Threading.Tasks;

using Model;
using Model.Interface;
using Repository.Interface;
using Service.Interface;

namespace Service
{
    public class ToDoService : IToDoService
    {
        #region Fields
        private readonly IToDoRepository _repository;
        #endregion

        #region Constructor
        public ToDoService(IToDoRepository repository) => _repository = repository;
        #endregion

        #region Method
        public async Task<int> CreateToDo(int userId, ICreateDTO toDo) => await _repository.CreateToDo(userId, toDo);
        public async Task<ToDoDTO> GetToDoById(int id, int toDoId) => await _repository.GetToDoById(id, toDoId);
        public async Task<int> CreateToDoList(int userId, IEnumerable<ICreateDTO> toDoLists) => await _repository.CreateToDoList(userId, toDoLists);
        public async Task<IEnumerable<ToDoDTO>> GetToDoByPaging(int userId, int skip = 1, int limit = 50, string searchString = "") => await _repository.GetToDoByPaging(userId, skip, limit, searchString);
        public async Task<bool> UpdateToDo(int id, IToDoUpdateDTO toDo) => await _repository.UpdateToDo(id, toDo);
        public async Task<bool> UpdateToDoDescription(int id, IUpdateDescriptionDTO updateDescriptionDTO) => await _repository.UpdateToDoDescription(id, updateDescriptionDTO);
        public async Task<bool> UpdateToDoName(int id, IUpdateNameDTO updateNameDTO) => await _repository.UpdateToDoName(id, updateNameDTO);
        public async Task<bool> Delete(int id, int userId) => await _repository.Delete(id, userId);       
        #endregion
    }
}
