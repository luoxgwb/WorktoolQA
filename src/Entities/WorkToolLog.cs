using Utility.Enums;

namespace Entities
{
    public class WorkToolLog : BaseEntity<int>
    {
        public WorkToolLog()
        {
            Message = string.Empty;
        }
        /// <summary>
        /// 一次操作的id
        /// </summary>
        public Guid OperationID { get; set; }
        public string Message { get; set; }
        /// <summary>
        /// 提问者名称
        /// </summary>
        public string ReceivedName { get; set; }
        /// <summary>
        /// QA所在群名（群聊）
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// QA所在群备注名（群聊）
        /// </summary>
        public string GroupRemark { get; set; }
        /// <summary>
        /// 0:系统消息 1:用户消息 2:机器人消息 
        /// </summary>
        public LogType Type { get; set; }
    }
}
