using System.Collections.Generic;
using System.Threading.Tasks;

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

        public async Task<int> CreateToDo(ICreateDTO toDo) => await _repository.CreateToDo(toDo);
        public async Task<IToDoDTO> GetToDoById(int id) => await _repository.GetToDoById(id);
        public async Task<IEnumerable<IToDoDTO>> GetToDoByPaging(string searchString = "", int skip = 1, int limit = 50) => await _repository.GetToDoByPaging(searchString, skip, limit);
        #endregion
    }
}
