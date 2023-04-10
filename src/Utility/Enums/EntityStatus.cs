using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Utility.Enums
{
    /// </summary>
    public enum EntityStatus : short
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Display(Name = "正常")]
        [Description("正常")]
        Normal = 1,
        /// <summary>
        /// 禁用
        /// </summary>
        [Display(Name = "禁用")]
        [Description("禁用")]
        Disable = 0,
        /// <summary>
        /// 删除
        /// </summary>
        [Display(Name = "删除")]
        [Description("删除")]
        Delete = -1
    }
}
