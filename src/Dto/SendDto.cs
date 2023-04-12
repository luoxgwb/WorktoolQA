using System.Text.Json.Serialization;

namespace Dto
{
    public class SendDto
    {
        public SendDto()
        {
            List = new List<List>();
        }
        /// <summary>
        /// 通讯类型 固定值=2
        /// </summary>
        [JsonPropertyName("socketType")]
        public long SocketType { get; set; } = 2;
        [JsonPropertyName("list")]
        public List<List> List { get; set; }

    }
    public class List
    {
        public List()
        {
            ReceivedContent = string.Empty;
            TitleList = new List<string>();
            AtList = new List<string>();
        }
        /// <summary>
        /// 消息类型 固定值=203
        /// </summary>
        [JsonPropertyName("type")]
        public long Type { get; set; } = 203;
        /// <summary>
        /// 昵称或群名
        /// </summary>
        [JsonPropertyName("titleList")]
        public List<string> TitleList { get; set; }
        /// <summary>
        /// 发送文本内容 (\n换行)
        /// </summary>
        [JsonPropertyName("receivedContent")]
        public string ReceivedContent { get; set; }
        /// <summary>
        /// at的人(at所有人用"@所有人")
        /// </summary>
        [JsonPropertyName("atList")]
        public List<string> AtList { get; set; }
    }
}
