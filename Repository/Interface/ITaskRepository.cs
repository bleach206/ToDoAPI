using System.Collections.Generic;
using System.Threading.Tasks;

using Model.Interface;

namespace Repository.Interface
{
    public interface ITaskRepository
    {
        Task<IEnumerable<ITaskDTO>> GetTasksByPaging(int skip = 1, int limit = 50);
        Task<IEnumerable<ITaskDTO>> GetTasksByToDoId(int id, int skip = 1, int limit = 50);
        Task<ITaskDTO> GetTaskById(int id);
    }
}
