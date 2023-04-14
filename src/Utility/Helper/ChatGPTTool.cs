using Dto;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OpenAI_API;
using OpenAI_API.Chat;
using Utility.Enums;
using Utility.Models;

namespace Utility.Helper
{
    public class ChatGPTTool
    {

        private readonly ChatGPTSettings _openAiSettings;
        public ChatGPTTool(IOptionsSnapshot<ChatGPTSettings> chatGPTSettings)
        {
            _openAiSettings = chatGPTSettings.Value;
        }

        public async Task<string> GetGPTMessageAsync(string message, List<MessageList> messageLists)
        {
            var key = _openAiSettings.Key;
            var orginId = _openAiSettings.OrginId;
            var messageCount = _openAiSettings.MessageCount;

            var api = new OpenAIAPI(new APIAuthentication(key, orginId));

            var chat = api.Chat.CreateConversation();

            //因为This model's maximum context length is 4097 tokens.
            //所以只保留第一次对话和最后四次对话
            var newMessageLists = new List<MessageList>();
            newMessageLists.AddRange(messageLists.Skip(0).Take(2));
            newMessageLists.AddRange(messageLists.Skip(Math.Max(0, messageLists.Count - messageCount * 2)).Take(messageCount * 2));

            foreach (var item in newMessageLists)
            {
                chat.AppendMessage(new ChatMessage
                {
                    Content = item.Message,
                    Role = ChatMessageRole.FromString(item.MessageType),
                });
            }
            chat.AppendMessage(new ChatMessage
            {
                Content = message,
                Role = ChatMessageRole.User,
            });

            return await chat.GetResponseFromChatbotAsync();
        }

    }
}
