using System.Text.Json.Serialization;

namespace WorkToolChatGPTAPI.Model
{
    public class RequestViewModel
    {
        public RequestViewModel()
        {
            Spoken = string.Empty;
            RawSpoken = string.Empty;
            ReceivedName = string.Empty;
            GroupName = string.Empty;
            GroupRemark = string.Empty;
            RoomType = string.Empty;
            AtMe = true;
        }
        /// <summary>
        /// 问题文本
        /// </summary>
        [JsonPropertyName("spoken")]
        public string Spoken { get; set; }
        /// <summary>
        /// 原始问题文本
        /// </summary>
        [JsonPropertyName("rawSpoken")]
        public string RawSpoken { get; set; }
        /// <summary>
        /// 提问者名称
        /// </summary>
        [JsonPropertyName("receivedName")]
        public string ReceivedName { get; set; }
        /// <summary>
        /// QA所在群名（群聊）
        /// </summary>
        [JsonPropertyName("groupName")]
        public string GroupName { get; set; }
        /// <summary>
        /// QA所在群备注名（群聊）
        /// </summary>
        [JsonPropertyName("groupRemark")]
        public string GroupRemark { get; set; }
        /// <summary>
        /// QA所在房间类型 1=外部群 2=外部联系人 3=内部群 4=内部联系人
        /// </summary>
        [JsonPropertyName("roomType")]
        public string RoomType { get; set; }
        /// <summary>
        /// 是否@机器人（群聊）
        /// </summary>
        [JsonPropertyName("atMe")]
        public bool AtMe { get; set; }
        /// <summary>
        /// 消息类型 0=未知 1=文本 2=图片 5=视频 7=小程序 8=链接 9=文件
        /// </summary>
        [JsonPropertyName("textType")]
        public int TextType { get; set; }

    }
}
