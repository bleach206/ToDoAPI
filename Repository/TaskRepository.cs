using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Model;
using Model.Interface;
using Repository.Interface;

using Dapper;

namespace Repository
{
    public class TaskRepository : ITaskRepository
    {
        #region Fields

        private readonly string _connection;
        #endregion

        #region Constructor
        public TaskRepository(string connection) => _connection = connection;

        #endregion

        #region Methods

        public async Task<IEnumerable<ITaskDTO>> GetTasksByPaging(int skip = 1, int limit = 50)
        {
            try
            {               
                using (var cnn = new SqlConnection(_connection))
                {
                    var queryParameter = new DynamicParameters();                
                    queryParameter.Add("@PageNumber", dbType: DbType.Int32, value: skip);
                    queryParameter.Add("@PageSize", dbType: DbType.Int32, value: limit);

                    return await cnn.QueryAsync<TaskDTO>("[dbo].[usp_GetTaskByPageNumberAndPageSize]", param: queryParameter, commandType: CommandType.StoredProcedure);
                }
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<ITaskDTO>> GetTasksByToDoId(int id, int skip = 1,int limit = 50)
        {
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {
                    var queryParameter = new DynamicParameters();
                    queryParameter.Add("@Id", dbType: DbType.Int32, value: id);
                    queryParameter.Add("@PageNumber", dbType: DbType.Int32, value: skip);
                    queryParameter.Add("@PageSize", dbType: DbType.Int32, value: limit);

                    return await cnn.QueryAsync<TaskDTO>("[dbo].[usp_GetTaskByToDoIdAndPageNumberAndPageSize]", param: queryParameter, commandType: CommandType.StoredProcedure);
                }
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ITaskDTO> GetTaskById(int id)
        {
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {
                    var queryParameter = new DynamicParameters();
                    queryParameter.Add("@Id", dbType: DbType.Int32, value: id);
                    var task = await cnn.QueryAsync<TaskDTO>("[dbo].[usp_GetTaskById]", param: queryParameter, commandType: CommandType.StoredProcedure);
                    return task.FirstOrDefault();
                }
            }
            catch (SqlException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
