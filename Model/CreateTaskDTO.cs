using System;
using System.ComponentModel.DataAnnotations;

using Model.Interface;

namespace Model
{
    [Serializable]
    public class CreateTaskDTO : ICreateTaskDTO
    {
        /// <summary>
        /// Task name
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(maximumLength: 255)]
        public string Name { get; set; }

        /// <summary>
        /// Is task completed?
        /// </summary>
        [Required]
        public bool IsCompleted { get; set; }
    }
}
