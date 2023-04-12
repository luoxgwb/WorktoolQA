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
        private readonly IMemoryCache _memoryCache;
        private readonly ChatGPTSettings _chatGPTSettings;
        public ChatGPTTool(IMemoryCache memoryCache, IOptionsSnapshot<ChatGPTSettings> chatGPTSettings)
        {
            _memoryCache = memoryCache;
            _chatGPTSettings = chatGPTSettings.Value;
        }

        public async Task<ResultDto> GetGPTMessageAsync(RequestDto requestDto)
        {
            var key = _chatGPTSettings.Key;
            var orginId = _chatGPTSettings.OrginId;
            var maxTokens = _chatGPTSettings.MaxTokens;
            var cacheSecound = _chatGPTSettings.CacheSecound;

            var res = new ResultDto();

            var conversationKey = requestDto.ReceivedName + requestDto.GroupName + requestDto.GroupRemark;
            var cacheKey = $"conversation_{conversationKey}";

            var chatRequest = new ChatRequest
            {
                MaxTokens = Convert.ToInt32(maxTokens)
            };

            OpenAIAPI api = new OpenAIAPI(new APIAuthentication(key, orginId));
            {
                Conversation chat;

                if (_memoryCache.TryGetValue(cacheKey, out Conversation conversation))
                {
                    chat = conversation;
                }
                else
                {
                    chat = api.Chat.CreateConversation(chatRequest);
                    _memoryCache.Set(cacheKey, chat, TimeSpan.FromSeconds(cacheSecound));
                }

                chat.AppendUserInput(requestDto.Spoken);


                int retries = 3;
                //重试时间0.5s
                int delayInMilliseconds = 500;

                while (retries > 0)
                {
                    try
                    {
                        res = new ResultDto();
                        res.Data.Info.Text = await chat.GetResponseFromChatbotAsync();
                        break;
                    }
                    catch (Exception ex)
                    {
                        retries--;
                        res.Code = 500;
                        res.Message = ex.Message;
                        await Task.Delay(delayInMilliseconds);
                    }
                }
            }

            return res;
        }

    }
}
