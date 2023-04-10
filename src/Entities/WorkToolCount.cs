using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class WorkToolCount : BaseEntity<int>
    {
        [Required]
        public int CountId { get; set; }
        [Required]
        public int Count { get; set; }
    }
}
