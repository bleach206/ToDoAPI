using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Common;
using Model;
using Model.Interface;
using Repository.Interface;

using Dapper;

namespace Repository
{
    public class ToDoRepository : IToDoRepository
    {
        #region Fields

        private readonly string _connection;
        #endregion

        #region Constructor

        public ToDoRepository(string connection) => _connection = connection;
        #endregion

        #region Methods

        public async Task<int> CreateToDo(ICreateDTO toDo)
        {
            try
            {
                var sql = "[dbo].[usp_InsertToDoList]";
                var tvp = new TabledValuedParameter(toDo.Tasks.CopyToDataTable(), "TaskTableType");
                using (var cnn = new SqlConnection(_connection))
                {
                    return await cnn.ExecuteScalarAsync<int>(sql, new { toDo.Name, toDo.Description, TVP = tvp }, commandType: CommandType.StoredProcedure);
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

        public async Task<IEnumerable<IToDoDTO>> GetAllList(string searchString = "", int skip = 1, int limit = 50)
        {
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {
                    var toDoDictionary = new Dictionary<int, ToDoDTO>();
                    var queryParameter = new DynamicParameters();
                    queryParameter.Add("@SearchTerm", value: searchString, size: 255);
                    queryParameter.Add("@PageNumber", dbType: DbType.Int32, value: skip);
                    queryParameter.Add("@PageSize", dbType: DbType.Int32, value: limit);

                    var data = await cnn.QueryAsync<ToDoDTO, TaskDTO, ToDoDTO>("[dbo].[usp_GetToDoBySearchTernAndPageNumberAndPageSize]", (todo, task) =>
                    {
                        ToDoDTO toDoDTO;
                        if (!toDoDictionary.TryGetValue(todo.Id, out toDoDTO))
                        {
                            toDoDTO = todo;
                            toDoDictionary.Add(todo.Id, toDoDTO);
                        }

                        if (toDoDTO.Tasks == null)
                        {
                            toDoDTO.Tasks = new List<TaskDTO>();
                        }
                        toDoDTO.Tasks.Add(task);

                        return toDoDTO;
                    },
                    splitOn: "ID",
                    param: queryParameter,
                    commandType: CommandType.StoredProcedure);
                    return data.Distinct().ToList();
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
