using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Utility.Enums
{
    public enum LogType : short
    {
        /// <summary>
        /// 系统
        /// </summary>
        [Display(Name = "系统")]
        [Description("系统")]
        System = 0,
        /// <summary>
        /// 用户
        /// </summary>
        [Display(Name = "用户")]
        [Description("用户")]
        User = 1,
        /// <summary>
        /// 机器人
        /// </summary>
        [Display(Name = "机器人")]
        [Description("机器人")]
        Bot = 2,
    }
}
