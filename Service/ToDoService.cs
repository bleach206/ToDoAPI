using Service.Interface;
using System.Threading.Tasks;

using Model.Interface;
using Repository.Interface;

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
        #endregion
    }
}
