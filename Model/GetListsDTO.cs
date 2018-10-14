using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

using Model.Interface;

namespace Model
{
    public class GetListsDTO : IGetListsDTO
    {        
        /// <summary>
        /// number of records to skip for pagination
        /// </summary>
        [Range(1, int.MaxValue)]
        [FromQuery(Name = "skip")]
        public int Skip { get; set; } = 1;
        /// <summary>
        /// maximum number of records to return
        /// </summary>        
        [Range(1, 50)]
        [FromQuery(Name = "limit")]
        public int Limit { get; set; } = 50;        
        /// <summary>
        /// search string 
        /// </summary>
        [FromQuery(Name ="searchString")]
        public string SearchString { get; set; }
    }
}
