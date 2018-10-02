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

        public async Task<int> CreateToDo(ICreateDTO toDo)
        {
            return await _repository.CreateToDo(toDo);
        }

        public async Task<IEnumerable<IToDoDTO>> GetAllList(string searchString = "", int skip = 1, int limit = 50)
        {
            return await _repository.GetAllList(searchString, skip, limit);
        }
        #endregion
    }
}
