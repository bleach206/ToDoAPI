using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Model.Interface;

namespace Model
{
    /// <summary>
    /// Creation To do list DTO
    /// </summary>
    [Serializable]
    public class CreateDTO : ICreateDTO
    {
        /// <summary>
        /// Name of to do list
        /// </summary>
        [StringLength(255)]
        [Required(AllowEmptyStrings = false)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Name { get; set; }

        /// <summary>
        /// Optional description of to do list
        /// </summary>
        [StringLength(255)]
        public string Description { get; set; }

        /// <summary>
        /// To do list tasks
        /// </summary>
        [Required]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public IEnumerable<CreateTaskDTO> Tasks { get; set; }
    }
}
