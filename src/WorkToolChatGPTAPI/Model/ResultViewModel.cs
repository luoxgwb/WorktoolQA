using System.Text.Json.Serialization;

namespace WorkToolChatGPTAPI.Model
{

    public class ResultViewModel
    {
        public ResultViewModel()
        {
            Data = new Data();
        }
        /// <summary>
        /// 0 调用成功 -1或其他值 调用失败并回复message
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; } = 0;
        /// <summary>
        /// 对本次接口调用的信息描述
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = "success";
        /// <summary>
        /// 返回数据
        /// </summary>
        [JsonPropertyName("data")]
        public Data Data { get; set; }
    }

    public class Data
    {
        public Data()
        {
            Info = new Info();
        }

        /// <summary>
        /// 5000 回答类型为文本
        /// </summary>
        [JsonPropertyName("type")]
        public int Type { get; set; } = 5000;
        /// <summary>
        /// 回答结果集合
        /// </summary>
        [JsonPropertyName("info")]
        public Info Info { get; set; }

    }

    public class Info
    {
        public Info()
        {
            Text = string.Empty;
        }
        /// <summary>
        /// 回答文本(您期望的回复内容) \n可换行 空字符串则不回复
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }

    }


}
