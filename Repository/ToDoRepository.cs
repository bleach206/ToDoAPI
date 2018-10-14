using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using Common;
using Model;
using Model.Interface;
using Repository.Interface;

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

        public async Task<int> CreateToDo(int userId, ICreateDTO toDo)
        {
            try
            {
                var sql = "[dbo].[usp_InsertToDo]";       
                using (var cnn = new SqlConnection(_connection))
                {
                    return await cnn.ExecuteScalarAsync<int>(sql, new { UserId = userId, toDo.Name, toDo.Description }, commandType: CommandType.StoredProcedure);
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

        public async Task<int> CreateToDoList(int userId, IEnumerable<ICreateDTO> toDoLists)
        {
            try
            {
                var sql = "[dbo].[usp_InsertToDoList]";
                var tdvp = new TabledValuedParameter(toDoLists.CopyToDataTable(), "ToDoTableType");
                using (var cnn = new SqlConnection(_connection))
                {
                    return await cnn.ExecuteScalarAsync<int>(sql, new { UserId = userId, TDVP = tdvp }, commandType: CommandType.StoredProcedure);
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

        public async Task<ToDoDTO> GetToDoById(int id, int toDoId)
        {
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {                  
                    var queryParameter = new DynamicParameters();                   
                    queryParameter.Add("@Id", dbType: DbType.Int32, value: id);
                    queryParameter.Add("@ToDoId", dbType: DbType.Int32, value: toDoId);

                    var data = await cnn.QueryAsync<ToDoDTO>("[dbo].[usp_GetToDoById]", 
                    param: queryParameter,
                    commandType: CommandType.StoredProcedure);
                    return data.FirstOrDefault();
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

        public async Task<IEnumerable<ToDoDTO>> GetToDoByPaging(int userId, int skip = 1, int limit = 50, string searchString = "")
        {
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {                  
                    var queryParameter = new DynamicParameters();
                    queryParameter.Add("@UserId", dbType: DbType.Int32, value: userId);
                    queryParameter.Add("@SearchTerm", value: searchString, size: 255);
                    queryParameter.Add("@PageNumber", dbType: DbType.Int32, value: skip);
                    queryParameter.Add("@PageSize", dbType: DbType.Int32, value: limit);

                    var data = await cnn.QueryAsync<ToDoDTO>("[dbo].[usp_GetToDoBySearchTermAndPageNumberAndPageSize]",            
                    param: queryParameter,
                    commandType: CommandType.StoredProcedure);
                    return data;
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

        public async Task<bool> UpdateToDo(int id, IToDoUpdateDTO toDo)
        {
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {                   
                    var queryParameter = new DynamicParameters();
                    queryParameter.Add("@UserId", dbType:DbType.Int32, value: toDo.UserId);
                    queryParameter.Add("@Id", dbType: DbType.Int32, value: id);
                    queryParameter.Add("@Name", value: toDo.Name, size: 255);
                    queryParameter.Add("@Description", value: toDo.Description, size: 255);
                    var result = await cnn.ExecuteAsync("[dbo].[usp_UpdateToDo]", param: queryParameter, commandType: CommandType.StoredProcedure);
                    return Convert.ToBoolean(result);
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

        public async Task<bool> UpdateToDoDescription(int id, IUpdateDescriptionDTO updateDescriptionDTO)
        {
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {                 
                    var queryParameter = new DynamicParameters();
                    queryParameter.Add("@Id", dbType: DbType.Int32, value: id);
                    queryParameter.Add("@UserId", dbType: DbType.Int32, value: updateDescriptionDTO.UserId);
                    queryParameter.Add("@Description", value: updateDescriptionDTO.Description, size: 255);
                    var result = await cnn.ExecuteAsync("[dbo].[usp_UpdateToDoDescription]", param: queryParameter, commandType: CommandType.StoredProcedure);
                    return Convert.ToBoolean(result);
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

        public async Task<bool> UpdateToDoName(int id, IUpdateNameDTO updateNameDTO)
        {
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {                   
                    var queryParameter = new DynamicParameters();
                    queryParameter.Add("@Id", dbType: DbType.Int32, value: id);
                    queryParameter.Add("@UserId", dbType: DbType.Int32, value: updateNameDTO.UserId);
                    queryParameter.Add("@Name", value: updateNameDTO.Name, size: 255);
                    var result = await cnn.ExecuteAsync("[dbo].[usp_UpdateToDoNam]", param: queryParameter, commandType: CommandType.StoredProcedure);
                    return Convert.ToBoolean(result);
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

        public async Task<bool> Delete(int id, int userId)
        {
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {
                    var queryParameter = new DynamicParameters();
                    queryParameter.Add("@Id", dbType: DbType.Int32, value: id);
                    queryParameter.Add("@UserId", dbType: DbType.Int32, value: userId);                   
                    var result = await cnn.ExecuteAsync("[dbo].[usp_DeleteToDo]", param: queryParameter, commandType: CommandType.StoredProcedure);
                    return Convert.ToBoolean(result);
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
