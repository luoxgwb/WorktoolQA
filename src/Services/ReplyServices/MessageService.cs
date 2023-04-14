using Dto;
using Services.IReplyServices;
using System.Text.Json;
using Utility.Dependencies;
using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using Utility.Enums;
using System.Net.Http.Headers;
using Utility.Helper;
using Utility.Models;
using Microsoft.Extensions.Options;
using OpenAI_API.Chat;
using System;
using Microsoft.Extensions.Caching.Memory;

namespace Services.ReplyServices
{
    public class MessageService : IMessageService, IScoped
    {
        private readonly SqlDbContext _context;
        private readonly ILogger<MessageService> _logger;
        private readonly ChatGPTTool _chatGPTTool;
        private readonly ChatGPTSettings _chatGPTSettings;
        private readonly IMemoryCache _memoryCache;

        public MessageService(SqlDbContext context, ILogger<MessageService> logger, ChatGPTTool chatGPTTool, IOptionsSnapshot<ChatGPTSettings> chatGPTSettings, IMemoryCache memoryCache)
        {
            _context = context;
            _logger = logger;
            _chatGPTTool = chatGPTTool;
            _chatGPTSettings = chatGPTSettings.Value;
            _memoryCache = memoryCache;
        }

        public async Task<ResultDto> SendMessageAsync(RequestDto requestDto)
        {
            var operationID = Guid.NewGuid();

            AddLog(operationID, LogType.User, requestDto, requestDto.Spoken);

            var res = new ResultDto();
            var maxCount = _chatGPTSettings.MaxCount;
            var countId = _chatGPTSettings.CountId;

            var workToolCountmodel = new WorkToolCount();

            var model = await _context.WorkToolCount.Where(x => x.CountId == countId).FirstOrDefaultAsync();

            if (model != null)
            {
                workToolCountmodel = model;
            }
            else
            {
                var workToolCount = new WorkToolCount
                {
                    CountId = countId,
                    Count = Convert.ToInt32(maxCount)
                };
                await _context.AddAsync(workToolCount);

                workToolCountmodel = workToolCount;
            }


            //敏感词
            var search = IllegalWordHelper.GetIllegalWordsSearch();
            var spokenReplace = search.Replace(requestDto.Spoken);
            if (spokenReplace != requestDto.Spoken)
            {
                res.Data.Info.Text = $"\nINFO:\n发言中包含敏感词，请重试:\n{spokenReplace}";

                AddLog(operationID, LogType.System, requestDto, res.Data.Info.Text);

                await _context.SaveChangesAsync();
                await SendRawMessage(requestDto, res);
                return new ResultDto();
            }

            //次数查询
            if (requestDto.Spoken == "剩余次数")
            {
                res.Data.Info.Text = $"\nINFO:\n剩余{workToolCountmodel.Count}次";

                AddLog(operationID, LogType.System, requestDto, res.Data.Info.Text);

                await _context.SaveChangesAsync();
                await SendRawMessage(requestDto, res);
                return new ResultDto();
            }

            //已达上限
            if (workToolCountmodel.Count <= 0)
            {
                res.Data.Info.Text = $"\nINFO:\n次数已达上限";

                AddLog(operationID, LogType.System, requestDto, res.Data.Info.Text);

                await _context.SaveChangesAsync();
                await SendRawMessage(requestDto, res);
                return new ResultDto();
            }

            var messageLists = new List<MessageList>();

            var cacheKey = $"{requestDto.ReceivedName}_{requestDto.GroupName}_{requestDto.GroupRemark}";
            if (_memoryCache.TryGetValue(cacheKey, out List<MessageList> newMessageLists))
            {
                messageLists = newMessageLists;
            }

            var gptMessage = await _chatGPTTool.GetGPTMessageAsync(requestDto.Spoken, messageLists);

            //添加缓存
            messageLists.Add(new MessageList() { MessageType = ChatMessageRole.User, Message = requestDto.Spoken });
            messageLists.Add(new MessageList() { MessageType = ChatMessageRole.Assistant, Message = gptMessage });
            _memoryCache.Set(cacheKey, messageLists, TimeSpan.FromSeconds(_chatGPTSettings.CacheSecound));

            workToolCountmodel.Count--;

            res.Data.Info.Text = $"\n{gptMessage}";

            AddLog(operationID, LogType.Bot, requestDto, gptMessage);

            await _context.SaveChangesAsync();
            await SendRawMessage(requestDto, res);
            return new ResultDto();

        }


        private void AddLog(Guid operationID, LogType LogType, RequestDto requestDto, string message)
        {
            var workToolLog = new WorkToolLog
            {
                OperationID = operationID,
                Type = LogType,
                ReceivedName = requestDto.ReceivedName,
                GroupName = requestDto.GroupName,
                GroupRemark = requestDto.GroupRemark,
                Message = message
            };
            _context.WorkToolLog.Add(workToolLog);
        }

        private async Task<string> SendRawMessage(RequestDto requestDto, ResultDto res)
        {
            var sendDto = new SendDto();
            var newList = new List
            {
                ReceivedContent = res.Data.Info.Text,
                TitleList = new List<string> { requestDto.GroupName },
                AtList = new List<string> { requestDto.ReceivedName }
            };
            sendDto.List.Add(newList);

            var url = "https://worktool.asrtts.cn/wework/sendRawMessage?robotId=" + _chatGPTSettings.RobotId;
            var client = new HttpClient();

            var jsonRequest = JsonSerializer.Serialize(sendDto, new JsonSerializerOptions() { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
            HttpContent content = new StringContent(jsonRequest);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var sendRawResult = await client.PostAsync(url, content);

            sendRawResult.EnsureSuccessStatusCode();
            return await sendRawResult.Content.ReadAsStringAsync();
        }
    }
}
