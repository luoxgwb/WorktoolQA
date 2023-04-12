namespace Utility.Models
{
    public class ChatGPTSettings
    {
        public ChatGPTSettings()
        {
            Key = string.Empty;
            OrginId = string.Empty;
            RobotId = string.Empty;
        }
        /// <summary>
        /// OpenAI Key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 机构ID
        /// </summary>
        public string OrginId { get; set; }
        /// <summary>
        /// 机器人id
        /// </summary>
        public string RobotId { get; set; }
        /// <summary>
        /// 上下文在缓存中保存时间，单位秒
        /// </summary>
        public int CacheSecound { get; set; }
        /// <summary>
        /// 最大回复Tokens
        /// </summary>
        public int MaxTokens { get; set; }
        /// <summary>
        /// 使用机器人次数
        /// </summary>
        public int MaxCount { get; set; }
        /// <summary>
        /// 机器人次数id
        /// </summary>
        public int CountId { get; set; }

    }
}
