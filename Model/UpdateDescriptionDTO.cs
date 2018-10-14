using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using Model.Interface;

namespace Model
{
    public class UpdateDescriptionDTO : IUpdateDescriptionDTO
    {
        [DataMember(Name = "userId")]
        [Range(1, int.MaxValue)]
        [Required]
        public int UserId { get ; set ; }
        [DataMember(Name = "description")]
        [StringLength(maximumLength: 255)]
        [Required(AllowEmptyStrings = false)]
        public string Description { get ; set ; }
    }
}
