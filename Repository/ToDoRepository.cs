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
                var sql = "[dbo].[usp_InsertToDo]";       
                using (var cnn = new SqlConnection(_connection))
                {
                    return await cnn.ExecuteScalarAsync<int>(sql, new { toDo.Name, toDo.Description }, commandType: CommandType.StoredProcedure);
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

        public async Task<IToDoDTO> GetToDoById(int id)
        {
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {
                    var toDoDictionary = new Dictionary<int, ToDoDTO>();
                    var queryParameter = new DynamicParameters();                   
                    queryParameter.Add("@Id", dbType: DbType.Int32, value: id);

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

        public async Task<IEnumerable<IToDoDTO>> GetToDoByPaging(int userId, int skip = 1, int limit = 50, string searchString = "")
        {
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {
                    var toDoDictionary = new Dictionary<int, ToDoDTO>();
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

        public async Task<bool> UpdateToDo(IToDoDTO toDo)
        {
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {
                    var toDoDictionary = new Dictionary<int, ToDoDTO>();
                    var queryParameter = new DynamicParameters();
                    queryParameter.Add("@Id", dbType: DbType.Int32, value: toDo.Id);
                    queryParameter.Add("@Name", value: toDo.Name, size: 255);
                    queryParameter.Add("@Description", value: toDo.Description, size: 255);
                    var result = await cnn.ExecuteAsync("[dbo].[usp_UpdateToDo]", param: queryParameter, commandType: CommandType.StoredProcedure);
                    return Convert.ToBoolean(result.ToString());
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

        public async Task<bool> UpdateToDoDescription(int id, string description)
        {
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {
                    var toDoDictionary = new Dictionary<int, ToDoDTO>();
                    var queryParameter = new DynamicParameters();
                    queryParameter.Add("@Id", dbType: DbType.Int32, value: id);                  
                    queryParameter.Add("@Description", value: description, size: 255);
                    var result = await cnn.ExecuteAsync("[dbo].[usp_UpdateToDoDescription]", param: queryParameter, commandType: CommandType.StoredProcedure);
                    return Convert.ToBoolean(result.ToString());
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

        public async Task<bool> UpdateToDoName(int id, string name)
        {
            try
            {
                using (var cnn = new SqlConnection(_connection))
                {
                    var toDoDictionary = new Dictionary<int, ToDoDTO>();
                    var queryParameter = new DynamicParameters();
                    queryParameter.Add("@Id", dbType: DbType.Int32, value: id);
                    queryParameter.Add("@Name", value: name, size: 255);
                    var result = await cnn.ExecuteAsync("[dbo].[usp_UpdateToDonName]", param: queryParameter, commandType: CommandType.StoredProcedure);
                    return Convert.ToBoolean(result.ToString());
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
