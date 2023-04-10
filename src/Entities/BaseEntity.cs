using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utility.Enums;

namespace Entities
{
    public class BaseEntity<T>
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public T Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        public DateTime CreatedTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 数据状态
        /// </summary>
        [Required]
        public EntityStatus State { get; set; } = EntityStatus.Normal;
    }
}
