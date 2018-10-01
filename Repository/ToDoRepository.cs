using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

using Common;
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
        #endregion        
    }
}
