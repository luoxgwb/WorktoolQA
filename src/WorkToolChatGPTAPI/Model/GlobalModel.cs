using OpenAI_API.Chat;

namespace WorkToolChatGPTAPI.Model
{
    public static class GlobalModel
    {
        public static Dictionary<string, Conversation> ConversationDic = new();
        public static int Count;
    }
}
