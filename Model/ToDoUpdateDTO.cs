using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Model.Interface;

namespace Model
{   
    [Serializable]
    public class ToDoUpdateDTO : IToDoUpdateDTO
    {       
        [DataMember(Name = "userId")]
        [Range(1, int.MaxValue)]
        [Required]
        public int UserId { get; set; }
        [DataMember(Name = "name")]
        [StringLength(maximumLength: 255)]
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
        [DataMember(Name = "description")]
        [StringLength(maximumLength: 255)]
        public string Description { get; set; }     
    }
}
