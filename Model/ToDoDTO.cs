using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Model.Interface;

namespace Model
{
    public class ToDoDTO : IToDoDTO
    {        
        [DataMember(Name = "id")]
        [Range(1, int.MaxValue)]
        public int Id { get; set; }
        [DataMember(Name = "name")]
        [StringLength(maximumLength: 255)]
        public string Name { get; set; }
        [DataMember(Name = "description")]
        [StringLength(maximumLength: 255)]
        public string Description { get; set; }
        [DataMember(Name = "isCompleted")]
        public bool IsCompleted { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
